using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom Post-Processing/SRP/Simple Edge")]
    public class SimpleEdgeVolume : VolumeBase
    {
        public FloatParameter Multiplier = new ClampedFloatParameter(1,0,20, false);
    }
}