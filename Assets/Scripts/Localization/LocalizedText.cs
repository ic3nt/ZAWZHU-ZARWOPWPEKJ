using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string key;

    private LocalizationManager localizationManager;
    private TMP_Text text;

    void Awake()
    {
        // ищем объект с тегом LocalizationManager и берем у него компонент LocalizationManager

        if (localizationManager == null)
        {
            localizationManager = GameObject.FindGameObjectWithTag("LocalizationManager").GetComponent<LocalizationManager>();
        }
        if(text == null)
        {
            text = GetComponent<TMP_Text>();
        }
        localizationManager.OnLanguageChanged += UpdateText;
    }

    void Start()
    {
        // обновляем текст

        UpdateText();
    }

    private void OnDestroy()
    {
        // вызывается при удалении объекта LocalizationManager

        localizationManager.OnLanguageChanged -= UpdateText;
    }

    virtual protected void UpdateText()
    {
        // метод для обновления текста

        if (gameObject == null) return;

        if(localizationManager == null)
        {
            localizationManager = GameObject.FindGameObjectWithTag("LocalizationManager").GetComponent<LocalizationManager>();
        }
        if (text == null)
        {
            text = GetComponent<TMP_Text>();
        }
        text.text = localizationManager.GetLocalizedValue(key);
    }
}