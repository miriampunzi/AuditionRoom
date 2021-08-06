using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerformanceStateMachineNoGame : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private List<Actor> actors;
    private List<ActorMonoBehavior> actorsMonoBehavior;

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

    public PerformanceStateMachineNoGame(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
        actors = EnvironmentStatusNoGame.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
    }

    public void Execute()
    {
        if (indexPerformingActor < EnvironmentStatusNoGame.NUM_ACTORS)
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
                    if (StoryNoGame.wasYesPressed && !StoryNoGame.wasNoPressed)
                    {
                        indexPerformancesScript++;
                        currentStatePerformance = StatePerformance.Performace;
                    }

                    // END
                    StoryNoGame.CleanDeskVariables();

                    break;

                case StatePerformance.Performace:
                    if (!StoryNoGame.hasAskedForReplay)
                    {
                        // BEGIN PERFORMANCE
                        if (actors[indexPerformingActor].isHuman)
                        {
                            if (!hasStartedPlayingAnimation)
                            {
                                //actorsMonoBehavior[indexPerformingActor].PlayVictory();
                                actorsMonoBehavior[indexPerformingActor].PlayAnimation();
                                hasStartedPlayingAnimation = true;
                            }
                        }
                        else {
                            actors[indexPerformingActor].isForPerformance = true;
                            actors[indexPerformingActor].PerformAction();
                        }

                        // FINISHED PERFORMANCE
                        if (actors[indexPerformingActor].isHuman)
                        {
                            //if (hasStartedPlayingAnimation && !actorsMonoBehavior[indexPerformingActor].IsPlayingWinning())
                            if (hasStartedPlayingAnimation && !actorsMonoBehavior[indexPerformingActor].IsPlayingAnimation())
                            {
                                currentStatePerformance = StatePerformance.Replay;
                                indexPerformancesScript++;
                                hasStartedPlayingAnimation = false;
                            }
                        }
                        else
                        {
                            if (!actors[indexPerformingActor].IsPlayingPerformance())
                            {
                                currentStatePerformance = StatePerformance.Replay;
                                indexPerformancesScript++;
                            }
                        }
                    }
                    else
                    {
                        // BEGIN REPLAY
                        if (actors[indexPerformingActor].isHuman)
                        {
                            if (!hasStartedPlayingAnimation)
                            {
                                //actorsMonoBehavior[indexPerformingActor].PlayVictory();
                                actorsMonoBehavior[indexPerformingActor].PlayAnimation();
                                hasStartedPlayingAnimation = true;
                            }
                        }
                        else
                        {
                            actors[indexPerformingActor].PerformReplay();
                        }

                        // FINISHED REPLAY
                        if (actors[indexPerformingActor].isHuman)
                        {
                            //if (hasStartedPlayingAnimation && !actorsMonoBehavior[indexPerformingActor].IsPlayingWinning())
                            if (hasStartedPlayingAnimation && !actorsMonoBehavior[indexPerformingActor].IsPlayingAnimation())
                            {
                                currentStatePerformance = StatePerformance.Replay;
                                indexPerformancesScript++;
                                hasStartedPlayingAnimation = false;
                            }
                        }
                        else
                        {
                            if (!actors[indexPerformingActor].IsPlayingReplay())
                            {
                                currentStatePerformance = StatePerformance.Replay;
                                indexPerformancesScript++;
                            }
                        }
                    }

                    break;

                case StatePerformance.Replay:
                    // YES
                    if (StoryNoGame.wasYesPressed && !StoryNoGame.wasNoPressed)
                    {
                        actors[indexPerformingActor].SetupForReplay();
                        StoryNoGame.hasAskedForReplay = true;
                        indexPerformancesScript--;
                        currentStatePerformance = StatePerformance.Performace;
                        StoryNoGame.wasYesPressed = false;
                    }

                    // NO
                    else if (!StoryNoGame.wasYesPressed && StoryNoGame.wasNoPressed)
                    {
                        actors[indexPerformingActor].SetupForReplay();
                        StoryNoGame.hasAskedForReplay = false;
                        indexPerformancesScript = 0;
                        indexPerformingActor++;
                        currentStatePerformance = StatePerformance.Presentation;
                        StoryNoGame.wasNoPressed = false;
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

            StoryNoGame.NextState();
        }
    }

    public void ResetStateMachine()
    {
        indexPerformancesScript = 0;
        indexPerformingActor = 0;
        currentStatePerformance = StatePerformance.Presentation;

        actors = EnvironmentStatusNoGame.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
    }
}
