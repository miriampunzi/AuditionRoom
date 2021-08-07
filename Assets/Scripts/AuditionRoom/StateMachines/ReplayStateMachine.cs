using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReplayStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private List<Actor> actors;
    private List<ActorMonoBehavior> actorsMonoBehavior;

    private enum StateReplay
    {
        Question,
        Performance,
        Continue
    }

    private ArrayList replayScript = new ArrayList()
    {
        "Press the button of the performance you want to replay",
        "Performance...",
        "Do you want to replay other performances?"
    };

    private StateReplay currentStateReplay = StateReplay.Question;
    private int indexReplayScript = 0;

    public static bool trapdoorCoverDown = false;
    public static bool hasPerformedReplay = false;
    public static bool hasStartedPlayingAnimation = false;

    public ReplayStateMachine(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
        actors = EnvironmentStatus.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
    }

    public void Execute()
    {
        // Learning in background underground
        for (int i = 0; i < actors.Count; i++)
        {
            if (i != Story.idActorForReplay - 1)
            {
                //actors[i].isForPerformance = false;
                actors[i].rightArmAgent.isForPerformance = false;
                actors[i].leftArmAgent.isForPerformance = false;
                actors[i].headChestAgent.isForPerformance = false;

                //actors[i].LearnInBackground();
                actors[i].rightArmAgent.LearnInBackground();
                actors[i].leftArmAgent.LearnInBackground();
                actors[i].headChestAgent.LearnInBackground();
            }
        }

        // update text script
        if (indexReplayScript < replayScript.Count)
        {
            scriptTextMesh.text = (string)replayScript[indexReplayScript];

            switch (currentStateReplay)
            {
                case StateReplay.Question:
                    if (Story.hasAskedForReplay)
                    {
                        indexReplayScript++;
                        currentStateReplay = StateReplay.Performance;

                        Story.hasAskedForReplay = false;

                        Story.wasYesPressed = false;
                        Story.wasNoPressed = false;
                    }

                    break;

                case StateReplay.Performance:
                    // ON ENTER STATE
                    if (!Story.trapdoorCoverUp)
                    {
                        actors[Story.idActorForReplay - 1].transform.position = new Vector3(
                            actors[Story.idActorForReplay - 1].transform.position.x,
                            actors[Story.idActorForReplay - 1].transform.position.y + 0.1f,
                            actors[Story.idActorForReplay - 1].transform.position.z);
                        actors[Story.idActorForReplay - 1].trapdoorCover.GoUpSlow();
                        Story.trapdoorCoverUp = true;

                        if (actors[Story.idActorForReplay - 1].isHuman)
                        {

                        }
                        else
                        {
                            //actors[Story.idActorForReplay - 1].SetupForReplay();
                            actors[Story.idActorForReplay - 1].rightArmAgent.SetupForReplay();
                            actors[Story.idActorForReplay - 1].leftArmAgent.SetupForReplay();
                            actors[Story.idActorForReplay - 1].headChestAgent.SetupForReplay();
                        }
                    }

                    // REPLAY PERFORMANCE
                    if (actors[Story.idActorForReplay - 1].isHuman)
                    {
                        if (!hasStartedPlayingAnimation && !actors[Story.idActorForReplay - 1].trapdoorCover.IsGoingUpSlow() && !trapdoorCoverDown)
                        {
                            actorsMonoBehavior[Story.idActorForReplay - 1].PlayAnimation();
                            hasStartedPlayingAnimation = true;
                        }
                    }
                    else
                    {
                        if (!actors[Story.idActorForReplay - 1].trapdoorCover.IsGoingUpSlow() && !trapdoorCoverDown)
                        {
                            //actors[Story.idActorForReplay - 1].PerformReplay();
                            actors[Story.idActorForReplay - 1].rightArmAgent.PerformReplay();
                            actors[Story.idActorForReplay - 1].leftArmAgent.PerformReplay();
                            actors[Story.idActorForReplay - 1].headChestAgent.PerformReplay();
                            hasPerformedReplay = true;
                        }
                    }

                    // ON EXIT STATE
                    if (actors[Story.idActorForReplay - 1].isHuman)
                    {
                        //if (hasStartedPlayingAnimation && !actorsMonoBehavior[Story.idActorForReplay - 1].IsPlayingWinning())
                        if (hasStartedPlayingAnimation && !actorsMonoBehavior[Story.idActorForReplay - 1].IsPlayingAnimation())
                        {
                            actors[Story.idActorForReplay - 1].trapdoorCover.GoDownFast();
                            trapdoorCoverDown = true;

                            hasStartedPlayingAnimation = false;

                            Story.wasYesPressed = false;
                            Story.wasNoPressed = false;
                        }

                        if (trapdoorCoverDown && !actors[Story.idActorForReplay - 1].trapdoorCover.IsGoingDownFast())
                        {
                            currentStateReplay = StateReplay.Continue;
                            indexReplayScript++;
                            trapdoorCoverDown = false;

                            Story.CleanDeskVariables();
                        }
                    }
                    else
                    {
                        //if (hasPerformedReplay && !actors[Story.idActorForReplay - 1].IsPlayingReplay())
                        if (hasPerformedReplay && !actors[Story.idActorForReplay - 1].rightArmAgent.IsPlayingReplay())
                        {
                            actors[Story.idActorForReplay - 1].trapdoorCover.GoDownFast();
                            trapdoorCoverDown = true;

                            hasPerformedReplay = false;

                            Story.wasYesPressed = false;
                            Story.wasNoPressed = false;
                        }

                        if (trapdoorCoverDown && !actors[Story.idActorForReplay - 1].trapdoorCover.IsGoingDownFast())
                        {
                            currentStateReplay = StateReplay.Continue;
                            indexReplayScript++;
                            trapdoorCoverDown = false;

                            Story.CleanDeskVariables();
                        }
                    }

                    break;

                case StateReplay.Continue:
                    // YES
                    if (Story.wasYesPressed && !Story.wasNoPressed)
                    {
                        indexReplayScript = 0;
                        currentStateReplay = StateReplay.Question;

                        Story.wasYesPressed = false;
                        Story.wasNoPressed = false;

                        Story.trapdoorCoverUp = false;
                        trapdoorCoverDown = false;
                    }
                    // NO
                    else if (!Story.wasYesPressed && Story.wasNoPressed)
                    {
                        Story.wasNoPressed = false;
                        Story.wasYesPressed = false;

                        Story.trapdoorCoverUp = false;
                        trapdoorCoverDown = false;

                        for (int i = 0; i < actorsMonoBehavior.Count; i++)
                            actorsMonoBehavior[i].PlayIdle();

                        Story.NextState();
                    }

                    break;
            }
        }
    }

    public void ResetStateMachine()
    {
        currentStateReplay = StateReplay.Question;
        indexReplayScript = 0;
        trapdoorCoverDown = false;
        hasPerformedReplay = false;

        actors = EnvironmentStatus.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
    }
}
