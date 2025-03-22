using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusPoints : MonoBehaviour
{
    public ChunkManager chunkManager;

    public EnemySpawner enemySpawner;

    public Collider collider;

    public GameObject doorWork;

    public GameObject doorLock;

    private int plusPoints;

    void Start()
    {
        doorLock.SetActive(false);
        doorWork.SetActive(true);

        if (chunkManager.currentFloor > 0)
        {
            plusPoints = Random.Range(4, 25);

            if (chunkManager.currentFloor > 10)
            {
                plusPoints = Random.Range(28, 43);

                if (chunkManager.currentFloor > 20)
                {
                    plusPoints = Random.Range(45, 59);

                    if (chunkManager.currentFloor > 30)
                    {
                        plusPoints = Random.Range(60, 74);

                        if (chunkManager.currentFloor > 40)
                        {
                            plusPoints = Random.Range(88, 160);

                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerPointsInRound>(out var pp))
            {
                pp.currfloor += 1;
                pp.points += plusPoints;
               

            }

            doorLock.SetActive(true);
            doorWork.SetActive(false);

            collider.enabled = false;
           

            enemySpawner.SpawnEnemies();


        }
    }
}