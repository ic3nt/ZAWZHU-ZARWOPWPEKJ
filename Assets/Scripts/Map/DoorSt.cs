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
        // тута значит при старте мы чекаем флоат с шансами и с помощью рандома дверь блокается или не блокается

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
        // если объект с тегом Key и одноименным компонентом соприкасается с дверью, при этом дверь закрыта, то мы открываем дверь удаляем объект ключа и.т.д, ну ты понял. тут программистом не надо быть что бы понять че тут делается

        if ((other.CompareTag("Key")) && isLocked)
        {
            if (other.TryGetComponent<Key>(out var key))
            {
                audioSource.Play();
                isLocked = false;
                Destroy(key.KeyObject);
                Debug.Log("Key");
            }
        }

    }
}