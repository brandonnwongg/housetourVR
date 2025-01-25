using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ChildController : MonoBehaviour
{
    public ParentController parentController; // Reference to the parent
    private XRGrabInteractable grabInteractable;

    public bool IsBeingGrabbed { get; private set; } // Tracks if the child is currently being grabbed
    private Rigidbody rb;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable is missing on the child.");
        }

        if (parentController == null)
        {
            Debug.LogError("ParentController reference is not assigned.");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody is missing on the child.");
        }
    }



private void OnEnable()
{
    grabInteractable.selectEntered.AddListener(OnChildGrabbed);
    grabInteractable.selectExited.AddListener(OnChildReleased);
}

private void OnChildGrabbed(SelectEnterEventArgs args)
{
    Debug.Log($"Child {gameObject.name} grabbed");

    IsBeingGrabbed = true;

    if (parentController != null)
    {
        parentController.DetachChild(transform);
    }

    if (rb != null)
    {
        // Ensure the child can move freely
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.None; // Allow full physics-based movement
    }

    Debug.Log($"{gameObject.name} grabbed and detached from parent.");
}

private void OnChildReleased(SelectExitEventArgs args)
{
    Debug.Log($"Child {gameObject.name} released");

    IsBeingGrabbed = false;

    if (rb != null)
    {
        // Ensure gravity is enabled
        rb.useGravity = true;

        // Reset velocity to prevent lingering movement
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Remove all constraints for free movement
        rb.constraints = RigidbodyConstraints.None;

        // Apply a downward force to ensure the object begins falling
        rb.AddForce(Vector3.down * 1.0f, ForceMode.Impulse);

        Debug.Log($"{gameObject.name} released and ready to fall.");
    }
}






}
