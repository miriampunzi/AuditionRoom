using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Story : MonoBehaviour
{
    // state machines for game
    public static RecordingStateMachine recordingStateMachine;
    public static LoadingStateMachine loadingStateMachine;
    public static PerformanceStateMachine performanceStateMachine;
    public static ReplayStateMachine replayStateMachine;
    public static VotingStateMachine votingStateMachine;

    // state machines for no game
    public static RecordingStateMachineNoGame recordingStateMachineNoGame;
    public static PerformanceStateMachineNoGame performanceStateMachineNoGame;
    public static ReplayStateMachineNoGame replayStateMachineNoGame;
    public static VotingStateMachineNoGame votingStateMachineNoGame;

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

        if (EnvironmentStatus.isGame)
        {
            recordingStateMachine = new RecordingStateMachine();
            loadingStateMachine = new LoadingStateMachine();
            performanceStateMachine = new PerformanceStateMachine();
            replayStateMachine = new ReplayStateMachine();
            votingStateMachine = new VotingStateMachine();
        }
        else
        {
            recordingStateMachineNoGame = new RecordingStateMachineNoGame();
            performanceStateMachineNoGame = new PerformanceStateMachineNoGame();
            replayStateMachineNoGame = new ReplayStateMachineNoGame();
            votingStateMachineNoGame = new VotingStateMachineNoGame();
        }        
    }

    public void Update()
    {
        switch (currentState)
        {
            case State.Recording:
                if (EnvironmentStatus.isGame)
                    recordingStateMachine.Execute();
                else
                    recordingStateMachineNoGame.Execute();
                break;

            case State.LoadingPerformances:
                if (EnvironmentStatus.isGame)
                {
                    loadingStateMachine.SetScript(
                        new ArrayList()
                        {
                            "Processing movement...",
                            "Actors are learning...",
                            "Almost done..."
                        }
                    );
                    loadingStateMachine.Execute();
                }
                break;

            case State.Performance:
                if (EnvironmentStatus.isGame)
                    performanceStateMachine.Execute();
                else
                    performanceStateMachineNoGame.Execute();
                break;

            case State.Replay:
                if (EnvironmentStatus.isGame)
                    replayStateMachine.Execute();
                else
                    replayStateMachineNoGame.Execute();
                break;

            case State.LoadingVoting:
                if (EnvironmentStatus.isGame)
                {
                    loadingStateMachine.SetScript(
                        new ArrayList()
                        {
                        "Loading...",
                        "Loading...",
                        "Loading..."
                        }
                    );
                    loadingStateMachine.Execute();
                }
                break;

            case State.Voting:
                if (EnvironmentStatus.isGame)
                    votingStateMachine.Execute();
                else
                    votingStateMachineNoGame.Execute();
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
                if (EnvironmentStatus.isGame)
                {
                    currentState = State.LoadingPerformances;
                    recordingStateMachine.ResetStateMachine();
                }
                else
                {
                    currentState = State.Performance;
                    recordingStateMachineNoGame.ResetStateMachine();
                }

                break;

            case State.LoadingPerformances:
                if (EnvironmentStatus.isGame)
                {
                    currentState = State.Performance;
                    loadingStateMachine.ResetStateMachine();
                }

                break;

            case State.Performance:
                // YES
                if (wasYesPressed && !wasNoPressed)
                {
                    if (EnvironmentStatus.isGame)
                    {
                        wasYesPressed = false;
                        currentState = State.Replay;
                        performanceStateMachine.ResetStateMachine();
                    }
                    else
                    {
                        wasYesPressed = false;
                        currentState = State.Replay;
                        performanceStateMachineNoGame.ResetStateMachine();
                    }
                }
                // NO
                else if (!wasYesPressed && wasNoPressed)
                {
                    if (EnvironmentStatus.isGame)
                    {
                        wasNoPressed = false;
                        currentState = State.LoadingVoting;
                        performanceStateMachine.ResetStateMachine();
                    }
                    else
                    {
                        wasNoPressed = false;
                        currentState = State.Voting;
                        performanceStateMachineNoGame.ResetStateMachine();
                    }
                }

                break;

            case State.Replay:
                if (EnvironmentStatus.isGame)
                {
                    currentState = State.LoadingVoting;
                    replayStateMachine.ResetStateMachine();
                }
                else
                {
                    currentState = State.Voting;
                    replayStateMachineNoGame.ResetStateMachine();
                }

                break;

            case State.LoadingVoting:
                if (EnvironmentStatus.isGame)
                {
                    currentState = State.Voting;
                    loadingStateMachine.ResetStateMachine();
                }

                break;

            case State.Voting:
                if (EnvironmentStatus.isGame)
                {
                    currentState = State.Performance;
                    ResetState();

                    EnvironmentStatus.SetupRound();

                    performanceStateMachine.ResetStateMachine();
                    replayStateMachine.ResetStateMachine();
                    votingStateMachine.ResetStateMachine();
                }
                else
                {
                    currentState = State.Performance;
                    ResetState();

                    EnvironmentStatus.SetupRound();

                    performanceStateMachineNoGame.ResetStateMachine();
                    replayStateMachineNoGame.ResetStateMachine();
                    votingStateMachineNoGame.ResetStateMachine();
                }

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
