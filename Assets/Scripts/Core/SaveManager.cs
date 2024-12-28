using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string settingsFilePath;

    void Start()
    {
        settingsFilePath = Path.Combine(Application.persistentDataPath, "DD_Data.rkst");
    }

    public void Save(GameData.Data settings)
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(settingsFilePath, json);
        Debug.Log("Data saved to: " + settingsFilePath);
    }

    public GameData.Data Load()
    {
        if (File.Exists(settingsFilePath))
        {
            // Если файл существует, загружаем его
            string json = File.ReadAllText(settingsFilePath);
            GameData.Data settings = JsonUtility.FromJson<GameData.Data>(json);
            Debug.Log("Data loaded from: " + settingsFilePath);
            return settings;
        }
        else
        {
            // Если файл не существует, создаем новый с дефолтными значениями
            Debug.LogWarning("Data file not found. Creating a new file with default settings.");
            GameData.Data defaultSettings = new GameData.Data(); // Здесь вы можете настроить дефолтные значения, если нужно
            Save(defaultSettings); // Сохраняем новый файл с дефолтными значениями
            return defaultSettings; // Возвращаем дефолтные данные
        }
    }

    public void DeleteDataFile()
    {
        if (File.Exists(settingsFilePath))
        {
            File.Delete(settingsFilePath);
            Debug.Log("Data file deleted.");
        }
        else
        {
            Debug.LogWarning("Data file not found to delete.");
        }
    }
}
