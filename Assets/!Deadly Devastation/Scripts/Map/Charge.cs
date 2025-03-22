using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    public float chargeam;
    public float cooldownTime; // Время кулдауна в секундах
    private float nextAttackTime = 0f; // Время следующего удара
    public Collider chargeoll;

    public Transform PartPos;

    public GameObject particles;

    public AudioSource aus;

    void OnTriggerStay(Collider other)
    {
        if (Time.time >= nextAttackTime && other.CompareTag("Player"))
        {
            if (other.TryGetComponent<HealthManager>(out var hp))
            {
                if (hp.healthAmount < 100)
                {
                    Instantiate(particles, PartPos.position, PartPos.rotation);
                    hp.Heal(chargeam);
                    aus.Play();
                    nextAttackTime = Time.time + cooldownTime; // Устанавливаем время следующего удара с учетом кулдауна
                }
              
              

                
            }
        }
    }
}
