using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FloorsPlacer : NetworkBehaviour
{
    private Transform lowestPlayer;

    public Floor[] ChunkPrefabs;

    [Space(10)]
    public Floor[] SpecialFloors; // Массив особых этажей, которые могут появляться
    public int specialFloorInterval = 5; // Интервал для появления особых этажей

    [Space(10)]
    public Floor[] BossFloors; // Массив чанков с боссами, которые могут появляться
    public int[] BossFloorLevels = new int[] { 20, 40, 60 }; // Этажи, на которых должны спавниться боссы
    public bool canBossDuplicate = false; // Может ли босс дублироваться (если был убит, может ли он заспавниться снова)
    private HashSet<int> defeatedBosses = new HashSet<int>(); // Множество убитых боссов (если дублирование отключено)

    [Space (10)]
    public Floor[] SecretFloors; // Массив секретных этажей, которые могут появляться случайно
    [Range(0f, 100f)]
    public float secretFloorChance = 10f;

    [Space(10)]
    public Floor FirstChunk;
    public int totalFloors; // Общее количество этажей
    private int currentFloor = 1; // Начальный этаж

    private List<Floor> spawnedChunks = new List<Floor>();

    private void Start()
    {
        // Добавляем первый этаж и обновляем текущее значение этажа
        Floor firstChunk = Instantiate(FirstChunk);
        spawnedChunks.Add(firstChunk);
    }

    private void Update()
    {
        if (lowestPlayer == null || !lowestPlayer.gameObject.activeInHierarchy)
        {
            FindLowestPlayerByYAxis();
        }

        // Спавним новые чанки, если необходимо
        if (currentFloor < totalFloors && IsServer)
        {
            if (lowestPlayer.position.y < spawnedChunks[spawnedChunks.Count - 1].end.position.y + 10)
            {
                SpawnChunk();
            }
        }
    }

    void FindLowestPlayerByYAxis()
    {
        if (!IsServer) return;

        // Находим самого низкого игрока
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.Log("Нет игроков с тегом 'Player'.");
            return;
        }

        lowestPlayer = players[0].transform; // Предположим, что первый игрок - самый низкий

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
        Floor newChunk;

        if (System.Array.Exists(BossFloorLevels, level => level == currentFloor))
        {
            newChunk = SpawnBossFloor();
        }
        else if (currentFloor % specialFloorInterval == 0 && SpecialFloors.Length > 0)
        {
            newChunk = Instantiate(SpecialFloors[UnityEngine.Random.Range(0, SpecialFloors.Length)]);
        }
        else
        {
            newChunk = Instantiate(ChunkPrefabs[newChunkIndex]);
        }

        if (Random.value <= secretFloorChance && SecretFloors.Length > 0)
        {
            newChunk = Instantiate(SecretFloors[UnityEngine.Random.Range(0, SecretFloors.Length)]);
        }

        newChunk.transform.position = spawnedChunks[spawnedChunks.Count - 1].end.position - newChunk.begin.localPosition;

        newChunk.transform.rotation = (currentFloor % 2 == 0) ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.Euler(0f, 0f, 0f);

        newChunk.currentFloor = currentFloor;
        newChunk.floor = totalFloors - currentFloor;

        spawnedChunks.Add(newChunk);
        currentFloor++;

        if (spawnedChunks.Count > 4)
        {
            Destroy(spawnedChunks[0].gameObject);
            spawnedChunks.RemoveAt(0);
        }
    }

    private Floor SpawnBossFloor()
    {
        Floor newChunk = null;

        if (BossFloors.Length > 0)
        {
            List<int> availableBossIndexes = new List<int>();
            for (int i = 0; i < BossFloors.Length; i++)
            {
                if (canBossDuplicate || !defeatedBosses.Contains(i))
                {
                    availableBossIndexes.Add(i);
                }
            }

            if (availableBossIndexes.Count > 0)
            {
                int randomIndex = availableBossIndexes[UnityEngine.Random.Range(0, availableBossIndexes.Count)];
                newChunk = Instantiate(BossFloors[randomIndex]);
                if (!canBossDuplicate)
                {
                    defeatedBosses.Add(randomIndex);
                }
            }
        }

        return newChunk;
    }


    public int GetPlayerFloor(GameObject player)
    {
        float playerYPosition = player.transform.position.y;

        float chunkHeight = spawnedChunks[0].end.position.y - spawnedChunks[0].begin.position.y;
        return Mathf.FloorToInt(playerYPosition / chunkHeight);
    }
}

