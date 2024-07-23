using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerNew : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
 
    }

    public void MenuButton()
    {
        animator.SetTrigger("DefaultMenu");
    }

    public void SingleplayerButton()
    {
        animator.SetTrigger("SingleplayerMenu");
    }
    void Update()
    {

    }
}
