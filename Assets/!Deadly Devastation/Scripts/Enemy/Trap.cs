using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public Chunk chunk;

    public GameObject VacuumCl;

    [Range(0, 1)]
    public float ChanceOfStaying;

    private void Start()
    {
        if (chunk.floorNumber > 0)
        {
            ChanceOfStaying = 0f;

            if (chunk.floorNumber > 5)
            {
                ChanceOfStaying = 0.3f;

                if (chunk.floorNumber > 20)
                {
                    ChanceOfStaying = 0.8f;

                    if (chunk.floorNumber > 30)
                    {
                        ChanceOfStaying = 0.9f;

                        if (chunk.floorNumber > 40)
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
