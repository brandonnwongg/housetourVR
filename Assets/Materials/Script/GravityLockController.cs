using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GravityLockController : MonoBehaviour
{
    private Rigidbody rb; // Reference to the Rigidbody
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable; // Reference to the XRGrabInteractable
    public float groundY = 0f; // Y-position of the floor (adjust to match your floor level)
    public bool lockToFloor = true; // Ensures object stays on the floor when not grabbed

    void Awake()
    {
        // Get references to Rigidbody and XRGrabInteractable
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Subscribe to grab and release events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    void Update()
    {
        // Lock object to floor if not grabbed
        if (!grabInteractable.isSelected && lockToFloor)
        {
            Vector3 position = transform.position;
            position.y = groundY; // Keep object at ground level
            transform.position = position;
            rb.velocity = Vector3.zero; // Prevent sliding
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Disable gravity and allow full movement when grabbed
        rb.useGravity = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Re-enable gravity after releasing the object
        rb.useGravity = true;
    }
}
