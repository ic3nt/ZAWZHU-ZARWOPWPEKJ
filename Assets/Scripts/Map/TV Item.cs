using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVItem : MonoBehaviour
{
    public GameObject video;

    [Range(0, 1)]
    public float ChanceOfgo = 1f;

    
    private void Start()
    {

        video.SetActive(false);
        if (Random.value > ChanceOfgo)
        {
            StartCoroutine(Go());
        }
      
    }

    public IEnumerator Go()
    {
      
        yield return new WaitForSeconds(Random.Range(20,60));
        video.SetActive(true);


    }
}