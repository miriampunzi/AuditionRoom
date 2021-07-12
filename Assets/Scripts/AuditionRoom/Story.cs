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

    [SerializeField] private GameObject playerPrefab;
        
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
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Instantiate(playerPrefab, new Vector3(0, 0, -3.8f), Quaternion.identity);

            GameObject VRCamera = GameObject.Find("VRCamera");
            Camera camera = VRCamera.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = new Vector3(0, 0, -3.8f);
        }

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
                currentState = State.Recording;
                ResetState();
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
