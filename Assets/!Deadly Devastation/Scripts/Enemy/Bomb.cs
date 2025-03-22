using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bomb : MonoBehaviour
{
    public float Radius; 
    public float Force; 
    public AudioSource aus; 
    public GameObject particalExplosion;

    public GameObject ball;

    public GameObject coll;

    void Start()
    { 
   
        StartCoroutine(Boom());
       
    }





    public void Explos()
    {
        ExplosForce();
        aus.Play();
        Instantiate(particalExplosion, transform.position, transform.rotation); 
    }

    public IEnumerator Boom()
    { 
        yield return new WaitForSeconds(3);
        Destroy(ball);
        Instantiate(coll, transform.position, transform.rotation);
        Explos();
     
        

    }


    


    public void ExplosForce()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, Radius); //выставляем радиус проверки объектов для взаимодействия
        foreach (Collider hit in col)
        {
            Rigidbody rg = hit.GetComponent<Rigidbody>(); //задаем переменную компонента rigibody


            if (rg)
            {
                rg.AddExplosionForce(Force, transform.position, Radius, 3f); //для всех объектов в заданом радиусе, с наличием rigidbody применяем силу взрыва


            }
        }
    }
}
