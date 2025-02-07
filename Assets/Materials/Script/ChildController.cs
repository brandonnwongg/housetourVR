using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// The ChildController manages objects that can be picked up and moved independently.
/// It ensures proper physics behavior when grabbed and released in an XR environment.
/// </summary>
public class ChildController : MonoBehaviour
{
    public ParentController parentController; // Reference to the parent object
    private XRGrabInteractable grabInteractable; // XR Interaction component for grabbing

    public bool IsBeingGrabbed { get; private set; } // Tracks whether the object is currently being grabbed
    private Rigidbody rb; // Rigidbody component for physics control

    /// <summary>
    /// Initializes the object by retrieving necessary components and validating references.
    /// </summary>
    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // Ensure the object has the required XRGrabInteractable component
        if (grabInteractable == null)
        {
            Debug.LogError($"XRGrabInteractable is missing on object: {gameObject.name}, located in the hierarchy: {GetFullHierarchyPath()}");
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>(); // Automatically add component if missing
        }

        // Ensure the ParentController reference is assigned
        if (parentController == null)
        {
            Debug.LogError($"ParentController reference is not assigned for object: {gameObject.name}, located in the hierarchy: {GetFullHierarchyPath()}");
        }

        // Ensure the object has a Rigidbody for physics interactions
        if (rb == null)
        {
            Debug.LogError($"Rigidbody is missing on object: {gameObject.name}, located in the hierarchy: {GetFullHierarchyPath()}");
        }
    }

    /// <summary>
    /// Helper method to retrieve the full hierarchy path of the object for debugging purposes.
    /// </summary>
    private string GetFullHierarchyPath()
    {
        string path = gameObject.name;
        Transform current = transform.parent;

        while (current != null)
        {
            path = $"{current.name}/{path}";
            current = current.parent;
        }

        return path;
    }

    /// <summary>
    /// Subscribes to XR interaction events when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnChildGrabbed);
        grabInteractable.selectExited.AddListener(OnChildReleased);
    }

    /// <summary>
    /// Called when the object is grabbed.
    /// Detaches it from the parent and allows free movement.
    /// </summary>
    private void OnChildGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log($"Child {gameObject.name} grabbed");

        IsBeingGrabbed = true;

        // Detach the object from the parent if it's currently attached
        if (parentController != null)
        {
            parentController.DetachChild(transform);
        }

        if (rb != null)
        {
            // Ensure the object can move freely while grabbed
            rb.useGravity = false; // Disable gravity to prevent immediate falling
            rb.constraints = RigidbodyConstraints.None; // Remove any movement restrictions
        }

        Debug.Log($"{gameObject.name} grabbed and detached from parent.");
    }

    /// <summary>
    /// Called when the object is released.
    /// Re-enables gravity and ensures a natural physics response.
    /// </summary>
    private void OnChildReleased(SelectExitEventArgs args)
    {
        Debug.Log($"Child {gameObject.name} released");

        IsBeingGrabbed = false;

        if (rb != null)
        {
            // Enable gravity to ensure the object falls naturally
            rb.useGravity = true;

            // Reset velocity to prevent unintended movement after release
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Remove all constraints for free movement after being released
            rb.constraints = RigidbodyConstraints.None;

            // Apply a slight downward force to ensure the object begins falling
            rb.AddForce(Vector3.down * 1.0f, ForceMode.Impulse);

            Debug.Log($"{gameObject.name} released and ready to fall.");
        }
    }
}
