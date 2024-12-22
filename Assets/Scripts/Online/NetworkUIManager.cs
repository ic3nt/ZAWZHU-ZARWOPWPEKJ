using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkUIManager : MonoBehaviour
{
    public TMP_InputField ipInputField; 
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;
    public GameObject connectionUI;
    public GameObject gameUI;
    public TMP_Text debugText;
    public TMP_Text ipLobbyText;

    void Start()
    {
        connectionUI.gameObject.SetActive(true);
        gameUI.gameObject.SetActive(false);
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
        serverButton.onClick.AddListener(StartServer);

        debugText.text = ("Network UI Manager initialized. Ready to start.");
    }

    void StartHost()
    {
        debugText.text = ("Attempting to start Host...");

        if (NetworkManager.Singleton.StartHost())
        {
            ipLobbyText.text = ("CONNECTED TO LOBBY | HOST");
            debugText.text = ("Host started successfully. Listening for clients...");
            connectionUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(true);
        }
        else
        {
            debugText.text = ("Failed to start Host!");
        }
    }

    void StartClient()
    {
        string ipAddress = ipInputField.text;
        debugText.text = ("Attempting to start Client. Connecting to IP: {ipAddress}");

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ipAddress, 7777);

        if (NetworkManager.Singleton.StartClient())
        {
            ipLobbyText.text = ("CONNECTED TO LOBBY: {ipAddress} | CLIENT");
            debugText.text = ($"Client started successfully. Connecting to {ipAddress}...");
            connectionUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(true);
        }
        else
        {
            debugText.text = ("Failed to start Client!");
        }
    }

    void StartServer()
    {
        debugText.text = ("Attempting to start Server...");
        if (NetworkManager.Singleton.StartServer())
        {
            debugText.text = ("Server started successfully. Waiting for clients to connect...");
            connectionUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(true);
        }
        else
        {
            debugText.text = ("Failed to start Server!");
        }
    }
}
