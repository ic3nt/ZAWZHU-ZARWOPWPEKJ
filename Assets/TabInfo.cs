using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabInfo : MonoBehaviour
{
    public GameObject TimerText;
    public GameObject PlayersFrontsWindow;

    void Start()
    {
        TimerText.SetActive(false);
        PlayersFrontsWindow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            TimerText.SetActive(true);
            PlayersFrontsWindow.SetActive(true);
        }
        else
        {
            TimerText.SetActive(false);
            PlayersFrontsWindow.SetActive(false);
        }
    }
}
