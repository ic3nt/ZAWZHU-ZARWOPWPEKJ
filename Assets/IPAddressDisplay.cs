using System;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IPAddressDisplay : MonoBehaviour
{
    public TextMeshProUGUI ipAddressText; 

    void Start()
    {
        string ipAddress = GetLocalIPAddress();
        ipAddressText.text = "IP Address: " + ipAddress;
    }

    private string GetLocalIPAddress()
    {
        string localIP = "Not available";

        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipv in host.AddressList)
            {
                if (ipv.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ipv.ToString();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Unable to get local IP Address: " + ex);
        }

        return localIP;
    }
}
