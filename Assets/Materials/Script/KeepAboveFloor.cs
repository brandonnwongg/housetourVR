using UnityEngine;

public class KeepAboveFloor : MonoBehaviour
{
    private Rigidbody rb;
    public float floorY = 0f; // Adjust to match your floor's Y position

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Ensure the object stays above the floor
        if (rb != null)
        {
            if (transform.position.y < floorY)
            {
                Vector3 position = transform.position;
                position.y = floorY;
                transform.position = position;

                // Reset velocities to stop further movement
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
