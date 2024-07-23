using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWithWait : MonoBehaviour
{
    public float DestTime;

    public GameObject DestObj;

    void Start()
    {
        StartCoroutine(Destroy());
    }

    public IEnumerator Destroy()
    {
 
        yield return new WaitForSeconds(DestTime);
        Destroy(DestObj);

    }

   
}
