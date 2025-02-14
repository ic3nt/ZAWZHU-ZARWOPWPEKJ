using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom Post-Processing/SRP/Drunk")]
    public class DrunkVolume : VolumeBase
    {
        public FloatParameter Speed = new ClampedFloatParameter(1,0,30, false);
    }
}