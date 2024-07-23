using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDamage : MonoBehaviour
{
    public AudioSource aus;
    public float damageB;
    public GameObject part;
    public GameObject vac;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<HealthManager>(out var hp))
            {
                hp.TakeDamage(damageB);
                aus.Play();
                Instantiate(part, gameObject.transform.position, gameObject.transform.rotation);
                Destroy(vac);
               
            }
        }
    }
}
