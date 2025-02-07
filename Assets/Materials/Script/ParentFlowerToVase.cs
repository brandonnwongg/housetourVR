using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ParentOnGrabWithPositionReset : MonoBehaviour
{
    public GameObject childObject; // The flower

    private XRGrabInteractable grabInteractable;

    private Vector3 initialLocalPosition; // The flower's local position relative to the vase
    private Quaternion initialLocalRotation; // The flower's local rotation relative to the vase

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

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Parent the flower to the vase
        childObject.transform.SetParent(transform);

        // Reset local position and rotation to maintain its position inside the vase
        childObject.transform.localPosition = initialLocalPosition;
        childObject.transform.localRotation = initialLocalRotation;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Unparent the flower when the vase is released (optional)
        // If you want the flower to stay in the vase, you can comment this line
        childObject.transform.SetParent(null);
    }
}
