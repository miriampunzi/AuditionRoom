using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Story : MonoBehaviour
{
    // COMMON PARAMETERS
    [SerializeField] private GameObject playerPrefab;

    private List<Actor> actors;
    TextMeshPro scriptTextMesh;

    public enum State
    {
        Performance,
        Replay,
        Voting
    }

    public static State currentState = State.Performance;

    private bool trapdoorCoverUp = false;
    private bool hasStartedPlaying = false;
    private bool hasGoneDownFast = false;

    // DESK BUTTONS
    // yes button value
    public static bool wasYesPressed = false;

    // no button value
    public static bool wasNoPressed = false;

    // trapdoor button value for replay
    public static bool hasAskedForReplay = false;
    public static int idActorForReplay = -1;

    // trapdoor button value for voting
    public static bool hasVoted = false;
    public static int bestActorVoted = -1;

    // PERFORMANCES PARAMETERS
    private enum StatePerformance
    {
        Presentation,
        Performace,
        Replay,
        Liking,
        Bye
    }

    private ArrayList performancesScript = new ArrayList()
    {
        "Now it’s the turn of the actor $. Are you ready to see the performance?",
        "Performance...",
        "Do you want to see a replay?",
        "Did you like the performance?"
    };

    private StatePerformance currentStatePerformance;

    private int indexPerformancesScript = 0;
    private int indexPerformingActor = 0;

    // REPLAY PARAMETERS
    private enum StateReplay
    {
        Question,
        Performance,
        Continue
    }

    private ArrayList replayScript = new ArrayList()
    {
        "Which actor do you want to ask for a replay",
        "Performance...",
        "Do you want to see other replays?"
    };

    private StateReplay currentStateReplay;
    private int indexReplayScript = 0;

    private bool trapdoorCoverDown = false;

    // VOTING PARAMETERS
    private enum StateVoting
    {
        ActorsAppear,
        Choosing,
        Win,
        Bye
    }

    private ArrayList votingScript = new ArrayList()
    {
        "Now it’s time to vote",
        "Which was the best actor?",
        "Congratulations actor num ",
        "Bye bye the others!"
    };

    private StateVoting currentStateVoting;
    private int indexVotingScript = 0;

    private bool hasStartedPlayingWin = false;

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
        actors = EnvironmentStatus.getActors();
        currentStatePerformance = StatePerformance.Presentation;
    }

    public void Update()
    {
        
        switch (currentState)
        {
            case State.Performance:
                PerformancesStateMachine();
                break;

            case State.Replay:
                ReplayStateMachine();
                break;

            case State.Voting:
                VotingStateMachine();
                break;
        }
    }

    private void PerformancesStateMachine()
    {
        if (indexPerformingActor < EnvironmentStatus.NUM_ACTORS)
        {
            // update text script
            if (indexPerformancesScript < performancesScript.Count)
            {
                if (currentStatePerformance == StatePerformance.Presentation)
                {
                    string modifiedText = ((string)performancesScript[indexPerformancesScript]).Replace("$", "" + (indexPerformingActor + 1));
                    scriptTextMesh.text = modifiedText;
                }
                else
                    scriptTextMesh.text = (string)performancesScript[indexPerformancesScript];
            }

            switch (currentStatePerformance)
            {
                case StatePerformance.Presentation:
                    // BEGIN
                    if (!trapdoorCoverUp)
                    {
                        actors[indexPerformingActor].transform.position = new Vector3(actors[indexPerformingActor].transform.position.x, actors[indexPerformingActor].transform.position.y + 0.1f, actors[indexPerformingActor].transform.position.z);
                        actors[indexPerformingActor].trapdoorCover.GoUpSlow();
                        trapdoorCoverUp = true;
                    }

                    // TODO CONTROLLA CHE I TRAPDOORCOVER SIANO SU PRIMA DI CONTROLLARE IL SI PER EVITARE CHE SUCCEDANO COSE QUANDO L'UTENTE CLICCA SI A CASO

                    // YES
                    if (trapdoorCoverUp && wasYesPressed)
                    {
                        indexPerformancesScript++;
                        currentStatePerformance = StatePerformance.Performace;
                        actors[indexPerformingActor].PlayAnimation();
                        trapdoorCoverUp = false;
                    }

                    // END
                    CleanDeskVariables();

                    break;

                case StatePerformance.Performace:
                    // BEGIN
                    if (!hasStartedPlaying)
                    {
                        actors[indexPerformingActor].PlayAnimation();
                        hasStartedPlaying = true;
                    }

                    // TIME EXPIRED
                    if (hasStartedPlaying && !actors[indexPerformingActor].IsPlayingAnimation())
                    {
                        currentStatePerformance = StatePerformance.Replay;
                        indexPerformancesScript++;
                        hasStartedPlaying = false;
                    }
                    break;

                case StatePerformance.Replay:
                    // YES
                    if (wasYesPressed && !wasNoPressed)
                    {
                        indexPerformancesScript--;
                        currentStatePerformance = StatePerformance.Performace;
                        wasYesPressed = false;

                    }
                    // NO
                    else if (!wasYesPressed && wasNoPressed)
                    {
                        indexPerformancesScript++;
                        currentStatePerformance = StatePerformance.Liking;
                        wasNoPressed = false;
                    }

                    break;

                case StatePerformance.Liking:
                    // YES
                    if (wasYesPressed && !wasNoPressed)
                    {
                        scriptTextMesh.text = "Great, see him/her later!";
                        wasYesPressed = false;
                        indexPerformancesScript++;
                        currentStatePerformance = StatePerformance.Bye;
                    }
                    // NO
                    else if (!wasYesPressed && wasNoPressed)
                    {
                        scriptTextMesh.text = "BYE-BYE!!";
                        wasNoPressed = false;
                        indexPerformancesScript++;
                        currentStatePerformance = StatePerformance.Bye;
                    }

                    break;

                case StatePerformance.Bye:
                    // BEGIN
                    if (!hasGoneDownFast)
                    {
                        actors[indexPerformingActor].trapdoorCover.GoDownFast();
                        hasGoneDownFast = true;
                    }

                    // TIME EXPIRED
                    if (hasGoneDownFast && !actors[indexPerformingActor].trapdoorCover.IsGoingDownFast())
                    {
                        indexPerformancesScript = 0;
                        currentStatePerformance = StatePerformance.Presentation;
                        indexPerformingActor++;
                        hasGoneDownFast = false;
                    }

                    break;

                default:
                    scriptTextMesh.color = Color.red;
                    scriptTextMesh.text = "YOU SHOULDN'T ENTER HERE";

                    break;
            }
        }
        // SEE OLD PERFORMANCES
        else
        {
            scriptTextMesh.text = "Now all the actors have finished. Do you want to see old performances?";

            // YES
            if (wasYesPressed && !wasNoPressed)
            {
                wasYesPressed = false;
                currentState = State.Replay;
                CleanVariables();
            }
            // NO
            else if (!wasYesPressed && wasNoPressed)
            {
                wasNoPressed = false;
                currentState = State.Voting;
                CleanVariables();
            }
        }
    }

    private void ReplayStateMachine()
    {
        // update text script
        if (indexReplayScript < replayScript.Count)
        {
            scriptTextMesh.text = (string)replayScript[indexReplayScript];

            switch (currentStateReplay)
            {
                case StateReplay.Question:
                    if (hasAskedForReplay)
                    {
                        indexReplayScript++;
                        currentStateReplay = StateReplay.Performance;
                        hasAskedForReplay = false;
                    }

                    break;

                case StateReplay.Performance:
                    if (!trapdoorCoverUp)
                    {
                        actors[idActorForReplay - 1].transform.position = new Vector3(
                            actors[idActorForReplay - 1].transform.position.x,
                            actors[idActorForReplay - 1].transform.position.y + 0.1f,
                            actors[idActorForReplay - 1].transform.position.z);
                        actors[idActorForReplay - 1].trapdoorCover.GoUpSlow();
                        trapdoorCoverUp = true;
                    }

                    if (!actors[idActorForReplay - 1].trapdoorCover.IsGoingUpSlow() && !hasStartedPlaying && !trapdoorCoverDown)
                    {
                        actors[idActorForReplay - 1].PlayAnimation();
                        hasStartedPlaying = true;
                    }

                    if (hasStartedPlaying && !actors[idActorForReplay - 1].IsPlayingAnimation())
                    {
                        hasStartedPlaying = false;
                        actors[idActorForReplay - 1].trapdoorCover.GoDownSlow();
                        trapdoorCoverDown = true;
                    }

                    if (trapdoorCoverDown && !actors[idActorForReplay - 1].trapdoorCover.IsGoingDownSlow())
                    {
                        currentStateReplay = StateReplay.Continue;
                        indexReplayScript++;
                        trapdoorCoverDown = false;
                    }

                    break;

                case StateReplay.Continue:
                    // YES
                    if (wasYesPressed && !wasNoPressed)
                    {
                        indexReplayScript = 0;
                        currentStateReplay = StateReplay.Question;
                        wasYesPressed = false;

                        trapdoorCoverUp = false;
                        hasStartedPlaying = false;
                        trapdoorCoverDown = false;
                    }
                    // NO
                    else if (!wasYesPressed && wasNoPressed)
                    {
                        wasNoPressed = false;
                        currentState = State.Voting;
                        CleanVariables();
                    }

                    break;
            }
        }
    }

    private void VotingStateMachine()
    {
        // update text script
        if (indexVotingScript < votingScript.Count)
        {
            scriptTextMesh.text = (string)votingScript[indexVotingScript];
        }

        switch (currentStateVoting)
        {
            case StateVoting.ActorsAppear:
                // BEGIN
                if (!trapdoorCoverUp)
                {
                    for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                    {
                        actors[i].transform.position = new Vector3(actors[i].transform.position.x, actors[i].transform.position.y + 0.5f, actors[i].transform.position.z);
                        actors[i].trapdoorCover.GoUpSlow();
                    }
                    trapdoorCoverUp = true;
                }

                // TIME EXPIRED
                if (trapdoorCoverUp && !actors[EnvironmentStatus.NUM_ACTORS - 1].trapdoorCover.IsGoingUpSlow())
                {
                    trapdoorCoverUp = true;
                    indexVotingScript++;
                    currentStateVoting = StateVoting.Choosing;
                }

                break;

            case StateVoting.Choosing:
                if (hasVoted)
                {
                    indexVotingScript++;
                    currentStateVoting = StateVoting.Win;
                    hasVoted = false;
                }

                break;

            case StateVoting.Win:
                // BEGIN
                if (!hasStartedPlayingWin)
                {
                    for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                    {
                        if (bestActorVoted == actors[i].id)
                        {
                            actors[i].PlayVictory();
                        }
                        else
                        {
                            actors[i].PlayDefeat();
                        }
                    }

                    hasStartedPlayingWin = true;
                }

                // TIME EXPIRED
                if (hasStartedPlayingWin && !actors[bestActorVoted - 1].IsPlayingWinning())
                {
                    indexVotingScript++;
                    currentStateVoting = StateVoting.Bye;
                    hasStartedPlayingWin = false;
                }

                break;

            case StateVoting.Bye:
                // BEGIN
                if (!hasGoneDownFast)
                {
                    for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                        if (actors[i].id != bestActorVoted)
                            actors[i].trapdoorCover.GoDownFast();
                    hasGoneDownFast = true;
                }

                // TIME EXPIRED
                if (hasGoneDownFast && !actors[0].trapdoorCover.IsGoingDownFast())
                {
                    scriptTextMesh.text = "THE END";
                    hasGoneDownFast = false;
                }

                break;

            default:
                scriptTextMesh.color = Color.red;
                scriptTextMesh.text = "YOU SHOULDN'T ENTER HERE";

                break;
        }
    }
        
    private void CleanVariables()
    {
        trapdoorCoverUp = false;
        hasStartedPlaying = false;
        hasGoneDownFast = false;
        trapdoorCoverDown = false;
        hasStartedPlayingWin = false;
    }

    private void CleanDeskVariables()
    {
        wasYesPressed = false;
        wasNoPressed = false;

        hasAskedForReplay = false;
        idActorForReplay = -1;

        hasVoted = false;
        bestActorVoted = -1;
    }
}
