using EasyTransition;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField]
    public float waitTime = 2f; // Время ожидания перед загрузкой сцены

    public InitGameManager initGameManager;
    public SaveManager saveManager;

    private GameData.Data Data;

    void Start()
    {
        if (saveManager == null)
        {
            Debug.LogError("SaveManager is not assigned!");
            return;
        }

        Data = saveManager.Load();

        if (Data == null)
        {
            Data = new GameData.Data
            {
                isFirstRun = true,
                isPlayerAgreedPlay = false
            };
            saveManager.Save(Data);
        }

        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);

        if (Data.isFirstRun)
        {
            HandleFirstRun();
        }
        else if (!Data.isPlayerAgreedPlay)
        {
            HandlePlayerNotAgreed();
        }
        else
        {
            LoadMainMenu();
        }
    }

    private void HandleFirstRun()
    {
        Debug.Log("First run detected.");
        initGameManager.IsFirstGameRun = true;

        // Обновляем данные и сохраняем их
        Data.isFirstRun = false;
        saveManager.Save(Data);

        SceneManager.LoadScene("IsFirstGameOpenScene");
    }

    private void HandlePlayerNotAgreed()
    {
        Debug.Log("Player has not agreed to play.");
        initGameManager.isPlayerAgreedPlay = false;

        SceneManager.LoadScene("IsFirstGameOpenScene");
    }

    private void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu...");

        if (initGameManager.transitionManager != null)
        {
            initGameManager.IsFirstGameRun = false;
            initGameManager.isPlayerAgreedPlay = true;


            initGameManager.transitionManager.GetComponent<DemoLoadScene>().LoadScene("IsMenuScene");
        }
        else
        {
            Debug.LogError("TransitionManager is not assigned!");
        }
    }
}
