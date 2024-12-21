using EasyTransition;
using System.Collections;
using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField]
    public float waitTime;
    public InitGameManager initGameManager;

    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);

        if (PlayerPrefs.GetInt("isFirstRun", 0) == 0)
        {
            initGameManager.IsFirstGameRun = true;
            PlayerPrefs.SetInt("isFirstRun", 1);
            SceneManager.LoadScene("IsFirstGameOpenScene");
        }
        else if (PlayerPrefs.GetInt("isPlayerAgreedPlay", 0) == 0)
        {
            initGameManager.isPlayerAgreedPlay = false;
            SceneManager.LoadScene("IsFirstGameOpenScene");
        }
        else
        {
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
}
