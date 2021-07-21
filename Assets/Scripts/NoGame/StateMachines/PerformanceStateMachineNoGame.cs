using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerformanceStateMachineNoGame : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private List<Actor> actors;

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

    public PerformanceStateMachineNoGame(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
        actors = EnvironmentStatusNoGame.getActors();
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
                        actors[indexPerformingActor].PerformAction();

                        // FINISHED PERFORMANCE
                        if (!actors[indexPerformingActor].IsPlayingPerformance())
                        {
                            currentStatePerformance = StatePerformance.Replay;
                            indexPerformancesScript++;
                        }
                    }
                    else
                    {
                        // BEGIN REPLAY
                        actors[indexPerformingActor].PerformReplay();

                        // FINISHED REPLAY
                        if (!actors[indexPerformingActor].IsPlayingReplay())
                        {
                            currentStatePerformance = StatePerformance.Replay;
                            indexPerformancesScript++;
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
    }
}
