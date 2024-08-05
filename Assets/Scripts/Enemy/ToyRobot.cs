using UnityEngine;
using UnityEngine.AI;


using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Collections;

public class ToyRobot : MonoBehaviour
{
    private Transform closestPlayer;

    public AudioSource aud1;

    public AudioSource aud2;
    public AudioSource aud3;

    private NavMeshAgent aiAgent;

    private Animator anim;

    public EnemyDamage dmg;


    void Start()
    {

        anim = GetComponent<Animator>();
        aiAgent = gameObject.GetComponent<NavMeshAgent>();
        anim.SetBool("IsRun", false);
        aud3.enabled = false;
        StartCoroutine(Stop());

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

    public IEnumerator Stop()
    {
        aud3.enabled = false;
        aud1.Play();
        anim.SetBool("IsRun", false);
        dmg.HitColl.enabled = false;
        aiAgent.isStopped = true;
        yield return new WaitForSeconds(Random.Range(2,5));
        aud1.Stop();

        StartCoroutine(Go());
        

        
    }
    public IEnumerator Go()
    {
        aud3.enabled = true;
        aud2.Play();
        anim.SetBool("IsRun", true);
        dmg.HitColl.enabled = true;
        aiAgent.isStopped = false;

        yield return new WaitForSeconds(Random.Range(1, 2));
        aud2.Stop();

        StartCoroutine(Stop());
       

        


    }

}