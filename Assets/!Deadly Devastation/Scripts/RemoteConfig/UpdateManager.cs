using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.Services.Core;
using System;

public class UpdateManager : MonoBehaviour
{
    public MenuManager menuManager;
    public GameObject updateWindow;
    public GameObject mainButtonsGroup;

    const string VERSION_KEY = "newAppVersion";

    public struct UserAttributes { }
    public struct AppAttributes { }

    async void Awake()
    {
        updateWindow.SetActive(false);

        await UnityServices.InitializeAsync();

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        RemoteConfigService.Instance.FetchConfigs(
            new UserAttributes(),
            new AppAttributes()
        );
    }

    void ApplyRemoteSettings(ConfigResponse response)
    {
        if (response.requestOrigin != ConfigOrigin.Remote &&
            response.requestOrigin != ConfigOrigin.Cached)
            return;

        string remoteVersion =
            RemoteConfigService.Instance.appConfig.GetString(VERSION_KEY, "");

        if (!string.IsNullOrEmpty(remoteVersion) &&
            Application.version != remoteVersion)
        {
            mainButtonsGroup.SetActive(false);
            updateWindow.SetActive(true);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"Game client version: {Application.version} | Remote version: {remoteVersion}");
#endif
    }

    void OnDestroy()
    {
        if (RemoteConfigService.Instance != null)
            RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteSettings;
    }
}
