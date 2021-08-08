using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RightArmAgent : Agent
{
    public Transform actorTransform;

    public bool isForPerformance = false;
    public Vector3 initialPosition;

    // FOR ML ALGORITHM
    public int countStep = 0;          // counts the actions to take in each episode
    private bool hasRecorded = false;   // has the avatar to copy finished the performance?
    private int indexReplay = 0;        // index in the array of performed rotations to do replay
    private float moveSpeed = 5f;       // speed to move cubes used as target as IK. For now it's random, I don't know which number is the best

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

    void Start()
    {
        // clear previous performed rotations
        performedRotationsRightArm.Clear();
        performedRotationsRightForeArm.Clear();
        performedRotationsRightHand.Clear();

        // get own body parts
        GetBodyParts(actorTransform);

        // READ RIGHT ARM ROTATIONS PERFORMED BY AVATAR TO COPY FROM FILE
        string fileRightArmRotations = "RightArm.csv";

        using (StreamReader reader = new StreamReader(fileRightArmRotations))
        {
            string line;
            reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(';');
                Quaternion qra = new Quaternion(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                EnvironmentStatus.rotationsRightArm.Add(qra);
                Quaternion qrfa = new Quaternion(float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]));
                EnvironmentStatus.rotationsRightForeArm.Add(qrfa);
                Quaternion qrh = new Quaternion(float.Parse(parts[8]), float.Parse(parts[9]), float.Parse(parts[10]), float.Parse(parts[11]));
                EnvironmentStatus.rotationsRightHand.Add(qrh);
            }
        }
    }

    public void GetBodyParts(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (!child.CompareTag("Untagged"))
                Debug.Log(actorTransform.gameObject.name + " " + child.tag);

            switch (child.tag)
            {
                case "RightArm": actorRightArm = child.transform; break;
                case "RightForeArm": actorRightForeArm = child.transform; break;
                case "RightHand": actorRightHand = child.transform; break;
                case "RightTarget": targetRightArm = child.transform; break;
                default: break;
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

        // clear previous performed rotations
        performedRotationsRightArm.Clear();
        performedRotationsRightForeArm.Clear();
        performedRotationsRightHand.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // which frame
        sensor.AddObservation(countStep / (EnvironmentStatus.numActions - 1.0f));
    }

    public void PerformAction()
    {
        // the avatar perform the same number of actions performed by the acatar to copy
        if (countStep < EnvironmentStatus.numActions)
        {
            RequestDecision();
            countStep++;

            // save the rotations performed by the actor (left and right arm)
            if (isForPerformance)
            {
                performedRotationsRightArm.Add(actorRightArm.localRotation);
                performedRotationsRightForeArm.Add(actorRightForeArm.localRotation);
                performedRotationsRightHand.Add(actorRightHand.localRotation);
            }
        }
    }

    public void LearnInBackground()
    {
        RequestDecision();
        countStep++;
        if (countStep >= EnvironmentStatus.numActions)
        {
            countStep = 0;
            EndEpisode();
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
            targetRightArm.localPosition += ((Vector3)performedPositionsRightTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            targetRightArm.Rotate(
                (transform.localRotation.x + ((Vector3)performedRotationsRightTarget[indexReplay]).x) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.y + ((Vector3)performedRotationsRightTarget[indexReplay]).y) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.z + ((Vector3)performedRotationsRightTarget[indexReplay]).z) * Time.deltaTime * moveSpeed * 500);

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

        // reset index in array for replay
        indexReplay = 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // MOVE RIGHT ARM

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
        if (isForPerformance)
        {
            performedPositionsRightTarget.Add(new Vector3(moveRightArmX, moveRightArmY, moveRightArmZ));
            performedRotationsRightTarget.Add(new Vector3(rotateRightArmX, rotateRightArmY, rotateRightArmZ));
        }

        // CALCULATE REWARD
        float reward = 0.0f;

        //float rightArmX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f) * 180.0f + 180.0f;
        //float rightArmY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f) * 180.0f + 180.0f;
        //float rightArmZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f) * 180.0f + 180.0f;

        //Quaternion qra = Quaternion.Euler(rightArmX, rightArmY, rightArmZ);
        //actorRightArm.localRotation = qra;

        if (countStep < EnvironmentStatus.rotationsRightArm.Count)
            reward += (-1 + 2 * Mathf.Abs(Quaternion.Dot(actorRightArm.localRotation, (Quaternion)EnvironmentStatus.rotationsRightArm[countStep]))) * 0.66f;

        //float rightForeArmX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f) * 180.0f + 180.0f;
        //float rightForeArmY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f) * 180.0f + 180.0f;
        //float rightForeArmZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f) * 180.0f + 180.0f;

        //Quaternion qrfa = Quaternion.Euler(rightForeArmX, rightForeArmY, rightForeArmZ);
        //actorRightForeArm.localRotation = qrfa;

        if (countStep < EnvironmentStatus.rotationsRightForeArm.Count)
            reward += (-1 + 2 * Mathf.Abs(Quaternion.Dot(actorRightForeArm.localRotation, (Quaternion)EnvironmentStatus.rotationsRightForeArm[countStep]))) * 0.20f;

        //float rightHandX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f) * 180.0f + 180.0f;
        //float rightHandY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f) * 180.0f + 180.0f;
        //float rightHandZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f) * 180.0f + 180.0f;

        //Quaternion qrh = Quaternion.Euler(rightHandX, rightHandY, rightHandZ);
        //actorRightHand.localRotation = qrh;

        if (countStep < EnvironmentStatus.rotationsRightArm.Count)
            reward += (-1 + 2 * Mathf.Abs(Quaternion.Dot(actorRightHand.localRotation, (Quaternion)EnvironmentStatus.rotationsRightArm[countStep]))) * 0.13f;

        AddReward(reward);

        
    }
}
