using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsPBosss : MonoBehaviour
{
    public AudioSource aus;
    public Transform point;
    public GameObject Boss;



    public Collider coll;
    public GameObject doorwork;

    public GameObject doorLock;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Instantiate(Boss, point.position, point.rotation);
            doorLock.SetActive(true);
            doorwork.SetActive(false);

            coll.enabled = false;

            if (other.TryGetComponent<PlayerPointsInRound>(out var p))
            {
                p.points += 148;


            }

            


        }
    }

}
