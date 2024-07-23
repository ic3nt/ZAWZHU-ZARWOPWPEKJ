using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using EasyTransition;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public RectTransform LogoRectTransform;
    public RectTransform MenuRectTransform;
    public RectTransform SinglePlayerRectTransform;
    public RectTransform CatalogRectTransform;
    public RectTransform ManualRectTransform;
    public RectTransform SettingsRectTransform;
    public RectTransform CreditsRectTransform;

    public DemoLoadScene loadScene;

    public float topPosY, middlePosY;
    public float leftPosX, middlePosX;
    public float left_onscreenPosX, left_unscreenPosX;
    public float tweenDuration;


    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        MenuRectTransform.DOAnchorPosX(middlePosX, tweenDuration);
        LogoRectTransform.DOAnchorPosX(left_onscreenPosX, tweenDuration);
    }

    void Update()
    {
        
    }

    public void SinglePlayerButton()
    {
        SinglePlayerRectTransform.DOAnchorPosY(middlePosY, tweenDuration);
        MenuRectTransform.DOAnchorPosX(leftPosX, tweenDuration);
        LogoRectTransform.DOAnchorPosX(left_unscreenPosX, tweenDuration);
    }
    public void SinglePlayerPlayButton()
    {
        loadScene.LoadScene("TEST");
    }
    public void CatalogButton()
    {
        CatalogRectTransform.DOAnchorPosY(middlePosY, tweenDuration);
        MenuRectTransform.DOAnchorPosX(leftPosX, tweenDuration);
        LogoRectTransform.DOAnchorPosX(left_unscreenPosX, tweenDuration);
    }
    public void ManualButton()
    {
        ManualRectTransform.DOAnchorPosY(middlePosY, tweenDuration);
        CatalogRectTransform.DOAnchorPosY(topPosY, tweenDuration);
        MenuRectTransform.DOAnchorPosX(leftPosX, tweenDuration);
        LogoRectTransform.DOAnchorPosX(left_unscreenPosX, tweenDuration);
    }
    public void SettingsButton()
    {
        SettingsRectTransform.DOAnchorPosY(middlePosY, tweenDuration);
        MenuRectTransform.DOAnchorPosX(leftPosX, tweenDuration);
        LogoRectTransform.DOAnchorPosX(left_unscreenPosX, tweenDuration);
    }
    public void CreditsButton()
    {
        CreditsRectTransform.DOAnchorPosY(middlePosY, tweenDuration);
        MenuRectTransform.DOAnchorPosX(leftPosX, tweenDuration);
        LogoRectTransform.DOAnchorPosX(left_unscreenPosX, tweenDuration);
    }
    public void ExitToMenuButton()
    {
        SinglePlayerRectTransform.DOAnchorPosY(topPosY, tweenDuration);
        CatalogRectTransform.DOAnchorPosY(topPosY, tweenDuration);
        ManualRectTransform.DOAnchorPosY(topPosY, tweenDuration);
        SettingsRectTransform.DOAnchorPosY(topPosY, tweenDuration);
        CreditsRectTransform.DOAnchorPosY(topPosY, tweenDuration);
        MenuRectTransform.DOAnchorPosX(middlePosX, tweenDuration);
        LogoRectTransform.DOAnchorPosX(left_onscreenPosX, tweenDuration);
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
