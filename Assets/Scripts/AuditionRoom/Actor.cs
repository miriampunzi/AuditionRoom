using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Animations;
using UnityEngine;

public class Actor : Agent, IComparer<Actor>
{
    public int idActor;
    public int numActor;

    public bool isHuman;

    // FOR ML ALGORITHM
    private int countStep = 0;          // counts the actions to take in each episode
    private bool hasRecorded = false;   // has the avatar to copy finished the performance?
    private int indexReplay = 0;        // index in the array of performed rotations to do replay
    private float moveSpeed = 5f;       // speed to move cubes used as target as IK. For now it's random, I don't know which number is the best

    // ENVIRONMENT OBJECTS
    Transform avatarToCopy;

    Transform avatarToCopyRightArm;
    Transform avatarToCopyRightForeArm;
    Transform avatarToCopyRightHand;

    Transform avatarToCopyLeftArm;
    Transform avatarToCopyLeftForeArm;
    Transform avatarToCopyLeftHand;

    Animator animatorAvatarToCopy;
    public TrapdoorCover trapdoorCover;

    // RIGHT ARM
    Transform actorRightArm;
    Transform actorRightForeArm;
    Transform actorRightHand;

    // cube on the right hand used to perform IK 
    Transform targetRightArm;    

    // performed rotations of right arm parts
    public ArrayList performedRotationsRightArm = new ArrayList();
    public ArrayList performedRotationsRightForeArm = new ArrayList();
    public ArrayList performedRotationsRightHand = new ArrayList();

    // performed rotations target cube used for IK
    private ArrayList performedPositionsRightTarget = new ArrayList();
    private ArrayList performedRotationsRightTarget = new ArrayList();

    // LEFT ARM
    Transform actorLeftArm;
    Transform actorLeftForeArm;
    Transform actorLeftHand;

    // cube on the right hand used to perform IK 
    Transform targerLeftArm;

    // performed rotations of left arm parts
    public ArrayList performedRotationsLeftArm = new ArrayList();
    public ArrayList performedRotationsLeftForeArm = new ArrayList();
    public ArrayList performedRotationsLeftHand = new ArrayList();

    // performed rotations target cube used for IK
    private ArrayList performedPositionsLeftTarget = new ArrayList();
    private ArrayList performedRotationsLeftTarget = new ArrayList();

    void Start()
    {
        // get avatar to copy body and body parts
        avatarToCopy = GameObject.FindGameObjectWithTag("AvatarToCopy").transform;
        animatorAvatarToCopy = avatarToCopy.GetComponent<Animator>();

        avatarToCopyRightArm = GameObject.FindGameObjectWithTag("RightArmAvatarToCopy").transform;
        avatarToCopyRightForeArm = GameObject.FindGameObjectWithTag("RightForeArmAvatarToCopy").transform;
        avatarToCopyRightHand = GameObject.FindGameObjectWithTag("RightHandAvatarToCopy").transform;

        avatarToCopyLeftArm = GameObject.FindGameObjectWithTag("LeftArmAvatarToCopy").transform;
        avatarToCopyLeftForeArm = GameObject.FindGameObjectWithTag("LeftForeArmAvatarToCopy").transform;
        avatarToCopyLeftHand = GameObject.FindGameObjectWithTag("LeftHandAvatarToCopy").transform;

        // clear previous performed rotations
        performedRotationsRightArm.Clear();
        performedRotationsRightForeArm.Clear();
        performedRotationsRightHand.Clear();

        performedRotationsLeftArm.Clear();
        performedRotationsLeftForeArm.Clear();
        performedRotationsLeftHand.Clear();

        // get own body parts
        GetBodyParts(transform);

        // read rotations performed by avatar to copy from file
        string fileName = "1file.csv";

        using (StreamReader reader = new StreamReader(fileName))
        {
            string line;
            reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(';');
                Quaternion qra = new Quaternion(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                //EnvironmentStatus.rotationsRightArm.Add(qra);
                EnvironmentStatus.rotationsRightArm.Add(qra);
                Quaternion qrfa = new Quaternion(float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]));
                //EnvironmentStatus.rotationsRightArm.Add(qrfa);
                EnvironmentStatus.rotationsRightForeArm.Add(qrfa);
                Quaternion qrh = new Quaternion(float.Parse(parts[8]), float.Parse(parts[9]), float.Parse(parts[10]), float.Parse(parts[11]));
                //EnvironmentStatus.rotationsRightArm.Add(qrh);
                EnvironmentStatus.rotationsRightHand.Add(qrh);

                EnvironmentStatus.numActions++;
            }
        }
    }

    public void GetBodyParts(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            switch (child.tag)
            {
                case "RightArm": actorRightArm = child.transform; break;
                case "RightForeArm": actorRightForeArm = child.transform; break;
                case "RightHand": actorRightHand = child.transform; break;
                case "RightTarget": targetRightArm = child.transform; break;
                case "LeftArm": actorLeftArm = child.transform; break;
                case "LeftForeArm": actorLeftForeArm = child.transform; break;
                case "LeftHand": actorLeftHand = child.transform; break;
                case "LeftTarget": targerLeftArm = child.transform; break;
            }

            if (child.childCount > 0)
            {
                GetBodyParts(child);
            }
        }
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
        //animatorAvatarToCopy.Play("Standing Greeting", -1, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // which frame
        sensor.AddObservation(countStep / (EnvironmentStatus.numActions - 1.0f));

        // own right arm parts
        //sensor.AddObservation(actorRightArm.localRotation);
        //sensor.AddObservation(actorRightForeArm.localRotation);
        //sensor.AddObservation(actorRightHand.localRotation);

        //// avatar to copy's right arm parts
        //sensor.AddObservation(avatarToCopyRightArm.localRotation);
        //sensor.AddObservation(avatarToCopyRightForeArm.localRotation);
        //sensor.AddObservation(avatarToCopyRightHand.localRotation);

        //// own left arm parts
        //sensor.AddObservation(actorLeftArm.localRotation);
        //sensor.AddObservation(actorLeftForeArm.localRotation);
        //sensor.AddObservation(actorLeftHand.localRotation);

        //// avatar to copy's left arm parts
        //sensor.AddObservation(avatarToCopyLeftArm.localRotation);
        //sensor.AddObservation(avatarToCopyLeftForeArm.localRotation);
        //sensor.AddObservation(avatarToCopyLeftHand.localRotation);
    }

    private void Update()
    {
        // if the avatar to copy has not done the performance
        //if (!hasRecorded)
        //{
        //    // is the avatar to copy still playing the animation?
        //    if (animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).IsName("Standing Greeting") &&
        //    animatorAvatarToCopy.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
        //    {
        //        // saving rotations performed by avatar to copy
        //        EnvironmentStatus.rotationsRightArm.Add(avatarToCopyRightArm.rotation);
        //        EnvironmentStatus.rotationsRightForeArm.Add(avatarToCopyRightForeArm.rotation);
        //        EnvironmentStatus.rotationsRightHand.Add(avatarToCopyRightHand.rotation);

        //        EnvironmentStatus.rotationsLeftArm.Add(avatarToCopyLeftArm.rotation);
        //        EnvironmentStatus.rotationsLeftForeArm.Add(avatarToCopyLeftForeArm.rotation);
        //        EnvironmentStatus.rotationsLeftHand.Add(avatarToCopyLeftHand.rotation);

        //        EnvironmentStatus.numActions++;
        //    }
        //    else
        //    {
        //        hasRecorded = true; // this variable avoid the repeating of the saving of rotations performed by avatar to copy
        //    }
        //}
    }

    public void PerformAction()
    {
        // the avatar perform the same number of actions performed by the acatar to copy
        if (countStep < EnvironmentStatus.numActions)
        {
            RequestDecision();
            countStep++;

            // save the rotations performed by the actor (left and right arm)
            performedRotationsRightArm.Add(actorRightArm.localRotation);
            performedRotationsRightForeArm.Add(actorRightForeArm.localRotation);
            performedRotationsRightHand.Add(actorRightHand.localRotation);
            
            performedRotationsLeftArm.Add(actorLeftArm.localRotation);
            performedRotationsLeftForeArm.Add(actorLeftForeArm.localRotation);
            performedRotationsLeftHand.Add(actorLeftHand.localRotation);
        }
    }

    public bool IsPlayingPerformance()
    {
        return countStep < EnvironmentStatus.numActions;
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
        float reward = 0.0f;
        // values used to move the target cube for the right arm
        float rightArmX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f) * 180.0f + 180.0f;
        float rightArmY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f) * 180.0f + 180.0f;
        float rightArmZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f) * 180.0f + 180.0f;
        //float rightArmW = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f);
        Quaternion qra = Quaternion.Euler(rightArmX, rightArmY, rightArmZ);
        actorRightArm.localRotation = qra;

        if (countStep < EnvironmentStatus.rotationsRightArm.Count)
            reward += (-1 + 2 * Mathf.Abs(Quaternion.Dot(qra, (Quaternion)EnvironmentStatus.rotationsRightArm[countStep]))) * 0.66f;

        float rightForeArmX = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f) * 180.0f + 180.0f;
        float rightForeArmY = Mathf.Clamp(actions.ContinuousActions[4], -1f, 1f) * 180.0f + 180.0f;
        float rightForeArmZ = Mathf.Clamp(actions.ContinuousActions[5], -1f, 1f) * 180.0f + 180.0f;
        //float rightForeArmW = Mathf.Clamp(actions.ContinuousActions[7], -1f, 1f) * 180.0f;
        Quaternion qrfa = Quaternion.Euler(rightForeArmX, rightForeArmY, rightForeArmZ);
        actorRightForeArm.localRotation = qrfa;

        if (countStep < EnvironmentStatus.rotationsRightForeArm.Count)
            reward += (-1 + 2 * Mathf.Abs(Quaternion.Dot(qrfa, (Quaternion)EnvironmentStatus.rotationsRightForeArm[countStep]))) * 0.20f;

        float rightHandX = Mathf.Clamp(actions.ContinuousActions[6], -1f, 1f) * 180.0f + 180.0f;
        float rightHandY = Mathf.Clamp(actions.ContinuousActions[7], -1f, 1f) * 180.0f + 180.0f;
        float rightHandZ = Mathf.Clamp(actions.ContinuousActions[8], -1f, 1f) * 180.0f + 180.0f;
        //float rightHandW = Mathf.Clamp(actions.ContinuousActions[11], -1f, 1f) * 180.0f;
        Quaternion qrh = Quaternion.Euler(rightHandX, rightHandY, rightHandZ);
        actorRightHand.localRotation = qrh;

        if (countStep < EnvironmentStatus.rotationsRightArm.Count)
            reward += (-1 + 2 * Mathf.Abs(Quaternion.Dot(qrh, (Quaternion)EnvironmentStatus.rotationsRightArm[countStep]))) * 0.13f;

        AddReward(reward);
        //Debug.Log(Quaternion.Dot(qra, (Quaternion)rotationsRArm[countStep]));

        // #########################################################################

        //// values used to move the target cube for the right arm
        float moveRightArmX = actions.ContinuousActions[0];
        float moveRightArmY = actions.ContinuousActions[1];
        float moveRightArmZ = actions.ContinuousActions[2];

        float rotateRightArmX = actions.ContinuousActions[3];
        float rotateRightArmY = actions.ContinuousActions[4];
        float rotateRightArmZ = actions.ContinuousActions[5];

        //// move the target cube for the right arm
        //targetRightArm.localPosition += new Vector3(moveRightArmX, moveRightArmY, moveRightArmZ) * Time.deltaTime * moveSpeed;
        //targetRightArm.Rotate(
        //    (transform.localRotation.x + rotateRightArmX) * Time.deltaTime * moveSpeed * 500,
        //    (transform.localRotation.y + rotateRightArmY) * Time.deltaTime * moveSpeed * 500,
        //    (transform.localRotation.z + rotateRightArmZ) * Time.deltaTime * moveSpeed * 500);

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
