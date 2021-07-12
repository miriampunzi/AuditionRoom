using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerformanceStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private List<Actor> actors;

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
    private int indexPerformingActor = 5;

    public PerformanceStateMachine(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
        actors = EnvironmentStatus.getActors();
    }

    public void Execute()
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
                    if (!Story.trapdoorCoverUp)
                    {
                        actors[indexPerformingActor].transform.position = new Vector3(actors[indexPerformingActor].transform.position.x, actors[indexPerformingActor].transform.position.y + 0.1f, actors[indexPerformingActor].transform.position.z);
                        actors[indexPerformingActor].trapdoorCover.GoUpSlow();
                        Story.trapdoorCoverUp = true;
                    }

                    // TODO CONTROLLA CHE I TRAPDOORCOVER SIANO SU PRIMA DI CONTROLLARE IL SI PER EVITARE CHE SUCCEDANO COSE QUANDO L'UTENTE CLICCA SI A CASO

                    // YES
                    if (Story.trapdoorCoverUp && Story.wasYesPressed)
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
                    if (Story.wasYesPressed && !Story.wasNoPressed)
                    {
                        actors[indexPerformingActor].SetupForReplay();
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
                        actors[indexPerformingActor].trapdoorCover.GoDownFast();
                        Story.hasGoneDownFast = true;
                    }

                    // TIME EXPIRED
                    if (Story.hasGoneDownFast && !actors[indexPerformingActor].trapdoorCover.IsGoingDownFast())
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
