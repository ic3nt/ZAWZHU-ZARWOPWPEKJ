using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private ElevatorController elevatorController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            elevatorController.StartElevatorSequence();
        }
    }
}
