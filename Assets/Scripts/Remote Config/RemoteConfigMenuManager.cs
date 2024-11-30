using UnityEngine;
using Unity.Services.RemoteConfig;
using System.Threading.Tasks;

public class RemoteConfigMenuManager : MonoBehaviour
{
    public struct UserAttributes { }
    public struct AppAttributes { }

    private bool multiplayerAvailable;

    public GameObject[] multiplayerGameObjects;

    // нигде не используется, пока что, потому что скрипт не рабочий 

    async void Start()
    {
        await InitializeRemoteConfig();

        foreach (var gameObject in multiplayerGameObjects)
        {
            gameObject.SetActive(multiplayerAvailable);
        }
    }

    private async Task InitializeRemoteConfig()
    {
        ConfigManager.FetchCompleted += OnFetchCompleted;
        await ConfigManager.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
    }

    private void OnFetchCompleted(ConfigResponse response)
    {
        if (response.status == ConfigRequestStatus.Success)
        {
            multiplayerAvailable = ConfigManager.appConfig.GetBool("multiplayerAvailable", false);
        }
        else
        {
            Debug.LogError("Failed to fetch remote config.");
        }
    }
}
