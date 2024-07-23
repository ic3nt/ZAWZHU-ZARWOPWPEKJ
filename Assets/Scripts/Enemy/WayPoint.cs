using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{


    [SerializeField]
    protected float debugDrawRadius;

    public virtual void OnDrawGozmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, debugDrawRadius);

    }
}
