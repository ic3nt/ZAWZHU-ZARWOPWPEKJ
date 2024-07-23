using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToTheHell : MonoBehaviour
{
   
   
    void Update()
    {
        if ((Input.GetKey(KeyCode.H)) && (Input.GetKey(KeyCode.U)) && (Input.GetKey(KeyCode.N)) && (Input.GetKey(KeyCode.T)) && (Input.GetKey(KeyCode.LeftShift)))
        {
            SceneManager.LoadScene(5);
        }
    }
}
