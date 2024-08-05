using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damageEn;
    public float cooldownTime;
    private float nextAttackTime = 0f;
    public Collider HitColl;

    void OnTriggerStay(Collider other)
    {
        if (Time.time >= nextAttackTime && other.CompareTag("Player"))
        {
            if (other.TryGetComponent<HealthManager>(out var hp))
            {
                hp.TakeDamage(damageEn);
                nextAttackTime = Time.time + cooldownTime;
            }
        }
    }
}
