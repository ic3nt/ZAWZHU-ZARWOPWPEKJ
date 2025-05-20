using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusPoints : MonoBehaviour
{
    public Chunk chunk;

    public EnemySpawner enemySpawner;

    public Collider collider;

    public GameObject doorWork;

    public GameObject doorLock;

    private int plusPoints;

    void Start()
    {
        doorLock.SetActive(false);
        doorWork.SetActive(true);

        if (chunk.floorNumber > 0)
        {
            plusPoints = Random.Range(4, 25);

            if (chunk.floorNumber > 10)
            {
                plusPoints = Random.Range(28, 43);

                if (chunk.floorNumber > 20)
                {
                    plusPoints = Random.Range(45, 59);

                    if (chunk.floorNumber > 30)
                    {
                        plusPoints = Random.Range(60, 74);

                        if (chunk.floorNumber > 40)
                        {
                            plusPoints = Random.Range(88, 160);

                        }
                    }
                }
            }
        }
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