using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ConnectManager : MonoBehaviour
{
    public GameObject NoConnectionWarningWindows;
    public BoxCollider BoxCollider;
    public GameObject Objects;

    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NoConnectionWarningWindows.SetActive(true);
            BoxCollider.enabled = false;
            Objects.SetActive(true);
            Debug.Log("Offline");
        }
        else
        {
            NoConnectionWarningWindows.SetActive(false);
            BoxCollider.enabled = true;
            Objects.SetActive(false);
            Debug.Log("Online");
        }
    }
}
