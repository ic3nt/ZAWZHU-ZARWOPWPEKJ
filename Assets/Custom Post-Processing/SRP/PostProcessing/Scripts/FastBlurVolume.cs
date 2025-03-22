using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom Post-Processing/SRP/FastBlur")]
    public class FastBlurVolume : VolumeBase
    {
        public FloatParameter Scale = new ClampedFloatParameter(5,1,20, false);
    }
}