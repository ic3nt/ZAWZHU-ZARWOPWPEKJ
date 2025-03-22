using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{
    public TMP_Text timerText;
    private float timeElapsed;
    private bool isTimerRunning = true;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            timeElapsed = 0f;
            UpdateTimerTextClientRpc(timeElapsed);
        }
    }

    void Update()
    {
        if (IsServer && isTimerRunning)
        {
            timeElapsed += Time.deltaTime;
            UpdateTimerTextClientRpc(timeElapsed);
        }
    }

    [ClientRpc]
    void UpdateTimerTextClientRpc(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        if (IsServer)
        {
            isTimerRunning = false;
        }
    }

    public void StartTimer()
    {
        if (IsServer)
        {
            isTimerRunning = true;
            timeElapsed = 0f;
        }
    }
}
