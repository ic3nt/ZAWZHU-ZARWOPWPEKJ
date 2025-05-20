using System;
using UnityEngine;

public class EnemySpawner : Enemy
{
    public Enemy[] enemyList;
    public int maxSpawnPower;

    public Transform[] spawnPoints;

    private System.Random random = new System.Random();

    public Chunk chunkManager;

    private void Start()
    {
        if (chunkManager.floorNumber >= 0)
        {
            maxSpawnPower = 0;

            if (chunkManager.floorNumber > 5)
            {
                maxSpawnPower = 2;

                if (chunkManager.floorNumber > 10)
                {
                    maxSpawnPower = 4;
                    if (chunkManager.floorNumber > 15)
                    {
                        maxSpawnPower = 8;
                        if (chunkManager.floorNumber > 20)
                        {
                            maxSpawnPower = 10;
                            if (chunkManager.floorNumber > 30)
                            {
                                maxSpawnPower = 18;

                                if (chunkManager.floorNumber > 50)
                                {
                                    maxSpawnPower = 20;
                                    if (chunkManager.floorNumber > 70)
                                    {
                                        maxSpawnPower = 30;
                                    }
                                }

                            }

                        }
                    }
                }
            }
        }
        
    }

    

    public void SpawnEnemies()
    {
        int currentSpawnPower = 0;
        while (currentSpawnPower < maxSpawnPower)
        {
            Transform randompoint = spawnPoints[random.Next(0, spawnPoints.Length)];
            Enemy randomEnemy = enemyList[random.Next(0, enemyList.Length)];
            Instantiate(randomEnemy.enemyPrefab, randompoint.position, Quaternion.identity);
            currentSpawnPower += randomEnemy.spawnPower;
        }
    }
}