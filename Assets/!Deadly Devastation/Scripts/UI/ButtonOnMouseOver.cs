using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Object;

    public void Start()
    {
        Object.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Object.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Object.SetActive(false);
    }
}
