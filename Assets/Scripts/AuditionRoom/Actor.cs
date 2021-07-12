using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Animations;
using UnityEngine;

public class Actor : Agent, IComparer<Actor>
{
    public int idActor;

    // FOR ML ALGORITHM
    private int countStep = 0;          // counts the actions to take in each episode
    private bool hasRecorded = false;   // has the avatar to copy finished the performance?
    private int indexReplay = 0;        // index in the array of performed rotations to do replay
    private float moveSpeed = 5f;       // speed to move cubes used as target as IK. For now it's random, I don't know which number is the best

    // ENVIRONMENT OBJECTS
    public Transform avatarToCopy;
    Animator animatorAvatarToCopy;
    public TrapdoorCover trapdoorCover;

    // RIGHT ARM
    public Transform actorRightArm;
    public Transform actorRightForeArm;
    public Transform actorRightHand;

    public Transform avatarToCopyRightArm;
    public Transform avatarToCopyRightForeArm;
    public Transform avatarToCopyRightHand;

    // cube on the right hand used to perform IK 
    public Transform targetRightArm;    

    // performed rotations of right arm parts
    public ArrayList performedRotationsRightArm = new ArrayList();
    public ArrayList performedRotationsRightForeArm = new ArrayList();
    public ArrayList performedRotationsRightHand = new ArrayList();

    // performed rotations target cube used for IK
    private ArrayList performedPositionsRightTarget = new ArrayList();
    private ArrayList performedRotationsRightTarget = new ArrayList();

    // LEFT ARM
    public Transform actorLeftArm;
    public Transform actorLeftForeArm;
    public Transform actorLeftHand;

    public Transform avatarToCopyLeftArm;
    public Transform avatarToCopyLeftForeArm;
    public Transform avatarToCopyLeftHand;

    // cube on the right hand used to perform IK 
    public Transform targerLeftArm;

    // performed rotations of left arm parts
    public ArrayList performedRotationsLeftArm = new ArrayList();
    public ArrayList performedRotationsLeftForeArm = new ArrayList();
    public ArrayList performedRotationsLeftHand = new ArrayList();

    // performed rotations target cube used for IK
    private ArrayList performedPositionsLeftTarget = new ArrayList();
    private ArrayList performedRotationsLeftTarget = new ArrayList();

    void Start()
    {
        animatorAvatarToCopy = avatarToCopy.GetComponent<Animator>();
    }

    public override void OnEpisodeBegin()
    {
        // reset number of steps for the current episode
        countStep = 0;

        // reset positions of cubes used as target for IK
        targetRightArm.localPosition = Vector3.zero;
        targetRightArm.localRotation = Quaternion.identity;

        targerLeftArm.localPosition = Vector3.zero;
        targerLeftArm.localRotation = Quaternion.identity;

        // clear previous performed rotations
        performedRotationsRightArm.Clear();
        performedRotationsRightForeArm.Clear();
        performedRotationsRightHand.Clear();

        performedRotationsLeftArm.Clear();
        performedRotationsLeftForeArm.Clear();
        performedRotationsLeftHand.Clear();

        // move the avatar to copy
        animatorAvatarToCopy.Play("Standing Greeting", -1, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // own right arm parts
        sensor.AddObservation(actorRightArm.localRotation);
        sensor.AddObservation(actorRightForeArm.localRotation);
        sensor.AddObservation(actorRightHand.localRotation);

        // avatar to copy's right arm parts
        sensor.AddObservation(avatarToCopyRightArm.localRotation);
        sensor.AddObservation(avatarToCopyRightForeArm.localRotation);
        sensor.AddObservation(avatarToCopyRightHand.localRotation);

        // own left arm parts
        sensor.AddObservation(actorLeftArm.localRotation);
        sensor.AddObservation(actorLeftForeArm.localRotation);
        sensor.AddObservation(actorLeftHand.localRotation);

        // avatar to copy's left arm parts
        sensor.AddObservation(avatarToCopyLeftArm.localRotation);
        sensor.AddObservation(avatarToCopyLeftForeArm.localRotation);
        sensor.AddObservation(avatarToCopyLeftHand.localRotation);
    }

    private void Update()
    {
        // if the avatar to copy has not done the performance
        if (!hasRecorded)
        {
            // is the avatar to copy still playing the animation?
            if (animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).IsName("Standing Greeting") &&
            animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
            {
                // saving rotations performed by avatar to copy
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
                hasRecorded = true; // this variable avoid the repeating of the saving of rotations performed by avatar to copy
            }
        }
    }

    public void PerformAction()
    {
        // the avatar perform the same number of actions performed by the acatar to copy
        if (countStep < Human.numActions)
        {
            RequestDecision();
            countStep++;

            // save the rotations performed by the actor (left and right arm)
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

    // not working properly
    // this method is called always after SetupForReplay()
    public void PerformReplay()
    {
        // scan all the arrat of performed rotations
        if (indexReplay < performedPositionsRightTarget.Count)
        {
            float moveSpeed = 1f;

            // make the cube targets for IK move exactly like before 
            targetRightArm.localPosition += ((Vector3) performedPositionsRightTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            targetRightArm.Rotate(
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
        // reset all target positions and rotations
        targetRightArm.localPosition = Vector3.zero;
        targetRightArm.localRotation = Quaternion.identity;

        targerLeftArm.localPosition = Vector3.zero;
        targerLeftArm.localRotation = Quaternion.identity;

        // reset index in array for replay
        indexReplay = 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // values used to move the target cube for the right arm
        float moveRightArmX = actions.ContinuousActions[0];
        float moveRightArmY = actions.ContinuousActions[1];
        float moveRightArmZ = actions.ContinuousActions[2];

        float rotateRightArmX = actions.ContinuousActions[3];
        float rotateRightArmY = actions.ContinuousActions[4];
        float rotateRightArmZ = actions.ContinuousActions[5];

        // move the target cube for the right arm
        targetRightArm.localPosition += new Vector3(moveRightArmX, moveRightArmY, moveRightArmZ) * Time.deltaTime * moveSpeed;
        targetRightArm.Rotate(
            (transform.localRotation.x + rotateRightArmX) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.y + rotateRightArmY) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.z + rotateRightArmZ) * Time.deltaTime * moveSpeed * 500);

        // save movement performed by the right target cube
        performedPositionsRightTarget.Add(new Vector3(moveRightArmX, moveRightArmY, moveRightArmZ));
        performedRotationsRightTarget.Add(new Vector3(rotateRightArmX, rotateRightArmY, rotateRightArmZ));

        // values used to move the target cube for the left arm
        float moveLeftArmX = actions.ContinuousActions[6];
        float moveLeftArmY = actions.ContinuousActions[7];
        float moveLeftArmZ = actions.ContinuousActions[8];

        float rotateLeftArmX = actions.ContinuousActions[9];
        float rotateLeftArmY = actions.ContinuousActions[10];
        float rotateLeftArmZ = actions.ContinuousActions[11];

        // move the target cube for the left arm
        targerLeftArm.localPosition += new Vector3(moveLeftArmX, moveLeftArmY, moveLeftArmZ) * Time.deltaTime * moveSpeed;
        targerLeftArm.Rotate(
            (transform.localRotation.x + rotateLeftArmX) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.y + rotateLeftArmY) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.z + rotateLeftArmZ) * Time.deltaTime * moveSpeed * 500);

        // save movement performed by the left target cube
        performedPositionsLeftTarget.Add(new Vector3(moveLeftArmX, moveLeftArmY, moveLeftArmZ));
        performedRotationsLeftTarget.Add(new Vector3(rotateLeftArmX, rotateLeftArmY, rotateLeftArmZ));
    }

    // method used to be able to order a list of Actors based on their idActor
    public int Compare(Actor x, Actor y)
    {
        return x.idActor.CompareTo(y.idActor);
    }
}

// class used to be able to order a list of Actors based on their idActor
class ActorComparer : IComparer<Actor>
{
    public int Compare(Actor x, Actor y)
    {
        return x.idActor.CompareTo(y.idActor);
    }
}
