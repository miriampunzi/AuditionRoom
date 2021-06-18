using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public string nameAnimationToPlay;
    public TrapdoorCover trapdoorCover;
    private Vector3 initialPosition;

    Animator animator;
    public int id;
    public int gender;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;

        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void PlayAnimation()
    {
        animator.enabled = true;
        animator.Play(nameAnimationToPlay, -1, 0f);
    }

    public bool IsPlayingAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(nameAnimationToPlay) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
