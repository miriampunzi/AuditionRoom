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

    public Vector3 initialPosition;

    public TrapdoorCover trapdoorCover;

    // FOR ML ALGORITHM
    private int countStep = 0;          // counts the actions to take in each episode
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
        performedRotationsRightArm.Clear();
        performedRotationsRightForeArm.Clear();
        performedRotationsRightHand.Clear();

        performedRotationsLeftArm.Clear();
        performedRotationsLeftForeArm.Clear();
        performedRotationsLeftHand.Clear();

        // get own body parts
        GetBodyParts(transform);

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
                case "RightArm": actorRightArm = child.transform; break;
                case "RightForeArm": actorRightForeArm = child.transform; break;
                case "RightHand": actorRightHand = child.transform; break;
                case "RightTarget": targetRightArm = child.transform; break;
                case "LeftArm": actorLeftArm = child.transform; break;
                case "LeftForeArm": actorLeftForeArm = child.transform; break;
                case "LeftHand": actorLeftHand = child.transform; break;
                case "LeftTarget": targetLeftArm = child.transform; break;
                case "Head": actorHead = child.transform; break;
                case "HeadTarget": targetHead = child.transform; break;
                case "Chest": actorChest = child.transform; break;
                case "ChestTarget": targetChest = child.transform; break;
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

        targetLeftArm.localPosition = Vector3.zero;
        targetLeftArm.localRotation = Quaternion.identity;

        targetHead.localPosition = Vector3.zero;
        targetHead.localRotation = Quaternion.identity;

        targetChest.localPosition = Vector3.zero;
        targetChest.localRotation = Quaternion.identity;

        // clear previous performed rotations
        performedRotationsRightArm.Clear();
        performedRotationsRightForeArm.Clear();
        performedRotationsRightHand.Clear();

        performedRotationsLeftArm.Clear();
        performedRotationsLeftForeArm.Clear();
        performedRotationsLeftHand.Clear();

        performedPositionsHeadTarget.Clear();
        performedPositionsChestTarget.Clear();
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

            performedRotationsHead.Add(actorHead.localRotation);

            performedRotationsChest.Add(actorChest.localRotation);
        }
    }

    public void LearningInBackground()
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
            targetRightArm.localPosition += ((Vector3) performedPositionsRightTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            targetRightArm.Rotate(
                (transform.localRotation.x + ((Vector3) performedRotationsRightTarget[indexReplay]).x) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.y + ((Vector3) performedRotationsRightTarget[indexReplay]).y) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.z + ((Vector3) performedRotationsRightTarget[indexReplay]).z) * Time.deltaTime * moveSpeed * 500);

            targetLeftArm.localPosition += ((Vector3)performedPositionsLeftTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
            targetLeftArm.Rotate(
                (transform.localRotation.x + ((Vector3)performedRotationsLeftTarget[indexReplay]).x) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.y + ((Vector3)performedRotationsLeftTarget[indexReplay]).y) * Time.deltaTime * moveSpeed * 500,
                (transform.localRotation.z + ((Vector3)performedRotationsLeftTarget[indexReplay]).z) * Time.deltaTime * moveSpeed * 500);

            // TODO INDEX OUT OF BOUND NEL SECONDO ROUND. FORSE DEVO FARE QUALCHE CLEAR

            targetHead.localPosition += ((Vector3) performedPositionsHeadTarget[indexReplay]) * Time.deltaTime * moveSpeed * 5;
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
        return indexReplay < performedPositionsRightTarget.Count;
    }

    public void SetupForReplay()
    {
        // reset all target positions and rotations
        targetRightArm.localPosition = Vector3.zero;
        targetRightArm.localRotation = Quaternion.identity;

        targetLeftArm.localPosition = Vector3.zero;
        targetLeftArm.localRotation = Quaternion.identity;

        targetHead.localPosition = Vector3.zero;
        targetHead.localRotation = Quaternion.identity;

        targetChest.localPosition = Vector3.zero;
        targetChest.localRotation = Quaternion.identity;

        // reset index in array for replay
        indexReplay = 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // RIGHT ARM

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
        targetRightArm.localPosition += new Vector3(moveRightArmX, moveRightArmY, moveRightArmZ) * Time.deltaTime * moveSpeed;
        targetRightArm.Rotate(
            (transform.localRotation.x + rotateRightArmX) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.y + rotateRightArmY) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.z + rotateRightArmZ) * Time.deltaTime * moveSpeed * 500);

        // save movement performed by the right target cube
        performedPositionsRightTarget.Add(new Vector3(moveRightArmX, moveRightArmY, moveRightArmZ));
        performedRotationsRightTarget.Add(new Vector3(rotateRightArmX, rotateRightArmY, rotateRightArmZ));

        // LEFT ARM

        // values used to move the target cube for the left arm
        float moveLeftArmX = actions.ContinuousActions[6];
        float moveLeftArmY = actions.ContinuousActions[7];
        float moveLeftArmZ = actions.ContinuousActions[8];

        float rotateLeftArmX = actions.ContinuousActions[9];
        float rotateLeftArmY = actions.ContinuousActions[10];
        float rotateLeftArmZ = actions.ContinuousActions[11];

        // move the target cube for the left arm
        targetLeftArm.localPosition += new Vector3(moveLeftArmX, moveLeftArmY, moveLeftArmZ) * Time.deltaTime * moveSpeed;
        targetLeftArm.Rotate(
            (transform.localRotation.x + rotateLeftArmX) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.y + rotateLeftArmY) * Time.deltaTime * moveSpeed * 500,
            (transform.localRotation.z + rotateLeftArmZ) * Time.deltaTime * moveSpeed * 500);

        // save movement performed by the left target cube
        performedPositionsLeftTarget.Add(new Vector3(moveLeftArmX, moveLeftArmY, moveLeftArmZ));
        performedRotationsLeftTarget.Add(new Vector3(rotateLeftArmX, rotateLeftArmY, rotateLeftArmZ));

        // HEAD

        // values used to move the target cube for the head
        float moveHeadX = actions.ContinuousActions[12];
        float moveHeadY = actions.ContinuousActions[13];
        float moveHeadZ = actions.ContinuousActions[14];

        float rotateHeadX = actions.ContinuousActions[15];
        float rotateHeadY = actions.ContinuousActions[16];
        float rotateHeadZ = actions.ContinuousActions[17];

        // move the target cube for the head
        targetHead.localPosition += new Vector3(moveHeadX, moveHeadY, moveHeadZ) * Time.deltaTime * moveSpeed;
        targetHead.Rotate(
            (transform.localRotation.x + rotateHeadX) * Time.deltaTime * moveSpeed * 100,
            (transform.localRotation.y + rotateHeadY) * Time.deltaTime * moveSpeed * 100,
            (transform.localRotation.z + rotateHeadZ) * Time.deltaTime * moveSpeed * 100);

        // save movement performed by the head target cube
        performedPositionsHeadTarget.Add(new Vector3(moveHeadX, moveHeadY, moveHeadZ));
        performedRotationsHeadTarget.Add(new Vector3(rotateHeadX, rotateHeadY, rotateHeadZ));

        // CHEST

        // values used to move the target cube for the head
        float moveChestX = actions.ContinuousActions[18];
        float moveChestY = actions.ContinuousActions[19];
        float moveChestZ = actions.ContinuousActions[20];

        float rotateChestX = actions.ContinuousActions[21];
        float rotateChestY = actions.ContinuousActions[22];
        float rotateChestZ = actions.ContinuousActions[23];

        // move the target cube for the head
        targetChest.localPosition += new Vector3(moveChestX, moveChestY, moveChestZ) * Time.deltaTime * moveSpeed;
        targetChest.Rotate(
            (transform.localRotation.x + rotateChestX) * Time.deltaTime * moveSpeed * 100,
            (transform.localRotation.y + rotateChestY) * Time.deltaTime * moveSpeed * 100,
            (transform.localRotation.z + rotateChestZ) * Time.deltaTime * moveSpeed * 100);

        // save movement performed by the head target cube
        performedPositionsChestTarget.Add(new Vector3(moveChestX, moveChestY, moveChestZ));
        performedRotationsChestTarget.Add(new Vector3(rotateChestX, rotateChestY, rotateChestZ));
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
