using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractive : MonoBehaviour, IInteract
{
    [Header("Event On Interact")]
    [Space(10)]
    [SerializeField] private UnityEvent onInteract;

    [Header("Animator")]
    [Space(10)]
    [SerializeField] private Animator buttonAnimator;
    public void Interact()
    {
        Debug.Log("Interact");
        onInteract?.Invoke();

        buttonAnimator.SetTrigger("Click");
    }
}
