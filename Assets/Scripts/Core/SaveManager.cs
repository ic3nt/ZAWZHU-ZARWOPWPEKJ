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
            string json = File.ReadAllText(settingsFilePath);
            GameData.Data settings = JsonUtility.FromJson<GameData.Data>(json);
            Debug.Log("Data loaded from: " + settingsFilePath);
            return settings;
        }
        else
        {
            Debug.LogWarning("Data file not found. Creating a new file with default settings.");
            GameData.Data defaultSettings = new GameData.Data(); 
            Save(defaultSettings);
            return defaultSettings;
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
