using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;
using System;
using EasyTransition;
using UnityEngine.U2D;

public class NetworkUnstable : MonoBehaviour
{
    public FirstPersonLook firstPersonLook;
    public FirstPersonMovement firstPersonMovement;
  //  public Animator animatorUI;
    public GameObject NetworkUnstableWindow;
    public float timeRemaining = 91;
    public bool timerIsRunning = false;
    public float startDelay;

    public TransitionSettings transition;

    public TextMeshProUGUI timeText;

    private void Start()
    {
        NetworkUnstableWindow.SetActive(false);
        timerIsRunning = false;
        firstPersonMovement.GetComponent<FirstPersonMovement>().enabled = true;
        firstPersonLook.GetComponent<FirstPersonLook>().enabled = true;
    }
    public void LoadScene(string _sceneName)
    {
        TransitionManager.Instance().Transition(_sceneName, transition, startDelay);
    }

    void FixedUpdate()
    {
        // проверка инета, далее будут добавлены другие типы проверки

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // инета нет, либо он ужасен

            // animatorUI.SetTrigger("UnstableNetwork");
            NetworkUnstableWindow.SetActive(true);
            firstPersonMovement.GetComponent<FirstPersonMovement>().enabled = false;
            firstPersonLook.GetComponent<FirstPersonLook>().enabled = false;
            Debug.Log("Unstable network connecting!");

            if (!timerIsRunning)
            {
                timerIsRunning = true;
                timeRemaining = 91;
            }

            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                }
                else
                {
                    timeRemaining = 0;
                    timerIsRunning = false;
                    Debug.Log("Timer end!");
                    LoadScene("IsMenuScene");
                }
            }

            TimeSpan timeSpan = TimeSpan.FromSeconds(timeRemaining);
            timeText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
        else
        {
            // инет норм

            NetworkUnstableWindow.SetActive(false);
            Debug.Log("The network connection is normal!");
            firstPersonMovement.GetComponent<FirstPersonMovement>().enabled = true;
            firstPersonLook.GetComponent<FirstPersonLook>().enabled = true;
            timerIsRunning = false;
            timeRemaining = 91;
        }
    }
}
