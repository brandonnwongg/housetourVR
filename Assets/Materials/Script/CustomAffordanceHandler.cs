using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem;

public class CustomAffordanceHandler : MonoBehaviour
{
    public Material material; // The material to handle
    private ShaderPropertyHandler propertyHandler;

    void Start()
    {
        // Initialize the ShaderPropertyHandler
        propertyHandler = gameObject.AddComponent<ShaderPropertyHandler>();
        propertyHandler.material = material;

        // Check for '_RimColor' or set to default
        propertyHandler.SetPropertyColor(Color.red);
    }

    public void OnAffordanceStateChange()
    {
        // Update affordance color dynamically
        propertyHandler.SetPropertyColor(Color.green);
    }

    public void OnResetAffordance()
    {
        // Reset to the default state
        propertyHandler.ResetToDefault();
    }
}
