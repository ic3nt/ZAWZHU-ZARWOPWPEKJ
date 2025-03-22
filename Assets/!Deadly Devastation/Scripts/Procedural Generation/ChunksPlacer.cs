using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Unity.Netcode;

public class ChunksPlacer : NetworkBehaviour
{
    private Transform lowestPlayer;
    public ChunkManager[] ChunkPrefabs;
    public ChunkManager TenthFloorPrefab;
    public ChunkManager FirstChunk;
    public int totalFloors; // Переименовано в totalFloors для ясности
    private int currentFloor = 0; // Изменено имя переменной на currentFloor

    private List<ChunkManager> spawnedChunks = new List<ChunkManager>();

    private void Start()
    {
        // Добавить первый кусок (chunk) и обновить текущее значение этажа
        ChunkManager firstChunk = Instantiate(FirstChunk);
        spawnedChunks.Add(firstChunk);
        currentFloor = 1; // Первый этаж создан
    }

    private void Update()
    {
        if (lowestPlayer == null || !lowestPlayer.gameObject.activeInHierarchy)
        {
            FindLowestPlayerByYAxis();
        }

        // Проверяем, можно ли спавнить новый кусок (chunk)
        if (currentFloor < totalFloors && IsServer)
        {
            if (lowestPlayer.position.y < spawnedChunks[spawnedChunks.Count - 1].End.position.y + 10)
            {
                SpawnChunk();
            }
        }
    }

    void FindLowestPlayerByYAxis()
    {
        if (!IsServer) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.Log("Нет игроков с тегом 'Player'.");
            return;
        }

        lowestPlayer = players[0].transform; // предположим, что первый игрок - самый низкий

        foreach (GameObject player in players)
        {
            if (player.transform.position.y < lowestPlayer.position.y)
            {
                lowestPlayer = player.transform;
            }
        }

        Debug.Log("Самый низкий игрок: " + lowestPlayer.name + " на высоте Y: " + lowestPlayer.position.y);
    }

    private void SpawnChunk()
    {
        int newChunkIndex = UnityEngine.Random.Range(0, ChunkPrefabs.Length);
        ChunkManager newChunk;

        // Определяем, какой кусок (chunk) спавнить
        if (currentFloor % 10 == 0) // Если текущий этаж кратен 10, используем TenthFloorPrefab
        {
            newChunk = Instantiate(TenthFloorPrefab);
        }
        else
        {
            newChunk = Instantiate(ChunkPrefabs[newChunkIndex]);
        }

        // Устанавливаем позицию нового куска (chunk)
        newChunk.transform.position = spawnedChunks[spawnedChunks.Count - 1].End.position - newChunk.Begin.localPosition;

        // Установка вращения нового куска (chunk)
        newChunk.transform.rotation = (currentFloor % 2 == 0) ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.Euler(0f, 0f, 0f);

        // Обновляем значения текущего этажа куска (chunk)
        newChunk.currentFloor = currentFloor;
        newChunk.Floor = totalFloors - currentFloor;

        // Добавление нового куска (chunk) в список
        spawnedChunks.Add(newChunk);
        currentFloor++;

        // Удаляем старый кусок (chunk), если их больше 6
        if (spawnedChunks.Count > 6)
        {
            Destroy(spawnedChunks[0].gameObject);
            spawnedChunks.RemoveAt(0);
        }
    }
}

