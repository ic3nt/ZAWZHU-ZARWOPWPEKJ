using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public Chunkk ch;

    public GameObject VacuumCl;

    [Range(0, 1)]
    public float ChanceOfStaying;

    private void Start()
    {
        if (ch.currentenflr > 0)
        {
            ChanceOfStaying = 0f;

            if (ch.currentenflr > 5)
            {
                ChanceOfStaying = 0.3f;

                if (ch.currentenflr > 20)
                {
                    ChanceOfStaying = 0.8f;

                    if (ch.currentenflr > 30)
                    {
                        ChanceOfStaying = 0.9f;

                        if (ch.currentenflr > 40)
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
