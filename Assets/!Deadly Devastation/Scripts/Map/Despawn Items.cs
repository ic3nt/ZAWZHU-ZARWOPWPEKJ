using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DespawnItems : MonoBehaviour
{

    public GameObject Items;



    void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("Player"))
        {
            Destroy(Items);

        }
     
    }
}
