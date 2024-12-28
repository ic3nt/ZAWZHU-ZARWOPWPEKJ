using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Space(10)]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullScreenToggle;
    public Toggle vSyncToggle;

    private Resolution[] resolutions;

    private void Start()
    {
        // Получаем доступные разрешения экрана
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(option));

            // Определяем текущее разрешение
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Подписываемся на события изменения параметров
        resolutionDropdown.onValueChanged.AddListener(delegate { UpdateResolution(); });
        qualityDropdown.onValueChanged.AddListener(delegate { UpdateGraphicsQuality(); });
        fullScreenToggle.onValueChanged.AddListener(delegate { UpdateFullScreen(); });
        vSyncToggle.onValueChanged.AddListener(delegate { UpdateVSync(); });

        // Загружаем настройки
        LoadSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("resolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("graphicsQuality", qualityDropdown.value);
        PlayerPrefs.SetInt("isFullScreen", fullScreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("vSync", vSyncToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();
        Debug.Log("Settings saved");
    }

    public void LoadSettings()
    {
        Debug.Log("Loaded resolutionIndex: " + PlayerPrefs.GetInt("resolutionIndex", -1));
        Debug.Log("Loaded graphicsQuality: " + PlayerPrefs.GetInt("graphicsQuality", -1));
        Debug.Log("Loaded isFullScreen: " + PlayerPrefs.GetInt("isFullScreen", -1));
        Debug.Log("Loaded vSync: " + PlayerPrefs.GetInt("vSync", -1));


        // Загружаем индекс разрешения и проверяем его корректность
        int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", resolutions.Length - 1);
        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
        {
            Debug.LogWarning("Invalid resolution index. Using default.");
            resolutionIndex = resolutions.Length - 1; // Используем последнее разрешение
        }

        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Загружаем качество графики
        int qualityIndex = PlayerPrefs.GetInt("graphicsQuality", 3);
        qualityDropdown.value = qualityIndex;
        qualityDropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(qualityIndex);

        // Загружаем настройку полноэкранного режима
        bool isFullScreen = PlayerPrefs.GetInt("isFullScreen", 1) == 1;
        fullScreenToggle.isOn = isFullScreen;
        Screen.fullScreen = isFullScreen;

        // Загружаем настройку VSync
        bool vSync = PlayerPrefs.GetInt("vSync", 1) == 1;
        vSyncToggle.isOn = vSync;
        QualitySettings.vSyncCount = vSync ? 1 : 0;

        Debug.Log("Settings loaded successfully");
    }

    public void UpdateResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, fullScreenToggle.isOn);
        SaveSettings();
    }

    public void UpdateGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        SaveSettings();
    }

    public void UpdateFullScreen()
    {
        Screen.fullScreen = fullScreenToggle.isOn;
        SaveSettings();
    }

    public void UpdateVSync()
    {
        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0;
        SaveSettings();
    }
}
