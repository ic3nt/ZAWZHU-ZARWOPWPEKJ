using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Collections;

public class InternetConnectionMonitor : MonoBehaviour
{
    public GameObject warningUI;
    public float pingThreshold = 150f;
    private float checkInterval = 5f;
    private float lastCheckTime;

    void Start()
    {
        warningUI.SetActive(false);
        lastCheckTime = Time.time;

        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("PingResponse", HandlePingResponse);
    }

    void Update()
    {
        if (Time.time - lastCheckTime >= checkInterval)
        {
            SendPing();
            lastCheckTime = Time.time;
        }
    }

    void SendPing()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                using (FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp))
                {
                    writer.WriteValueSafe(Time.realtimeSinceStartup); 

                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("PingResponse", client.ClientId, writer);
                }
            }
        }
    }

    void HandlePingResponse(ulong clientId, FastBufferReader reader)
    {
        float sentTime = 0;
        reader.ReadValueSafe(out sentTime);

        float currentPing = (Time.realtimeSinceStartup - sentTime) * 1000; 
        Debug.Log($"Ping Response Received from Client {clientId} at {currentPing} ms");

        if (currentPing > pingThreshold)
        {
            ShowWarningUI();
        }
        else
        {
            HideWarningUI();
        }
    }

    void ShowWarningUI()
    {
        warningUI.SetActive(true);
    }

    void HideWarningUI()
    {
        warningUI.SetActive(false);
    }
}
