using System;
using UnityEngine;
using UnityEngine.Events;

public class AnimationOnMouseEnter : MonoBehaviour
{
    private Animator animator;
    public string animationTriggerMouseEnterName;
    public string animationTriggerMouseExitName;
    public bool otherAnimatorLink = false;
    public Animator otherAnimator;
    public string otherAnimationTriggerMouseExitName;
    public string otherAnimationTriggerMouseEnterName;

    
    public UnityEvent OnMouseDownEvent;

    // тут вообще все просто, думаю объяснения не нужны, мне вот еще интересно для кого я все это пишу ¯\_(ツ)_/¯

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

        if (otherAnimatorLink)
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
        if (otherAnimatorLink)
        {
            otherAnimator.SetBool(otherAnimationTriggerMouseExitName, true);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Click");
        OnMouseDownEvent?.Invoke();
    }
}
