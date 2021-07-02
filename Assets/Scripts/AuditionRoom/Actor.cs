using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Animations;
using UnityEngine;

public class Actor : Agent, IComparer<Actor>
{
    public string nameAnimationToPlay;
    public AnimatorController victoryAnimatorController;
    public AnimatorController defeatedAnimatorController;

    public TrapdoorCover trapdoorCover;

    //Animator animator;
    public int id;
    public int gender;

    public Transform actorRightArm;
    public Transform actorRightForeArm;
    public Transform actorRightHand;

    public Transform avatarToCopyRightArm;
    public Transform avatarToCopyRightForeArm;
    public Transform avatarToCopyRightHand;

    public Transform avatarToCopy;
    Animator animatorAvatarToCopy;
    public Transform target;

    int countStep = 0;
    public ArrayList performedRotationsRightArm = new ArrayList();
    public ArrayList performedRotationsRightForeArm = new ArrayList();
    public ArrayList performedRotationsRightHand = new ArrayList();

    bool hasRecorded = false;

    // THINGS FOR REPLAY
    private ArrayList performedPositions = new ArrayList();
    private ArrayList performedRotations = new ArrayList();
    private int indexReplay = 0;

    // Start is called before the first frame update
    void Start()
    {
        animatorAvatarToCopy = avatarToCopy.GetComponent<Animator>();
        //animator = GetComponent<Animator>();
        //animator.enabled = false;
    }

    public override void OnEpisodeBegin()
    {
        countStep = 0;

        target.localPosition = Vector3.zero;
        target.localRotation = Quaternion.identity;

        performedRotationsRightArm.Clear();
        performedRotationsRightForeArm.Clear();
        performedRotationsRightHand.Clear();

        animatorAvatarToCopy.Play("Standing Greeting", -1, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(actorRightArm.localRotation);
        sensor.AddObservation(actorRightForeArm.localRotation);
        sensor.AddObservation(actorRightHand.localRotation);

        sensor.AddObservation(avatarToCopyRightArm.localRotation);
        sensor.AddObservation(avatarToCopyRightForeArm.localRotation);
        sensor.AddObservation(avatarToCopyRightHand.localRotation);
    }

    private void Update()
    {
        // the avatar 0 initializes list of right actions
        if (!hasRecorded) //&& idActor == 0)
        {
            if (animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).IsName("Standing Greeting") &&
            animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
            {
                Human.rotationsRightArm.Add(avatarToCopyRightArm.rotation);
                Human.rotationsRightForeArm.Add(avatarToCopyRightForeArm.rotation);
                Human.rotationsRightHand.Add(avatarToCopyRightHand.rotation);
                Human.numActions++;
            }
            else
            {
                hasRecorded = true;
            }
        }
        // the other avatars wait for the initialization
        else if (!hasRecorded)
        {
            if (!animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).IsName("Standing Greeting") ||
            animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                hasRecorded = true;
            }
        }
        // when the right actions are saved
        //else
        //{
        //    // perform actions to create a movement
        //    if (countStep < Human.numActions)
        //    {
        //        RequestDecision();
        //        countStep++;

        //        performedRotationsRightArm.Add(actorRightArm.rotation);
        //        performedRotationsRightForeArm.Add(actorRightForeArm.rotation);
        //        performedRotationsRightHand.Add(actorRightHand.rotation);
        //    }
        //    else if (countStep == Human.numActions && !EnvironmentStatus.feedbackProvided)
        //    {
        //        Human.DNNRewardFunction();
        //        EnvironmentStatus.feedbackProvided = true;
        //    }

        //    if (EnvironmentStatus.feedbackProvided)
        //    {
        //        EndEpisode();
        //    }
        //}
    }

    public void PerformAction()
    {
        if (countStep < Human.numActions)
        {
            RequestDecision();
            countStep++;

            performedRotationsRightArm.Add(actorRightArm.rotation);
            performedRotationsRightForeArm.Add(actorRightForeArm.rotation);
            performedRotationsRightHand.Add(actorRightHand.rotation);
        }
    }

    public bool IsPlayingPerformance()
    {
        return countStep < Human.numActions;
    }

    public void PerformReplay()
    {
        Debug.Log("indexReplay: " + indexReplay);
        Debug.Log("performedPositions.Count: " + performedPositions.Count);

        if (indexReplay < performedPositions.Count)
        {
            float moveSpeed = 1f;
            target.localPosition += ((Vector3) performedPositions[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            target.Rotate(
                ((Vector3) performedRotations[indexReplay]).x * Time.deltaTime * moveSpeed * 500,
                ((Vector3)performedRotations[indexReplay]).y * Time.deltaTime * moveSpeed * 500,
                ((Vector3)performedRotations[indexReplay]).z * Time.deltaTime * moveSpeed * 500);
            
            indexReplay++;
        }        
    }

    public bool IsPlayingReplay()
    {
        return indexReplay < performedPositions.Count;
    }

    public void SetupForReplay()
    {
        target.localPosition = Vector3.zero;
        target.localRotation = Quaternion.identity;
        indexReplay = 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        float moveZ = actions.ContinuousActions[2];

        float rotateX = actions.ContinuousActions[3];
        float rotateY = actions.ContinuousActions[4];
        float rotateZ = actions.ContinuousActions[5];

        // TODO for how much I have to multiply the movement to make it more visible?
        float moveSpeed = 1f;
        target.localPosition += new Vector3(moveX, moveY, moveZ) * Time.deltaTime * moveSpeed * 5;
        target.Rotate(
            (transform.localRotation.x + rotateX) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.y + rotateY) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.z + rotateZ) * Time.deltaTime * moveSpeed * 500);

        performedPositions.Add(new Vector3(moveX, moveY, moveZ));
        performedRotations.Add(new Vector3(rotateX, rotateY, rotateZ));
    }

    //public void PlayAnimation()
    //{
    //    animator.enabled = true;
    //    animator.Rebind();
    //    animator.Play(nameAnimationToPlay, -1, 0f);
    //}

    //public bool IsPlayingAnimation()
    //{
    //    return animator.GetCurrentAnimatorStateInfo(0).IsName(nameAnimationToPlay) &&
    //        animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1;
    //}

    //public void PlayVictory()
    //{
    //    animator.runtimeAnimatorController = victoryAnimatorController;
    //    animator.enabled = true;
    //    animator.Rebind();
    //    animator.Play("Victory", -1, 0f);
    //}

    //public void PlayDefeat()
    //{
    //    animator.runtimeAnimatorController = defeatedAnimatorController;
    //    animator.enabled = true;
    //    animator.Rebind();
    //    animator.Play("Defeated", -1, 0f);
    //}

    //public bool IsPlayingWinning()
    //{
    //    return animator.GetCurrentAnimatorStateInfo(0).IsName("Victory") &&
    //        animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1;
    //}

    public int Compare(Actor x, Actor y)
    {
        return x.id.CompareTo(y.id);
    }
}

class ActorComparer : IComparer<Actor>
{
    public int Compare(Actor x, Actor y)
    {
        return x.id.CompareTo(y.id);
    }
}
