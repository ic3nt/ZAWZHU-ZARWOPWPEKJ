using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitGameManager : MonoBehaviour
{
    [Header("Initialization Settings")]
    [Space]
    [SerializeField]
    public bool IsFirstGameRun = true;

    [SerializeField]
    public bool isPlayerAgreedPlay = false;

    [Header("Initialization Managers")]
    [Space]
    [SerializeField]
    public GameObject gameManager;

    [SerializeField]
    public LocalizationManager localizationManager;

    [SerializeField]
    public GameObject transitionManager;

    [SerializeField]
    public DiscordController discordController;

    private void Start()
    {
        if (localizationManager.CurrentLanguage == "en_US")
        {
            discordController.details = "Initialization...";
        }
        if (localizationManager.CurrentLanguage == "ru_RU")
        {
            discordController.details = "Инициализация...";
        }
        if (localizationManager.CurrentLanguage == "de_DE")
        {
            discordController.details = "Initialization...";
        }
        if (localizationManager.CurrentLanguage == "es_ES")
        {
            discordController.details = "Initialization...";
        }
    }

    private void Awake()
    {
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        if (gameManager != null)
        {
            DontDestroyOnLoad(gameManager.gameObject);
            Debug.Log($"GameManager '{gameManager.name}' is now persistent.");
        }
        else
        {
            Debug.LogWarning("GameManager is not assigned in InitGameManager.");
        }

        if (localizationManager != null)
        {
            DontDestroyOnLoad(localizationManager.gameObject);
            Debug.Log($"LocalizationManager '{localizationManager.name}' is now persistent.");
        }
        else
        {
            Debug.LogWarning("LocalizationManager is not assigned in InitGameManager.");
        }

        if (transitionManager != null)
        {
            DontDestroyOnLoad(transitionManager);
            Debug.Log($"TransitionManager '{transitionManager.name}' is now persistent.");
        }
        else
        {
            Debug.LogWarning("TransitionManager is not assigned in InitGameManager.");
        }

        if (discordController != null)
        {
            DontDestroyOnLoad(discordController);
            Debug.Log($"DiscordController '{discordController.name}' is now persistent.");
        }
        else
        {
            Debug.LogWarning("DiscordController is not assigned in InitGameManager.");
        }
    }

}
