using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabInteractionHandler : MonoBehaviour
{
    public FlowerVaseController flowerVaseController; // Reference to the FlowerVaseController
    public XRGrabInteractable vaseGrabInteractable; // Vase grab interactable
    public XRGrabInteractable flowerGrabInteractable; // Flower grab interactable

    void Start()
    {
        if (flowerVaseController == null || vaseGrabInteractable == null || flowerGrabInteractable == null)
        {
            Debug.LogError("References are missing in GrabInteractionHandler!");
            return;
        }

        // Listen to grab and release events for the vase
        vaseGrabInteractable.selectEntered.AddListener(OnVaseGrabbed);
        vaseGrabInteractable.selectExited.AddListener(OnVaseReleased);

        // Listen to grab and release events for the flower
        flowerGrabInteractable.selectEntered.AddListener(OnFlowerGrabbed);
        flowerGrabInteractable.selectExited.AddListener(OnFlowerReleased);
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        vaseGrabInteractable.selectEntered.RemoveListener(OnVaseGrabbed);
        vaseGrabInteractable.selectExited.RemoveListener(OnVaseReleased);

        flowerGrabInteractable.selectEntered.RemoveListener(OnFlowerGrabbed);
        flowerGrabInteractable.selectExited.RemoveListener(OnFlowerReleased);
    }

    private void OnVaseGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Vase grabbed.");
    }

    private void OnVaseReleased(SelectExitEventArgs args)
    {
        Debug.Log("Vase released.");
    }

    private void OnFlowerGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Flower grabbed.");
        flowerVaseController.DetachFlowerFromVase(); // Detach flower from the vase
    }

    private void OnFlowerReleased(SelectExitEventArgs args)
    {
        Debug.Log("Flower released.");
        // Optionally reattach the flower to the vase after release
        flowerVaseController.AttachFlowerToVase();
    }
}
