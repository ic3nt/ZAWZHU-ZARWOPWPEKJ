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

    // тут все просто если уметь работать с remote config, скрипт для проверки обновлений

    void Awake()
    {
        // отключаем некоторые объекты на сцене и проверяем remote config

        UpdateWindow.SetActive(false);
        DisableWindow.SetActive(true);
        ConfigManager.FetchCompleted += AppyRemoteSettings;
        ConfigManager.FetchConfigs(new usersAttributes(), new appAttributes());
    }

    private void AppyRemoteSettings(ConfigResponse configResponse)
    {
        // присваиваем newAppVersion стрингу newAppVersion в remote config

        string newAppVersion = ConfigManager.appConfig.GetString("newAppVersion");

        // если newAppVersion, не та которая нужна, то (вся логика действий написана и понятна)

        if (!string.IsNullOrEmpty(newAppVersion) && Application.version != newAppVersion)
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
