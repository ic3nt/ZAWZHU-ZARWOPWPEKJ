using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FirstOpenSceneManager : MonoBehaviour
{
    public RectTransform toggleAgreeRectTransform;
    public RectTransform buttonAgreeRectTransform;

    public Toggle toggleAgree;
    public Button buttonAgree;

    public DemoLoadScene loadScene;
    public Animator animator;

    public float middleTogglePosX, rightTogglePosX;
    public float downButtonPosY, topButtonPosY;
    public float tweenDuration;

    void Start()
    {
        toggleAgreeRectTransform.DOAnchorPosX(middleTogglePosX, tweenDuration);
        buttonAgreeRectTransform.DOAnchorPosY(downButtonPosY, tweenDuration);
        buttonAgree.interactable = false;

        toggleAgree.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            buttonAgree.interactable = true;
            toggleAgreeRectTransform.DOAnchorPosX(rightTogglePosX, tweenDuration);
            buttonAgreeRectTransform.DOAnchorPosY(topButtonPosY, tweenDuration);
        }
        else
        {
            buttonAgree.interactable = false;
            toggleAgreeRectTransform.DOAnchorPosX(middleTogglePosX, tweenDuration);
            buttonAgreeRectTransform.DOAnchorPosY(downButtonPosY, tweenDuration);
        }
    }

    public void SelectLocalizationAnimation()
    {
        animator.SetTrigger("IsSelectLocalizationTrigger");
    }
    
    public void EndAnimation()
    {
        animator.SetTrigger("IsEndTrigger");
    }

    public void GoToMenuScene()
    {
        loadScene.LoadScene("IsMenuScene");
    }
}
