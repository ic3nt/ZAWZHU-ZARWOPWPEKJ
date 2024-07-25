using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ConnectManager : MonoBehaviour
{
    public GameObject NoConnectionWarningWindows;

    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NoConnectionWarningWindows.SetActive(true);
            Debug.Log("Offline");
        }
        else
        {
            NoConnectionWarningWindows.SetActive(false);
            Debug.Log("Online");
        }
    }
}
