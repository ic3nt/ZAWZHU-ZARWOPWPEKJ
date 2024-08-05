using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MenuManagerNew : MonoBehaviour
{
    public Animator animator;
    public Animator animatorSettings;
    public Animator animatorNoConnectionWarning;
    public Animator animatorMultiplayerUnavailableWarning;
    public Animator animatorYouAreDeveloper;
    public Animator animatorManual;
    public Animator animatorStore;
    public Animator animatorCatalog;

    private bool isMenu;
    private bool isSettings;
    private bool isManual;
    private bool isStore;
    private bool isCatalog;

    public GameObject ManualMonstersObjects;
    public GameObject ToyRobotObject;
    public GameObject MimicObject;

    private bool isPlayedSettingsAnimation;
    private bool isSettingsOpen = false;
    private bool isSettingsAnimationPlaying = false;

    private bool isPlayedManualAnimation;
    private bool isManualOpen = false;
    private bool isManualAnimationPlaying = false;

    private bool isPlayedStoreAnimation;
    private bool isStoreOpen = false;
    private bool isStoreAnimationPlaying = false;

    private bool isPlayedCatalogAnimation;
    private bool isCatalogOpen = false;
    private bool isCatalogAnimationPlaying = false;

    void Start()
    {
        isPlayedStoreAnimation = false;
        isPlayedManualAnimation = false;
        isPlayedSettingsAnimation = false;
        isPlayedCatalogAnimation = false;
        isMenu = true;
        isSettings = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
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
            if (!animatorManual.GetCurrentAnimatorStateInfo(0).IsName("OpenManual") &&
                !animatorManual.GetCurrentAnimatorStateInfo(0).IsName("CloseManual"))
            {
                isPlayedManualAnimation = false;
            }
        }

        if (isStore)
        {
            if (!isStoreOpen && !isPlayedStoreAnimation)
            {
                StoreOpen();
                isStoreOpen = true;
                isPlayedStoreAnimation = true;
                Debug.Log("Store");
            }
        }
        else
        {
            if (isStore && !isPlayedStoreAnimation)
            {
                StoreClose();
                isStoreOpen = false;
                isPlayedStoreAnimation = true;
            }
        }

        if (isPlayedStoreAnimation)
        {
            if (!animatorStore.GetCurrentAnimatorStateInfo(0).IsName("OpenStore") &&
                !animatorStore.GetCurrentAnimatorStateInfo(0).IsName("CloseStore"))
            {
                isPlayedStoreAnimation = false;
            }
        }

        if (isCatalog)
        {
            if (!isCatalogOpen && !isPlayedCatalogAnimation)
            {
                CatalogOpen();
                isCatalogOpen = true;
                isPlayedCatalogAnimation = true;
                Debug.Log("Catalog");
            }
        }
        else
        {
            if (isCatalog && !isPlayedCatalogAnimation)
            {
                CatalogDefault();
                isCatalogOpen = false;
                isPlayedCatalogAnimation = true;
            }
        }

        if (isPlayedCatalogAnimation)
        {
            if (!animatorCatalog.GetCurrentAnimatorStateInfo(0).IsName("Open") &&
                !animatorCatalog.GetCurrentAnimatorStateInfo(0).IsName("Default"))
            {
                isPlayedCatalogAnimation = false;
            }
        }
    }

    public void MenuButton()
    {
        isSettings = false;
        isMenu = true;
        isManual = false;
        isStore = false;
        isCatalog = false;
        animator.SetTrigger("DefaultMenu");
    }

    public void PlayButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        animator.SetTrigger("PlayMenu");
    }

    public void QuitButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        animator.SetTrigger("QuitMenu");
    }

    public void CreditsButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        animator.SetTrigger("CreditsMenu");
    }

    public void SettingsButton()
    {
        isSettings = true;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        animator.SetTrigger("SettingsMenu");
    }
    public void CatalogButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = true;
        animator.SetTrigger("CatalogMenu");
        animatorManual.SetTrigger("CloseManual"); 
        animatorStore.SetTrigger("CloseStore");
        ManualMonstersObjects.SetActive(false);
    }
    public void ManualButton()
    {
        isManual = true;
        isStore = false;
        isSettings = false;
        isMenu = false;
        isCatalog = false;
        animator.SetTrigger("ManualMenu");
        animatorManual.SetTrigger("OpenManual");
        ManualMonstersObjects.SetActive(true);
    }
    public void StoreButton()
    {
        isManual = false;
        isStore = true;
        isSettings = false;
        isMenu = false;
        isCatalog = false;
        animator.SetTrigger("StoreMenu");
        animatorStore.SetTrigger("OpenStore");
    }
    void SettingsOpen()
    {
        animatorSettings.SetTrigger("Open");
        animatorNoConnectionWarning.SetTrigger("Close");
        animatorMultiplayerUnavailableWarning.SetTrigger("Close");
        animatorYouAreDeveloper.SetTrigger("Close");
    }
    void SettingsClose()
    {
        animatorSettings.SetTrigger("Close");
        animatorNoConnectionWarning.SetTrigger("Open");
        animatorMultiplayerUnavailableWarning.SetTrigger("Open");
        animatorYouAreDeveloper.SetTrigger("Open");
    }
    void ManualOpen()
    {
        animatorManual.SetTrigger("OpenManual");
        ManualMonstersObjects.SetActive(true);
    }
    void ManualClose()
    {
        animatorManual.SetTrigger("CloseManual");
        ManualMonstersObjects.SetActive(false);
    }
    void StoreOpen()
    {
        animatorManual.SetTrigger("OpenStore");
    }
    void StoreClose()
    {
        animatorStore.SetTrigger("CloseStore");
    }
    void CatalogOpen()
    {
        animatorCatalog.SetTrigger("Open");
        ManualMonstersObjects.SetActive(false);
    }
    void CatalogDefault()
    {
        animatorCatalog.SetTrigger("Default");
    }
    public void ApplicationQuit()
    {
        Application.Quit();
        Debug.Log("Left the game (((");
    }
    public void SingleplayerButton()
    {
        Debug.Log("Single-player mode");
    }
    public void MultiplayerButton()
    {
        Debug.Log("Multiplayer mode");
    }
    public void ManualToyRobot()
    {
        ManualMonstersObjects.SetActive(true);
        ToyRobotObject.SetActive(true);
        MimicObject.SetActive(false);
    }
    public void ManualMimic()
    {
        ManualMonstersObjects.SetActive(true);
        ToyRobotObject.SetActive(false);
        MimicObject.SetActive(true);
    }
}
