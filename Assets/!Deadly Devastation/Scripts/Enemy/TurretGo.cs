using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretGo : MonoBehaviour
{

    private Transform closestPlayer;

    public Animator an;


    private NavMeshAgent aiAgent;

    void Start()
    {
       aiAgent = gameObject.GetComponent<NavMeshAgent>();

    }


    void Update()
    {
        an.SetBool("iswalk", aiAgent.velocity.x != 0 || aiAgent.velocity.z != 0);

        if (closestPlayer == null || !closestPlayer.gameObject.activeInHierarchy)
        {
            FindClosestPlayer();
        }


        if (closestPlayer != null)
        {
            aiAgent.SetDestination(closestPlayer.position);
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


}
