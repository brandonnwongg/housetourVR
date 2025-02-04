using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// EnforceConstraints is responsible for managing the physics constraints of an object, 
/// ensuring proper behavior when grabbed and released using XR interactions.
/// </summary>
public class EnforceConstraints : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool isReleased = false; // Tracks whether the object has been released

    public float velocityThreshold = 0.1f; // Threshold for detecting when the object has settled
    public float groundThreshold = 0.02f; // Distance from the ground to consider as settled

    /// <summary>
    /// Called when the object is first initialized.
    /// Ensures that the object starts with appropriate physics constraints.
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Ensure the Rigidbody starts with constraints and gravity enabled
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll; // Freeze all position and rotation
            rb.useGravity = true; // Enable gravity by default
        }

        // Listen to grab and release events from XR interaction system
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

    /// <summary>
    /// Called when the object is destroyed.
    /// Unsubscribes from event listeners to prevent memory leaks.
    /// </summary>
    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    /// <summary>
    /// Runs at a fixed time step and ensures that the object settles properly when released.
    /// If the object is close to the ground and has minimal movement, it is frozen in place.
    /// </summary>
    private void FixedUpdate()
    {
        if (isReleased && rb != null)
        {
            // Check if the object's velocity and angular velocity are below the defined threshold
            if (rb.velocity.magnitude < velocityThreshold && rb.angularVelocity.magnitude < velocityThreshold)
            {
                RaycastHit hit;
                // Use a raycast to check if the object is near the ground
                if (Physics.Raycast(transform.position, Vector3.down, out hit, groundThreshold))
                {
                    // Freeze the object to prevent unnecessary physics calculations
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    isReleased = false; // Reset release state
                }
            }
        }
    }

    /// <summary>
    /// Called when the object is grabbed using XR interaction.
    /// Enables free movement by removing constraints and disabling gravity.
    /// </summary>
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (rb != null)
        {
            // Remove all constraints to allow full movement while grabbed
            rb.constraints = RigidbodyConstraints.None;

            // Disable gravity for smoother interaction
            rb.useGravity = false;

            // Reset velocity to prevent unintended movement after grabbing
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Detach the object from any parent to allow independent movement
            transform.SetParent(null);

            Debug.Log($"{gameObject.name} grabbed, gravity disabled.");
        }
    }

    /// <summary>
    /// Called when the object is released using XR interaction.
    /// Re-enables gravity and applies constraints to ensure stability.
    /// </summary>
    private void OnReleased(SelectExitEventArgs args)
    {
        if (rb != null)
        {
            // Re-enable gravity to allow the object to fall naturally
            rb.useGravity = true;

            // Remove all position constraints
            rb.constraints = RigidbodyConstraints.None;

            // Apply rotation constraints to prevent excessive spinning
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            Debug.Log($"{gameObject.name} released, gravity re-enabled.");
        }
    }
}
