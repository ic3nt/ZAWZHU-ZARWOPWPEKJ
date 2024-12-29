using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening.Core.Easing;
using EasyTransition;
using PlayFab.AdminModels;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown frameRateDropdown;
    public TMP_Dropdown windowModeDropdown;
    public Slider gammaSlider;
    public Toggle visualMoverToggle;
    public VisualMover visualMover;

    public GameObject saveManager; 

    private GameData.Data Data;

    public LocalizationManager localizationManager;

    private void Start()
    {
        if (saveManager == null)
        {
            GameObject saveManagerObject = GameObject.FindWithTag("GameManager");
            if (saveManagerObject != null)
            {
                saveManager = saveManagerObject;
                Debug.Log("GameManager automatically assigned.");
            }
            else
            {
                Debug.LogError("No object with tag 'GameManager' found in the scene!");
            }
        }

        if (localizationManager == null)
        {
            GameObject localizationManagerObject = GameObject.FindWithTag("LocalizationManager");
            if (localizationManagerObject != null)
            {
                localizationManager = localizationManagerObject.GetComponent<LocalizationManager>();
                Debug.Log("LocalizationManager automatically assigned.");
            }
            else
            {
                Debug.LogError("No object with tag 'LocalizationManager' found in the scene!");
            }
        }

        LoadSettings();

        frameRateDropdown.onValueChanged.AddListener(delegate { UpdateFrameRate(); });
        windowModeDropdown.onValueChanged.AddListener(delegate { UpdateWindowMode(); });
        gammaSlider.onValueChanged.AddListener(delegate { UpdateGamma(); });
        visualMoverToggle.onValueChanged.AddListener(delegate { UpdateVisualMover(); });

    }

    public void SaveSettings()
    {
        Debug.Log("Saving settings...");

        Data = new GameData.Data
        {
            isFirstRun = false,
            isPlayerAgreedPlay = true,
            language = localizationManager.currentLanguage,
            frameRateIndex = frameRateDropdown.value,
            windowModeIndex = windowModeDropdown.value,
            gammaValue = gammaSlider.value,
            isVisualMoverEnabled = visualMoverToggle.isOn
        };

        saveManager.GetComponent<SaveManager>().Save(Data);
    }

    public void LoadSettings()
    {
        Debug.Log("Loading settings...");

        Data = saveManager.GetComponent<SaveManager>().Load();
        ApplySettings(Data);
    }

    private void ApplySettings(GameData.Data settings)
    {

        frameRateDropdown.value = settings.frameRateIndex;
        frameRateDropdown.RefreshShownValue();
        UpdateFrameRate();

        windowModeDropdown.value = settings.windowModeIndex;
        windowModeDropdown.RefreshShownValue();
        UpdateWindowMode();

        gammaSlider.value = settings.gammaValue;
        UpdateGamma();

        visualMoverToggle.isOn = settings.isVisualMoverEnabled;
        UpdateVisualMover();
    }

    public void UpdateFrameRate()
    {
        int frameRateIndex = frameRateDropdown.value;
        Debug.Log("Applying FrameRate: " + frameRateIndex);

        switch (frameRateIndex)
        {
            case 0: // V-Sync
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
                break;
            case 1: // Unlimited
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
                break;
            case 2: // 144 FPS
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 144;
                break;
            case 3: // 120 FPS
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 120;
                break;
            case 4: // 60 FPS
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 60;
                break;
            case 5: // 30 FPS
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 30;
                break;
        }

        SaveSettings();
    }

    public void UpdateWindowMode()
    {
        int windowModeIndex = windowModeDropdown.value;
        Debug.Log("Applying WindowMode: " + windowModeIndex);

        switch (windowModeIndex)
        {
            case 0: // Fullscreen
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1: // Windowed (no border)
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case 2: // Maximized
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case 3: // Windowed
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }

        SaveSettings();
    }

    public void UpdateGamma()
    {
        float gamma = gammaSlider.value;
        //Debug.Log("Applying Gamma: " + gamma);
        RenderSettings.ambientLight = new Color(gamma, gamma, gamma);
        //SaveSettings();
    }

    public void UpdateVisualMover()
    {
        bool isEnabled = visualMoverToggle.isOn;
        Debug.Log("Applying VisualMover: " + (isEnabled ? "Enabled" : "Disabled"));

        if (visualMover != null)
        {
            visualMover.enabled = isEnabled;
        }

        SaveSettings();
    }
}
