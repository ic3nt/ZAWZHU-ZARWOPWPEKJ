using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorSt : NetworkBehaviour
{
    public AudioSource audioSource;

    public bool IsLocked; // Добавлена точка с запятой здесь

    [Range(0, 1)]
    public float ChanceOfLock = 0.5f;

    public override void OnNetworkSpawn()
    {
        // Выполняется только для сервера
        if (IsOwner)
        {
            IsLocked = Random.value > ChanceOfLock;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key") && IsLocked)
        {
            if (other.TryGetComponent<Key>(out var key))
            {
                // Проверяем, является ли объект сервером
                

                // Уничтожаем объект ключа
                Destroy(key.KeyObject);
                Debug.Log("Ключ использован для разблокировки двери");

                // Вызываем RPC для разблокировки двери
                UnlockDoorServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnlockDoorServerRpc()
    {
        if (IsLocked)
        {
            Debug.Log("UnlockDoorServerRpc вызван");
            IsLocked = false; // Разблокируем дверь на сервере
            audioSource?.Play(); // Проигрываем звук разблокировки
            UpdateClientsClientRpc(); // Уведомляем всех клиентов о разблокировке
            Debug.Log("Дверь разблокирована");
        }
        else
        {
            Debug.Log("Дверь уже разблокирована. RPC не будет выполнен.");
        }
    }

    [ClientRpc]
    private void UpdateClientsClientRpc()
    {
        IsLocked = false; // Разблокируем дверь на клиентах
        audioSource?.Play(); // Проигрываем звук для всех клиентов
    }
}
