using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startServerButton;

    [SerializeField] private Button startClientButton;

    [SerializeField] private GameObject buttonsPanel;

    [SerializeField] private GameObject menuCamera;
    // Start is called before the first frame update
    void Start()
    {
        startClientButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartClient())
            {
                OnSuccessConnection();
                print("Client started!");
            }
            else
            {
                print("Client not started");
            }
        });
        startServerButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartServer())
            {
                OnSuccessConnection();
                print("Server started!");
            }
            else
            {
                print("Server not started");
            }
        });
        startHostButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartHost())
            {
                OnSuccessConnection();
                print("Host started!");
            }
            else
            {
                print("Host not started");
            }
        });
    }

    private void OnSuccessConnection()
    {
        buttonsPanel.SetActive(false);
        menuCamera.SetActive(false);
    }
}
