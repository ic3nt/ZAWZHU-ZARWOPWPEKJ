using Discord;
using RKS.DD.Core.Managers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class ButtonInteractive : Interactable
{
    [Header("Event On Interact")]
    [SerializeField] private UnityEvent onInteract;

    [Header("Animator")]
    [SerializeField] private Animator buttonAnimator;

    private AudioManager audioManager;

    [Inject]
    public void Construct(AudioManager audio)
    {
        audioManager = audio;
    }

    public override void Interact()
    {
        Debug.Log("Button clicked!");

        onInteract?.Invoke();

        if (buttonAnimator != null)
            buttonAnimator.SetTrigger("Click");
    }
}
