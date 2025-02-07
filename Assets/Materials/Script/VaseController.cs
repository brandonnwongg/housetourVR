using UnityEngine;

public class VaseController : MonoBehaviour
{
    public Transform flower; // Reference to the flower object
    private bool isFlowerAttached = true; // Tracks if the flower is attached to the vase

    private Vector3 flowerInitialOffset; // Offset of the flower relative to the vase
    private Quaternion flowerInitialRotation; // Rotation of the flower relative to the vase

    void Start()
    {
        if (flower != null)
        {
            // Store the initial local position and rotation of the flower relative to the vase
            flowerInitialOffset = transform.InverseTransformPoint(flower.position);
            flowerInitialRotation = Quaternion.Inverse(transform.rotation) * flower.rotation;
        }
    }

    void Update()
    {
        if (isFlowerAttached && flower != null)
        {
            // Keep the flower aligned with the vase
            flower.position = transform.TransformPoint(flowerInitialOffset);
            flower.rotation = transform.rotation * flowerInitialRotation;
        }
    }

    public void AttachFlower()
    {
        if (flower == null) return;

        isFlowerAttached = true;

        // Parent the flower to the vase
        flower.SetParent(transform);

        // Align the flower's position and rotation to its original offset
        flower.localPosition = flowerInitialOffset; // Use stored offset for accurate alignment
        flower.localRotation = flowerInitialRotation; // Use stored rotation for upright alignment

        Debug.Log("Flower attached to the vase.");
    }

    public void DetachFlower()
    {
        if (flower == null) return;

        isFlowerAttached = false;

        // Detach the flower from the vase
        flower.SetParent(null);

        Debug.Log("Flower detached from the vase.");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Attach the flower when it enters the vase's trigger zone
        if (other.transform == flower && !flower.GetComponent<FlowerController>().IsBeingGrabbed)
        {
            AttachFlower();
        }
    }
}
