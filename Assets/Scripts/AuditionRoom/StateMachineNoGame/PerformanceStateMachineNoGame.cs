using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerformanceStateMachineNoGame : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;

    private enum StatePerformance
    {
        Presentation,
        Performace,
        Replay
    }

    private ArrayList performancesScript = new ArrayList()
    {
        "Ready to see Avatar $?",
        "Performance...",
        "Do you want to see a replay?",
    };

    private StatePerformance currentStatePerformance = StatePerformance.Presentation;

    private int indexPerformancesScript = 0;
    private int indexPerformingActor = 0;

    private bool hasStartedPlayingAnimation = false;

    public PerformanceStateMachineNoGame()
    {
        scriptTextMesh = GameObject.FindGameObjectWithTag("Script").GetComponent<TextMeshPro>();
    }

    public void Execute()
    {
        // Learning in background underground
        for (int i = 0; i < EnvironmentStatus.allActors.Count; i++)
        {
            if (EnvironmentStatus.allActors[i].idPerformance == -1)
            {
                EnvironmentStatus.allActors[i].rightArmAgent.isForPerformance = false;
                EnvironmentStatus.allActors[i].leftArmAgent.isForPerformance = false;
                EnvironmentStatus.allActors[i].headChestAgent.isForPerformance = false;

                EnvironmentStatus.allActors[i].rightArmAgent.LearnInBackground();
                EnvironmentStatus.allActors[i].leftArmAgent.LearnInBackground();
                EnvironmentStatus.allActors[i].headChestAgent.LearnInBackground();
            }
        }

        if (indexPerformingActor < EnvironmentStatus.performingActors.Count)
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
                    // YES
                    if (Story.wasYesPressed && !Story.wasNoPressed)
                    {
                        indexPerformancesScript++;
                        currentStatePerformance = StatePerformance.Performace;
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
                        else {
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
                            EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.isForPerformance = false;
                            EnvironmentStatus.performingActors[indexPerformingActor].leftArmAgent.isForPerformance = false;
                            EnvironmentStatus.performingActors[indexPerformingActor].headChestAgent.isForPerformance = false;

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
                                EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.isForPerformance = false;
                                EnvironmentStatus.performingActors[indexPerformingActor].leftArmAgent.isForPerformance = false;
                                EnvironmentStatus.performingActors[indexPerformingActor].headChestAgent.isForPerformance = false;

                                currentStatePerformance = StatePerformance.Replay;
                                indexPerformancesScript++;
                            }
                        }
                    }

                    // END
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
                        EnvironmentStatus.performingActors[indexPerformingActor].rightArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[indexPerformingActor].leftArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[indexPerformingActor].headChestAgent.SetupForReplay();

                        Story.hasAskedForReplay = false;
                        indexPerformancesScript = 0;
                        indexPerformingActor++;
                        currentStatePerformance = StatePerformance.Presentation;
                        Story.wasNoPressed = false;
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
