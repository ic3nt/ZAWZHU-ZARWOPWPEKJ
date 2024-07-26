using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerManager : MonoBehaviour
{
    public GameObject UI;

    public void StartHost()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton is null. Ensure you have a NetworkManager component in the scene.");
            return;
        }

        NetworkManager.Singleton.StartHost();
        Debug.Log("Хост запущен.");
        UI.SetActive(false);
    }


    public void StartClient()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton is null. Ensure you have a NetworkManager component in the scene.");
            return;
        }

        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsHost)
        {
            Debug.LogError("Cannot start client. The server is not started.");
            return;
        }

        NetworkManager.Singleton.StartClient();
        Debug.Log("Клиент запущен и подключается к серверу.");
        UI.SetActive(false);
    }


    private void OnApplicationQuit()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Сервер остановлен.");
        }
    }
}
