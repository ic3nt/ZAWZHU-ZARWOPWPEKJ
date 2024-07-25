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

    public void PlayButton()
    {
        animator.SetTrigger("PlayMenu");
    }
    public void QuitButton()
    {
        animator.SetTrigger("PlayMenu");
    }
    void Update()
    {

    }
}
