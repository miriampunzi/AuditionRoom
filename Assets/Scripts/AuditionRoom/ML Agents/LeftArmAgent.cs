using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class LeftArmAgent : Agent
{
    public Transform actorTransform;

    public bool isForPerformance = false;
    public Vector3 initialPosition;

    // FOR ML ALGORITHM
    public int countStep = 0;          // counts the actions to take in each episode
    private bool hasRecorded = false;   // has the avatar to copy finished the performance?
    private int indexReplay = 0;        // index in the array of performed rotations to do replay
    private float moveSpeed = 5f;       // speed to move cubes used as target as IK. For now it's random, I don't know which number is the best

    // LEFT ARM
    Transform actorLeftArm;
    Transform actorLeftForeArm;
    Transform actorLeftHand;

    // cube on the right hand used to perform IK 
    Transform targetLeftArm;

    // performed rotations of left arm parts
    public ArrayList performedRotationsLeftArm = new ArrayList();
    public ArrayList performedRotationsLeftForeArm = new ArrayList();
    public ArrayList performedRotationsLeftHand = new ArrayList();

    // performed rotations target cube used for IK
    private ArrayList performedPositionsLeftTarget = new ArrayList();
    private ArrayList performedRotationsLeftTarget = new ArrayList();

    void Start()
    {
        // clear previous performed rotations
        performedRotationsLeftArm.Clear();
        performedRotationsLeftForeArm.Clear();
        performedRotationsLeftHand.Clear();

        // get own body parts
        GetBodyParts(actorTransform);

        // READ LEFT ARM ROTATIONS PERFORMED BY AVATAR TO COPY FROM FILE
        string fileLeftArmRotations = "LeftArm.csv";

        using (StreamReader reader = new StreamReader(fileLeftArmRotations))
        {
            string line;
            reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(';');
                Quaternion qla = new Quaternion(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                EnvironmentStatus.rotationsLeftArm.Add(qla);
                Quaternion qlfa = new Quaternion(float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]));
                EnvironmentStatus.rotationsLeftForeArm.Add(qlfa);
                Quaternion qlh = new Quaternion(float.Parse(parts[8]), float.Parse(parts[9]), float.Parse(parts[10]), float.Parse(parts[11]));
                EnvironmentStatus.rotationsLeftHand.Add(qlh);
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
                case "LeftArm": actorLeftArm = child.transform; break;
                case "LeftForeArm": actorLeftForeArm = child.transform; break;
                case "LeftHand": actorLeftHand = child.transform; break;
                case "LeftTarget": targetLeftArm = child.transform; break;
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
        targetLeftArm.localPosition = Vector3.zero;
        targetLeftArm.localRotation = Quaternion.identity;

        // clear previous performed rotations
        performedRotationsLeftArm.Clear();
        performedRotationsLeftForeArm.Clear();
        performedRotationsLeftHand.Clear();
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
                performedRotationsLeftArm.Add(actorLeftArm.localRotation);
                performedRotationsLeftForeArm.Add(actorLeftForeArm.localRotation);
                performedRotationsLeftHand.Add(actorLeftHand.localRotation);
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
        if (indexReplay < performedPositionsLeftTarget.Count)
        {
            float moveSpeed = 1f;

            // make the cube targets for IK move exactly like before 
            targetLeftArm.localPosition += ((Vector3)performedPositionsLeftTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            targetLeftArm.Rotate(
                (transform.localRotation.x + ((Vector3)performedRotationsLeftTarget[indexReplay]).x) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.y + ((Vector3)performedRotationsLeftTarget[indexReplay]).y) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.z + ((Vector3)performedRotationsLeftTarget[indexReplay]).z) * Time.deltaTime * moveSpeed * 500);

            indexReplay++;
        }
    }

    public bool IsPlayingReplay()
    {
        return indexReplay < performedPositionsLeftTarget.Count;
    }

    public void SetupForReplay()
    {
        // reset all target positions and rotations
        targetLeftArm.localPosition = Vector3.zero;
        targetLeftArm.localRotation = Quaternion.identity;

        // reset index in array for replay
        indexReplay = 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // MOVE LEFT ARM

        // values used to move the target cube for the left arm
        float moveLeftArmX = actions.ContinuousActions[0];
        float moveLeftArmY = actions.ContinuousActions[1];
        float moveLeftArmZ = actions.ContinuousActions[2];

        float rotateLeftArmX = actions.ContinuousActions[3];
        float rotateLeftArmY = actions.ContinuousActions[4];
        float rotateLeftArmZ = actions.ContinuousActions[5];

        // move the target cube for the left arm
        targetLeftArm.localPosition += new Vector3(moveLeftArmX, moveLeftArmY, moveLeftArmZ) * Time.deltaTime * moveSpeed;
        targetLeftArm.Rotate(
            (transform.localRotation.x + rotateLeftArmX) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.y + rotateLeftArmY) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.z + rotateLeftArmZ) * Time.deltaTime * moveSpeed * 500);

        // save movement performed by the left target cube
        if (isForPerformance)
        {
            performedPositionsLeftTarget.Add(new Vector3(moveLeftArmX, moveLeftArmY, moveLeftArmZ));
            performedRotationsLeftTarget.Add(new Vector3(rotateLeftArmX, rotateLeftArmY, rotateLeftArmZ));
        }

        // CALCULATE REWARD

        float rewardLeftArm = 0.0f;

        //float leftArmX = Mathf.Clamp(actions.ContinuousActions[9], -1f, 1f) * 180.0f + 180.0f;
        //float leftArmY = Mathf.Clamp(actions.ContinuousActions[10], -1f, 1f) * 180.0f + 180.0f;
        //float leftArmZ = Mathf.Clamp(actions.ContinuousActions[11], -1f, 1f) * 180.0f + 180.0f;

        //Quaternion qla = Quaternion.Euler(leftArmX, leftArmY, leftArmZ);
        //actorLeftArm.localRotation = qla;

        if (countStep < EnvironmentStatus.rotationsLeftArm.Count)
            rewardLeftArm += (-1 + 2 * Mathf.Abs(Quaternion.Dot(actorLeftArm.localRotation, (Quaternion)EnvironmentStatus.rotationsLeftArm[countStep]))) * 0.66f;

        //float leftForeArmX = Mathf.Clamp(actions.ContinuousActions[12], -1f, 1f) * 180.0f + 180.0f;
        //float leftForeArmY = Mathf.Clamp(actions.ContinuousActions[13], -1f, 1f) * 180.0f + 180.0f;
        //float leftForeArmZ = Mathf.Clamp(actions.ContinuousActions[14], -1f, 1f) * 180.0f + 180.0f;

        //Quaternion qlfa = Quaternion.Euler(leftForeArmX, leftForeArmY, leftForeArmZ);
        //actorLeftForeArm.localRotation = qlfa;

        if (countStep < EnvironmentStatus.rotationsLeftForeArm.Count)
            rewardLeftArm += (-1 + 2 * Mathf.Abs(Quaternion.Dot(actorLeftForeArm.localRotation, (Quaternion)EnvironmentStatus.rotationsLeftForeArm[countStep]))) * 0.20f;

        //float leftHandX = Mathf.Clamp(actions.ContinuousActions[15], -1f, 1f) * 180.0f + 180.0f;
        //float leftHandY = Mathf.Clamp(actions.ContinuousActions[16], -1f, 1f) * 180.0f + 180.0f;
        //float leftHandZ = Mathf.Clamp(actions.ContinuousActions[17], -1f, 1f) * 180.0f + 180.0f;

        //Quaternion qlh = Quaternion.Euler(leftHandX, leftHandY, leftHandZ);
        //actorLeftHand.localRotation = qlh;

        if (countStep < EnvironmentStatus.rotationsLeftArm.Count)
            rewardLeftArm += (-1 + 2 * Mathf.Abs(Quaternion.Dot(actorLeftHand.localRotation, (Quaternion)EnvironmentStatus.rotationsLeftArm[countStep]))) * 0.13f;

        AddReward(rewardLeftArm);

        
    }
}
