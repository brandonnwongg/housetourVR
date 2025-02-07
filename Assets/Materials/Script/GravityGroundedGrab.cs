using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GravityGroundedGrab : MonoBehaviour
{
    private Rigidbody rb; // Reference to the Rigidbody
    private XRGrabInteractable grabInteractable; // Reference to the XRGrabInteractable
    private bool isGrabbed = false; // Whether the object is currently grabbed
    public float groundY = 1f; // Y-position of the floor
    public float snapSpeed = 5f; // Speed to snap the object to the floor after release
    public bool freezeRotation = true; // Whether to freeze rotation when grounded

    void Awake()
    {
        // Get Rigidbody and XRGrabInteractable components
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab and release events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    void FixedUpdate()
    {
        if (!isGrabbed)
        {
            // Keep the object grounded when not grabbed
            Vector3 position = rb.position;
            position.y = groundY; // Snap to the ground's Y position
            rb.position = Vector3.Lerp(rb.position, position, Time.fixedDeltaTime * snapSpeed);

            // Freeze unnecessary rotation
            if (freezeRotation)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
        else
        {
            // Allow free movement when grabbed
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        rb.useGravity = false; // Disable gravity while grabbed
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        rb.useGravity = true; // Re-enable gravity when released
    }
}
