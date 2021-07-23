using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

// class used to be able to access the animator component of the Actor, because the method GetComponent<>() is from the class MonoBehaviour
public class ActorMonoBehavior : MonoBehaviour
{
    public int idActor;
    public int numActor;

    public bool isHuman;

    private Animator animator;

    // name of pre-recorded animation to play if the actor is a human
    public string nameAnimationToPlay;

    // controllers to play victory or defeat animation after voting phase
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

    // method used to be able to order a list of ActorMonoBehavior based on their idActor
    public int Compare(Actor x, Actor y)
    {
        return x.idActor.CompareTo(y.idActor);
    }
}

// class used to be able to order a list of ActorMonoBehavior based on their idActor
class ActorMonoBehaviorComparer : IComparer<ActorMonoBehavior>
{
    public int Compare(ActorMonoBehavior x, ActorMonoBehavior y)
    {
        return x.idActor.CompareTo(y.idActor);
    }
}