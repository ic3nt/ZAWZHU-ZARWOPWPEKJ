using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class ChunkManager : NetworkBehaviour
{
    public TextMeshPro floorCounterText;

    public Transform Begin;
    public Transform End;
    public int currentFloor;

    public int Floor;

    public int GetPrefabIndex()
    {
        // Предполагается, что вы используете индекс массива ChunkPrefabs для определения индекса
        // Если это не так, адаптируйте метод соответственно
        for (int i = 0; i < FindObjectOfType<ChunksPlacer>().ChunkPrefabs.Length; i++)
        {
            if (FindObjectOfType<ChunksPlacer>().ChunkPrefabs[i] == this)
            {
                return i;
            }
        }

        // Если не найдено, вернуть -1 или другое значение, уведомляющее об ошибке
        return -1; // Важно! Замените это на ваше значение по умолчанию или используйте механизм обработки ошибок.
    }


    void Start()
    {
        floorCounterText.text = (Floor - 1) + " ↓";
    }
}