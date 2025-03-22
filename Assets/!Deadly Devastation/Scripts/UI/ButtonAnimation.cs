using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    public Button ButtonTarget;
    public Animator ButtonAnimator;

    public void OnPointerClick(PointerEventData eventData)
    {
        ButtonAnimator.SetTrigger("Pressed");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonAnimator.SetTrigger("Highlighted");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ButtonAnimator.SetTrigger("Highlighted");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ButtonAnimator.SetTrigger("Normal");
    }
    public void Update()
    {
        if (ButtonTarget != true) 
        {
            ButtonAnimator.SetTrigger("Normal");
        }
    }
}
