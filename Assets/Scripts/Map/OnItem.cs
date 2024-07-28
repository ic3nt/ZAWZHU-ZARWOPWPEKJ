using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnItem : MonoBehaviour
{
    public GameObject video;

    [Range(0, 1)]
    public float ChanceOfgo = 1f;

    public bool IsOn;
    
    private void Start()
    {
        IsOn = false;
        video.SetActive(false);
        if (Random.value > ChanceOfgo)
        {
            IsOn = true;
            
        }
      
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player")) && IsOn)
        {
            StartCoroutine(Go());
        }
    }

    public IEnumerator Go()
    {
      
        yield return new WaitForSeconds(Random.Range(0.1f,0.6f));
        video.SetActive(true);


    }
}