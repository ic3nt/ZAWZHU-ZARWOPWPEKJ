using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSt : MonoBehaviour
{
    public AudioSource aus;
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

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("canPickUp")) && isLocked )
        {
            if (other.TryGetComponent<Key>(out var ke))
            {
                aus.Play();
                isLocked = false;
                Destroy(ke.keyobj);
            }
        }


    }
}