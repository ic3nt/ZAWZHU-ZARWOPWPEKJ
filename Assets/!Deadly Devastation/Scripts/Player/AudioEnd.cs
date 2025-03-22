using UnityEngine;
using System.Collections;

public class AudioEnd: MonoBehaviour
{
    public AudioSource selectedAudioSource;

    private AudioListener audioListener;

    void Start()
    {
        audioListener = GetComponent<AudioListener>();
    }

    void Update()
    {
        if (selectedAudioSource != null)
        {
            AnimationCurve curve = new AnimationCurve();

            // Устанавливаем громкость на максимальное значение для выбранного аудиосорса
            curve.AddKey(0f, 1f);

            // Применяем настройки к аудиосорсу
            selectedAudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
        }
    }
}