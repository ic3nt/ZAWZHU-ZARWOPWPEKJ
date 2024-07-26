using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using System;

public class UpdateManager : MonoBehaviour
{
    [SerializeField]
    public GameObject UpdateWindow;
    public GameObject DisableWindow;
    void Awake()
    {
        UpdateWindow.SetActive(false);
        DisableWindow.SetActive(true);
        ConfigManager.FetchCompleted += AppyRemoteSettings;
        ConfigManager.FetchConfigs(new usersAttributes(), new appAttributes());
    }

    private void AppyRemoteSettings(ConfigResponse configResponse)
    {
        string newAppVersion = ConfigManager.appConfig.GetString("newAppVersion");

        if(!string.IsNullOrEmpty(newAppVersion) && Application.version != newAppVersion)
        {
            DisableWindow.SetActive(false);
            StartCoroutine(UpdateWindowWaitForSecondCoroutine());
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

    private IEnumerator UpdateWindowWaitForSecondCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        UpdateWindow.SetActive(true);
    }
}
