using DG.Tweening;
using Discord;
using EasyTransition;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ManualEntry
{
    public string Name;
    public string DescriptionLocalizationKey;
    public string Type;
    public Transform Target;
    public Vector3 onScreenPosition;
    public Vector3 offScreenPosition;
    public Vector3 onScreenRotation;
    public Vector3 offScreenRotation;
}

public class MenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    public GameObject Kail;
    public GameObject mainButtonsGroup;
    public GameObject world;
    public GameObject manualObjects;
    public GameObject mainCamera;

    [Header("Game Manager")]
    public DiscordController discordController;
    public LocalizationManager localizationManager;
    public GameObject transitionManager;

    [HideInInspector] public bool initSuccessful;

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

    public List<ManualEntry> manualEntries = new List<ManualEntry>();

    private ManualEntry selectedEntry = null;

    public TextMeshProUGUI currentSelectionText;
    public TextMeshProUGUI currentSelectionDescription;
    public RectTransform manualLabel;
    public RectTransform backgroundManual;
    public RectTransform mainWindow;
    public RectTransform infoWindow;

    private Transform target3D;

    private Vector3 onScreenPositionTarget;
    private Vector3 offScreenPositionTarget;
    private Vector3 onScreenRotationTarget;
    private Vector3 offScreenRotationTarget;

    public float shakeStrength = 10f;
    public int shakeVibrato = 10;
    public float shakeDuration = 0.3f;

    [Space(10)]

    public float moveDuration = 1f;
    public float rotationDuration = 1f;

    public float shakeStrength3D = 10f;
    public int shakeVibrato3D = 10;
    public float shakeDuration3D = 0.3f;

    [Header("Animation States")]

    private bool isPlayedSettingsAnimation;
    private bool isSettingsOpen = false;
    private bool isSettingsAnimationPlaying = false;
    private bool isManualOpen = false;
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
        isPlayedSettingsAnimation = false;
        isPlayedCatalogAnimation = false;
        isMenu = true;
        isSettings = false;
        isManual = false;
        isStore = false;
        isCatalog = false;
        isNotSelect = true;

        // статус в дискорде дефолтный

        switch (localizationManager.CurrentLanguage)
        {
            case "en_US":
                discordController.state = "He just sits on the menu and that's it.";
                discordController.details = "Menu";
                break;
            case "ru_RU":
                discordController.state = "Просто сидит в меню и все.";
                discordController.details = "Меню";
                break;
            case "de_DE":
                discordController.state = "Es steht einfach auf der Speisekarte und das war’s.";
                discordController.details = "Speisekarte";
                break;
            case "es_ES":
                discordController.state = "Sólo se sienta en el menú y eso es todo.";
                discordController.details = "Menú";
                break;
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
                    initSuccessful = true;
                }
                else
                {
                    mainButtonsGroup.SetActive(false);
                    StartCoroutine(ErrorInitWindowWaitForSecondCoroutine());
                    Debug.LogError("No object with tag 'TransitionManager' found in the scene!");
                    initSuccessful = false;
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
                    initSuccessful = true;
                }
                else
                {
                    mainButtonsGroup.SetActive(false);
                    StartCoroutine(ErrorInitWindowWaitForSecondCoroutine());
                    Debug.LogError("No object with tag 'LocalizationManager' found in the scene!");
                    initSuccessful = false;
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
                    initSuccessful = true;
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
        // стадия - меню

        if (!isMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MenuButton();
                Debug.Log("Menu");

                switch (localizationManager.CurrentLanguage)
                {
                    case "en_US":
                        discordController.state = "He just sits on the menu and that's it.";
                        discordController.details = "Menu";
                        break;
                    case "ru_RU":
                        discordController.state = "Просто сидит в меню и все.";
                        discordController.details = "Меню";
                        break;
                    case "de_DE":
                        discordController.state = "Es steht einfach auf der Speisekarte und das war’s.";
                        discordController.details = "Speisekarte";
                        break;
                    case "es_ES":
                        discordController.state = "Sólo se sienta en el menú y eso es todo.";
                        discordController.details = "Menú";
                        break;
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

                switch (localizationManager.CurrentLanguage)
                {
                    case "en_US":
                        discordController.state = "Sets up Deadly Devastation...";
                        break;
                    case "ru_RU":
                        discordController.state = "Настраивает Deadly Devastation...";
                        break;
                    case "es_ES":
                        discordController.state = "Configura Deadly Devastation...";
                        break;
                    case "de_DE":
                        discordController.state = "Konfiguriert Deadly Devastation...";
                        break;
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
            if (!isManualOpen)
            {
                ManualOpen();
                isManualOpen = true;
                Debug.Log("Manual");

                switch (localizationManager.CurrentLanguage)
                {
                    case "en_US":
                        discordController.state = "Carefully examines the manual...";
                        break;
                    case "ru_RU":
                        discordController.state = "Внимательно изучает справочник...";
                        break;
                    case "es_ES":
                        discordController.state = "Estudiando atentamente el libro de referencia...";
                        break;
                    case "de_DE":
                        discordController.state = "Das Nachschlagewerk sorgfältig studieren...";
                        break;
                }
            }
        }
        else
        {
                ManualClose();
                isManualOpen = false;
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

                switch (localizationManager.CurrentLanguage)
                {
                    case "en_US":
                        discordController.state = "On a shopping trip...";
                        break;
                    case "ru_RU":
                        discordController.state = "На шоппинге...";
                        break;
                    case "es_ES":
                        discordController.state = "On a shopping trip...";
                        break;
                    case "de_DE":
                        discordController.state = "On a shopping trip...";
                        break;
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

                switch (localizationManager.CurrentLanguage)
                {
                    case "en_US":
                        discordController.state = "Looks at the catalog...";
                        break;
                    case "ru_RU":
                        discordController.state = "Рассматривает каталог...";
                        break;
                    case "es_ES":
                        discordController.state = "Mirando el catálogo...";
                        break;
                    case "de_DE":
                        discordController.state = "Blick in den Katalog...";
                        break;
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

        switch (localizationManager.CurrentLanguage)
        {
            case "en_US":
                discordController.state = "He just sits on the menu and that's it.";
                discordController.details = "Menu";
                break;
            case "ru_RU":
                discordController.state = "Просто сидит в меню и все.";
                discordController.details = "Меню";
                break;
            case "de_DE":
                discordController.state = "Es steht einfach auf der Speisekarte und das war’s.";
                discordController.details = "Speisekarte";
                break;
            case "es_ES":
                discordController.state = "Sólo se sienta en el menú y eso es todo.";
                discordController.details = "Menú";
                break;
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

        switch (localizationManager.CurrentLanguage)
        {
            case "en_US":
                discordController.state = "Multiplayer or Single-player? Hmmm.";
                discordController.details = "Menu";
                break;
            case "ru_RU":
                discordController.state = "Мультиплеер или Одиночная игра? Хммм.";
                discordController.details = "Меню";
                break;
            case "de_DE":
                discordController.state = "Mehrspieler oder Einzelspieler? Hmmm.";
                discordController.details = "Speisekarte";
                break;
            case "es_ES":
                discordController.state = "¿Multijugador o un jugador? Mmm.";
                discordController.details = "Menú";
                break;
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

        switch (localizationManager.CurrentLanguage)
        {
            case "en_US":
                discordController.state = "WANTS TO QUIT THE GAME ((((";
                discordController.details = "Menu";
                break;
            case "ru_RU":
                discordController.state = "ХОЧЕТ ВЫЙТИ ИЗ ИГРЫ ((((";
                discordController.details = "Меню";
                break;
            case "de_DE":
                discordController.state = "WILL DAS SPIEL VERLASSEN ((((";
                discordController.details = "Speisekarte";
                break;
            case "es_ES":
                discordController.state = "QUIERE SALIR DEL JUEGO ((((";
                discordController.details = "Menú";
                break;
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

        switch (localizationManager.CurrentLanguage)
        {
            case "en_US":
                discordController.state = "Admires the developers ^^";
                discordController.details = "Menu";
                break;
            case "ru_RU":
                discordController.state = "Любуется разработчиками ^^";
                discordController.details = "Меню";
                break;
            case "de_DE":
                discordController.state = "Von den Entwicklern geliebt ^^";
                discordController.details = "Speisekarte";
                break;
            case "es_ES":
                discordController.state = "Amado por los desarrolladores ^^";
                discordController.details = "Menú";
                break;
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
    }
    public void ManualButton()
    {
        isManual = true;
        isStore = false;
        isSettings = false;
        isMenu = false;
        isCatalog = false;
        isNotSelect = true;
      //  animator.SetTrigger("ManualMenu");
      //  animatorManual.SetTrigger("OpenManual");
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
        //  animatorManual.SetTrigger("OpenManual");
        mainCamera.SetActive(false);
        world.SetActive(false);
        manualObjects.SetActive(true);
    }
    public void ManualClose()
    {
        // animatorManual.SetTrigger("CloseManual");
        mainCamera.SetActive(true);
        world.SetActive(true);
        manualObjects.SetActive(false);
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

    public void SelectObject(string name)
    {
        selectedEntry = manualEntries.Find(entry => entry.Name == name);

        if (selectedEntry != null)
        {
            target3D = selectedEntry.Target;
            Debug.Log($"Выбран объект: {selectedEntry.Name}, target3D обновлен!");

            onScreenPositionTarget = selectedEntry.onScreenPosition;
            onScreenRotationTarget = selectedEntry.onScreenRotation;
            offScreenPositionTarget = selectedEntry.offScreenPosition;
            offScreenRotationTarget = selectedEntry.offScreenRotation;

            UpdateSelectionText();
            WindowInfoOpen();
            MoveUp();
        }
        else
        {
            Debug.LogWarning($"Объект с именем {name} не найден в списке!");
        }
    }


    // Обновление UI
    private void UpdateSelectionText()
    {
        if (currentSelectionText != null)
        {
            currentSelectionText.text = selectedEntry != null ? $"{selectedEntry.Name}" : "???";
            currentSelectionDescription.GetComponent<LocalizedText>().key = selectedEntry != null ? $"{selectedEntry.DescriptionLocalizationKey}" : "???";
            currentSelectionDescription.GetComponent<LocalizedText>().UpdateText();
        }
        Debug.Log($"Обновлен Target: {(target3D != null ? target3D.name : "null")}");
    }


    public void SetUpPosition(string name, Vector3 position, Vector3 rotation)
    {
        ManualEntry entry = manualEntries.Find(e => e.Name == name);
        if (entry != null)
        {
            entry.onScreenPosition = position;
            entry.onScreenRotation = rotation;
        }
    }

    public void SetBottomPosition(string name, Vector3 position, Vector3 rotation)
    {
        ManualEntry entry = manualEntries.Find(e => e.Name == name);
        if (entry != null)
        {
            entry.offScreenPosition = position;
            entry.offScreenRotation = rotation;
        }
    }


    public void WindowInfoOpen()
    {
        float screenHeight = Screen.height;
        mainWindow.DOAnchorPosY(-screenHeight, 0.3f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                mainWindow.gameObject.SetActive(false);
                infoWindow.gameObject.SetActive(true);
                infoWindow.anchoredPosition = new Vector2(infoWindow.anchoredPosition.x, -screenHeight);

                infoWindow.DOAnchorPosY(0f, 0.3f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        manualLabel.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                        infoWindow.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                        backgroundManual.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                    });
            });
    }

    public void WindowInfoClose()
    {
        float screenHeight = Screen.height;
        MoveDown();
        infoWindow.DOAnchorPosY(-screenHeight, 0.3f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                infoWindow.gameObject.SetActive(false);
                mainWindow.gameObject.SetActive(true);
                mainWindow.anchoredPosition = new Vector2(infoWindow.anchoredPosition.x, -screenHeight);

                mainWindow.DOAnchorPosY(0f, 0.3f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        manualLabel.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                        mainWindow.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                        backgroundManual.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);
                    });
            });
    }

    private void MoveUp()
    {
        target3D.DOMove(onScreenPositionTarget, moveDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                target3D.DOShakePosition(shakeDuration3D, shakeStrength3D, shakeVibrato3D);
            });

        target3D.DORotate(onScreenRotationTarget, rotationDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                target3D.DOShakeRotation(shakeDuration3D, shakeStrength3D, shakeVibrato3D);
            });
    }

    private void MoveDown()
    {
        target3D.DOMove(offScreenPositionTarget, moveDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                target3D.DOShakePosition(shakeDuration3D, shakeStrength3D, shakeVibrato3D);
            });

        target3D.DORotate(offScreenRotationTarget, rotationDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                target3D.DOShakeRotation(shakeDuration3D, shakeStrength3D, shakeVibrato3D);
            });
    }
}

