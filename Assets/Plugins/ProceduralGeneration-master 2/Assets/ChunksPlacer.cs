using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Unity.Netcode;

public class ChunksPlacer : NetworkBehaviour
{
    private Transform lowestPlayer; // Changed from closestPlayer to lowestPlayer
    public Chunkk[] ChunkPrefabs;
    public Chunkk TenthFloorPrefab;
    public Chunkk FirstChunk;
    public int floors;
    public int curflr = 0;

    private List<Chunkk> spawnedChunks = new List<Chunkk>();
    private int currentRotationIndex = 0;

    private void Start()
    {
        spawnedChunks.Add(FirstChunk);
    }

    private void Update()
    {
        if (lowestPlayer == null || !lowestPlayer.gameObject.activeInHierarchy)
        {
            FindLowestPlayerByYAxis(); // Updated to use the new method
        }

        if ((curflr < floors) && IsServer)
        {
            if (lowestPlayer.position.y < spawnedChunks[spawnedChunks.Count - 1].End.position.y + 10)
            {
                int newChunkind = UnityEngine.Random.Range(0, ChunkPrefabs.Length);
                SpawnChunkClientRpc(newChunkind);
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

        GameObject lowestPlayerObj = players[0]; // предположим, что первый игрок - самый низкий

        // Ищем игрока с минимальным значением по оси Y
        for (int i = 1; i < players.Length; i++)
        {
            if (players[i].transform.position.y < lowestPlayerObj.transform.position.y)
            {
                lowestPlayerObj = players[i];
            }
        }

        // Устанавливаем lowestPlayer
        lowestPlayer = lowestPlayerObj.transform;

        // Выводим информацию о самом низком игроке
        Debug.Log("Самый низкий игрок: " + lowestPlayer.name + " на высоте Y: " + lowestPlayer.transform.position.y);
    }

    [ClientRpc]
    private void SpawnChunkClientRpc(int newChunkind)
    {
        Chunkk newChunk;

        curflr += 1;

        if (curflr == 19) // Check if it's the tenth floor 
        {
            newChunk = Instantiate(TenthFloorPrefab);
        }
        else
        {
            newChunk = Instantiate(ChunkPrefabs[newChunkind]);
        }

        newChunk.transform.position = spawnedChunks[spawnedChunks.Count - 1].End.position - newChunk.Begin.localPosition;

        if (currentRotationIndex % 2 == 0)
        {
            newChunk.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            newChunk.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        newChunk.currentenflr = curflr + 1;
        newChunk.floor = floors - newChunk.currentenflr;

        spawnedChunks.Add(newChunk);
        currentRotationIndex++;

        if (spawnedChunks.Count >= 6)
        {
            Destroy(spawnedChunks[0].gameObject);
            spawnedChunks.RemoveAt(0);
        }
    }
}
