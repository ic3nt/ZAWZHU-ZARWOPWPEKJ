using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Floor : NetworkBehaviour
{
    public TextMeshPro floorCounterText; // Для отображения текущего этажа

    public Transform begin;
    public Transform end;

    public int currentFloor;

    public int floor;

    public int GetPrefabIndex()
    {
        // Получаем индекс чанка среди всех префабов
        var chunkPlacer = FindObjectOfType<FloorsPlacer>();
        for (int i = 0; i < chunkPlacer.ChunkPrefabs.Length; i++)
        {
            if (chunkPlacer.ChunkPrefabs[i] == this)
            {
                return i;
            }
        }

        return -1; // Ошибка
    }

    void Start()
    {
        // Обновляем UI с этажом
        UpdateFloorText();
    }

    // Метод для обновления UI текста этажа
    public void UpdateFloorText()
    {
        if (floorCounterText != null)
        {
            floorCounterText.text = (floor - 1) + " ↓";
        }
    }
}
