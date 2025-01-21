using UnityEngine;

public class GlobalGravityEnforcer : MonoBehaviour
{
    void Start()
    {
        // Find all objects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Check if the object has a Rigidbody
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Enable gravity
                rb.useGravity = true;

                // Log for debugging (optional)
                Debug.Log($"Gravity enforced on {obj.name}");
            }
        }
    }

    void Update()
    {
        // Optional: Continuously enforce gravity for new objects
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null && !rb.useGravity)
            {
                rb.useGravity = true;
            }
        }
    }
}
