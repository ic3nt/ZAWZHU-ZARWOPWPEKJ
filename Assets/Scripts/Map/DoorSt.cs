using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorSt : NetworkBehaviour
{
    public AudioSource audioSource;

    [HideInInspector]
    public NetworkVariable<bool> IsLocked = new NetworkVariable<bool>(true); // Начальное состояние - заблокировано

    [Range(0, 1)]
    public float ChanceOfLock = 0.5f;

    private void Start()
    {
        if (!IsOwner) return; // Выполняем только для владельца/клиента
        // Определяем, заблокирована ли дверь в зависимости от ChanceOfLock
        IsLocked.Value = Random.value > ChanceOfLock;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key") && IsLocked.Value)
        {
            if (other.TryGetComponent<Key>(out var key))
            {
                UnlockDoorServerRpc();  // Изменено на правильный вызов метода
                Destroy(key.KeyObject);
                Debug.Log("Ключ использован для разблокировки двери");
            }
        }
    }

    [ServerRpc]
    private void UnlockDoorServerRpc() // Исправлено имя метода
    {
        IsLocked.Value = false; // Разблокируем дверь
        audioSource.Play(); // Проигрываем звук разблокировки
        UpdateClientsClientRpc(); // Уведомляем всех клиентов обновить свое состояние
    }

    [ClientRpc]
    private void UpdateClientsClientRpc() // Исправлено имя метода
    {
        IsLocked.Value = false;
        audioSource.Play(); // Опционально проигрываем звук для всех клиентов
    }
}
