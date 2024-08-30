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
            GetComponent<FirstPersonMovement>().enabled = false;
            playerCamera.GetComponent<FirstPersonLook>().enabled = false;

            playerCamera.GetComponent<Camera>().enabled = false;
        }


        if (IsClient)
        {
            GetComponent<FirstPersonMovement>().enabled = true;
            playerCamera.GetComponent<FirstPersonLook>().enabled = true;

            playerCamera.GetComponent<Camera>().enabled = true;
        }
    }

    
}
