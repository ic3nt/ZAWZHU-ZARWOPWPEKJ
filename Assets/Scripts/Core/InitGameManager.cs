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
    public LocalizationManager localizationManager;

    [SerializeField]
    public GameObject transitionManager;

    private void Awake()
    {
        InitializeSystems();
    }

    private void InitializeSystems()
    {
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
    }

}
