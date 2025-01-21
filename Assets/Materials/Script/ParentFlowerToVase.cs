using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ParentOnGrabWithConstantTracking : MonoBehaviour
{
    public GameObject childObject; // The flower

    private XRGrabInteractable grabInteractable;

    private Vector3 initialLocalPosition; // The flower's local position relative to the vase
    private Quaternion initialLocalRotation; // The flower's local rotation relative to the vase

    private bool isGrabbed = false; // Track if the vase is being grabbed

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null && childObject != null)
        {
            // Store the initial local position and rotation of the flower
            initialLocalPosition = childObject.transform.localPosition;
            initialLocalRotation = childObject.transform.localRotation;

            // Subscribe to grab and release events
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
        else
        {
            Debug.LogError("GrabInteractable or child object is not assigned.");
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void Update()
    {
        // Constantly update the flower's position and rotation while the vase is grabbed
        if (isGrabbed && childObject != null)
        {
            childObject.transform.localPosition = initialLocalPosition;
            childObject.transform.localRotation = initialLocalRotation;
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Mark as grabbed
        isGrabbed = true;

        // Parent the flower to the vase
        childObject.transform.SetParent(transform);

        // Reset local position and rotation to ensure the flower stays in place
        childObject.transform.localPosition = initialLocalPosition;
        childObject.transform.localRotation = initialLocalRotation;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Mark as released
        isGrabbed = false;

        // Keep the flower parented to the vase after release
        // Optional: Unparent the flower if needed (comment out this line if you don't want to unparent)
        childObject.transform.SetParent(null);
    }
}
