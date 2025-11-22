// БЛЯТЬ Я ПОТОМ ВЕСЬ УЖАС НАХУЙ СНЕСУ А ТО ЭТО ПИЗДЕЦБ ПУСТЬ ПОКА ОСТАНЕТСЯ БЯЛТЬ ЭТО УЖАС

using DG.Tweening;
using Discord;
using EasyTransition;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Diagnostics;
using Zenject;
using RKS.DD.Core.Managers;
using RKS.DD.Core;
using RKS.DD.Menu;

#if UNITY_EDITOR
[CustomEditor(typeof(MenuManager))]
public class ManualManagerEditor : Editor
{
    private string selectedName = "";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MenuManager script = (MenuManager)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Manual 3D object position settings", EditorStyles.boldLabel);

        if (script.manualEntries.Count == 0)
        {
            EditorGUILayout.HelpBox("The manual list is empty!", MessageType.Warning);
            return;
        }

        List<string> options = new List<string>();
        foreach (var entry in script.manualEntries)
        {
            if (entry.Target != null)
            {
                options.Add(entry.Target.name);
            }
        }

        if (options.Count == 0)
        {
            EditorGUILayout.HelpBox("There are no objects with specified targets.", MessageType.Warning);
            return;
        }

        int selectedIndex = options.IndexOf(selectedName);
        selectedIndex = EditorGUILayout.Popup("Select Object:", selectedIndex, options.ToArray());
        selectedName = selectedIndex >= 0 ? options[selectedIndex] : selectedName;

        ManualEntry selectedEntry = script.manualEntries.Find(entry => entry.Target != null && entry.Target.name == selectedName);

        if (selectedEntry == null || selectedEntry.Target == null)
        {
            EditorGUILayout.HelpBox("The selected object is missing!", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Record coordinates in the screen area"))
        {
            selectedEntry.onScreenPosition = selectedEntry.Target.position;
            selectedEntry.onScreenRotation = selectedEntry.Target.rotation.eulerAngles;
            EditorUtility.SetDirty(script);
        }

        if (GUILayout.Button("Record coordinates in outside the screen area"))
        {
            selectedEntry.offScreenPosition = selectedEntry.Target.position;
            selectedEntry.offScreenRotation = selectedEntry.Target.rotation.eulerAngles;
            EditorUtility.SetDirty(script);
        }
    }
}
#endif

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

public class MenuManager : RKSBehaviour
{
    [Header("Menu Objects")]
    public GameObject Kail;
    public GameObject mainButtonsGroup;
    public GameObject world;
    public GameObject manualObjects;
    public GameObject mainCamera;

    [Header("Transition Settings")]
    public TransitionSettings transition;
    [HideInInspector] public float startDelay;
    
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

    protected override void OnReady()
    {
        Audio.Play("MenuMusic");

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

        switch (Localization.currentLanguage)
        {
            case "en_US":
                DiscordRPC.state = "He just sits on the menu and that's it.";
                DiscordRPC.details = "Menu";
                break;
            case "ru_RU":
                DiscordRPC.state = "Просто сидит в меню и все.";
                DiscordRPC.details = "Меню";
                break;
            case "de_DE":
                DiscordRPC.state = "Es steht einfach auf der Speisekarte und das war’s.";
                DiscordRPC.details = "Speisekarte";
                break;
            case "es_ES":
                DiscordRPC.state = "Sólo se sienta en el menú y eso es todo.";
                DiscordRPC.details = "Menú";
                break;
        }
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

                switch (Localization.currentLanguage)
                {
                    case "en_US":
                        DiscordRPC.state = "He just sits on the menu and that's it.";
                        DiscordRPC.details = "Menu";
                        break;
                    case "ru_RU":
                        DiscordRPC.state = "Просто сидит в меню и все.";
                        DiscordRPC.details = "Меню";
                        break;
                    case "de_DE":
                        DiscordRPC.state = "Es steht einfach auf der Speisekarte und das war’s.";
                        DiscordRPC.details = "Speisekarte";
                        break;
                    case "es_ES":
                        DiscordRPC.state = "Sólo se sienta en el menú y eso es todo.";
                        DiscordRPC.details = "Menú";
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

                switch (Localization.currentLanguage)
                {
                    case "en_US":
                        DiscordRPC.state = "Sets up Deadly Devastation...";
                        break;
                    case "ru_RU":
                        DiscordRPC.state = "Настраивает Deadly Devastation...";
                        break;
                    case "es_ES":
                        DiscordRPC.state = "Configura Deadly Devastation...";
                        break;
                    case "de_DE":
                        DiscordRPC.state = "Konfiguriert Deadly Devastation...";
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

                switch (Localization.currentLanguage)
                {
                    case "en_US":
                        DiscordRPC.state = "Carefully examines the manual...";
                        break;
                    case "ru_RU":
                        DiscordRPC.state = "Внимательно изучает справочник...";
                        break;
                    case "es_ES":
                        DiscordRPC.state = "Estudiando atentamente el libro de referencia...";
                        break;
                    case "de_DE":
                        DiscordRPC.state = "Das Nachschlagewerk sorgfältig studieren...";
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

                switch (Localization.currentLanguage)
                {
                    case "en_US":
                        DiscordRPC.state = "On a shopping trip...";
                        break;
                    case "ru_RU":
                        DiscordRPC.state = "На шоппинге...";
                        break;
                    case "es_ES":
                        DiscordRPC.state = "On a shopping trip...";
                        break;
                    case "de_DE":
                        DiscordRPC.state = "On a shopping trip...";
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

                switch (Localization.currentLanguage)
                {
                    case "en_US":
                        DiscordRPC.state = "Looks at the catalog...";
                        break;
                    case "ru_RU":
                        DiscordRPC.state = "Рассматривает каталог...";
                        break;
                    case "es_ES":
                        DiscordRPC.state = "Mirando el catálogo...";
                        break;
                    case "de_DE":
                        DiscordRPC  .state = "Blick in den Katalog...";
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

        Kail.GetComponent<RotateModelByInput>().enabled = true;

        switch (Localization.currentLanguage)
        {
            case "en_US":
                DiscordRPC.state = "He just sits on the menu and that's it.";
                DiscordRPC.details = "Menu";
                break;
            case "ru_RU":
                DiscordRPC.state = "Просто сидит в меню и все.";
                DiscordRPC.details = "Меню";
                break;
            case "de_DE":
                DiscordRPC.state = "Es steht einfach auf der Speisekarte und das war’s.";
                DiscordRPC.details = "Speisekarte";
                break;
            case "es_ES":
                DiscordRPC.state = "Sólo se sienta en el menú y eso es todo.";
                DiscordRPC.details = "Menú";
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

        switch (Localization.currentLanguage)
        {
            case "en_US":
                DiscordRPC.state = "Multiplayer or Single-player? Hmmm.";
                DiscordRPC.details = "Menu";
                break;
            case "ru_RU":
                DiscordRPC.state = "Мультиплеер или Одиночная игра? Хммм.";
                DiscordRPC.details = "Меню";
                break;
            case "de_DE":
                DiscordRPC.state = "Mehrspieler oder Einzelspieler? Hmmm.";
                DiscordRPC.details = "Speisekarte";
                break;
            case "es_ES":
                DiscordRPC.state = "¿Multijugador o un jugador? Mmm.";
                DiscordRPC.details = "Menú";
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

        switch (Localization.currentLanguage)
        {
            case "en_US":
                DiscordRPC.state = "WANTS TO QUIT THE GAME ((((";
                DiscordRPC.details = "Menu";
                break;
            case "ru_RU":
                DiscordRPC.state = "ХОЧЕТ ВЫЙТИ ИЗ ИГРЫ ((((";
                DiscordRPC.details = "Меню";
                break;
            case "de_DE":
                DiscordRPC.state = "WILL DAS SPIEL VERLASSEN ((((";
                DiscordRPC.details = "Speisekarte";
                break;
            case "es_ES":
                DiscordRPC.state = "QUIERE SALIR DEL JUEGO ((((";
                DiscordRPC.details = "Menú";
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

        switch (Localization.currentLanguage)
        {
            case "en_US":
                DiscordRPC.state = "Admires the developers ^^";
                DiscordRPC.details = "Menu";
                break;
            case "ru_RU":
                DiscordRPC.state = "Любуется разработчиками ^^";
                DiscordRPC.details = "Меню";
                break;
            case "de_DE":
                DiscordRPC.state = "Von den Entwicklern geliebt ^^";
                DiscordRPC.details = "Speisekarte";
                break;
            case "es_ES":
                DiscordRPC.state = "Amado por los desarrolladores ^^";
                DiscordRPC.details = "Menú";
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

        Kail.GetComponent<RotateModelByInput>().enabled = false;
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
        Transition.LoadScene("TEST");
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