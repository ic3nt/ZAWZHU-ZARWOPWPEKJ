using System;
using UnityEngine;

public class EnemySpawner : Enemy
{
    public Enemy[] enemyList;
    public int maxSpawnPower;

    public Transform[] spawnpoints;

    private System.Random random = new System.Random();

    public Chunkk chunkk;

    private void Start()
    {
        if (chunkk.currentenflr >= 0)
        {
            maxSpawnPower = 0;

            if (chunkk.currentenflr > 5)
            {
                maxSpawnPower = 2;

                if (chunkk.currentenflr > 10)
                {
                    maxSpawnPower = 4;
                    if (chunkk.currentenflr > 15)
                    {
                        maxSpawnPower = 8;
                        if (chunkk.currentenflr > 20)
                        {
                            maxSpawnPower = 10;
                            if (chunkk.currentenflr > 30)
                            {
                                maxSpawnPower = 18;

                                if (chunkk.currentenflr > 50)
                                {
                                    maxSpawnPower = 20;
                                    if (chunkk.currentenflr > 70)
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
            Transform randompoint = spawnpoints[random.Next(0, spawnpoints.Length)];
            Enemy randomEnemy = enemyList[random.Next(0, enemyList.Length)];
            Instantiate(randomEnemy.enemyPrefab, randompoint.position, Quaternion.identity);
            currentSpawnPower += randomEnemy.spawnPower;
        }
    }
}