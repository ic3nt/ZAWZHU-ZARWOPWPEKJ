using System;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveManager : MonoBehaviour
{
    [Header("File Configuration")]
    [Tooltip("The name of the save file.")]
    [SerializeField, ReadOnly] private string fileName = "DD_Data.rkst";

    private string settingsFilePath;

    private readonly char[] rechAlphabet = { 'R', 'E', 'C', 'H' };

    void Start()
    {
        settingsFilePath = Path.Combine(Application.persistentDataPath, fileName);
    }

    public void Save(GameData.Data settings)
    {
        string json = JsonUtility.ToJson(settings, true);
        string rechData = ConvertJsonToRech(json);

        StringBuilder fileContent = new StringBuilder();
        fileContent.AppendLine("\n\n$$$$$$$\\  $$$$$$$$\\  $$$$$$\\  $$$$$$$\\  $$\\   $$\\     $$\\       $$$$$$$\\  $$$$$$$$\\ $$\\    $$\\  $$$$$$\\   $$$$$$\\ $$$$$$$$\\  $$$$$$\\ $$$$$$$$\\ $$$$$$\\  $$$$$$\\  $$\\   $$\\ \r\n$$  __$$\\ $$  _____|$$  __$$\\ $$  __$$\\ $$ |  \\$$\\   $$  |      $$  __$$\\ $$  _____|$$ |   $$ |$$  __$$\\ $$  __$$\\\\__$$  __|$$  __$$\\\\__$$  __|\\_$$  _|$$  __$$\\ $$$\\  $$ |\r\n$$ |  $$ |$$ |      $$ /  $$ |$$ |  $$ |$$ |   \\$$\\ $$  /       $$ |  $$ |$$ |      $$ |   $$ |$$ /  $$ |$$ /  \\__|  $$ |   $$ /  $$ |  $$ |     $$ |  $$ /  $$ |$$$$\\ $$ |\r\n$$ |  $$ |$$$$$\\    $$$$$$$$ |$$ |  $$ |$$ |    \\$$$$  /        $$ |  $$ |$$$$$\\    \\$$\\  $$  |$$$$$$$$ |\\$$$$$$\\    $$ |   $$$$$$$$ |  $$ |     $$ |  $$ |  $$ |$$ $$\\$$ |\r\n$$ |  $$ |$$  __|   $$  __$$ |$$ |  $$ |$$ |     \\$$  /         $$ |  $$ |$$  __|    \\$$\\$$  / $$  __$$ | \\____$$\\   $$ |   $$  __$$ |  $$ |     $$ |  $$ |  $$ |$$ \\$$$$ |\r\n$$ |  $$ |$$ |      $$ |  $$ |$$ |  $$ |$$ |      $$ |          $$ |  $$ |$$ |        \\$$$  /  $$ |  $$ |$$\\   $$ |  $$ |   $$ |  $$ |  $$ |     $$ |  $$ |  $$ |$$ |\\$$$ |\r\n$$$$$$$  |$$$$$$$$\\ $$ |  $$ |$$$$$$$  |$$$$$$$$\\ $$ |          $$$$$$$  |$$$$$$$$\\    \\$  /   $$ |  $$ |\\$$$$$$  |  $$ |   $$ |  $$ |  $$ |   $$$$$$\\  $$$$$$  |$$ | \\$$ |\r\n\\_______/ \\________|\\__|  \\__|\\_______/ \\________|\\__|          \\_______/ \\________|    \\_/    \\__|  \\__| \\______/   \\__|   \\__|  \\__|  \\__|   \\______| \\______/ \\__|  \\__|\n\n");
        fileContent.AppendLine("==== Data ====");
        fileContent.AppendLine(rechData);

        File.WriteAllText(settingsFilePath, fileContent.ToString());
        Debug.Log("Data saved in RECH format to: " + settingsFilePath);
    }

    public GameData.Data Load()
    {
        if (File.Exists(settingsFilePath))
        {
            try
            {
                string fileContent = File.ReadAllText(settingsFilePath);
                int rechDataStartIndex = fileContent.IndexOf("==== Data ====") + "==== Data ====\n".Length;

                if (rechDataStartIndex >= 0)
                {
                    string rechData = fileContent.Substring(rechDataStartIndex).Trim();
                    if (rechData.Length % 4 != 0)
                    {
                        throw new FormatException("Invalid RECH data format: Length is not divisible by 4.");
                    }
                    string json = ConvertRechToJson(rechData);
                    GameData.Data settings = JsonUtility.FromJson<GameData.Data>(json);

                    Debug.Log("Data loaded from file: " + settingsFilePath);
                    return settings;
                }
                else
                {
                    Debug.LogError("File format is incorrect. Missing RECH Data section.");
                    return new GameData.Data();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to load data: " + ex.Message);
                return new GameData.Data();
            }
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

    public void ResetToDefault()
    {
        GameData.Data defaultSettings = new GameData.Data();
        Save(defaultSettings);
        Debug.Log("Data reset to default settings.");
    }

    private string ConvertJsonToRech(string json)
    {
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        StringBuilder rechBuilder = new StringBuilder();

        Debug.Log("Original JSON: " + json);

        foreach (byte b in jsonBytes)
        {
            int highBits = (b >> 6) & 0b11;
            int midBits = (b >> 4) & 0b11;
            int lowBits1 = (b >> 2) & 0b11;
            int lowBits2 = b & 0b11;

            rechBuilder.Append(rechAlphabet[highBits]);
            rechBuilder.Append(rechAlphabet[midBits]);
            rechBuilder.Append(rechAlphabet[lowBits1]);
            rechBuilder.Append(rechAlphabet[lowBits2]);
        }

        string rechString = rechBuilder.ToString();
        Debug.Log("Converted RECH: " + rechString);

        return rechString;
    }

    private string ConvertRechToJson(string rechData)
    {
        Debug.Log("Input RECH: " + rechData);

        if (rechData.Length % 4 != 0)
        {
            throw new FormatException("Invalid RECH data format: Length is not divisible by 4.");
        }

        byte[] jsonBytes = new byte[rechData.Length / 4];

        for (int i = 0; i < rechData.Length; i += 4)
        {
            int highBits = Array.IndexOf(rechAlphabet, rechData[i]);
            int midBits = Array.IndexOf(rechAlphabet, rechData[i + 1]);
            int lowBits1 = Array.IndexOf(rechAlphabet, rechData[i + 2]);
            int lowBits2 = Array.IndexOf(rechAlphabet, rechData[i + 3]);

            if (highBits < 0 || midBits < 0 || lowBits1 < 0 || lowBits2 < 0)
            {
                throw new FormatException($"Invalid character in RECH data: {rechData[i]} {rechData[i + 1]} {rechData[i + 2]} {rechData[i + 3]}");
            }

            byte b = (byte)((highBits << 6) | (midBits << 4) | (lowBits1 << 2) | lowBits2);
            jsonBytes[i / 4] = b;
        }

        string json = Encoding.UTF8.GetString(jsonBytes);
        Debug.Log("Decoded JSON: " + json);

        return json;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SaveManager saveManager = (SaveManager)target;

            GUILayout.Space(10);
            GUILayout.Label("Save Manager Controls", EditorStyles.boldLabel);

            if (GUILayout.Button("Delete Save File"))
            {
                saveManager.DeleteDataFile();
            }

            if (GUILayout.Button("Reset to Default"))
            {
                saveManager.ResetToDefault();
            }

            GUILayout.Space(10);
            DrawDefaultInspector();
        }
    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }

    public class ReadOnlyAttribute : PropertyAttribute { }
#endif
}
