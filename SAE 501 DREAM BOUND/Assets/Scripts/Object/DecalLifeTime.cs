using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalLifetime : MonoBehaviour
{
    private float duration;
    private float startTime;
    private DecalProjector projector;
    private Material material;
    private bool initialized = false;
    
    // Cache the property ID to improve performance
    private static readonly int EmissiveColorId = Shader.PropertyToID("_EmissiveColor");
    private Color originalEmissiveColor;

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
        if (elapsed >= duration)
        {
            Destroy(gameObject);
            return;
        }

        float alpha = 1 - (elapsed / duration);
        if (projector != null && material != null)
        {
            // Fade the emissive color
            Color fadeColor = new Color(
                originalEmissiveColor.r,
                originalEmissiveColor.g,
                originalEmissiveColor.b,
                originalEmissiveColor.a * alpha
            );
            material.SetColor(EmissiveColorId, fadeColor);

            // You might also want to adjust the projector's opacity
            projector.fadeFactor = alpha;
        }
    }
}