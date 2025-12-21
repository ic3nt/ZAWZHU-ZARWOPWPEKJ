using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.Services.Core;
using System.Collections.Generic;
using System;

public class RemoteConfigManager : MonoBehaviour
{
    public GameObject UpdateWindow;

    const string VERSION_KEY = "newAppVersion";

    public struct UserAttributes { }
    public struct AppAttributes { }

    async void Awake()
    {
        await UnityServices.InitializeAsync();

        RemoteConfigService.Instance.FetchCompleted += OnFetchCompleted;
        RemoteConfigService.Instance.FetchConfigs(
            new UserAttributes(),
            new AppAttributes()
        );
    }

    void OnFetchCompleted(ConfigResponse response)
    {
        if (response.requestOrigin == ConfigOrigin.Remote ||
            response.requestOrigin == ConfigOrigin.Cached)
        {
            float remoteVersion =
                RemoteConfigService.Instance.appConfig.GetFloat(VERSION_KEY, 0f);

            float currentVersion;
            float.TryParse(Application.version, out currentVersion);

            if (currentVersion < remoteVersion)
            {
                UpdateWindow.SetActive(true);
            }
        }
    }

    void OnDestroy()
    {
        if (RemoteConfigService.Instance != null)
            RemoteConfigService.Instance.FetchCompleted -= OnFetchCompleted;
    }
}
