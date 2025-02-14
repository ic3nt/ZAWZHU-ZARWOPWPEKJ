using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom Post-Processing/SRP/Chromatic")]
    public class ChromaticVolume : VolumeBase
    {
        public FloatParameter Chroma = new ClampedFloatParameter(8, -40, 40, false);

    }
}