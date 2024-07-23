using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using System;

public class UpdateManager : MonoBehaviour
{
    [SerializeField]
    public GameObject UpdateWindow;
    void Awake()
    {
        UpdateWindow.SetActive(false);
        ConfigManager.FetchCompleted += AppyRemoteSettings;
        ConfigManager.FetchConfigs(new usersAttributes(), new appAttributes());
    }

    private void AppyRemoteSettings(ConfigResponse configResponse)
    {
        string newAppVersion = ConfigManager.appConfig.GetString("newAppVersion");

        if(!string.IsNullOrEmpty(newAppVersion) && Application.version != newAppVersion)
        {
            UpdateWindow.SetActive(true);
        }

#if DEBUG
        print("Version : " + Application.version + " - " + "remote version : " + newAppVersion);
#endif 
    }

    void OnDestroy()
    {
        ConfigManager.FetchCompleted -= AppyRemoteSettings;
    }

    struct usersAttributes { }

    struct appAttributes { }
}
