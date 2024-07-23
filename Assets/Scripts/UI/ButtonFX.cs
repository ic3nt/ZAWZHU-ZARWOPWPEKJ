using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverAudioSource;
    public AudioClip clickAudioSource;
    public void HoverButton()
    {
        audioSource.PlayOneShot(hoverAudioSource);
    }
    public void ClickButton()
    {
        audioSource.PlayOneShot(clickAudioSource);
    }
}
