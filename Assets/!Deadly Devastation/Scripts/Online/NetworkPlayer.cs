using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject playerCamera;

    // скрипт что бы короче игроки нормально смотрели, ходили и.т.д. ( без него все будет не очень, мы проверяли )

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            GetComponent<PlayerMovement>().enabled = false;
            playerCamera.GetComponent<PlayerCamera>().enabled = false;

            playerCamera.GetComponent<Camera>().enabled = false;
        }


        if (IsClient)
        {
            GetComponent<PlayerMovement>().enabled = true;
            playerCamera.GetComponent<PlayerCamera>().enabled = true;

            playerCamera.GetComponent<Camera>().enabled = true;
        }
    }

    
}
