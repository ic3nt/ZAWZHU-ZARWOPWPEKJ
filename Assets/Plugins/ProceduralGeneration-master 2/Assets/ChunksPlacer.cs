using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Unity.AI.Navigation;

public class ChunksPlacer : MonoBehaviour
{
    public Transform Player;
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
        if ((curflr < floors))
        {
            if (Player.position.y < spawnedChunks[spawnedChunks.Count - 1].End.position.y + 10)
            {
                SpawnChunk();
            }
        }
    }

    private void SpawnChunk()
    {
        curflr += 1;
        Chunkk newChunk;

        if (curflr == 19) // Check if it's the tenth floor 
        {
            newChunk = Instantiate(TenthFloorPrefab);
        }
        else
        {
            newChunk = Instantiate(ChunkPrefabs[Random.Range(0, ChunkPrefabs.Length)]);
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