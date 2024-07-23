using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;


public class LocalizationFirstOpenSelect : MonoBehaviour
{
    public DemoLoadScene loadScene;
    public Animator animator;
    
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
