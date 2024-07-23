using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject Camera;
    public GameObject SettingsWindow;
    public GameObject SettingsWindowInteractive;
    public GameObject PauseWindow;
    public GameObject PauseWindowInteractive;
    public GameObject TextTimer;
    public GameObject HealthWindow;
    public bool IsPauseActive;
    public DemoLoadScene loadScene;

    private void Start()
    {
        PauseWindow.SetActive(false);
        SettingsWindow.SetActive(false);
        SettingsWindowInteractive.SetActive(false);
        PauseWindowInteractive.SetActive(false);
        HealthWindow.SetActive(true);
        TextTimer.SetActive(true);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (IsPauseActive == false)
            {
                Pause();
            }

            else
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        Camera.GetComponent<FirstPersonLook>().enabled = false;
        PauseWindow.SetActive(true);
        PauseWindowInteractive.SetActive(true);
        HealthWindow.SetActive(false);
        TextTimer.SetActive(false);
        IsPauseActive = true;
        this.GetComponent<MouseLook>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        Camera.GetComponent<FirstPersonLook>().enabled = true;
        PauseWindow.SetActive(false);
        PauseWindowInteractive.SetActive(false);
        SettingsWindow.SetActive(false);
        SettingsWindowInteractive.SetActive(false);
        HealthWindow.SetActive(true);
        TextTimer.SetActive(true);
        IsPauseActive = false;
        this.GetComponent<MouseLook>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SettingsButton()
    {
        SettingsWindow.SetActive(true);
        SettingsWindowInteractive.SetActive(true);
    }
    public void SettingsExitButton()
    {
        SettingsWindow.SetActive(false);
        SettingsWindowInteractive.SetActive(false);
    }
    public void ResumeButton()
    {
        Resume();
    }
    public void MenuButton()
    {
        Resume();
        loadScene.LoadScene("IsMenuScene");
    }
}
