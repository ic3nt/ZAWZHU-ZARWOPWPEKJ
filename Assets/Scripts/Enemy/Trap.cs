using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public ChunkManager chunkManager;

    public GameObject VacuumCl;

    [Range(0, 1)]
    public float ChanceOfStaying;

    private void Start()
    {
        if (chunkManager.currentFloor > 0)
        {
            ChanceOfStaying = 0f;

            if (chunkManager.currentFloor > 5)
            {
                ChanceOfStaying = 0.3f;

                if (chunkManager.currentFloor > 20)
                {
                    ChanceOfStaying = 0.8f;

                    if (chunkManager.currentFloor > 30)
                    {
                        ChanceOfStaying = 0.9f;

                        if (chunkManager.currentFloor > 40)
                        {
                            ChanceOfStaying = 1f;

                        }
                    }
                }
            }
        }


        if (Random.value < ChanceOfStaying)
        {
            Instantiate(VacuumCl, transform.position, transform.rotation);
            VacuumCl.SetActive(true);
        }

    }


}
