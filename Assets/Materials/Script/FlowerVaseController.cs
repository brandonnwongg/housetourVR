using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlowerVaseController : MonoBehaviour
{
    public Transform vase; // Reference to the vase
    public Transform flower; // Reference to the flower

    private bool isFlowerAttachedToVase = true; // Track if the flower is attached to the vase

    void Start()
    {
        if (vase == null || flower == null)
        {
            Debug.LogError("Vase or Flower is not assigned in the Inspector!");
            return;
        }

        // Ensure the flower starts attached to the vase
        AttachFlowerToVase();
    }

    void Update()
    {
        if (isFlowerAttachedToVase && vase != null && flower != null)
        {
            // Ensure the flower follows the vase's position and rotation
            flower.position = vase.position;
            flower.rotation = vase.rotation;
        }
    }

    public void AttachFlowerToVase()
    {
        if (vase != null && flower != null)
        {
            isFlowerAttachedToVase = true;
            Debug.Log("Flower is now attached to the vase.");
        }
    }

    public void DetachFlowerFromVase()
    {
        isFlowerAttachedToVase = false;
        Debug.Log("Flower is now detached from the vase.");
    }
}
