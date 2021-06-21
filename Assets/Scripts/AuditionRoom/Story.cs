using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class Story : MonoBehaviour
{
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
        Waiting,
        Winning,
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

    private StatePerformance currentState;

    private int indexPerformancesScript = 0;
    private int indexPerformingActor = 5;
    
    private bool trapdoorCoverUp = false;
    private bool hasStartedPlaying = false;
    private bool hasGoneDownFast = false;

    private Actor[] actors;
    TextMeshPro scriptTextMesh;


    private void Start()
    {
        scriptTextMesh = GetComponent<TextMeshPro>();
        actors = EnvironmentStatus.getActors();
        currentState = StatePerformance.Presentation;
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

                switch (currentState)
                {
                    case StatePerformance.Presentation:
                        // BEGIN
                        if (!trapdoorCoverUp)
                        {
                            actors[indexPerformingActor].transform.position = new Vector3(actors[indexPerformingActor].transform.position.x, actors[indexPerformingActor].transform.position.y + 0.1f, actors[indexPerformingActor].transform.position.z);
                            actors[indexPerformingActor].trapdoorCover.GoUpSlow();
                            trapdoorCoverUp = true;
                        }

                        // YES
                        if (EnvironmentStatus.wasYesPressed)
                        {
                            indexPerformancesScript++;
                            currentState = StatePerformance.Performace;
                            actors[indexPerformingActor].PlayAnimation();
                            trapdoorCoverUp = false;
                        }

                        // END
                        EnvironmentStatus.wasYesPressed = false;

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
                            currentState = StatePerformance.Replay;
                            indexPerformancesScript++;
                            hasStartedPlaying = false;
                        }
                        break;

                    case StatePerformance.Replay:
                        // YES
                        if (EnvironmentStatus.wasYesPressed && !EnvironmentStatus.wasNoPressed)
                        {
                            indexPerformancesScript--;
                            currentState = StatePerformance.Performace;
                            EnvironmentStatus.wasYesPressed = false;

                        }
                        // NO
                        else if (!EnvironmentStatus.wasYesPressed && EnvironmentStatus.wasNoPressed)
                        {
                            indexPerformancesScript++;
                            currentState = StatePerformance.Liking;
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
                            currentState = StatePerformance.Bye;
                        }
                        // NO
                        else if (!EnvironmentStatus.wasYesPressed && EnvironmentStatus.wasNoPressed)
                        {
                            scriptTextMesh.text = "BYE-BYE!!";
                            EnvironmentStatus.wasNoPressed = false;
                            indexPerformancesScript++;
                            currentState = StatePerformance.Bye;
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
                        if (hasGoneDownFast && !actors[indexPerformingActor].trapdoorCover.IsPlayingAnimation())
                        {
                            indexPerformancesScript = 0;
                            currentState = StatePerformance.Presentation;
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
                    //scriptTextMesh.text = "YES CLICKED";
                    EnvironmentStatus.wasYesPressed = false;
                    SceneManager.LoadScene("Camera");
                }
                // NO
                else if (!EnvironmentStatus.wasYesPressed && EnvironmentStatus.wasNoPressed)
                {
                    scriptTextMesh.text = "NO CLICKED";
                    EnvironmentStatus.wasNoPressed = false;
                    EnvironmentStatus.isVotingTime = true;
                }
            }
        }
        // VOTING TIME
        else
        {
            scriptTextMesh.text = "Now it's time to vote";
            
            for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
            {
                actors[i].transform.position = new Vector3(actors[i].transform.position.x, actors[i].transform.position.y + 0.1f, actors[i].transform.position.z);
                actors[i].trapdoorCover.GoUpSlow();
            }
        }
    }
}
