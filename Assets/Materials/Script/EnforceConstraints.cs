using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class EnforceConstraints : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool isReleased = false; // Track if the object was released

    public float velocityThreshold = 0.1f; // Threshold below which the object is considered "settled"
    public float groundThreshold = 0.02f; // Small height above ground to consider "settled"

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Ensure Rigidbody starts fully frozen
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll; // Freeze all position and rotation
            rb.useGravity = true; // Enable gravity
        }

        // Listen to grab and release events
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
        else
        {
            Debug.LogError("XRGrabInteractable component is missing on " + gameObject.name);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void FixedUpdate()
    {
        if (isReleased && rb != null)
        {
            // Only consider the object settled if it's close to the ground and has low velocity
            if (rb.velocity.magnitude < velocityThreshold && rb.angularVelocity.magnitude < velocityThreshold)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, groundThreshold))
                {
                    // Freeze the object after it has stopped moving and is on the ground
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    isReleased = false; // Reset release state
                }
            }
        }
    }
private void OnGrabbed(SelectEnterEventArgs args)
{
    if (rb != null)
    {
        // Allow free movement while grabbed
        rb.constraints = RigidbodyConstraints.None;

        // Disable gravity while grabbed
        rb.useGravity = false;

        // Reset any existing velocity to avoid unintended motion
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Detach the object from any parent (if needed)
        transform.SetParent(null);

        Debug.Log($"{gameObject.name} grabbed, gravity disabled.");
    }
}


private void OnReleased(SelectExitEventArgs args)
{
    if (rb != null)
    {
        // Re-enable gravity to force the object to fall naturally
        rb.useGravity = true;

        // Remove constraints on position
        rb.constraints = RigidbodyConstraints.None;

        // Optionally, freeze rotation on specific axes for stability
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Debug.Log($"{gameObject.name} released, gravity re-enabled.");
    }
}


}