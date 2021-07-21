using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryNoGame : MonoBehaviour
{
    public static PerformanceStateMachineNoGame performanceStateMachine;
    public static ReplayStateMachineNoGame replayStateMachine;
    public static VotingStateMachineNoGame votingStateMachine;

    private TextMeshPro scriptTextMesh;

    [SerializeField] private GameObject playerPrefab;
        
    public enum State
    {
        Performance,
        Replay,
        Voting
    }

    public static State currentState = State.Performance;

    public static bool wasYesPressed = false;
    public static bool wasNoPressed = false;

    public static bool hasAskedForReplay = false;
    public static int idActorForReplay = -1;

    public static bool hasVoted = false;
    public static int bestActorVoted = -1;

    public static bool actorsPlaced = false;

    private void Start()
    {
        EnvironmentStatusNoGame.PlaceActors();

        scriptTextMesh = GetComponent<TextMeshPro>();

        performanceStateMachine = new PerformanceStateMachineNoGame(scriptTextMesh);
        replayStateMachine = new ReplayStateMachineNoGame(scriptTextMesh);
        votingStateMachine = new VotingStateMachineNoGame(scriptTextMesh);
    }

    public void Update()
    {
        switch (currentState)
        {
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

    public static void CleanDeskVariables()
    {
        wasYesPressed = false;
        wasNoPressed = false;

        hasAskedForReplay = false;
        idActorForReplay = -1;
    }

    public static void NextState()
    {
        switch (currentState)
        {
            case State.Performance:
                // YES
                if (wasYesPressed && !wasNoPressed)
                {
                    wasYesPressed = false;
                    currentState = State.Replay;
                }
                // NO
                else if (!wasYesPressed && wasNoPressed)
                {
                    wasNoPressed = false;
                    currentState = State.Voting;
                }

                break;

            case State.Replay:
                currentState = State.Voting;

                break;

            case State.Voting:
                currentState = State.Performance;
                ResetState();

                EnvironmentStatusNoGame.PlaceActors();

                performanceStateMachine.ResetStateMachine();
                replayStateMachine.ResetStateMachine();
                votingStateMachine.ResetStateMachine();

                break;
        }
    }

    public static void ResetState()
    {
        wasYesPressed = false;

        wasNoPressed = false;

        hasAskedForReplay = false;
        idActorForReplay = -1;

        hasVoted = false;
        bestActorVoted = -1;
    }

}
