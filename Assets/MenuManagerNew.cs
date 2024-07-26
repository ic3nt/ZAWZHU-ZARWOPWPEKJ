using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MenuManagerNew : MonoBehaviour
{
    private bool isPlayedSettingsAnimation;
    private bool isMenu;
    private bool isSettings;
    public Animator animator;
    public Animator animatorSettings;
    public Animator animatorNoConnectionWarning;

    private bool isSettingsOpen = false;
    private bool isSettingsAnimationPlaying = false;

    void Start()
    {
        isPlayedSettingsAnimation = false;
        isMenu = true;
        isSettings = false;
    }

    void Update()
    {
        if (!isMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MenuButton();
                Debug.Log("Menu");
            }
        }
        else
        {
            isMenu = true;
        }

        if (isSettings)
        {
            if (!isSettingsOpen && !isSettingsAnimationPlaying)
            {
                SettingsOpen();
                isSettingsOpen = true;
                isSettingsAnimationPlaying = true;
                Debug.Log("Settings");
            }
        }
        else
        {
            if (isSettingsOpen && !isSettingsAnimationPlaying)
            {
                SettingsClose();
                isSettingsOpen = false;
                isSettingsAnimationPlaying = true;
            }
        }

        if (isSettingsAnimationPlaying)
        {
            if (!animatorSettings.GetCurrentAnimatorStateInfo(0).IsName("Open") &&
                !animatorSettings.GetCurrentAnimatorStateInfo(0).IsName("Close"))
            {
                isSettingsAnimationPlaying = false;
            }
        }
    }

    public void MenuButton()
    {
        isSettings = false;
        isMenu = true;
        animator.SetTrigger("DefaultMenu");
    }

    public void PlayButton()
    {
        isSettings = false;
        isMenu = false;
        animator.SetTrigger("PlayMenu");
    }

    public void QuitButton()
    {
        isSettings = false;
        isMenu = false;
        animator.SetTrigger("QuitMenu");
    }

    public void SettingsButton()
    {
        isSettings = true;
        isMenu = false;
        animator.SetTrigger("SettingsMenu");
    }
    public void CatalogButton()
    {
        isSettings = false;
        isMenu = false;
        animator.SetTrigger("CatalogMenu");
    }
    void SettingsOpen()
    {
        animatorSettings.SetTrigger("Open");
        animatorNoConnectionWarning.SetTrigger("Open");
    }
    void SettingsClose()
    {
        animatorSettings.SetTrigger("Close");
        animatorNoConnectionWarning.SetTrigger("Close");
    }
}
