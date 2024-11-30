using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollisionSound : MonoBehaviour
{
    public AudioSource HitSound;
    public List<AudioClip> HitSoundClips;

    void OnCollisionEnter()
    {
        HitSound.clip = HitSoundClips[Random.Range(0, HitSoundClips.Count)];
        HitSound.Play();
    }
}
