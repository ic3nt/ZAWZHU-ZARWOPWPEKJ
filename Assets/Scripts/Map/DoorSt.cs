using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public class DoorSt : NetworkBehaviour
{
    public AudioSource audioSource;

    public bool IsLocked;

    [Range(0, 1)]
    public float ChanceOfLock = 0.5f;

    public Renderer doorControllerRenderer;
    public Material lockedMaterial;
    public Material unlockedMaterial;

    public SpriteRenderer stateSpriteRenderer;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            IsLocked = Random.value > ChanceOfLock;
            UpdateDoorAppearance();
        }
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
            UpdateDoorAppearance();
            ShakeSprite();
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
        UpdateDoorAppearance();
        ShakeSprite();
    }

    private void UpdateDoorAppearance()
    {
        Material[] materials = doorControllerRenderer.materials;

        if (materials.Length > 1)
        {
            materials[1] = IsLocked ? lockedMaterial : unlockedMaterial;
            doorControllerRenderer.materials = materials;
        }
        else
        {
            Debug.LogWarning("У объекта DoorRenderer установлено недостаточно материалов!");
        }

        stateSpriteRenderer.sprite = IsLocked ? lockedSprite : unlockedSprite;
    }

    private void ShakeSprite()
    {
        stateSpriteRenderer.transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true);
    }
}
