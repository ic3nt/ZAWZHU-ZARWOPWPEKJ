using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Forester : MonoBehaviour
{
    private Transform closestPlayer;

    private GameObject closestPlayerObj;

    private NavMeshAgent aiAgent;

    public EnemyDamage dmg;

    void Start()
    {
        aiAgent = gameObject.GetComponent<NavMeshAgent>();
        aiAgent.isStopped = true;
    }

    private void Update()
    {
        if (closestPlayer == null || !closestPlayer.gameObject.activeInHierarchy)
        {
            FindClosestPlayer();
        }

        if (closestPlayer != null)
        {
            aiAgent.SetDestination(closestPlayer.position);
        }

        if (closestPlayerObj != null)
        {
            Debug.Log("Closest player distance: " + Vector3.Distance(transform.position, closestPlayerObj.transform.position));
        }

        GameObject playerfov = GameObject.FindGameObjectWithTag("FovPlr");
        FieldOfView fov = playerfov.GetComponent<FieldOfView>();

        if (fov.canSee)
        {
            dmg.HitColl.enabled = false;
            aiAgent.isStopped = true;
        }
        else
        {
            dmg.HitColl.enabled = true;
            aiAgent.isStopped = false;
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistanceSqr = Mathf.Infinity;

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
