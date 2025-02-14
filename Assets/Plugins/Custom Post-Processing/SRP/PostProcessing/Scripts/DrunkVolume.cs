using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Custom Post-Processing/SRP/Drunk")]
public class DrunkVolume : VolumeBase
{
    public ClampedFloatParameter Speed = new ClampedFloatParameter(1, 0, 30, false);
    public ClampedFloatParameter Zoom = new ClampedFloatParameter(1, 0.5f, 2, false);
}
