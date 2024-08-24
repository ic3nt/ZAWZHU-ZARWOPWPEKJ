using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSt : MonoBehaviour
{
    public AudioSource audioSource;
    public bool isLocked;

    [Range(0, 1)]
    public float ChanceOfLock = 0.5f;

    private void Start()
    {
        if (Random.value > ChanceOfLock)
        {
            isLocked = true;
        }
        else
        {
            isLocked = false;
        }
    }

    void OnCollisionEnter(Collider other)
    {
        if ((other.CompareTag("Key")) && isLocked)
        {
            if (other.TryGetComponent<Key>(out var key))
            {
                audioSource.Play();
                isLocked = false;
                Destroy(key.KeyObject);
            }
        }

    }
}