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

    // ��� ��� ������ ���� ����� �������� � remote config, ������ ��� �������� ����������

    void Awake()
    {
        // ��������� ��������� ������� �� ����� � ��������� remote config

        updateWindow.SetActive(false);
        ConfigManager.FetchCompleted += AppyRemoteSettings;
        ConfigManager.FetchConfigs(new usersAttributes(), new appAttributes());
    }

    private void AppyRemoteSettings(ConfigResponse configResponse)
    {
        // ����������� newAppVersion ������� newAppVersion � remote config

        string newAppVersion = ConfigManager.appConfig.GetString("newAppVersion");

        // ���� newAppVersion, �� �� ������� �����, �� (��� ������ �������� �������� � �������)

        if (!string.IsNullOrEmpty(newAppVersion) && Application.version != newAppVersion)
        {
            mainButtonsGroup.SetActive(false);
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
        mainButtonsGroup.SetActive(true);
    }
}
