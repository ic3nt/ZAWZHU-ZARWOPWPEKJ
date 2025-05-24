using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaskItemInteractive : MonoBehaviour, IInteract
{
    public void Interact()
    {
        Debug.Log("Interact");
        RoundEvents.InvokeTaskCompleted(true);
    }
}
