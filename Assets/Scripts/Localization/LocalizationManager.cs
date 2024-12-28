using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public string currentLanguage;
    private Dictionary<string, string> localizedText;
    public static bool isReady = false;

    public SaveManager saveManager;  // Менеджер для сохранения и загрузки данных
    private GameData.Data Data;  // Данные игры, включая язык

    public delegate void ChangeLangText();
    public event ChangeLangText OnLanguageChanged;

    void Start()
    {
        Data = saveManager.Load();

        if (string.IsNullOrEmpty(Data.language))
        {
            if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
            {
                Data.language = "ru_RU";
            }
            else if (Application.systemLanguage == SystemLanguage.English)
            {
                Data.language = "en_US";
            }
            else if (Application.systemLanguage == SystemLanguage.German)
            {
                Data.language = "de_DE";
            }
            else if (Application.systemLanguage == SystemLanguage.Spanish)
            {
                Data.language = "es_ES";
            }
        }

        currentLanguage = Data.language;

        LoadLocalizedText(currentLanguage);
    }

    public void LoadLocalizedText(string langName)
    {
        string path = Application.streamingAssetsPath + "/Languages/" + langName + ".json";
        string dataAsJson;

        if (Application.platform == RuntimePlatform.Android)
        {
            WWW reader = new WWW(path);
            while (!reader.isDone) { }
            dataAsJson = reader.text;
        }
        else
        {
            dataAsJson = File.ReadAllText(path);
        }

        LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

        localizedText = new Dictionary<string, string>();
        for (int i = 0; i < loadedData.items.Length; i++)
        {
            localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
        }

        Data.language = langName;
        saveManager.Save(Data);

        currentLanguage = langName;
        isReady = true;

        OnLanguageChanged?.Invoke();
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        else
        {
            throw new Exception("Localized text with key \"" + key + "\" not found");
        }
    }

    public string CurrentLanguage
    {
        get
        {
            return currentLanguage;
        }
        set
        {
            LoadLocalizedText(value);
        }
    }

    public bool IsReady
    {
        get
        {
            return isReady;
        }
    }
}
