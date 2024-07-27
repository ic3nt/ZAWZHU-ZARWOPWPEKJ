using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVItem : MonoBehaviour
{
    public GameObject video;

    private void Start()
    {
        video.SetActive(false);
        StartCoroutine(Go());
    }

    public IEnumerator Go()
    {
      
        yield return new WaitForSeconds(Random.Range(5,60));
        video.SetActive(true);


    }
}