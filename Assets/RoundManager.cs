using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("Round Stat")]
    public bool playerInsideElevator;

    [Header("Managers")]
    [SerializeField] private ElevatorController elevatorController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            elevatorController.StartElevatorSequence();
        }
    }
}
