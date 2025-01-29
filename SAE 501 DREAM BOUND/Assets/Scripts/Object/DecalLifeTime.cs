using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalLifetime : MonoBehaviour
{
    private float duration; // Lifetime duration of the decal
    private float startTime; // Time when the decal is created
    private DecalProjector projector; // Reference to the DecalProjector component
    private Material material; // Material for the decal
    private bool initialized = false; // Tracks whether initialization is complete

    // Cached property ID for emissive color to improve performance
    private static readonly int EmissiveColorId = Shader.PropertyToID("_EmissiveColor");
    private Color originalEmissiveColor; // Original emissive color of the material

    // Initialize the decal with a specified lifetime duration
    public void Initialize(float duration)
    {
        this.duration = duration;
        this.startTime = Time.time;
        projector = GetComponent<DecalProjector>();

        if (projector != null)
        {
            // Create a material instance to avoid modifying the shared material
            material = new Material(projector.material);
            projector.material = material;

            // Store the original emissive color
            originalEmissiveColor = material.GetColor(EmissiveColorId);
        }

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        float elapsed = Time.time - startTime;

        // Destroy the decal once its lifetime expires
        if (elapsed >= duration)
        {
            Destroy(gameObject);
            return;
        }

        float alpha = 1 - (elapsed / duration); // Calculate the fade alpha value
        if (projector != null && material != null)
        {
            // Fade the emissive color based on the elapsed time
            Color fadeColor = new Color(
                originalEmissiveColor.r,
                originalEmissiveColor.g,
                originalEmissiveColor.b,
                originalEmissiveColor.a * alpha
            );
            material.SetColor(EmissiveColorId, fadeColor);

            // Optionally fade the projector's opacity
            projector.fadeFactor = alpha;
        }
    }
}
