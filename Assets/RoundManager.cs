using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("Round Stat")]
    public bool playerInsideElevator;
    public bool taskCompleted = false;

    [Header("Managers")]
    [SerializeField] private RKS.DD.Game.ElevatorController elevatorController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            elevatorController.StartSequence();
        }
    }
}
