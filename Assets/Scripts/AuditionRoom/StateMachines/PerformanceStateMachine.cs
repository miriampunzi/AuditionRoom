using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerformanceStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private List<Actor> actors;
    private List<ActorMonoBehavior> actorsMonoBehavior;

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

    public PerformanceStateMachine(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
        actors = EnvironmentStatus.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
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

                    // YES
                    if (!actors[indexPerformingActor].trapdoorCover.IsGoingUpSlow() && Story.trapdoorCoverUp && Story.wasYesPressed)
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
                            //actors[indexPerformingActor].isForPerformance = true;
                            actors[indexPerformingActor].rightArmAgent.isForPerformance = true;
                            actors[indexPerformingActor].leftArmAgent.isForPerformance = true;
                            actors[indexPerformingActor].headChestAgent.isForPerformance = true;
                            //actors[indexPerformingActor].PerformAction();
                            actors[indexPerformingActor].rightArmAgent.PerformAction();
                            actors[indexPerformingActor].leftArmAgent.PerformAction();
                            actors[indexPerformingActor].headChestAgent.PerformAction();
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
                            //if (!actors[indexPerformingActor].IsPlayingPerformance())
                            if (!actors[indexPerformingActor].rightArmAgent.IsPlayingPerformance())
                            {
                                //actors[indexPerformingActor].isForPerformance = false;
                                actors[indexPerformingActor].rightArmAgent.isForPerformance = false;
                                actors[indexPerformingActor].leftArmAgent.isForPerformance = false;
                                actors[indexPerformingActor].headChestAgent.isForPerformance = false;

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
                            //actors[indexPerformingActor].PerformReplay();
                            actors[indexPerformingActor].rightArmAgent.PerformReplay();
                            actors[indexPerformingActor].leftArmAgent.PerformReplay();
                            actors[indexPerformingActor].headChestAgent.PerformReplay();
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
                            //if (!actors[indexPerformingActor].IsPlayingReplay())
                            if (!actors[indexPerformingActor].rightArmAgent.IsPlayingReplay())
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
                        //actors[indexPerformingActor].SetupForReplay();
                        actors[indexPerformingActor].rightArmAgent.SetupForReplay();
                        actors[indexPerformingActor].leftArmAgent.SetupForReplay();
                        actors[indexPerformingActor].headChestAgent.SetupForReplay();

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

            for (int i = 0; i < actorsMonoBehavior.Count; i++)
                actorsMonoBehavior[i].PlayIdle();

            Story.NextState();
        }
    }

    public void ResetStateMachine()
    {
        indexPerformancesScript = 0;
        indexPerformingActor = 0;
        currentStatePerformance = StatePerformance.Presentation;

        actors = EnvironmentStatus.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
    }
}
