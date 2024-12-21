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

    [Space]
    public DemoLoadScene transitionManager;
    public Animator animator;

    [Space]
    public float middleTogglePosX, rightTogglePosX;
    public float downButtonPosY, topButtonPosY;
    public float tweenDuration;

    [Space]
    private Camera cameraToRotate;
    public float rotationSpeed = 10.0f; 

    private float rotationY = 0f;
    private float originalY;

    void Update()
    {
        rotationY += rotationSpeed * Time.deltaTime;

        if (rotationY >= 360f)
        {
            rotationY -= 360f;
        }

        cameraToRotate.transform.rotation = Quaternion.Euler(0, rotationY, 0);

        Vector3 newPosition = cameraToRotate.transform.position;
        cameraToRotate.transform.position = newPosition;
    }
    void Start()
    {
        toggleAgreeRectTransform.DOAnchorPosX(middleTogglePosX, tweenDuration);
        buttonAgreeRectTransform.DOAnchorPosY(downButtonPosY, tweenDuration);
        buttonAgree.interactable = false;

        toggleAgree.onValueChanged.AddListener(OnToggleValueChanged);

        cameraToRotate = Camera.main;
        originalY = cameraToRotate.transform.position.y;

        if (transitionManager == null)
        {
            GameObject managerObject = GameObject.FindWithTag("TransitionManager");
            if (managerObject != null)
            {
                transitionManager = managerObject.GetComponent<DemoLoadScene>();
                Debug.Log("TransitionManager automatically assigned.");
            }
            else
            {
                Debug.LogError("No object with tag 'TransitionManager' found in the scene!");
            }
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            buttonAgree.interactable = true;
            toggleAgreeRectTransform.DOAnchorPosX(rightTogglePosX, tweenDuration);
            buttonAgreeRectTransform.DOAnchorPosY(topButtonPosY, tweenDuration);
            PlayerPrefs.SetInt("isPlayerAgreedPlay", 1);
            PlayerPrefs.Save();
        }
        else
        {
            buttonAgree.interactable = false;
            toggleAgreeRectTransform.DOAnchorPosX(middleTogglePosX, tweenDuration);
            buttonAgreeRectTransform.DOAnchorPosY(downButtonPosY, tweenDuration);
            PlayerPrefs.SetInt("isPlayerAgreedPlay", 0);
            PlayerPrefs.Save();
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
        transitionManager.LoadScene("IsMenuScene");
    }
}
