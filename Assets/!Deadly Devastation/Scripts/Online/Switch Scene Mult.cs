using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneMult : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
            StartCoroutine(Kal());
        

       
    }

  

    public IEnumerator Kal()
    {
        yield return new WaitForSeconds(15);
        
    }


}
