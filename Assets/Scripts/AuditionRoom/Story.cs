using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Story : MonoBehaviour
{
    public static RecordingStateMachine recordingStateMachine;
    public static PerformanceStateMachine performanceStateMachine;
    public static ReplayStateMachine replayStateMachine;
    public static VotingStateMachine votingStateMachine;

    private TextMeshPro scriptTextMesh;

    [SerializeField] private GameObject ViveCameraRigPrefab;
    [SerializeField] private GameObject collidersViveCameraRigPrefab;

    public enum State
    {
        Recording,
        Performance,
        Replay,
        Voting
    }

    public static State currentState = State.Recording;

    public static bool trapdoorCoverUp = false;
    public static bool hasGoneDownFast = false;
    public static bool wasYesPressed = false;
    public static bool wasNoPressed = false;

    public static bool hasAskedForReplay = false;
    public static int idActorForReplay = -1;

    public static bool hasVoted = false;
    public static int bestActorVoted = -1;

    private void Start()
    {
        //if (GameObject.FindGameObjectWithTag("ViveCameraRig") == null)
        //{
        //    Instantiate(ViveCameraRigPrefab, new Vector3(0, 0, -3.7f), Quaternion.identity);

        //    GameObject VRCamera = GameObject.Find("Camera");
        //    Camera camera = VRCamera.GetComponent<Camera>();
        //    camera.clearFlags = CameraClearFlags.SolidColor;
        //    camera.backgroundColor = Color.black;
        //}

        //if (GameObject.FindGameObjectWithTag("ViveColliders") == null)
        //{
        //    Instantiate(ViveCameraRigPrefab, new Vector3(0, 0, -3.7f), Quaternion.identity);

        //    GameObject colliderEventCasterR = GameObject.Find("ViveColliders/Right/PoseTracker/ColliderEventCaster");
        //    colliderEventCasterR.SetActive(true);
        //    GameObject sphereColliderR = GameObject.Find("ViveColliders/Right/PoseTracker/ColliderEventCaster/SphereCollider");
        //    sphereColliderR.GetComponent<SphereCollider>().isTrigger = true;
        //    GameObject boxColliderR = GameObject.Find("ViveColliders/Right/PoseTracker/ColliderEventCaster/BoxCollider");
        //    boxColliderR.GetComponent<SphereCollider>().isTrigger = true;

        //    GameObject colliderEventCasterL = GameObject.Find("ViveColliders/Left/PoseTracker/ColliderEventCaster");
        //    colliderEventCasterL.SetActive(true);
        //    GameObject sphereColliderL = GameObject.Find("ViveColliders/Left/PoseTracker/ColliderEventCaster/SphereCollider");
        //    sphereColliderL.GetComponent<SphereCollider>().isTrigger = true;
        //    GameObject boxColliderL = GameObject.Find("ViveColliders/Left/PoseTracker/ColliderEventCaster/BoxCollider");
        //    boxColliderL.GetComponent<SphereCollider>().isTrigger = true;
        //}

        EnvironmentStatus.PlaceActors();

        scriptTextMesh = GetComponent<TextMeshPro>();

        recordingStateMachine = new RecordingStateMachine(scriptTextMesh);
        performanceStateMachine = new PerformanceStateMachine(scriptTextMesh);
        replayStateMachine = new ReplayStateMachine(scriptTextMesh);
        votingStateMachine = new VotingStateMachine(scriptTextMesh);
    }

    public void Update()
    {
        switch (currentState)
        {
            case State.Recording:
                recordingStateMachine.Execute();
                break;

            case State.Performance:
                performanceStateMachine.Execute();
                break;

            case State.Replay:
                replayStateMachine.Execute();
                break;

            case State.Voting:
                votingStateMachine.Execute();
                break;
        }
    }

    public static void CleanVariables()
    {
        trapdoorCoverUp = false;
        hasGoneDownFast = false;
    }

    public static void CleanDeskVariables()
    {
        wasYesPressed = false;
        wasNoPressed = false;

        hasAskedForReplay = false;
        idActorForReplay = -1;

        //hasVoted = false;
        //bestActorVoted = -1;
    }

    public static void NextState()
    {
        CleanVariables();

        switch (currentState)
        {
            case State.Recording:
                currentState = State.Performance;
                recordingStateMachine.ResetStateMachine();

                break;

            case State.Performance:
                // YES
                if (wasYesPressed && !wasNoPressed)
                {
                    wasYesPressed = false;
                    currentState = State.Replay;
                    performanceStateMachine.ResetStateMachine();
                }
                // NO
                else if (!wasYesPressed && wasNoPressed)
                {
                    wasNoPressed = false;
                    currentState = State.Voting;
                    performanceStateMachine.ResetStateMachine();
                }

                break;

            case State.Replay:
                currentState = State.Voting;
                replayStateMachine.ResetStateMachine();

                break;

            case State.Voting:
                currentState = State.Performance;
                ResetState();

                EnvironmentStatus.PlaceActors();

                performanceStateMachine.ResetStateMachine();
                replayStateMachine.ResetStateMachine();
                votingStateMachine.ResetStateMachine();

                break;
        }
    }

    public static void ResetState()
    {
        trapdoorCoverUp = false;
        hasGoneDownFast = false;

        wasYesPressed = false;

        wasNoPressed = false;

        hasAskedForReplay = false;
        idActorForReplay = -1;

        hasVoted = false;
        bestActorVoted = -1;
    }

}
