using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mimic : MonoBehaviour
{
    private Transform closestPlayer;
    public EnemyDamage dmg;

    private NavMeshAgent aiAgent;

    public Animator an;

    public AudioSource aud1;
    public AudioSource aud2;
    public AudioSource aud3;
    public AudioSource aud4;


    void Start()
    {
        aiAgent = GetComponent<NavMeshAgent>();
        aiAgent.stoppingDistance = 3;
        aiAgent.speed = 4;
        dmg.HitColl.enabled = false;
        StartCoroutine(Go());
        aiAgent.isStopped = false;
    }

  
    void Update()
    {
        an.SetBool("IsMove", aiAgent.velocity.x != 0 || aiAgent.velocity.z != 0);
        aud1.enabled = aiAgent.velocity.x != 0 || aiAgent.velocity.z != 0;

        if (closestPlayer == null || !closestPlayer.gameObject.activeInHierarchy)
        {
            FindClosestPlayer();
        }


        if (closestPlayer != null)
        {
            aiAgent.SetDestination(closestPlayer.position);
        }
    }

    public IEnumerator Go()
    {
        yield return new WaitForSeconds(Random.Range(12, 16));
        aiAgent.isStopped = true;
        aiAgent.stoppingDistance = 0.8f;
        aiAgent.speed = 14;
        an.SetTrigger("Agressive");
        aud2.Play();
        yield return new WaitForSeconds(1);
        aud3.Play();
        aud4.Play();
        aiAgent.isStopped = false;
        dmg.HitColl.enabled = true;
       

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
