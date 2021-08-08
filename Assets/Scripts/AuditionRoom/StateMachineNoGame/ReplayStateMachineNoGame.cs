using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReplayStateMachineNoGame : MonoBehaviour
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
        "Which actor do you want to ask for a replay",
        "Performance...",
        "Do you want to see other replays?"
    };

    private StateReplay currentStateReplay = StateReplay.Question;
    private int indexReplayScript = 0;

    public static bool hasPerformedReplay = false;
    public static bool hasStartedPlayingAnimation = false;

    public ReplayStateMachineNoGame()
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
                    }

                    break;

                case StateReplay.Performance:
                    // REPLAY PERFORMANCE
                    if (EnvironmentStatus.performingActors[Story.idActorForReplay - 1].isHuman)
                    {
                        if (!hasStartedPlayingAnimation)
                        {
                            EnvironmentStatus.performingActors[Story.idActorForReplay - 1].PlayAnimation();
                            hasStartedPlayingAnimation = true;
                        }
                    }
                    else
                    {
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].rightArmAgent.PerformReplay();
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].leftArmAgent.PerformReplay();
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].headChestAgent.PerformReplay();
                        hasPerformedReplay = true;
                    }

                    // ON EXIT STATE
                    if (EnvironmentStatus.performingActors[Story.idActorForReplay - 1].isHuman)
                    {
                        if (hasStartedPlayingAnimation && !EnvironmentStatus.performingActors[Story.idActorForReplay - 1].IsPlayingAnimation())
                        {
                            hasStartedPlayingAnimation = false;
                            currentStateReplay = StateReplay.Continue;
                            indexReplayScript++;                            
                        }
                    }
                    else
                    {
                        if (hasPerformedReplay && !EnvironmentStatus.performingActors[Story.idActorForReplay - 1].rightArmAgent.IsPlayingReplay())
                        {
                            hasPerformedReplay = false;
                            currentStateReplay = StateReplay.Continue;
                            indexReplayScript++;
                        }
                    }

                    break;

                case StateReplay.Continue:
                    // YES
                    if (Story.wasYesPressed && !Story.wasNoPressed)
                    {
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].rightArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].leftArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].headChestAgent.SetupForReplay();

                        indexReplayScript = 0;
                        currentStateReplay = StateReplay.Question;

                        Story.CleanDeskVariables();
                    }
                    // NO
                    else if (!Story.wasYesPressed && Story.wasNoPressed)
                    {
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].rightArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].leftArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[Story.idActorForReplay - 1].headChestAgent.SetupForReplay();

                        Story.CleanDeskVariables();

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
        hasPerformedReplay = false;
    }
}
