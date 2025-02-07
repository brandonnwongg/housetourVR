using UnityEngine;

public class ShaderPropertyHandler : MonoBehaviour
{
    public Material material; // The material to check
    public string propertyToCheck = "_RimColor"; // The property that might not exist
    public Color defaultColor = Color.white; // Default color to use if the property exists

    private bool propertyExists;

    void Start()
    {
        // Check if the material and property exist
        if (material != null)
        {
            propertyExists = material.HasProperty(propertyToCheck);
            if (!propertyExists)
            {
                Debug.LogWarning($"Material '{material.name}' does not have the property '{propertyToCheck}'.");
            }
        }
        else
        {
            Debug.LogError("Material is not assigned in the Inspector.");
        }
    }

    public void SetPropertyColor(Color color)
    {
        // Set the property only if it exists
        if (propertyExists)
        {
            material.SetColor(propertyToCheck, color);
        }
        else
        {
            Debug.LogWarning($"Cannot set color for property '{propertyToCheck}' as it does not exist on material '{material.name}'.");
        }
    }

    public void ResetToDefault()
    {
        // Reset the property to the default color if it exists
        if (propertyExists)
        {
            material.SetColor(propertyToCheck, defaultColor);
        }
    }
}
