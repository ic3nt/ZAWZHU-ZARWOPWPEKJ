using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BrokenMimic : MonoBehaviour
{
    private Transform closestPlayer;

    public Animator an;

    public AudioSource aus1;
    public AudioSource aus2;
  
    public AudioSource aus4;

    public bool IsBoss;

    public float brokeT;

    public EnemDamage dg;

    public Collider coll;

    private NavMeshAgent aiAgent;

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


    void Start()
    {
        if(IsBoss)
        {
            aus1.Play();
        }
        

        aus2.Play();
     
        StartCoroutine(Broke());
        aiAgent = gameObject.GetComponent<NavMeshAgent>();
        aiAgent.isStopped = false;
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

    IEnumerator Broke()
    {
        yield return new WaitForSeconds(brokeT);
        aiAgent.isStopped = true;
        an.SetTrigger("Broke");
        aus4.Play();
        aus2.Stop();
        dg.HitColl.enabled = false;
        coll.enabled = false;
    }
}
