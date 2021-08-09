using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReplayStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;

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

    public ReplayStateMachine()
    {
        scriptTextMesh = GameObject.FindGameObjectWithTag("Script").GetComponent<TextMeshPro>();
    }

    public void Execute()
    {
        // Learning in background underground
        for (int i = 0; i < EnvironmentStatus.performingActors.Count; i++)
        {
            if (i != Story.idActorForReplay - 1)
            {
                EnvironmentStatus.performingActors[i].rightArmAgent.isForPerformance = false;
                EnvironmentStatus.performingActors[i].leftArmAgent.isForPerformance = false;
                EnvironmentStatus.performingActors[i].headChestAgent.isForPerformance = false;

                EnvironmentStatus.performingActors[i].rightArmAgent.LearnInBackground();
                EnvironmentStatus.performingActors[i].leftArmAgent.LearnInBackground();
                EnvironmentStatus.performingActors[i].headChestAgent.LearnInBackground();
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
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].leftArmAgent.EndEpisode();
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].rightArmAgent.EndEpisode();
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].headChestAgent.EndEpisode();

                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].transform.position = new Vector3(
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].transform.position.x,
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].transform.position.y + 0.1f,
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].transform.position.z);
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].trapdoorCover.GoUpSlow();
                        Story.trapdoorCoverUp = true;

                        if (!EnvironmentStatus.performingActors[Story.idActorForReplay - 1].isHuman)
                        {
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].rightArmAgent.SetupForReplay();
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].leftArmAgent.SetupForReplay();
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].headChestAgent.SetupForReplay();
                        }
                    }

                    // REPLAY PERFORMANCE
                    if (EnvironmentStatus.performingActors[Story.idActorForReplay - 1].isHuman)
                    {
                        if (!hasStartedPlayingAnimation && !EnvironmentStatus.performingActors[Story.idActorForReplay - 1].trapdoorCover.IsGoingUpSlow() && !trapdoorCoverDown)
                        {
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].PlayAnimation();
                            hasStartedPlayingAnimation = true;
                        }
                    }
                    else
                    {
                        if (!EnvironmentStatus.performingActors[Story.idActorForReplay - 1].trapdoorCover.IsGoingUpSlow() && !trapdoorCoverDown)
                        {
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].rightArmAgent.PerformReplay();
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].leftArmAgent.PerformReplay();
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].headChestAgent.PerformReplay();
                            hasPerformedReplay = true;
                        }
                    }

                    // ON EXIT STATE
                    if (EnvironmentStatus.performingActors[Story.idActorForReplay - 1].isHuman)
                    {
                        if (hasStartedPlayingAnimation && !EnvironmentStatus.performingActors[Story.idActorForReplay - 1].IsPlayingAnimation())
                        {
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].trapdoorCover.GoDownFast();
                            trapdoorCoverDown = true;

                            hasStartedPlayingAnimation = false;

                            Story.wasYesPressed = false;
                            Story.wasNoPressed = false;
                        }

                        if (trapdoorCoverDown && !EnvironmentStatus.performingActors[Story.idActorForReplay - 1].trapdoorCover.IsGoingDownFast())
                        {
                            currentStateReplay = StateReplay.Continue;
                            indexReplayScript++;
                            trapdoorCoverDown = false;

                            Story.CleanDeskVariables();
                        }
                    }
                    else
                    {
                        if (hasPerformedReplay && !EnvironmentStatus.performingActors[Story.idActorForReplay - 1].rightArmAgent.IsPlayingReplay())
                        {
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].trapdoorCover.GoDownFast();
                            trapdoorCoverDown = true;

                            hasPerformedReplay = false;

                            Story.wasYesPressed = false;
                            Story.wasNoPressed = false;
                        }

                        if (trapdoorCoverDown && !EnvironmentStatus.performingActors[Story.idActorForReplay - 1].trapdoorCover.IsGoingDownFast())
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

                        for (int i = 0; i < EnvironmentStatus.performingActors.Count; i++)
                            EnvironmentStatus.performingActors[i].PlayIdle();

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
    }
}
