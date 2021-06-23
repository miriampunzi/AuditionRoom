using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class PerformanceStory : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private enum StatePerformance
    {
        Presentation,
        Performace,
        Replay,
        Liking,
        Bye
    }

    private enum StateVoting
    {
        ActorsAppear,
        Choosing,
        Win,
        Bye
    }

    private ArrayList performancesScript = new ArrayList()
    {
        "Now it’s the turn of the actor 1. Are you ready to see the performance?",
        "Performance...",
        "Do you want to see a replay?",
        "Did you like the performance?"
    };

    private ArrayList cameraScript = new ArrayList()
    {
        "Now all the actors have finished. Do you want to see old performances?"
    };

    private ArrayList votingScript = new ArrayList()
    {
        "Now it’s time to vote",
        "Which was the best actor?",
        "Congratulations actor num ",
        "Bye bye the others!"
    };

    private StatePerformance currentStatePerformance;
    private StateVoting currentStateVoting;

    private int indexPerformancesScript = 0;
    private int indexVotingScript = 0;
    private int indexPerformingActor = 5;
    
    private bool trapdoorCoverUp = false;
    private bool hasStartedPlaying = false;
    private bool hasGoneDownFast = false;
    private bool hasStartedPlayingWin = false;

    private List<Actor> actors;
    TextMeshPro scriptTextMesh;


    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Instantiate(playerPrefab, new Vector3(0, 0, -3.8f), Quaternion.identity);

            GameObject VRCamera = GameObject.Find("VRCamera");
            Camera camera = VRCamera.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

        scriptTextMesh = GetComponent<TextMeshPro>();
        actors = EnvironmentStatus.getActors();
        currentStatePerformance = StatePerformance.Presentation;
        currentStateVoting = StateVoting.ActorsAppear;
    }

    void Update()
    {
        if (!EnvironmentStatus.isVotingTime)
        {
            // PERFORMANCES
            if (indexPerformingActor < EnvironmentStatus.NUM_ACTORS)
            {
                if (indexPerformancesScript < performancesScript.Count)
                {
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
                        if (EnvironmentStatus.wasYesPressed)
                        {
                            indexPerformancesScript++;
                            currentStatePerformance = StatePerformance.Performace;
                            actors[indexPerformingActor].PlayAnimation();
                            trapdoorCoverUp = false;
                        }

                        // END
                        EnvironmentStatus.wasYesPressed = false;
                        EnvironmentStatus.wasNoPressed = false;

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
                        if (EnvironmentStatus.wasYesPressed && !EnvironmentStatus.wasNoPressed)
                        {
                            indexPerformancesScript--;
                            currentStatePerformance = StatePerformance.Performace;
                            EnvironmentStatus.wasYesPressed = false;

                        }
                        // NO
                        else if (!EnvironmentStatus.wasYesPressed && EnvironmentStatus.wasNoPressed)
                        {
                            indexPerformancesScript++;
                            currentStatePerformance = StatePerformance.Liking;
                            EnvironmentStatus.wasNoPressed = false;
                        }

                        break;

                    case StatePerformance.Liking:
                        // YES
                        if (EnvironmentStatus.wasYesPressed && !EnvironmentStatus.wasNoPressed)
                        {
                            scriptTextMesh.text = "Great, see him/her later!";
                            EnvironmentStatus.wasYesPressed = false;
                            indexPerformancesScript++;
                            currentStatePerformance = StatePerformance.Bye;
                        }
                        // NO
                        else if (!EnvironmentStatus.wasYesPressed && EnvironmentStatus.wasNoPressed)
                        {
                            scriptTextMesh.text = "BYE-BYE!!";
                            EnvironmentStatus.wasNoPressed = false;
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
                if (EnvironmentStatus.wasYesPressed && !EnvironmentStatus.wasNoPressed)
                {
                    EnvironmentStatus.wasYesPressed = false;
                    SceneManager.LoadScene("Replay");
                }
                // NO
                else if (!EnvironmentStatus.wasYesPressed && EnvironmentStatus.wasNoPressed)
                {
                    EnvironmentStatus.wasNoPressed = false;
                    EnvironmentStatus.isVotingTime = true;
                }
            }
        }
        // VOTING TIME
        else
        {
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
                    if (EnvironmentStatus.hasVoted)
                    {
                        indexVotingScript++;
                        currentStateVoting = StateVoting.Win;
                        EnvironmentStatus.hasVoted = false;
                    }

                    break;

                case StateVoting.Win:
                    // BEGIN
                    if (!hasStartedPlayingWin)
                    {
                        for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                        {
                            if (EnvironmentStatus.bestActorVoted == actors[i].id)
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
                    if (hasStartedPlayingWin && !actors[EnvironmentStatus.bestActorVoted - 1].IsPlayingWinning())
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
                            if (actors[i].id != EnvironmentStatus.bestActorVoted)
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
    }
}
