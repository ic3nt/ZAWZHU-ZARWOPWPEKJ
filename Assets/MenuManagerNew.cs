using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MenuManagerNew : MonoBehaviour
{
    public Animator animator;
    public Animator animatorSettings;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuButton();
        }
        
    }

    public void MenuButton()
    {
        animator.SetTrigger("DefaultMenu");
        animatorSettings.SetTrigger("Close");
    }
    public void PlayButton()
    {
        animator.SetTrigger("PlayMenu");
        animatorSettings.SetTrigger("Close");
    }
    public void QuitButton()
    {
        animator.SetTrigger("QuitMenu");
        animatorSettings.SetTrigger("Close");
    }
    public void SettingsButton()
    {
        animator.SetTrigger("SettingsMenu");
        animatorSettings.SetTrigger("Open");
    }
}
