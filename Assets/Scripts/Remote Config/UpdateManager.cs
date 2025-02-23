using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using System;

public class UpdateManager : MonoBehaviour
{
    [SerializeField]
    public GameObject updateWindow;
    public GameObject mainButtonsGroup;

    // тут все просто если уметь работать с remote config, скрипт для проверки обновлений

    void Awake()
    {
        // отключаем некоторые объекты на сцене и проверяем remote config
        updateWindow.SetActive(false);
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
            mainButtonsGroup.SetActive(false);
            updateWindow.SetActive(true);
        }
        else
        {
            mainButtonsGroup.SetActive(true);
        }

#if DEBUG
        print("Game client version : " + Application.version + " - " + "Remote version : " + newAppVersion);
#endif 
    }

    void OnDestroy()
    {
        ConfigManager.FetchCompleted -= AppyRemoteSettings;
    }

    struct usersAttributes { }

    struct appAttributes { }
}
