using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToTheHell : MonoBehaviour
{
    public GameObject Cheat;
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Instantiate(Cheat, transform.position, transform.rotation);
        }
    }
}
