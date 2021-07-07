using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class ActorMonoBehavior : MonoBehaviour
{
    Animator animator;
    public int id;

    public string nameAnimationToPlay;
    public AnimatorController victoryAnimatorController;
    public AnimatorController defeatedAnimatorController;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void PlayAnimation()
    {
        animator.Rebind();
        animator.Play(nameAnimationToPlay, -1, 0f);
    }

    public bool IsPlayingAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(nameAnimationToPlay) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1;
    }

    public void PlayVictory()
    {
        animator.runtimeAnimatorController = victoryAnimatorController;
        animator.enabled = true;
        animator.Rebind();
        animator.Play("Victory", -1, 0f);
    }

    public void PlayDefeat()
    {
        animator.runtimeAnimatorController = defeatedAnimatorController;
        animator.enabled = true;
        animator.Rebind();
        animator.Play("Defeated", -1, 0f);
    }

    public bool IsPlayingWinning()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Victory") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1;
    }
    public int Compare(Actor x, Actor y)
    {
        return x.id.CompareTo(y.id);
    }
}

class ActorMonoBehaviorComparer : IComparer<ActorMonoBehavior>
{
    public int Compare(ActorMonoBehavior x, ActorMonoBehavior y)
    {
        return x.id.CompareTo(y.id);
    }
}