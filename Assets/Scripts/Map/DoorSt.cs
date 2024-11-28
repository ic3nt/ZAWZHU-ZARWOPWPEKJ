using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorSt : NetworkBehaviour
{
    public AudioSource audioSource;
    public Material doorMaterial;

    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.green;

    public bool IsLocked;

    [Range(0, 1)]
    public float ChanceOfLock = 0.5f;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            IsLocked = Random.value > ChanceOfLock;
        }

        UpdateControllerDoorColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key") && IsLocked)
        {
            if (other.TryGetComponent<Key>(out var key))
            {
                Destroy(key.KeyObject);
                Debug.Log("Ключ использован для разблокировки двери");

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
            IsLocked = false;
            audioSource?.Play();
            UpdateClientsClientRpc();
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
        IsLocked = false;
        audioSource?.Play();
        UpdateControllerDoorColor();

    }

    private void UpdateControllerDoorColor()
    {
        if (doorMaterial != null)
        {
            Color targetColor = IsLocked ? lockedColor : unlockedColor;

            doorMaterial.SetColor("_Color", targetColor);

            doorMaterial.EnableKeyword("_EMISSION");
            doorMaterial.SetColor("_EmissionColor", targetColor);
        }
    }
}
