using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ParentController manages a parent object that can have multiple children.
/// It ensures children are properly attached, detached, and updated when the parent moves.
/// </summary>
public class ParentController : MonoBehaviour
{
    // List to store multiple children attached to this parent
    public List<Transform> children = new List<Transform>();

    // Dictionary to store position and rotation offsets of children relative to the parent
    private Dictionary<Transform, Vector3> childOffsets = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> childRotations = new Dictionary<Transform, Quaternion>();

    // Dictionary to track if a child is currently attached to the parent
    private Dictionary<Transform, bool> isChildAttached = new Dictionary<Transform, bool>();

    /// <summary>
    /// Called when the script starts.
    /// If Netcode is enabled, it initializes children appropriately.
    /// </summary>
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

    /// <summary>
    /// Called when the network initializes. Ensures child objects are properly attached.
    /// </summary>
    private void OnNetworkInitialized()
    {
        Debug.Log($"Network initialized. Attaching children for Parent: {gameObject.name}");
        Unity.Netcode.NetworkManager.Singleton.OnServerStarted -= OnNetworkInitialized;
        InitializeChildren();
    }

    /// <summary>
    /// Attaches all predefined children at the start.
    /// </summary>
    private void InitializeChildren()
    {
        foreach (Transform child in children)
        {
            AttachChild(child);
        }
    }

    /// <summary>
    /// Updates the position and rotation of all attached children to move with the parent.
    /// </summary>
    void Update()
    {
        foreach (Transform child in children)
        {
            if (isChildAttached.ContainsKey(child) && isChildAttached[child])
            {
                child.position = transform.TransformPoint(childOffsets[child]);
                child.rotation = transform.rotation * childRotations[child];
            }
        }
    }

    /// <summary>
    /// Attaches a child object to this parent.
    /// The child's position and rotation are stored relative to the parent.
    /// </summary>
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

        // Store position and rotation offsets relative to the parent
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

        // Adjust Rigidbody settings for the attached child
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

    /// <summary>
    /// Detaches a child from this parent, restoring its ability to move independently.
    /// </summary>
    public void DetachChild(Transform child)
    {
        Debug.Log($"Detaching child {child.name} from parent {gameObject.name}");

        if (child == null || !isChildAttached.ContainsKey(child) || !isChildAttached[child]) return;

        isChildAttached[child] = false;
        child.SetParent(null); // Remove the child from the parent's hierarchy

        // Restore Rigidbody physics properties
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

        Debug.Log($"{child.name} detached from {gameObject.name}");
    }

    /// <summary>
    /// Waits briefly after detaching a child and freezes it if it stops moving.
    /// Prevents floating or lingering motion after being released.
    /// </summary>
    private System.Collections.IEnumerator FreezeAfterSettling(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.5f); // Allow time for settling

        if (rb.velocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll; // Freeze all movement and rotation
            Debug.Log("Object has settled and is now frozen.");
        }
    }

    /// <summary>
    /// Detects when a collision occurs between this object and another.
    /// Used primarily for debugging collision behavior.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Object {gameObject.name} collided with {collision.gameObject.name}");
    }

    /// <summary>
    /// Detects when another collider enters this object's trigger zone.
    /// If the entering object is a child, it is reattached to the parent.
    /// </summary>
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

                    // Adjust Rigidbody settings for stability
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
