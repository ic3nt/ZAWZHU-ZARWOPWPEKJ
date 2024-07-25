using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerNew : MonoBehaviour
{
    public Animator animator;

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
        animator.SetTrigger("QuitMenu");
    }
    public void SettingsButton()
    {
        animator.SetTrigger("SettingsMenu");
    }
    void Update()
    {

    }
}
