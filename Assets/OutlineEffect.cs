using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Material outlineMaterial;

    [Range(0, 10)] public float outlineThickness = 2f;
    [Range(0, 5)] public float glowIntensity = 1f;
    [Range(0, 5)] public float animationSpeed = 1f;
    [Range(0, 5)] public float colorShiftSpeed = 1f;

    void Update()
    {
        if (outlineMaterial != null)
        {
            outlineMaterial.SetFloat("_OutlineThickness", outlineThickness);
            outlineMaterial.SetFloat("_GlowIntensity", glowIntensity);
            outlineMaterial.SetFloat("_AnimationSpeed", animationSpeed);
            outlineMaterial.SetFloat("_ColorShiftSpeed", colorShiftSpeed);
        }
    }
}
