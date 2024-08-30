using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startServerButton;
    [SerializeField] private Button startClientButton;

    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private GameObject menuCamera;

    [SerializeField] private TMP_InputField ipAddressInputField; 

    void Start()
    {
        startClientButton.onClick.AddListener(StartClient);
        startServerButton.onClick.AddListener(StartServer);
        startHostButton.onClick.AddListener(StartHost);
    }

    private void StartClient()
    {
        // подключение за клиента

        string ipAddress = ipAddressInputField.text;
        if (NetworkManager.Singleton.StartClient())
        {
            OnSuccessConnection();
            print("Client started!");
        }
        else
        {
            print("Client not started");
        }
    }

    private void StartServer()
    {
        // подключение за сервер

        if (NetworkManager.Singleton.StartServer())
        {
            OnSuccessConnection();
            print("Server started!");
        }
        else
        {
            print("Server not started");
        }
    }

    private void StartHost()
    {
        // подключение за хост

        if (NetworkManager.Singleton.StartHost())
        {
            OnSuccessConnection();
            print("Host started!");
        }
        else
        {
            print("Host not started");
        }
    }

    private void OnSuccessConnection()
    {
        // вызывается когда все норм

        buttonsPanel.SetActive(false);
        menuCamera.SetActive(false);
    }
}
