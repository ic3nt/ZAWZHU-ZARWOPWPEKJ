using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnMouseEnter : MonoBehaviour
{
    private Animator animator;
    public string animationTriggerMouseEnterName;
    public string animationTriggerMouseExitName;
    public bool otherAnimatorLink = false;
    public Animator otherAnimator;
    public string otherAnimationTriggerMouseExitName;
    public string otherAnimationTriggerMouseEnterName;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnMouseEnter()
    {
        if (animator != null)
        {
            animator.SetBool(animationTriggerMouseEnterName, true);
        }

        if (otherAnimatorLink == true)
        {
            otherAnimator.SetBool(otherAnimationTriggerMouseEnterName, true);
        }
    }

    private void OnMouseExit()
    {
        if (animator != null)
        {
            animator.SetBool(animationTriggerMouseExitName, true);
        }
        if (otherAnimatorLink == true)
        {
            otherAnimator.SetBool(otherAnimationTriggerMouseExitName, true);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Click");
    }
}
