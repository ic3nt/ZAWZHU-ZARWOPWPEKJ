using UnityEngine;

public class TaskItemInteractive : Interactable
{
    public override void Interact()
    {
        Debug.Log("Task item interacted!");
        RoundEvents.InvokeTaskCompleted(true);
    }
}
