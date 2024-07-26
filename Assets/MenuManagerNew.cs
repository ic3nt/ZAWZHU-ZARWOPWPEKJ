using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MenuManagerNew : MonoBehaviour
{
    public Animator animator;
    public Animator animatorSettings;
    public Animator animatorNoConnectionWarning;
    public Animator animatorManual;

    private bool isMenu;
    private bool isSettings;
    private bool isManual;

    private bool isPlayedSettingsAnimation;
    private bool isSettingsOpen = false;
    private bool isSettingsAnimationPlaying = false;

    private bool isPlayedManualAnimation;
    private bool isManualOpen = false;
    private bool isSettingsManualPlaying = false;

    void Start()
    {
        isPlayedManualAnimation = false;
        isPlayedSettingsAnimation = false;
        isMenu = true;
        isSettings = false;
        isManual = false;
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

        if (isManual)
        {
            if (!isManualOpen && !isPlayedManualAnimation)
            {
                ManualOpen();
                isManualOpen = true;
                isPlayedManualAnimation = true;
                Debug.Log("Manual");
            }
        }
        else
        {
            if (isManual && !isPlayedManualAnimation)
            {
                ManualClose();
                isManualOpen = false;
                isPlayedManualAnimation = true;
            }
        }

        if (isPlayedManualAnimation)
        {
            if (!animatorManual.GetCurrentAnimatorStateInfo(0).IsName("Open") &&
                !animatorManual.GetCurrentAnimatorStateInfo(0).IsName("Close"))
            {
                isPlayedManualAnimation = false;
            }
        }
    }

    public void MenuButton()
    {
        isSettings = false;
        isMenu = true;
        isManual = false;
        animator.SetTrigger("DefaultMenu");
    }

    public void PlayButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        animator.SetTrigger("PlayMenu");
    }

    public void QuitButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        animator.SetTrigger("QuitMenu");
    }

    public void SettingsButton()
    {
        isSettings = true;
        isMenu = false;
        isManual = false;
        animator.SetTrigger("SettingsMenu");
    }
    public void CatalogButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        animator.SetTrigger("CatalogMenu");
        animatorManual.SetTrigger("Close");
    }
    public void ManualButton()
    {
        isManual = true;
        isSettings = false;
        isMenu = false;
        animator.SetTrigger("ManualMenu");
        animatorManual.SetTrigger("Open");
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
    void ManualOpen()
    {
        animatorManual.SetTrigger("Open");
        animatorNoConnectionWarning.SetTrigger("Open");
    }
    void ManualClose()
    {
        animatorManual.SetTrigger("Close");
        animatorNoConnectionWarning.SetTrigger("Close");
    }
}
