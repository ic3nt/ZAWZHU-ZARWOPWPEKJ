using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using RKS.DD.Core;
using RKS.DD.Core.Managers;
using Zenject;

namespace RKS.DD.UI
{
    public class SettingsManager : RKSBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Dropdown frameRateDropdown;
        [SerializeField] private TMP_Dropdown windowModeDropdown;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle visualMoverToggle;
        [SerializeField] private VisualMover visualMover;

        protected override void OnReady()
        {
            LoadSettings();

            frameRateDropdown.onValueChanged.AddListener(_ => UpdateFrameRate());
            windowModeDropdown.onValueChanged.AddListener(_ => UpdateWindowMode());
            visualMoverToggle.onValueChanged.AddListener(_ => UpdateVisualMover());

            EventTrigger trigger = volumeSlider.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            entry.callback.AddListener(_ => UpdateVolume());
            trigger.triggers.Add(entry);
        }

        private void SaveSettings()
        {
            var data = Save.CurrentData;

            data.isFirstRun = false;
            data.isPlayerAgreedPlay = true;
            data.language = Localization != null ? Localization.currentLanguage : "en";
            data.frameRateIndex = frameRateDropdown.value;
            data.windowModeIndex = windowModeDropdown.value;
            data.volumeValue = volumeSlider.value;
            data.isVisualMoverEnabled = visualMoverToggle.isOn;

            Save.Save();
            Debug.Log("[SettingsManager] Settings saved.");
        }

        private void LoadSettings()
        {
            var data = Save.Load();

            if (data == null)
            {
                Debug.LogWarning("[SettingsManager] No save found. Creating default data.");
                data = new GameData.Data();
                Save.Save(data);
            }

            ApplySettings(data);
        }

        private void ApplySettings(GameData.Data settings)
        {
            // FPS
            frameRateDropdown.value = settings.frameRateIndex;
            frameRateDropdown.RefreshShownValue();
            UpdateFrameRate();

            // ќкно
            windowModeDropdown.value = settings.windowModeIndex;
            windowModeDropdown.RefreshShownValue();
            UpdateWindowMode();

            // √ромкость
            volumeSlider.value = settings.volumeValue;
            UpdateVolume();

            // ¬изуальные эффекты
            visualMoverToggle.isOn = settings.isVisualMoverEnabled;
            UpdateVisualMover();

            // язык
            if (Localization != null)
                Localization.SetLanguage(settings.language);
        }

        // ================== SETTINGS LOGIC ==================

        private void UpdateFrameRate()
        {
            int frameRateIndex = frameRateDropdown.value;
            Debug.Log($"[SettingsManager] Applying FrameRate preset: {frameRateIndex}");

            switch (frameRateIndex)
            {
                case 0: QualitySettings.vSyncCount = 1; Application.targetFrameRate = -1; break; // VSync
                case 1: QualitySettings.vSyncCount = 0; Application.targetFrameRate = -1; break; // Unlimited
                case 2: QualitySettings.vSyncCount = 0; Application.targetFrameRate = 144; break;
                case 3: QualitySettings.vSyncCount = 0; Application.targetFrameRate = 120; break;
                case 4: QualitySettings.vSyncCount = 0; Application.targetFrameRate = 60; break;
                case 5: QualitySettings.vSyncCount = 0; Application.targetFrameRate = 30; break;
            }

            SaveSettings();
        }

        private void UpdateWindowMode()
        {
            int windowModeIndex = windowModeDropdown.value;
            Debug.Log($"[SettingsManager] Applying WindowMode preset: {windowModeIndex}");

            switch (windowModeIndex)
            {
                case 0: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
                case 1: Screen.fullScreenMode = FullScreenMode.MaximizedWindow; break;
                case 2: Screen.fullScreenMode = FullScreenMode.MaximizedWindow; break;
                case 3: Screen.fullScreenMode = FullScreenMode.Windowed; break;
            }

            SaveSettings();
        }

        private void UpdateVolume()
        {
            float volume = volumeSlider.value;
            Audio.SetVolume(volume <= 0.01f ? 0f : volume);
            Debug.Log($"[SettingsManager] Applying Volume: {(volume <= 0.01f ? "Muted" : volume.ToString("0.00"))}");
            SaveSettings();
        }

        private void UpdateVisualMover()
        {
            bool isEnabled = visualMoverToggle.isOn;
            Debug.Log($"[SettingsManager] VisualMover: {(isEnabled ? "Enabled" : "Disabled")}");

            if (visualMover != null)
                visualMover.enabled = isEnabled;

            SaveSettings();
        }
    }
}
