using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Animations;
using UnityEngine;

public class Actor : Agent, IComparer<Actor>
{
    public TrapdoorCover trapdoorCover;

    public int id;
    public int gender;

    // RIGHT ARM
    public Transform actorRightArm;
    public Transform actorRightForeArm;
    public Transform actorRightHand;

    public Transform avatarToCopyRightArm;
    public Transform avatarToCopyRightForeArm;
    public Transform avatarToCopyRightHand;

    public Transform targerRightArm;

    public ArrayList performedRotationsRightArm = new ArrayList();
    public ArrayList performedRotationsRightForeArm = new ArrayList();
    public ArrayList performedRotationsRightHand = new ArrayList();

    private ArrayList performedPositionsRightTarget = new ArrayList();
    private ArrayList performedRotationsRightTarget = new ArrayList();

    // LEFT ARM
    public Transform actorLeftArm;
    public Transform actorLeftForeArm;
    public Transform actorLeftHand;

    public Transform avatarToCopyLeftArm;
    public Transform avatarToCopyLeftForeArm;
    public Transform avatarToCopyLeftHand;

    public Transform targerLeftArm;

    public ArrayList performedRotationsLeftArm = new ArrayList();
    public ArrayList performedRotationsLeftForeArm = new ArrayList();
    public ArrayList performedRotationsLeftHand = new ArrayList();

    private ArrayList performedPositionsLeftTarget = new ArrayList();
    private ArrayList performedRotationsLeftTarget = new ArrayList();

    // GENERAL
    public Transform avatarToCopy;
    Animator animatorAvatarToCopy;
    
    int countStep = 0;
    bool hasRecorded = false;

    // REPLAY
    
    private int indexReplay = 0;

    // Start is called before the first frame update
    void Start()
    {
        animatorAvatarToCopy = avatarToCopy.GetComponent<Animator>();
    }

    public override void OnEpisodeBegin()
    {
        countStep = 0;

        targerRightArm.localPosition = Vector3.zero;
        targerRightArm.localRotation = Quaternion.identity;

        performedRotationsRightArm.Clear();
        performedRotationsRightForeArm.Clear();
        performedRotationsRightHand.Clear();

        targerLeftArm.localPosition = Vector3.zero;
        targerLeftArm.localRotation = Quaternion.identity;

        performedRotationsLeftArm.Clear();
        performedRotationsLeftForeArm.Clear();
        performedRotationsLeftHand.Clear();

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

        sensor.AddObservation(actorLeftArm.localRotation);
        sensor.AddObservation(actorLeftForeArm.localRotation);
        sensor.AddObservation(actorLeftHand.localRotation);

        sensor.AddObservation(avatarToCopyLeftArm.localRotation);
        sensor.AddObservation(avatarToCopyLeftForeArm.localRotation);
        sensor.AddObservation(avatarToCopyLeftHand.localRotation);
    }

    private void Update()
    {
        if (!hasRecorded)
        {
            if (animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).IsName("Standing Greeting") &&
            animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
            {
                Human.rotationsRightArm.Add(avatarToCopyRightArm.rotation);
                Human.rotationsRightForeArm.Add(avatarToCopyRightForeArm.rotation);
                Human.rotationsRightHand.Add(avatarToCopyRightHand.rotation);

                Human.rotationsLeftArm.Add(avatarToCopyLeftArm.rotation);
                Human.rotationsLeftForeArm.Add(avatarToCopyLeftForeArm.rotation);
                Human.rotationsLeftHand.Add(avatarToCopyLeftHand.rotation);

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
            
            performedRotationsLeftArm.Add(actorLeftArm.rotation);
            performedRotationsLeftForeArm.Add(actorLeftForeArm.rotation);
            performedRotationsLeftHand.Add(actorLeftHand.rotation);
        }
    }

    public bool IsPlayingPerformance()
    {
        return countStep < Human.numActions;
    }

    public void PerformReplay()
    {
        if (indexReplay < performedPositionsRightTarget.Count)
        {
            float moveSpeed = 1f;
            targerRightArm.localPosition += ((Vector3) performedPositionsRightTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            targerRightArm.Rotate(
                (transform.localRotation.x + ((Vector3) performedRotationsRightTarget[indexReplay]).x) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.y + ((Vector3) performedRotationsRightTarget[indexReplay]).y) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.z + ((Vector3) performedRotationsRightTarget[indexReplay]).z) * Time.deltaTime * moveSpeed * 500);

            targerLeftArm.localPosition += ((Vector3)performedPositionsLeftTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            targerLeftArm.Rotate(
                (transform.localRotation.x + ((Vector3)performedRotationsLeftTarget[indexReplay]).x) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.y + ((Vector3)performedRotationsLeftTarget[indexReplay]).y) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.z + ((Vector3)performedRotationsLeftTarget[indexReplay]).z) * Time.deltaTime * moveSpeed * 500);

            indexReplay++;
        }        
    }

    public bool IsPlayingReplay()
    {
        return indexReplay < performedPositionsRightTarget.Count;
    }

    public void SetupForReplay()
    {
        targerRightArm.localPosition = Vector3.zero;
        targerRightArm.localRotation = Quaternion.identity;

        targerLeftArm.localPosition = Vector3.zero;
        targerLeftArm.localRotation = Quaternion.identity;

        indexReplay = 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // RIGHT ARM
        float moveRightArmX = actions.ContinuousActions[0];
        float moveRightArmY = actions.ContinuousActions[1];
        float moveRightArmZ = actions.ContinuousActions[2];

        float rotateRightArmX = actions.ContinuousActions[3];
        float rotateRightArmY = actions.ContinuousActions[4];
        float rotateRightArmZ = actions.ContinuousActions[5];

        // TODO for how much I have to multiply the movement to make it more visible?
        float moveSpeed = 1f;
        targerRightArm.localPosition += new Vector3(moveRightArmX, moveRightArmY, moveRightArmZ) * Time.deltaTime * moveSpeed * 5;
        targerRightArm.Rotate(
            (transform.localRotation.x + rotateRightArmX) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.y + rotateRightArmY) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.z + rotateRightArmZ) * Time.deltaTime * moveSpeed * 500);

        performedPositionsRightTarget.Add(new Vector3(moveRightArmX, moveRightArmY, moveRightArmZ));
        performedRotationsRightTarget.Add(new Vector3(rotateRightArmX, rotateRightArmY, rotateRightArmZ));

        // LEFT ARM
        float moveLeftArmX = actions.ContinuousActions[6];
        float moveLeftArmY = actions.ContinuousActions[7];
        float moveLeftArmZ = actions.ContinuousActions[8];

        float rotateLeftArmX = actions.ContinuousActions[9];
        float rotateLeftArmY = actions.ContinuousActions[10];
        float rotateLeftArmZ = actions.ContinuousActions[11];

        // TODO for how much I have to multiply the movement to make it more visible?
        targerLeftArm.localPosition += new Vector3(moveLeftArmX, moveLeftArmY, moveLeftArmZ) * Time.deltaTime * moveSpeed * 5;
        targerLeftArm.Rotate(
            (transform.localRotation.x + rotateLeftArmX) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.y + rotateLeftArmY) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.z + rotateLeftArmZ) * Time.deltaTime * moveSpeed * 500);

        performedPositionsLeftTarget.Add(new Vector3(moveLeftArmX, moveLeftArmY, moveLeftArmZ));
        performedRotationsLeftTarget.Add(new Vector3(rotateLeftArmX, rotateLeftArmY, rotateLeftArmZ));
    }

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
