using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatalogManager : MonoBehaviour
{
    [Header("Manual")]

    [Header("Buttons Monter Manual")]

    public GameObject ToyRobotButton;

    [Header("Window Monster Manual")]

    public GameObject ToyRobotWindow;

    [Header("Enemy")]

    public bool ToyRobotBool;

    void Start()
    {
        ToyRobotBool = PlayerPrefs.GetInt("ToyRobotBool") != 1;

        if (ToyRobotBool == true)
        {
            ToyRobotButton.SetActive(true);
            PlayerPrefs.SetInt("ToyRobot", (ToyRobotBool ? 0 : 1));
        }
        else 
        {
            ToyRobotButton.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}
