using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemDamage : MonoBehaviour
{
    public float damageEn;
    public float cooldownTime; // Время кулдауна в секундах
    private float nextAttackTime = 0f; // Время следующего удара
    public Collider HitColl;

    void OnTriggerStay(Collider other)
    {
        if (Time.time >= nextAttackTime && other.CompareTag("Player"))
        {
            if (other.TryGetComponent<HealthManager>(out var hp))
            {
                hp.TakeDamage(damageEn);
                nextAttackTime = Time.time + cooldownTime; // Устанавливаем время следующего удара с учетом кулдауна
            }
        }
    }
}
