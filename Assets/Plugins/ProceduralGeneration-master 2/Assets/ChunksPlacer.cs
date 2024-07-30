using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Unity.AI.Navigation;
using Unity.Netcode;

public class ChunksPlacer : NetworkBehaviour
{
    private Transform closestPlayer;
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
        if (closestPlayer == null || !closestPlayer.gameObject.activeInHierarchy)
        {
            FindClosestPlayer();
        }

        if ((curflr < floors) && IsServer)
        {
            if (closestPlayer.position.y < spawnedChunks[spawnedChunks.Count - 1].End.position.y + 10)
            {
                SpawnChunkServerRpc();
            }
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistanceSqr = Mathf.Infinity;
        GameObject closestPlayerObj = null;

        foreach (GameObject player in players)
        {
            Vector3 directionToTarget = player.transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestPlayerObj = player;
            }
        }

        if (closestPlayerObj != null)
        {
            closestPlayer = closestPlayerObj.transform;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnChunkServerRpc()
    {
        if (!IsServer) return;

        curflr++;
        if (curflr >= floors) return; // Early exit

        // Create chunk on the server
        Chunkk newChunk = Instantiate(curflr == 19 ? TenthFloorPrefab :
            ChunkPrefabs[Random.Range(0, ChunkPrefabs.Length)]);

        Vector3 spawnPosition = spawnedChunks[spawnedChunks.Count - 1].End.position - newChunk.Begin.localPosition;
        newChunk.transform.position = spawnPosition;

        newChunk.transform.rotation = (currentRotationIndex % 2 == 0)
            ? Quaternion.Euler(0f, 0f, 0f)
            : Quaternion.Euler(0f, 180f, 0f);

        newChunk.currentenflr = curflr + 1;
        newChunk.floor = floors - newChunk.currentenflr;

        spawnedChunks.Add(newChunk);
        currentRotationIndex++;

        if (spawnedChunks.Count >= 6)
        {
            Destroy(spawnedChunks[0].gameObject);
            spawnedChunks.RemoveAt(0);
        }

        // Spawn the NetworkObject for the chunk
        newChunk.GetComponent<NetworkObject>().Spawn();

        // Notify clients to spawn the new chunk
        SpawnChunkForClientsClientRpc(newChunk.GetComponent<NetworkObject>().NetworkObjectId, newChunk.transform.position, newChunk.transform.rotation);
    }


    [ClientRpc]
    private void SpawnChunkForClientsClientRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
    {
        // Find the prefab associated with the network object
        var prefabToSpawn = (curflr == 19) ? TenthFloorPrefab : ChunkPrefabs[Random.Range(0, ChunkPrefabs.Length)];

        // Instantiate a new chunk for clients
        Chunkk newChunk = Instantiate(prefabToSpawn);

        // Set the position and rotation
        newChunk.transform.position = position;
        newChunk.transform.rotation = rotation;

        // Set the floor information
        newChunk.currentenflr = curflr + 1; // Same as server
        newChunk.floor = floors - newChunk.currentenflr;

        // Add to spawned chunks
        spawnedChunks.Add(newChunk);

        if (spawnedChunks.Count >= 6)
        {
            Destroy(spawnedChunks[0].gameObject);
            spawnedChunks.RemoveAt(0);
        }
    }
}

