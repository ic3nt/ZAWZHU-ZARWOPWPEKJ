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
            

            GetComponent<FirstPersonMovement>().enabled = false;
            playerCamera.GetComponent<FirstPersonLook>().enabled = false;

            playerCamera.SetActive(false);
        }
    }
}
