using UnityEngine;

/// <summary>
/// GlobalGravityEnforcer is a script that enforces gravity on all objects
/// with Rigidbody components in the scene. It ensures that gravity is always enabled.
/// </summary>
public class GlobalGravityEnforcer : MonoBehaviour
{
    /// <summary>
    /// Called when the script is first initialized.
    /// Enables gravity on all objects with Rigidbody components at the start of the game.
    /// </summary>
    void Start()
    {
        // Find all objects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Loop through all objects to check for Rigidbody components
        foreach (GameObject obj in allObjects)
        {
            // Check if the object has a Rigidbody
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Enable gravity on the Rigidbody
                rb.useGravity = true;

                // Log for debugging purposes (optional)
                Debug.Log($"Gravity enforced on {obj.name}");
            }
        }
    }

    /// <summary>
    /// Called once per frame. Ensures that any newly created objects with Rigidbody
    /// components also have gravity enabled.
    /// </summary>
    void Update()
    {
        // Optional: Continuously enforce gravity for any new objects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Loop through all objects to check for Rigidbody components
        foreach (GameObject obj in allObjects)
        {
            // Ensure gravity is enabled for any Rigidbody
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null && !rb.useGravity)
            {
                rb.useGravity = true; // Enable gravity if it was disabled
            }
        }
    }
}
