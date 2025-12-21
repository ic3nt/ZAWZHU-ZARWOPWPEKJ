using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.Services.Core;
using System.Threading.Tasks;

public class RemoteConfigMenuManager : MonoBehaviour
{
    public struct UserAttributes { }
    public struct AppAttributes { }

    [SerializeField] private GameObject[] multiplayerGameObjects;

    private bool multiplayerAvailable = false;

    async void Start()
    {
        await InitializeRemoteConfig();

        foreach (var go in multiplayerGameObjects)
        {
            if (go != null)
                go.SetActive(multiplayerAvailable);
        }
    }

    private async Task InitializeRemoteConfig()
    {
        await UnityServices.InitializeAsync();

        RemoteConfigService.Instance.FetchCompleted += OnFetchCompleted;

        RemoteConfigService.Instance.FetchConfigs(
            new UserAttributes(),
            new AppAttributes()
        );
    }

    private void OnFetchCompleted(ConfigResponse response)
    {
        if (response.requestOrigin == ConfigOrigin.Remote ||
            response.requestOrigin == ConfigOrigin.Cached)
        {
            multiplayerAvailable =
                RemoteConfigService.Instance.appConfig.GetBool(
                    "multiplayerAvailable",
                    false
                );
        }
        else
        {
            Debug.LogError("Remote Config fetch failed");
        }
    }

    private void OnDestroy()
    {
        if (RemoteConfigService.Instance != null)
            RemoteConfigService.Instance.FetchCompleted -= OnFetchCompleted;
    }
}
