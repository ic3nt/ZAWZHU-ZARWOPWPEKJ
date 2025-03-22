using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PingDisplay : MonoBehaviour
{
    private float pingTime;
    private bool isPinging;

    // скрипт для отоброжения пинга

    void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            StartCoroutine(PingCoroutine());
        }
    }

    private IEnumerator PingCoroutine()
    {
        while (true)
        {
            if (!isPinging)
            {
                isPinging = true;
                pingTime = Time.time;

                PingServerClientRpc();

                yield return new WaitForSeconds(1f);
                isPinging = false;
            }

            yield return null;
        }
    }

    [ClientRpc]
    public void PingServerClientRpc()
    {
        CalculatePing();
    }

    private void CalculatePing()
    {
        float roundTripTime = Time.time - pingTime;
        Debug.Log($"Current ping: {roundTripTime * 1000} ms");
    }
}
