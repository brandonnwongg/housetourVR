using System.Collections.Generic;
using UnityEngine;

public class ParentController : MonoBehaviour
{
    public List<Transform> children = new List<Transform>(); // List to store multiple children
    private Dictionary<Transform, Vector3> childOffsets = new Dictionary<Transform, Vector3>(); // Position offsets
    private Dictionary<Transform, Quaternion> childRotations = new Dictionary<Transform, Quaternion>(); // Rotation offsets
    private Dictionary<Transform, bool> isChildAttached = new Dictionary<Transform, bool>(); // Tracks if a child is attached

void Start()
{
    if (Unity.Netcode.NetworkManager.Singleton != null && Unity.Netcode.NetworkManager.Singleton.IsListening)
    {
        InitializeChildren();
    }
    else if (Unity.Netcode.NetworkManager.Singleton != null)
    {
        Debug.LogWarning($"Network is not active. Delaying child attachment for Parent: {gameObject.name}");
        Unity.Netcode.NetworkManager.Singleton.OnServerStarted += OnNetworkInitialized;
    }
}

private void OnNetworkInitialized()
{
    Debug.Log($"Network initialized. Attaching children for Parent: {gameObject.name}");
    Unity.Netcode.NetworkManager.Singleton.OnServerStarted -= OnNetworkInitialized;
    InitializeChildren();
}

private void InitializeChildren()
{
    foreach (Transform child in children)
    {
        AttachChild(child);
    }
}


    void Update()
    {
        // Update position and rotation for attached children
        foreach (Transform child in children)
        {
            if (isChildAttached.ContainsKey(child) && isChildAttached[child])
            {
                child.position = transform.TransformPoint(childOffsets[child]);
                child.rotation = transform.rotation * childRotations[child];
            }
        }
    }
    
    public void AttachChild(Transform child)
    {
    if (child == null || (isChildAttached.ContainsKey(child) && isChildAttached[child])) return;

    // Ensure the NetworkManager is active before reparenting
    if (Unity.Netcode.NetworkManager.Singleton != null && !Unity.Netcode.NetworkManager.Singleton.IsListening)
    {
        Debug.LogWarning($"Cannot attach {child.name} because the network is not active.");
        return;
    }

    Debug.Log($"Attaching child {child.name} to parent {gameObject.name}");

    // Store position and rotation offsets
    childOffsets[child] = transform.InverseTransformPoint(child.position);
    childRotations[child] = Quaternion.Inverse(transform.rotation) * child.rotation;
    isChildAttached[child] = true;

    // Check if the object has a NetworkObject component
    var networkObject = child.GetComponent<Unity.Netcode.NetworkObject>();
    if (networkObject != null)
    {
        // Use Netcode for GameObjects method for reparenting
        networkObject.TrySetParent(transform, true);
    }
    else
    {
        // Standard Unity reparenting
        child.SetParent(transform);
    }

    // Manage Rigidbody physics settings
    var rb = child.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.useGravity = false; // Disable gravity to prevent objects from falling
        rb.velocity = Vector3.zero; // Reset velocity to prevent unintended motion
        rb.angularVelocity = Vector3.zero; // Reset angular velocity

        // Apply movement constraints to prevent sliding or rotation
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    Debug.Log($"{child.name} attached to {gameObject.name}");
   }


public void DetachChild(Transform child)
{
    Debug.Log($"Detaching child {child.name} from parent {gameObject.name}");

    if (child == null || !isChildAttached.ContainsKey(child) || !isChildAttached[child]) return;

    isChildAttached[child] = false;
    child.SetParent(null);

    // Reapply gravity and ensure the child stops moving
    var rb = child.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.useGravity = true; // Enable gravity
        rb.isKinematic = false; // Ensure physics simulation
        rb.velocity = Vector3.zero; // Reset velocity
        rb.angularVelocity = Vector3.zero; // Reset angular velocity

        // Add drag to reduce further movement
        rb.drag = 5f; // Linear drag for slowing down
        rb.angularDrag = 5f; // Angular drag for rotation stabilization

        // Apply constraints to stop movement after settling
        StartCoroutine(FreezeAfterSettling(rb));
    }

    // Ensure scale is not altered
    child.localScale = Vector3.one;
    Debug.Log($"{child.name} detached from {gameObject.name}");
}

private System.Collections.IEnumerator FreezeAfterSettling(Rigidbody rb)
{
    yield return new WaitForSeconds(0.5f); // Allow time for settling

    if (rb.velocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll; // Freeze all movement and rotation
        Debug.Log("Object has settled and is now frozen.");
    }
}


private void OnCollisionEnter(Collision collision)
{
    Debug.Log($"Object {gameObject.name} collided with {collision.gameObject.name}");
}




private void OnTriggerEnter(Collider other)
{
    Debug.Log($"Trigger detected with {other.gameObject.name}");

    foreach (Transform child in children)
    {
        if (other.transform == child)
        {
            Debug.Log($"Trigger detected for child {child.gameObject.name}");
            var childController = child.GetComponent<ChildController>();
            if (childController != null && !childController.IsBeingGrabbed)
            {
                AttachChild(child);

                // Disable gravity and freeze unnecessary constraints
                var rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false; // Disable gravity
                    rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                }

                Debug.Log($"{child.name} reattached to {gameObject.name}");
            }
        }
    }
}






}
