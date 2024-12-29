using Discord;
using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    public GameObject Kail;
    public GameObject mainButtonsGroup;

    [Header("Game Manager")]
    public DiscordController discordController;
    public LocalizationManager localizationManager;
    public GameObject transitionManager;

    [Header("Transition Settings")]
    public TransitionSettings transition;
    public float startDelay;

    [Header("Animators")]
    public Animator animator;
    public Animator animatorSettings;
    public Animator animatorNoConnectionWarning;
    public Animator animatorMultiplayerUnavailableWarning;
    public Animator animatorYouAreDeveloper;
    public Animator animatorManual;
    public Animator animatorStore;
    public Animator animatorCatalog;

    [Header("Menu States")]
    [Space(10)]
    private bool isMenu;
    private bool isSettings;
    private bool isManual;
    private bool isStore;
    private bool isCatalog;
    [Space(10)]
    private bool isNotSelect;
    private bool isToyRobot;
    private bool isMimic;

    [Header("Errors Window")]
    public GameObject InitErrorWindow;

    [Header("Manual")]
    public GameObject ManualMonsterButton;
    public GameObject ManualMonsterToyRobotWindow;
    public GameObject ManualMonsterMimicWindow;
    public GameObject ManualMonstersObjects;
    public GameObject ToyRobotObject;
    public GameObject MimicObject;

    [Header("Animation States")]
    [Space(10)]
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
        // делаем все чё надо

        Cursor.lockState = CursorLockMode.None;

        isPlayedStoreAnimation = false;
        isPlayedManualAnimation = false;
        isPlayedSettingsAnimation = false;
        isPlayedCatalogAnimation = false;
        isMenu = true;
        isSettings = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        isNotSelect = true;

        // статус в дискорде дефолтный

        if (localizationManager.CurrentLanguage == "en_US")
        {
            discordController.state = "He just sits on the menu and that's it.";
        }
        if (localizationManager.CurrentLanguage == "ru_RU")
        {
            discordController.state = "Просто сидит в меню и все.";
        }
        if (localizationManager.CurrentLanguage == "de_DE")
        {
            discordController.state = "Es steht einfach auf der Speisekarte und das war’s.";
        }
        if (localizationManager.CurrentLanguage == "es_ES")
        {
            discordController.state = "Sólo se sienta en el menú y eso es todo.";
        }

        if (localizationManager.CurrentLanguage == "en_US")
        {
            discordController.details = "Menu";
        }
        if (localizationManager.CurrentLanguage == "ru_RU")
        {
            discordController.details = "Меню";
        }
        if (localizationManager.CurrentLanguage == "de_DE")
        {
            discordController.details = "Speisekarte";
        }
        if (localizationManager.CurrentLanguage == "es_ES")
        {
            discordController.details = "Menú";
        }

    }

    public void Awake()
    {
        InitCheck();
        transitionManager.GetComponent<DemoLoadScene>().transition = transition;
        transitionManager.GetComponent<DemoLoadScene>().startDelay = startDelay;
    }

    //инициализация

    private void InitCheck()
    {
        if (Application.isPlaying)
        {
            if (transitionManager == null)
            {
                GameObject transitionManagerObject = GameObject.FindWithTag("TransitionManager");
                if (transitionManagerObject != null)
                {
                    InitErrorWindow.SetActive(false);
                    transitionManager = transitionManagerObject;
                    Debug.Log("TransitionManager automatically assigned.");
                }
                else
                {
                    mainButtonsGroup.SetActive(false);
                    StartCoroutine(ErrorInitWindowWaitForSecondCoroutine());
                    Debug.LogError("No object with tag 'TransitionManager' found in the scene!");
                }
            }

            if (localizationManager == null)
            {
                GameObject localizationManagerObject = GameObject.FindWithTag("LocalizationManager");
                if (localizationManagerObject != null)
                {
                    InitErrorWindow.SetActive(false);
                    localizationManager = localizationManagerObject.GetComponent<LocalizationManager>();
                    Debug.Log("LocalizationManager automatically assigned.");
                }
                else
                {
                    mainButtonsGroup.SetActive(false);
                    StartCoroutine(ErrorInitWindowWaitForSecondCoroutine());
                    Debug.LogError("No object with tag 'LocalizationManager' found in the scene!");
                }
            }
            if (discordController == null)
            {
                GameObject discordManagerObject = GameObject.FindWithTag("DiscordManager");
                if (discordManagerObject != null)
                {
                    InitErrorWindow.SetActive(false);
                    discordController = discordManagerObject.GetComponent<DiscordController>();
                    Debug.Log("DiscordController automatically assigned.");
                }
                else
                {
                    Debug.LogError("No object with tag 'DiscordManager' found in the scene!");
                }
            }
        }
    }

    private IEnumerator ErrorInitWindowWaitForSecondCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        InitErrorWindow.SetActive(true);
    }

    void Update()
    {
        // справочник

        if (isNotSelect == false)
        {
            ManualMonsterToyRobotWindow.SetActive(false);
            ManualMonsterMimicWindow.SetActive(false);
        }

        // стадия - меню

        if (!isMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MenuButton();
                Debug.Log("Menu");

                if (localizationManager.CurrentLanguage == "en_US")
                {
                    discordController.state = "He just sits on the menu and that's it.";
                }
                if (localizationManager.CurrentLanguage == "ru_RU")
                {
                    discordController.state = "Просто сидит в меню и все.";
                }
                if (localizationManager.CurrentLanguage == "de_DE")
                {
                    discordController.state = "Es steht einfach auf der Speisekarte und das war’s.";
                }
                if (localizationManager.CurrentLanguage == "es_ES")
                {
                    discordController.state = "Sólo se sienta en el menú y eso es todo.";
                }
            }
        }
        else
        {
            isMenu = true;
        }

        // стадия - настройки

        if (isSettings)
        {
            if (!isSettingsOpen && !isSettingsAnimationPlaying)
            {
                SettingsOpen();
                isSettingsOpen = true;
                isSettingsAnimationPlaying = true;
                Debug.Log("Settings");

                if (localizationManager.CurrentLanguage == "en_US")
                {
                    discordController.state = "Sets up Deadly Devastation...";
                }
                if (localizationManager.CurrentLanguage == "ru_RU")
                {
                    discordController.state = "Настраивает Deadly Devastation...";
                }
                if (localizationManager.CurrentLanguage == "es_ES")
                {
                    discordController.state = "Configura Deadly Devastation...";
                }
                if (localizationManager.CurrentLanguage == "de_DE")
                {
                    discordController.state = "Konfiguriert Deadly Devastation...";
                }
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

        // стадия - справочник

        if (isManual)
        {
            if (!isManualOpen && !isPlayedManualAnimation)
            {
                ManualOpen();
                isManualOpen = true;
                isPlayedManualAnimation = true;
                Debug.Log("Manual");

                if (localizationManager.CurrentLanguage == "en_US")
                {
                    discordController.state = "Carefully examines the manual...";
                }
                if (localizationManager.CurrentLanguage == "ru_RU")
                {
                    discordController.state = "Внимательно изучает справочник...";
                }
                if (localizationManager.CurrentLanguage == "es_ES")
                {
                    discordController.state = "Estudiando atentamente el libro de referencia...";
                }
                if (localizationManager.CurrentLanguage == "de_DE")
                {
                    discordController.state = "Das Nachschlagewerk sorgfältig studieren...";
                }
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

        // стадия - магазин

        if (isStore)
        {
            if (!isStoreOpen && !isPlayedStoreAnimation)
            {
                StoreOpen();
                isStoreOpen = true;
                isPlayedStoreAnimation = true;
                Debug.Log("Store");

                if (localizationManager.CurrentLanguage == "en_US")
                {
                    discordController.state = "On a shopping trip...";
                }
                if (localizationManager.CurrentLanguage == "ru_RU")
                {
                    discordController.state = "На шоппинге...";
                }
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

        // стадия - каталог

        if (isCatalog)
        {
            if (!isCatalogOpen && !isPlayedCatalogAnimation)
            {
                CatalogOpen();
                isCatalogOpen = true;
                isPlayedCatalogAnimation = true;
                Debug.Log("Catalog");

                if (localizationManager.CurrentLanguage == "en_US")
                {
                    discordController.state = "Looks at the catalog...";
                }
                if (localizationManager.CurrentLanguage == "ru_RU")
                {
                    discordController.state = "Рассматривает каталог...";
                }
                if (localizationManager.CurrentLanguage == "es_ES")
                {
                    discordController.state = "Mirando el catálogo...";
                }
                if (localizationManager.CurrentLanguage == "de_DE")
                {
                    discordController.state = "Blick in den Katalog...";
                }
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

    // далее идут методы кнопок и.т.д

    public void MenuButton()
    {
        isSettings = false;
        isMenu = true;
        isManual = false;
        isStore = false;
        isCatalog = false;
        isNotSelect = true;
        animator.SetTrigger("DefaultMenu");

        Kail.GetComponent<RotatingModel>().enabled = true;

        if (localizationManager.CurrentLanguage == "en_US")
        {
            discordController.state = "He just sits on the menu and that's it.";
        }
        if (localizationManager.CurrentLanguage == "ru_RU")
        {
            discordController.state = "Просто сидит в меню и все.";
        }
        if (localizationManager.CurrentLanguage == "de_DE")
        {
            discordController.state = "Es steht einfach auf der Speisekarte und das war’s.";
        }
        if (localizationManager.CurrentLanguage == "es_ES")
        {
            discordController.state = "Sólo se sienta en el menú y eso es todo.";
        }


        if (localizationManager.CurrentLanguage == "en_US")
        {
            discordController.details = "Menu";
        }
        if (localizationManager.CurrentLanguage == "ru_RU")
        {
            discordController.details = "Меню";
        }
        if (localizationManager.CurrentLanguage == "de_DE")
        {
            discordController.details = "Speisekarte";
        }
        if (localizationManager.CurrentLanguage == "es_ES")
        {
            discordController.details = "Menú";
        }
    }



    public void PlayButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        isNotSelect = true;
        animator.SetTrigger("PlayMenu");

        if (localizationManager.CurrentLanguage == "en_US")
        {
            discordController.state = "Multiplayer or Single-player? Hmmm.";
        }
        if (localizationManager.CurrentLanguage == "ru_RU")
        {
            discordController.state = "Мультиплеер или Одиночная игра? Хммм.";
        }
        if (localizationManager.CurrentLanguage == "es_ES")
        {
            discordController.state = "¿Multijugador o un jugador? Mmm.";
        }
        if (localizationManager.CurrentLanguage == "de_DE")
        {
            discordController.state = "Mehrspieler oder Einzelspieler? Hmmm.";
        }
    }

    public void QuitButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        isNotSelect = true;
        animator.SetTrigger("QuitMenu");

        if (localizationManager.CurrentLanguage == "en_US")
        {
            discordController.state = "WANTS TO QUIT THE GAME ((((";
        }
        if (localizationManager.CurrentLanguage == "ru_RU")
        {
            discordController.state = "ХОЧЕТ ВЫЙТИ ИЗ ИГРЫ ((((";
        }
        if (localizationManager.CurrentLanguage == "es_ES")
        {
            discordController.state = "QUIERE SALIR DEL JUEGO ((((";
        }
        if (localizationManager.CurrentLanguage == "de_DE")
        {
            discordController.state = "WILL DAS SPIEL VERLASSEN ((((";
        }
    }

    public void CreditsButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        isNotSelect = true;
        animator.SetTrigger("CreditsMenu");

        if (localizationManager.CurrentLanguage == "en_US")
        {
            discordController.state = "Admires the developers ^^";
        }
        if (localizationManager.CurrentLanguage == "ru_RU")
        {
            discordController.state = "Любуется разработчиками ^^";
        }
        if (localizationManager.CurrentLanguage == "es_ES")
        {
            discordController.state = "Amado por los desarrolladores ^^";
        }
        if (localizationManager.CurrentLanguage == "de_DE")
        {
            discordController.state = "Von den Entwicklern geliebt ^^";
        }
    }

    public void SettingsButton()
    {
        isSettings = true;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        isNotSelect = true;
        animator.SetTrigger("SettingsMenu");

        Kail.GetComponent<RotatingModel>().enabled = false;
    }
    public void CatalogButton()
    {
        isSettings = false;
        isMenu = false;
        isManual = false;
        isStore = false;
        isCatalog = true;
        isNotSelect = true;
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
        isNotSelect = true;
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
        isNotSelect = true;
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
        transitionManager.GetComponent<DemoLoadScene>().LoadScene("TEST");
    }
    public void MultiplayerButton()
    {
        Debug.Log("Multiplayer mode");
    }

    // справочник

    public void ManualToyRobot()
    {
        isNotSelect = false;
        isMimic = false;
        isToyRobot = true;
        ManualMonsterMimicWindow.SetActive(false);
        ManualMonsterToyRobotWindow.SetActive(true);
        ManualMonstersObjects.SetActive(true);
        ToyRobotObject.SetActive(true);
        MimicObject.SetActive(false);
    }
    public void ManualMimic()
    {
        isNotSelect = false;
        isToyRobot = false;
        isMimic = true;
        ManualMonsterMimicWindow.SetActive(true);
        ManualMonsterToyRobotWindow.SetActive(false);
        ManualMonstersObjects.SetActive(true);
        ToyRobotObject.SetActive(false);
        MimicObject.SetActive(true);
    }
    public void ManualInfoButton()
    {
        if (isMimic == true) 
        {
            ManualMonsterMimicWindow.SetActive(true);
            Debug.Log("Mimic Info");
        }
        if (isMimic == true)
        {
            ManualMonsterMimicWindow.SetActive(true);
            Debug.Log("Mimic Info");
        }
    }
}
