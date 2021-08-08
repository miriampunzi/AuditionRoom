using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerformanceStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;

    private enum StatePerformance
    {
        Presentation,
        Performace,
        Replay,
        Bye
    }

    private ArrayList performancesScript = new ArrayList()
    {
        "Now it’s the turn of the actor $. Are you ready to see the performance?",
        "Performance...",
        "Do you want to see a replay?",
        "Bye-bye, see you later!"
    };

    private StatePerformance currentStatePerformance = StatePerformance.Presentation;

    private int indexPerformancesScript = 0;
    private int indexPerformingActor = 0;

    private bool hasStartedPlayingAnimation = false;

    public PerformanceStateMachine()
    {
        scriptTextMesh = GameObject.FindGameObjectWithTag("Script").GetComponent<TextMeshPro>();
    }

    public void Execute()
    {
        if (indexPerformingActor < EnvironmentStatus.NUM_PERFORMING_ACTORS_GAME)
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
                    // BEGIN: ACTOR APPEAR IN THE ENVIRONMENT THROUGH THE TRAPDOOR
                    if (!Story.trapdoorCoverUp)
                    {
                        EnvironmentStatus.performingActors[indexPerformingActor].transform.position = 
                            new Vector3(EnvironmentStatus.performingActors[indexPerformingActor].transform.position.x,
                            EnvironmentStatus.performingActors[indexPerformingActor].transform.position.y + 0.1f,
                            EnvironmentStatus.performingActors[indexPerformingActor].transform.position.z);
                        EnvironmentStatus.performingActors[indexPerformingActor].trapdoorCover.GoUpSlow();
                        Story.trapdoorCoverUp = true;
                    }

                    // ON CLICK YES
                    if (!EnvironmentStatus.performingActors[indexPerformingActor].trapdoorCover.IsGoingUpSlow() && Story.trapdoorCoverUp && Story.wasYesPressed)
                    {
                        indexPerformancesScript++;
                        currentStatePerformance = StatePerformance.Performace;

                        Story.trapdoorCoverUp = false;                        
                    }

                    // END
                    Story.CleanDeskVariables();

                    break;

                case StatePerformance.Performace:

                    if (!Story.hasAskedForReplay)
                    {
                        // BEGIN PERFORMANCE
                        if (EnvironmentStatus.performingActors[indexPerformingActor].isHuman)
                        {
                            if (!hasStartedPlayingAnimation)
                            {
                                EnvironmentStatus.performingActors[indexPerformingActor].PlayAnimation();
                                hasStartedPlayingAnimation = true;
                            }
                        }
                        else
                        {
                            EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.isForPerformance = true;
                            EnvironmentStatus.performingActors[indexPerformingActor].leftArmAgent.isForPerformance = true;
                            EnvironmentStatus.performingActors[indexPerformingActor].headChestAgent.isForPerformance = true;

                            EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.PerformAction();
                            EnvironmentStatus.performingActors[indexPerformingActor].leftArmAgent.PerformAction();
                            EnvironmentStatus.performingActors[indexPerformingActor].headChestAgent.PerformAction();
                        }

                        // FINISHED PERFORMANCE
                        if (EnvironmentStatus.performingActors[indexPerformingActor].isHuman)
                        {
                            if (hasStartedPlayingAnimation && !EnvironmentStatus.performingActors[indexPerformingActor].IsPlayingAnimation())
                            {
                                currentStatePerformance = StatePerformance.Replay;
                                indexPerformancesScript++;
                                hasStartedPlayingAnimation = false;
                            }
                        }
                        else
                        {
                            if (!EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.IsPlayingPerformance())
                            {
                                EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.isForPerformance = false;
                                EnvironmentStatus.performingActors[indexPerformingActor].leftArmAgent.isForPerformance = false;
                                EnvironmentStatus.performingActors[indexPerformingActor].headChestAgent.isForPerformance = false;

                                currentStatePerformance = StatePerformance.Replay;
                                indexPerformancesScript++;
                            }
                        }
                    }
                    else
                    {
                        // BEGIN REPLAY
                        if (EnvironmentStatus.performingActors[indexPerformingActor].isHuman)
                        {
                            if (!hasStartedPlayingAnimation)
                            {
                                EnvironmentStatus.performingActors[indexPerformingActor].PlayAnimation();
                                hasStartedPlayingAnimation = true;
                            }
                        }
                        else
                        {
                            EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.PerformReplay();
                            EnvironmentStatus.performingActors[indexPerformingActor].leftArmAgent.PerformReplay();
                            EnvironmentStatus.performingActors[indexPerformingActor].headChestAgent.PerformReplay();
                        }

                        // FINISHED REPLAY
                        if (EnvironmentStatus.performingActors[indexPerformingActor].isHuman)
                        {
                            if (hasStartedPlayingAnimation && !EnvironmentStatus.performingActors[indexPerformingActor].IsPlayingAnimation())
                            {
                                currentStatePerformance = StatePerformance.Replay;
                                indexPerformancesScript++;
                                hasStartedPlayingAnimation = false;
                            }
                        }
                        else
                        {
                            if (!EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.IsPlayingReplay())
                            {
                                currentStatePerformance = StatePerformance.Replay;
                                indexPerformancesScript++;
                            }
                        }
                    }

                    Story.wasYesPressed = false;
                    Story.wasNoPressed = false;

                    break;

                case StatePerformance.Replay:
                    // YES
                    if (Story.wasYesPressed && !Story.wasNoPressed)
                    {
                        EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[indexPerformingActor].leftArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[indexPerformingActor].headChestAgent.SetupForReplay();

                        Story.hasAskedForReplay = true;
                        indexPerformancesScript--;
                        currentStatePerformance = StatePerformance.Performace;
                        Story.wasYesPressed = false;
                    }

                    // NO
                    else if (!Story.wasYesPressed && Story.wasNoPressed)
                    {
                        Story.hasAskedForReplay = false;
                        indexPerformancesScript++;
                        currentStatePerformance = StatePerformance.Bye;
                        Story.wasNoPressed = false;
                    }

                    break;

                case StatePerformance.Bye:
                    // BEGIN
                    if (!Story.hasGoneDownFast)
                    {
                        EnvironmentStatus.performingActors[indexPerformingActor].trapdoorCover.GoDownFast();
                        Story.hasGoneDownFast = true;
                    }

                    // TIME EXPIRED
                    if (Story.hasGoneDownFast && !EnvironmentStatus.performingActors[indexPerformingActor].trapdoorCover.IsGoingDownFast())
                    {
                        indexPerformancesScript = 0;
                        currentStatePerformance = StatePerformance.Presentation;
                        indexPerformingActor++;
                        Story.hasGoneDownFast = false;
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
            scriptTextMesh.text = "Now all the actors have finished. Do you want to replay one of the performances?";

            for (int i = 0; i < EnvironmentStatus.performingActors.Count; i++)
                EnvironmentStatus.performingActors[i].PlayIdle();

            Story.NextState();
        }
    }

    public void ResetStateMachine()
    {
        indexPerformancesScript = 0;
        indexPerformingActor = 0;
        currentStatePerformance = StatePerformance.Presentation;
    }
}
