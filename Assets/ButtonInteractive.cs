using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractive : Interactable
{
    [Header("Event On Interact")]
    [SerializeField] private UnityEvent onInteract;

    [Header("Animator")]
    [SerializeField] private Animator buttonAnimator;

    public override void Interact()
    {
        Debug.Log("Button clicked!");
        onInteract?.Invoke();

        if (buttonAnimator != null)
            buttonAnimator.SetTrigger("Click");
    }
}
