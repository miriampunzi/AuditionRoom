using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Animations;
using UnityEngine;

public class Actor : MonoBehaviour, IComparer<Actor>
{
    public int idPerformance;     // id actor in the performances (runtime)
    public int idActor;    // num of actor (static)

    public bool isHuman;
    public int gender;

    private Animator animator;

    // controllers to play victory or defeat animation after voting phase
    public AnimatorController victoryAnimatorController;
    public AnimatorController defeatedAnimatorController;
    public AnimatorController idleAnimatorController;

    public AnimatorController animationToPlay;
    // name of pre-recorded animation to play if the actor is a human
    public string nameAnimationToPlay;

    [HideInInspector]
    public Vector3 initialPosition;
    [HideInInspector]
    public Quaternion initialRotation;

    public TrapdoorCover trapdoorCover;

    // different ML brains
    [HideInInspector]
    public RightArmAgent rightArmAgent;
    [HideInInspector]
    public LeftArmAgent leftArmAgent;
    [HideInInspector]
    public HeadChestAgent headChestAgent;

    private void Start()
    {
        GetAgents(transform);
        animator = GetComponent<Animator>();
    }

    public void GetAgents(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            switch (child.tag)
            {
                case "RightTarget": rightArmAgent = child.transform.GetComponent<RightArmAgent>(); break;
                case "LeftTarget": leftArmAgent = child.transform.GetComponent<LeftArmAgent>(); break;
                case "ChestTarget": headChestAgent = child.transform.GetComponent<HeadChestAgent>(); break;
            }

            if (child.childCount > 0)
            {
                GetAgents(child);
            }
        }
    }

    public void SetAnimation(AnimatorController animationToPlay, string nameAnimationToPlay)
    {
        this.animationToPlay = animationToPlay;
        this.nameAnimationToPlay = nameAnimationToPlay;
    }

    public void PlayAnimation()
    {
        if (gender == 1)
        {
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.localRotation = initialRotation;
        }

        animator.runtimeAnimatorController = animationToPlay;
        animator.enabled = true;
        animator.Rebind();
        animator.Play(nameAnimationToPlay, -1, 0f);
    }

    public bool IsPlayingAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(nameAnimationToPlay) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1;
    }

    public void PlayIdle()
    {
        transform.localRotation = initialRotation;

        animator.runtimeAnimatorController = idleAnimatorController;
        animator.enabled = true;
        animator.Rebind();
        animator.Play("Idle", -1, 0f);
    }

    public void PlayVictory()
    {
        transform.localRotation = Quaternion.identity;

        animator.runtimeAnimatorController = victoryAnimatorController;
        animator.enabled = true;
        animator.Rebind();
        animator.Play("Win", -1, 0f);
    }

    public void PlayDefeat()
    {
        transform.localRotation = Quaternion.identity;

        animator.runtimeAnimatorController = defeatedAnimatorController;
        animator.enabled = true;
        animator.Rebind();
        animator.Play("Loose", -1, 0f);
    }

    public bool IsPlayingWinning()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Win") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1;
    }

    public bool IsPlayingDefeated()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Loose") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1;
    }

    public int Compare(Actor x, Actor y)
    {
        return x.idPerformance.CompareTo(y.idPerformance);
    }
}

// class used to be able to order a list of Actors based on their idActor
class ActorComparer : IComparer<Actor>
{
    public int Compare(Actor x, Actor y)
    {
        return x.idPerformance.CompareTo(y.idPerformance);
    }
}
