using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusPoints : MonoBehaviour
{
    public Chunkk ch;

    public EnemySpawner es;


    public Collider coll;

    public GameObject doorwork;

    public GameObject doorLock;

    private int plusp;

    void Start()
    {
        doorLock.SetActive(false);
        doorwork.SetActive(true);

        if (ch.currentenflr > 0)
        {
            plusp = Random.Range(4, 25);

            if (ch.currentenflr > 10)
            {
                plusp = Random.Range(28, 43);

                if (ch.currentenflr > 20)
                {
                    plusp = Random.Range(45, 59);

                    if (ch.currentenflr > 30)
                    {
                        plusp = Random.Range(60, 74);

                        if (ch.currentenflr > 40)
                        {
                            plusp = Random.Range(88, 160);

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
                pp.points += plusp;
               

            }

            doorLock.SetActive(true);
            doorwork.SetActive(false);

            coll.enabled = false;
           

            es.SpawnEnemies();


        }
    }
}