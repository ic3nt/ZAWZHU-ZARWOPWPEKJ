using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    private string currentLanguage;
    private Dictionary<string, string> localizedText;
    public static bool isReady = false;

	public delegate void ChangeLangText();
    public event ChangeLangText OnLanguageChanged;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian || Application.systemLanguage == SystemLanguage.SerboCroatian || Application.systemLanguage == SystemLanguage.Lithuanian || Application.systemLanguage == SystemLanguage.Latvian || Application.systemLanguage == SystemLanguage.Bulgarian || Application.systemLanguage == SystemLanguage.Estonian)
            {
                PlayerPrefs.SetString("Language", "ru_RU");
            }
            else if (Application.systemLanguage == SystemLanguage.Swedish || Application.systemLanguage == SystemLanguage.Czech || Application.systemLanguage == SystemLanguage.Slovenian || Application.systemLanguage == SystemLanguage.Unknown || Application.systemLanguage == SystemLanguage.Slovak || Application.systemLanguage == SystemLanguage.Romanian || Application.systemLanguage == SystemLanguage.Portuguese || Application.systemLanguage == SystemLanguage.Polish || Application.systemLanguage == SystemLanguage.Norwegian || Application.systemLanguage == SystemLanguage.Korean || Application.systemLanguage == SystemLanguage.Japanese || Application.systemLanguage == SystemLanguage.Italian || Application.systemLanguage == SystemLanguage.Indonesian || Application.systemLanguage == SystemLanguage.Icelandic || Application.systemLanguage == SystemLanguage.Hungarian || Application.systemLanguage == SystemLanguage.Greek)
            {
                PlayerPrefs.SetString("Language", "en_US");
            }
            else if (Application.systemLanguage == SystemLanguage.German)
            {
                PlayerPrefs.SetString("Language", "de_DE");
            }
            else if (Application.systemLanguage == SystemLanguage.Spanish)
            {
                PlayerPrefs.SetString("Language", "es_ES");
            }
        }
        currentLanguage = PlayerPrefs.GetString("Language");

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

        PlayerPrefs.SetString("Language", langName);
        currentLanguage = PlayerPrefs.GetString("Language");
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