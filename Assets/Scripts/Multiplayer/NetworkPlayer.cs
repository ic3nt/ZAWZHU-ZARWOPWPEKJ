using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{

    [SerializeField] private GameObject playerCamera;

    void Start()
    {
        if (!GetComponent<NetworkObject>().IsLocalPlayer)
        {
            playerCamera.SetActive(false);
            GetComponent<FirstPersonMovement>().enabled = false;
            GetComponent<FirstPersonLook>().enabled = false;
        }
    }
}
