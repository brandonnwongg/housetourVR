using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FlowerController : MonoBehaviour
{
    public VaseController vaseController; // Reference to the vase controller
    private XRGrabInteractable flowerGrabInteractable;

    public bool IsBeingGrabbed { get; private set; } // Tracks if the flower is currently being grabbed

    private Rigidbody rb;

    void Awake()
    {
        flowerGrabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (flowerGrabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable is missing on the flower.");
        }

        if (vaseController == null)
        {
            Debug.LogError("VaseController reference is not assigned.");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody is missing on the flower.");
        }
    }

    private void OnEnable()
    {
        flowerGrabInteractable.selectEntered.AddListener(OnFlowerGrabbed);
        flowerGrabInteractable.selectExited.AddListener(OnFlowerReleased);
    }

    private void OnDisable()
    {
        flowerGrabInteractable.selectEntered.RemoveListener(OnFlowerGrabbed);
        flowerGrabInteractable.selectExited.RemoveListener(OnFlowerReleased);
    }

    private void OnFlowerGrabbed(SelectEnterEventArgs args)
    {
        IsBeingGrabbed = true;

        // Detach the flower from the vase
        vaseController?.DetachFlower();

        Debug.Log("Flower grabbed and detached from the vase.");
    }

    private void OnFlowerReleased(SelectExitEventArgs args)
    {
        IsBeingGrabbed = false;

        // Allow the flower to behave naturally after release
        Debug.Log("Flower released.");
    }
}
