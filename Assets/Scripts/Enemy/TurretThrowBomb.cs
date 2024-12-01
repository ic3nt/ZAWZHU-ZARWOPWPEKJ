using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Random = UnityEngine.Random;
using UnityEngine;



public class TurretThrowBomb: MonoBehaviour
{
    public AudioSource audioSource;

    public Transform spawnPoint;

    public float throwForce;

    public GameObject bombObject;

    public GameObject partialExplosion;

    private void Start()
    {
        StartCoroutine(WaitOne());
    }

    private void ThrowBomb()
    {
        audioSource.Play();
        GameObject bomb = Instantiate(bombObject, spawnPoint.transform.position, spawnPoint.rotation);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        StartCoroutine(WaitTwo());
    }


    public IEnumerator WaitOne()
    {
        yield return new WaitForSeconds(Random.Range(0.4f, 0.8f));
        ThrowBomb();
        StartCoroutine(WaitOne());
    }

    public IEnumerator WaitTwo()
    {
        partialExplosion.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        partialExplosion.SetActive(false);
    }


}

