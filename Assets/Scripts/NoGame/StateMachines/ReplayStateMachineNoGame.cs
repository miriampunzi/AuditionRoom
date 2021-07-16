using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReplayStateMachineNoGame : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private List<Actor> actors;

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

    public ReplayStateMachineNoGame(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
        actors = EnvironmentStatus.getActors();
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
                    actors[StoryNoGame.idActorForReplay - 1].PerformReplay();
                    hasPerformedReplay = true;

                    // ON EXIT STATE
                    if (hasPerformedReplay && !actors[StoryNoGame.idActorForReplay - 1].IsPlayingReplay())
                    {
                        hasPerformedReplay = false; currentStateReplay = StateReplay.Continue;
                        indexReplayScript++;
                    }

                    break;

                case StateReplay.Continue:
                    // YES
                    if (StoryNoGame.wasYesPressed && !StoryNoGame.wasNoPressed)
                    {
                        actors[StoryNoGame.idActorForReplay - 1].SetupForReplay();
                        indexReplayScript = 0;
                        currentStateReplay = StateReplay.Question;

                        StoryNoGame.wasYesPressed = false;
                        StoryNoGame.wasNoPressed = false;
                    }
                    // NO
                    else if (!StoryNoGame.wasYesPressed && StoryNoGame.wasNoPressed)
                    {
                        actors[StoryNoGame.idActorForReplay - 1].SetupForReplay();
                        StoryNoGame.wasNoPressed = false;
                        StoryNoGame.wasYesPressed = false;

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
    }
}
