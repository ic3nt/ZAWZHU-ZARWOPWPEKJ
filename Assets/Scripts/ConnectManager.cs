using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ConnectManager : MonoBehaviour
{
    public GameObject NoConnectionWarningWindows;
    public Button button;
    public GameObject Objects;

    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NoConnectionWarningWindows.SetActive(true);
            button.gameObject.SetActive(false);
            Objects.SetActive(true);
            Debug.Log("Offline");
        }
        else
        {
            NoConnectionWarningWindows.SetActive(false);
            button.gameObject.SetActive(true);
            Objects.SetActive(false);
            Debug.Log("Online");
        }
    }
}
