using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSt : MonoBehaviour
{

    public bool islocked;

    [Range(0, 1)]
    public float ChanceOfLock = 0.1f;

    private void Start()
    {
        if (Random.value > ChanceOfLock)
        {
            islocked = true;

        }
        else
        {
            islocked = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("canPickUp")) && islocked )
        {
            if (other.TryGetComponent<Key>(out var ke))
            {
                Destroy(ke);
                islocked = false;
            }
        }


    }
}