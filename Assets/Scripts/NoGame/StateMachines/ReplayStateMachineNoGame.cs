using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReplayStateMachineNoGame : MonoBehaviour
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
        "Which actor do you want to ask for a replay",
        "Performance...",
        "Do you want to see other replays?"
    };

    private StateReplay currentStateReplay = StateReplay.Question;
    private int indexReplayScript = 0;

    public static bool hasPerformedReplay = false;
    public static bool hasStartedPlayingAnimation = false;

    public ReplayStateMachineNoGame(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
        actors = EnvironmentStatus.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
    }

    public void Execute()
    {
        // update text script
        if (indexReplayScript < replayScript.Count)
        {
            scriptTextMesh.text = (string)replayScript[indexReplayScript];

            switch (currentStateReplay)
            {
                case StateReplay.Question:
                    if (StoryNoGame.hasAskedForReplay)
                    {
                        indexReplayScript++;
                        currentStateReplay = StateReplay.Performance;
                        StoryNoGame.hasAskedForReplay = false;
                    }

                    break;

                case StateReplay.Performance:
                    // REPLAY PERFORMANCE
                    if (actors[StoryNoGame.idActorForReplay - 1].isHuman)
                    {
                        if (!hasStartedPlayingAnimation)
                        {
                            actorsMonoBehavior[StoryNoGame.idActorForReplay - 1].PlayAnimation();
                            hasStartedPlayingAnimation = true;
                        }
                    }
                    else
                    {
                        //actors[StoryNoGame.idActorForReplay - 1].PerformReplay();
                        actors[StoryNoGame.idActorForReplay - 1].rightArmAgent.PerformReplay();
                        actors[StoryNoGame.idActorForReplay - 1].leftArmAgent.PerformReplay();
                        actors[StoryNoGame.idActorForReplay - 1].headChestAgent.PerformReplay();
                        hasPerformedReplay = true;
                    }

                    // ON EXIT STATE
                    if (actors[StoryNoGame.idActorForReplay - 1].isHuman)
                    {
                        if (hasStartedPlayingAnimation && !actorsMonoBehavior[StoryNoGame.idActorForReplay - 1].IsPlayingAnimation())
                        {
                            hasStartedPlayingAnimation = false;
                            currentStateReplay = StateReplay.Continue;
                            indexReplayScript++;                            
                        }
                    }
                    else
                    {
                        if (hasPerformedReplay && !actors[StoryNoGame.idActorForReplay - 1].rightArmAgent.IsPlayingReplay())
                        {
                            hasPerformedReplay = false;
                            currentStateReplay = StateReplay.Continue;
                            indexReplayScript++;
                        }
                    }

                    break;

                case StateReplay.Continue:
                    // YES
                    if (StoryNoGame.wasYesPressed && !StoryNoGame.wasNoPressed)
                    {
                        //actors[StoryNoGame.idActorForReplay - 1].SetupForReplay();
                        actors[StoryNoGame.idActorForReplay - 1].rightArmAgent.SetupForReplay();
                        actors[StoryNoGame.idActorForReplay - 1].leftArmAgent.SetupForReplay();
                        actors[StoryNoGame.idActorForReplay - 1].headChestAgent.SetupForReplay();

                        indexReplayScript = 0;
                        currentStateReplay = StateReplay.Question;

                        StoryNoGame.CleanDeskVariables();
                    }
                    // NO
                    else if (!StoryNoGame.wasYesPressed && StoryNoGame.wasNoPressed)
                    {
                        //actors[StoryNoGame.idActorForReplay - 1].SetupForReplay();
                        actors[StoryNoGame.idActorForReplay - 1].rightArmAgent.SetupForReplay();
                        actors[StoryNoGame.idActorForReplay - 1].leftArmAgent.SetupForReplay();
                        actors[StoryNoGame.idActorForReplay - 1].headChestAgent.SetupForReplay();

                        StoryNoGame.CleanDeskVariables();

                        StoryNoGame.NextState();
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

        actors = EnvironmentStatusNoGame.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
    }
}
