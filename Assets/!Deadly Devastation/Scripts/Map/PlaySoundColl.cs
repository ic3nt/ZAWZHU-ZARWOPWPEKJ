using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundColl : MonoBehaviour
{
    public AudioSource ausGR;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            
               ausGR.Play();
            
        }
    }

 
}
