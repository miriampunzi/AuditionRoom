using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class HeadChestAgent : Agent
{
    public Transform actorTransform;

    public bool isForPerformance = false;
    public Vector3 initialPosition;

    // FOR ML ALGORITHM
    public int countStep = 0;          // counts the actions to take in each episode
    private bool hasRecorded = false;   // has the avatar to copy finished the performance?
    private int indexReplay = 0;        // index in the array of performed rotations to do replay
    private float moveSpeed = 5f;       // speed to move cubes used as target as IK. For now it's random, I don't know which number is the best

    // HEAD
    Transform actorHead;
    Transform targetHead;
    public ArrayList performedRotationsHead = new ArrayList();
    public ArrayList performedPositionsHeadTarget = new ArrayList();
    public ArrayList performedRotationsHeadTarget = new ArrayList();

    // CHEST
    Transform actorChest;
    Transform targetChest;
    public ArrayList performedRotationsChest = new ArrayList();
    public ArrayList performedPositionsChestTarget = new ArrayList();
    public ArrayList performedRotationsChestTarget = new ArrayList();

    void Start()
    {
        // clear previous performed rotations
        performedRotationsHead.Clear();
        performedRotationsChest.Clear();

        // get own body parts
        GetBodyParts(actorTransform);

        // READ HEAD ROTATIONS PERFORMED BY AVATAR TO COPY FROM FILE
        string fileHeadRotations = "Head.csv";

        using (StreamReader reader = new StreamReader(fileHeadRotations))
        {
            string line;
            reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(';');
                Quaternion qh = new Quaternion(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                EnvironmentStatus.rotationsHead.Add(qh);
            }
        }

        // READ CHEST ROTATIONS PERFORMED BY AVATAR TO COPY FROM FILE
        string fileChestRotations = "Chest.csv";

        using (StreamReader reader = new StreamReader(fileChestRotations))
        {
            string line;
            reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(';');
                Quaternion qc = new Quaternion(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                EnvironmentStatus.rotationsChest.Add(qc);
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
                case "Head": actorHead = child.transform; break;
                case "HeadTarget": targetHead = child.transform; break;
                case "Chest": actorChest = child.transform; break;
                case "ChestTarget": targetChest = child.transform; break;
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
        targetHead.localPosition = Vector3.zero;
        targetHead.localRotation = Quaternion.identity;

        targetChest.localPosition = Vector3.zero;
        targetChest.localRotation = Quaternion.identity;

        // clear previous performed rotations
        performedRotationsHead.Clear();
        performedRotationsChest.Clear();
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
                performedRotationsHead.Add(actorHead.localRotation);

                performedRotationsChest.Add(actorChest.localRotation);
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
        if (indexReplay < performedPositionsChestTarget.Count)
        {
            float moveSpeed = 1f;

            targetHead.localPosition += ((Vector3)performedPositionsHeadTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            targetHead.Rotate(
                (transform.localRotation.x + ((Vector3)performedRotationsHeadTarget[indexReplay]).x) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.y + ((Vector3)performedRotationsHeadTarget[indexReplay]).y) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.z + ((Vector3)performedRotationsHeadTarget[indexReplay]).z) * Time.deltaTime * moveSpeed * 500);

            targetChest.localPosition += ((Vector3)performedPositionsChestTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            targetChest.Rotate(
                (transform.localRotation.x + ((Vector3)performedRotationsChestTarget[indexReplay]).x) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.y + ((Vector3)performedRotationsChestTarget[indexReplay]).y) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.z + ((Vector3)performedRotationsChestTarget[indexReplay]).z) * Time.deltaTime * moveSpeed * 500);

            indexReplay++;
        }
    }

    public bool IsPlayingReplay()
    {
        return indexReplay < performedPositionsChestTarget.Count;
    }

    public void SetupForReplay()
    {
        // reset all target positions and rotations
        targetHead.localPosition = Vector3.zero;
        targetHead.localRotation = Quaternion.identity;

        targetChest.localPosition = Vector3.zero;
        targetChest.localRotation = Quaternion.identity;

        // reset index in array for replay
        indexReplay = 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // MOVE HEAD

        // values used to move the target cube for the head
        float moveHeadX = actions.ContinuousActions[0];
        float moveHeadY = actions.ContinuousActions[1];
        float moveHeadZ = actions.ContinuousActions[2];

        float rotateHeadX = actions.ContinuousActions[3];
        float rotateHeadY = actions.ContinuousActions[4];
        float rotateHeadZ = actions.ContinuousActions[5];

        // move the target cube for the head
        targetHead.localPosition += new Vector3(moveHeadX, moveHeadY, moveHeadZ) * Time.deltaTime * moveSpeed;
        targetHead.Rotate(
            (transform.localRotation.x + rotateHeadX) * Time.deltaTime * moveSpeed * 100,
            (transform.localRotation.y + rotateHeadY) * Time.deltaTime * moveSpeed * 100,
            (transform.localRotation.z + rotateHeadZ) * Time.deltaTime * moveSpeed * 100);

        // save movement performed by the head target cube
        if (isForPerformance)
        {
            performedPositionsHeadTarget.Add(new Vector3(moveHeadX, moveHeadY, moveHeadZ));
            performedRotationsHeadTarget.Add(new Vector3(rotateHeadX, rotateHeadY, rotateHeadZ));
        }

        // MOVE CHEST

        // values used to move the target cube for the head
        float moveChestX = actions.ContinuousActions[6];
        float moveChestY = actions.ContinuousActions[7];
        float moveChestZ = actions.ContinuousActions[8];

        float rotateChestX = actions.ContinuousActions[9];
        float rotateChestY = actions.ContinuousActions[10];
        float rotateChestZ = actions.ContinuousActions[11];

        // move the target cube for the head
        targetChest.localPosition += new Vector3(moveChestX, moveChestY, moveChestZ) * Time.deltaTime * moveSpeed;
        targetChest.Rotate(
            (transform.localRotation.x + rotateChestX) * Time.deltaTime * moveSpeed * 100,
            (transform.localRotation.y + rotateChestY) * Time.deltaTime * moveSpeed * 100,
            (transform.localRotation.z + rotateChestZ) * Time.deltaTime * moveSpeed * 100);

        // save movement performed by the head target cube
        if (isForPerformance)
        {
            performedPositionsChestTarget.Add(new Vector3(moveChestX, moveChestY, moveChestZ));
            performedRotationsChestTarget.Add(new Vector3(rotateChestX, rotateChestY, rotateChestZ));
        }

        // CALCULATE REWARD HEAD & CHEST

        float reward = 0.0f;

        if (countStep < EnvironmentStatus.rotationsHead.Count)
            reward += (-1 + 2 * Mathf.Abs(Quaternion.Dot(actorHead.localRotation, (Quaternion)EnvironmentStatus.rotationsHead[countStep]))) * 0.40f;

        if (countStep < EnvironmentStatus.rotationsChest.Count)
            reward += (-1 + 2 * Mathf.Abs(Quaternion.Dot(actorChest.localRotation, (Quaternion)EnvironmentStatus.rotationsChest[countStep]))) * 0.60f;

        AddReward(reward);
    }
}
