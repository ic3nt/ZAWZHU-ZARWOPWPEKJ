using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class PatrolEnemy : MonoBehaviour
{
    public bool patrolWaiting;

    public float totalWaitTime = 3f;

    public float switchProbability = 0.2f;

    public List<WayPoint> patrolPoints;

    NavMeshAgent na;

    int patrind;
    bool travel;
    bool waiting;
    bool patrForw;
    float WaitTimer;



    void Start()
    {
        na = this.GetComponent<NavMeshAgent>();

        if (patrolPoints != null && patrolPoints.Count >= 2)
        {
            patrind = 0;
            SetDestination();
        }


    }

    
    void Update()
    
    {

        if (travel && na.remainingDistance <= 1f)
        {
            travel = false;
            if (patrolWaiting)
            {
                waiting = true;
                WaitTimer = 0;
            }
            else
            {
                ChangePatrolPoint();
                SetDestination();
            }
        }


        if (waiting)
        {

            WaitTimer += Time.deltaTime;
            if (WaitTimer >= totalWaitTime)
            {
                waiting = false;

                ChangePatrolPoint();
                SetDestination();
            }

        }


    }

    private void SetDestination()
    {
        if (patrolPoints != null)
        {
            Vector3 targVec = patrolPoints[patrind].transform.position;

            na.SetDestination(targVec);

            travel = true;
        }

    }



    private void ChangePatrolPoint()
    {

        if (UnityEngine.Random.Range(0,1f) <= switchProbability)
        {
            patrForw = !patrForw;

        }

        if (patrForw)
        {
            patrind = (patrind + 1) % patrolPoints.Count;
        }
        else
        {
            if(-- patrind < 0)
            {
                patrind = patrolPoints.Count - 1;
            }
        }

    }

}
