using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Random = UnityEngine.Random;
using UnityEngine;



public class Kidatbombs : MonoBehaviour
{
    public AudioSource aus;

    public Transform spawnPoint;

    public float throwForce;

    public GameObject Bomb;


    public GameObject particalExplosion;

    private void Start()
    {
        StartCoroutine(Wait());
    }

    private void ThrowBomb()
    {
        aus.Play();
        GameObject bomb = Instantiate(Bomb, spawnPoint.transform.position, spawnPoint.rotation);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        StartCoroutine(Wait1());

    }


    public IEnumerator Wait ()
    {
        yield return new WaitForSeconds(Random.Range(0.4f, 0.8f));
        ThrowBomb();
        StartCoroutine(Wait());
    }

    public IEnumerator Wait1()
    {
        particalExplosion.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        particalExplosion.SetActive(false);
    }


}

