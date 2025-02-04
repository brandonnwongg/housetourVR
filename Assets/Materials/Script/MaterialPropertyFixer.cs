using UnityEngine;

public class MaterialPropertyFixer : MonoBehaviour
{
    // The fallback property to use when the target property doesn't exist
    [SerializeField] private string fallbackProperty = "_BaseColor";
    [SerializeField] private string targetProperty = "_RimColor"; // The property causing the issue
    [SerializeField] private Color defaultColor = Color.white; // Default color if fallback is applied

    private void Start()
    {
        FixMaterials();
    }

    private void FixMaterials()
    {
        // Get all renderers in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (var renderer in renderers)
        {
            // Iterate through all materials assigned to this renderer
            foreach (var material in renderer.materials)
            {
                if (material != null && !material.HasProperty(targetProperty))
                {
                    Debug.LogWarning($"Material '{material.name}' with shader '{material.shader.name}' does not have the property '{targetProperty}'. Applying fallback.");

                    // Check if the fallback property exists
                    if (material.HasProperty(fallbackProperty))
                    {
                        // Apply the fallback property
                        material.SetColor(fallbackProperty, defaultColor);
                    }
                    else
                    {
                        Debug.LogError($"Material '{material.name}' does not have the fallback property '{fallbackProperty}' either. Please check the shader.");
                    }
                }
            }
        }
    }
}
