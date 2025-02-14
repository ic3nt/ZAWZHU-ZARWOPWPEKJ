using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Custom Post-Processing/SRP/CameraShake")]
public class CameraShakeVolume : VolumeBase
{
    public ClampedFloatParameter ShakeIntensity = new ClampedFloatParameter(0.1f, 0, 1, false);
    public ClampedFloatParameter ShakeSpeed = new ClampedFloatParameter(10, 0, 50, false);
}
