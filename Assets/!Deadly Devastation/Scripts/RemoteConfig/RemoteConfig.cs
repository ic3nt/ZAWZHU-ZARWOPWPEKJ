using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

public class RemoteConfig : MonoBehaviour
{
    public GameObject UpdateWindow;
    [SerializeField] float UpdateVersion;
    string Update = "newAppVersion";

    public struct userAttribute { };
    public struct appAttribute { };
    void Start()
    {
        FetchConfigData();

    }

    private void FetchConfigData()
    {
        ConfigManager.FetchCompleted += RemoteFetchComplete;
        ConfigManager.FetchConfigs<userAttribute, appAttribute>(new userAttribute(), new appAttribute());

    }

    public void RemoteFetchComplete(ConfigResponse response)
    {
        switch (response.requestOrigin)
        {
            case ConfigOrigin.Default:

                break;
            case ConfigOrigin.Cached:

                break;
            case ConfigOrigin.Remote:

                float CurrentVersion;
                float.TryParse(Application.version, out CurrentVersion);
                if (CurrentVersion < UpdateVersion)
                {
                    break;
                }
                ShowUpdatePouput();

                break;
            default:
                break;
        }

         void ShowUpdatePouput()
        {
            UpdateWindow.SetActive(true);
        }
    }
}
