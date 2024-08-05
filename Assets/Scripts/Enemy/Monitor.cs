using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Monitor : MonoBehaviour
{
    private Transform closestPlayer;

    public Collider TriggerColl;

    private NavMeshAgent aiAgent;

    public EnemyDamage dmg;

    void Start()
    {
        aiAgent = gameObject.GetComponent<NavMeshAgent>();
        aiAgent.isStopped = true;
        dmg.HitColl.enabled = false;
    }


    void Update()
    {

        
            if (closestPlayer == null || !closestPlayer.gameObject.activeInHierarchy)
            {
                FindClosestPlayer();
            }


            if (closestPlayer != null)
            {
                aiAgent.SetDestination(closestPlayer.position);
            }

    }


        void OnTriggerEnter(Collider other)
        {

        if (other.CompareTag("Player"))
            {
                TriggerColl.enabled = false;

               
                aiAgent.isStopped = false;
                TriggerColl.enabled = false;
                StartCoroutine(Broke());
            }


            
        }


        public IEnumerator Broke()
        {
        dmg.HitColl.enabled = true;
        yield return new WaitForSeconds(Random.Range(4, 10));
        aiAgent.isStopped = true;
        dmg.HitColl.enabled = false;


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
