using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class EnforceConstraints : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    public float groundY = 0.55f; // The Y position of the ground
    public bool useGroundYClamping = true; // Optionally use fixed ground Y clamping

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Listen to grab and release events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void FixedUpdate()
    {
        if (!grabInteractable.isSelected && rb != null)
        {
            if (useGroundYClamping)
            {
                // Ensure the object doesn't go below the ground
                Vector3 position = rb.position;
                if (position.y < groundY)
                {
                    position.y = groundY;
                    rb.position = position;

                    // Stop movement to prevent clipping or bouncing
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (rb != null)
        {
            // Remove constraints to allow free movement
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = false; // Disable gravity while being grabbed
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (rb != null)
        {
            // Allow the object to fall naturally to the ground
            rb.useGravity = true;

            // Reapply constraints to freeze Y position and rotations when touching the ground
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            // Optionally, reset the Y position if clamping is enabled
            if (useGroundYClamping)
            {
                Vector3 position = rb.position;
                if (position.y < groundY)
                {
                    position.y = groundY;
                    rb.position = position;

                    // Reset velocities to stop movement
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }
}
