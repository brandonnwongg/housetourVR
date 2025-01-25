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
        // Initialize offsets and attach all children
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
    Debug.Log($"Attaching child {child.name} to parent {gameObject.name}");

    if (child == null || isChildAttached.ContainsKey(child) && isChildAttached[child]) return;

    childOffsets[child] = transform.InverseTransformPoint(child.position);
    childRotations[child] = Quaternion.Inverse(transform.rotation) * child.rotation;
    isChildAttached[child] = true;

    child.SetParent(transform);

    // Disable gravity and freeze unnecessary constraints
    var rb = child.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.useGravity = false; // Disable gravity
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
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
        rb.constraints = RigidbodyConstraints.None; // Remove all constraints
        rb.useGravity = true; // Enable gravity
        rb.velocity = Vector3.zero; // Reset velocity
        rb.angularVelocity = Vector3.zero; // Reset angular velocity
    }

    Debug.Log($"{child.name} detached from {gameObject.name}");
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
