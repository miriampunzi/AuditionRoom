using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Story : MonoBehaviour
{
    public static RecordingStateMachine recordingStateMachine;
    public static LoadingStateMachine loadingStateMachine;
    public static PerformanceStateMachine performanceStateMachine;
    public static ReplayStateMachine replayStateMachine;
    public static VotingStateMachine votingStateMachine;

    private TextMeshPro scriptTextMesh;

    [SerializeField] private GameObject ViveCameraRigPrefab;
    [SerializeField] private GameObject collidersViveCameraRigPrefab;

    public enum State
    {
        Recording,
        LoadingPerformances,
        Performance,
        Replay,
        LoadingVoting,
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
        //    Instantiate(collidersViveCameraRigPrefab, new Vector3(0, 0, -3.7f), Quaternion.identity);
        //}

        EnvironmentStatus.Init();

        EnvironmentStatus.SetupRound();

        scriptTextMesh = GetComponent<TextMeshPro>();

        recordingStateMachine = new RecordingStateMachine();
        loadingStateMachine = new LoadingStateMachine();
        performanceStateMachine = new PerformanceStateMachine();
        replayStateMachine = new ReplayStateMachine();
        votingStateMachine = new VotingStateMachine();
    }

    public void Update()
    {
        switch (currentState)
        {
            case State.Recording:
                recordingStateMachine.Execute();
                break;

            case State.LoadingPerformances:
                loadingStateMachine.SetScript(
                    new ArrayList()
                    {
                        "Processing movement...",
                        "Actors are learning...",
                        "Almost done..."
                    }
                );
                loadingStateMachine.Execute();
                break;

            case State.Performance:
                performanceStateMachine.Execute();
                break;

            case State.Replay:
                replayStateMachine.Execute();
                break;

            case State.LoadingVoting:
                loadingStateMachine.SetScript(
                    new ArrayList()
                    {
                        "Loading...",
                        "Loading...",
                        "Loading..."
                    }
                );
                loadingStateMachine.Execute();
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
    }

    public static void NextState()
    {
        CleanVariables();

        switch (currentState)
        {
            case State.Recording:
                currentState = State.LoadingPerformances;
                recordingStateMachine.ResetStateMachine();

                break;

            case State.LoadingPerformances:
                currentState = State.Performance;
                loadingStateMachine.ResetStateMachine();

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
                    currentState = State.LoadingVoting;
                    performanceStateMachine.ResetStateMachine();
                }

                break;

            case State.Replay:
                currentState = State.LoadingVoting;
                replayStateMachine.ResetStateMachine();

                break;

            case State.LoadingVoting:
                currentState = State.Voting;
                loadingStateMachine.ResetStateMachine();

                break;

            case State.Voting:
                currentState = State.Performance;
                ResetState();

                EnvironmentStatus.SetupRound();

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
