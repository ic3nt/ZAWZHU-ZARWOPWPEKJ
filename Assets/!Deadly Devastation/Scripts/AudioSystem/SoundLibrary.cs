using UnityEngine;

[CreateAssetMenu(menuName = "Deadly Devastation/Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Tooltip("All sounds included in this library.")]
    public SoundData[] sounds;
}
