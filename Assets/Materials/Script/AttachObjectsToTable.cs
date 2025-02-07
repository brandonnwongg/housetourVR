using UnityEngine;

public class AttachObjectsToTable : MonoBehaviour
{
    public Transform table; // Reference to the table object
    public LayerMask cupLayer; // Layer assigned to cups or objects to attach
    public float detectionRadius = 1.0f; // Radius to detect objects on the table

    void Start()
    {
        if (table == null)
        {
            Debug.LogError("Table reference is missing in AttachObjectsToTable script!");
            return;
        }

        // Find all colliders within the detection radius
        Collider[] detectedObjects = Physics.OverlapSphere(table.position, detectionRadius, cupLayer);

        foreach (Collider col in detectedObjects)
        {
            // Make each detected object a child of the table
            col.transform.SetParent(table);
            Debug.Log($"Attached {col.name} to {table.name}");
        }
    }
}
