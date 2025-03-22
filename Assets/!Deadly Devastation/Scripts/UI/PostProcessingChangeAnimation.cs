using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PostProcessingChangeAnimation : MonoBehaviour
{
    public Volume PostProcessVolume;
    public AnimationCurve LensDistorationAnimationCurve;
    public AnimationCurve ChromaticAberrationAnimationCurve;

    private float LensDistorationIntensityLastTime;
    private float ChromaticAberrationIntensityLastTime;
    private LensDistortion LensDistoration;
    private ChromaticAberration ChromaticAberration;


    void Awake()
    {
        PostProcessVolume.profile.TryGet(out LensDistoration);
        PostProcessVolume.profile.TryGet(out ChromaticAberration);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            LensDistorationEffect();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            ChromaticAberrationEffect();
        }

        float LensDistorationIntensity = LensDistorationAnimationCurve.Evaluate(Time.realtimeSinceStartup - LensDistorationIntensityLastTime);
        LensDistoration.intensity.value = LensDistorationIntensity;

        float ChromaticAberrationIntensity = ChromaticAberrationAnimationCurve.Evaluate(Time.realtimeSinceStartup - ChromaticAberrationIntensityLastTime);
        ChromaticAberration.intensity.value = ChromaticAberrationIntensity;
    }

    public void LensDistorationEffect()
    {
        LensDistorationIntensityLastTime = Time.realtimeSinceStartup;
    }
    public void ChromaticAberrationEffect()
    {
        ChromaticAberrationIntensityLastTime = Time.realtimeSinceStartup;
    }
}
