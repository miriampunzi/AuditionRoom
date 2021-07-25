using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReplayStateMachine : MonoBehaviour
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

    public static bool trapdoorCoverDown = false;
    public static bool hasPerformedReplay = false;

    public ReplayStateMachine(TextMeshPro scriptTextMesh)
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
                    if (Story.hasAskedForReplay)
                    {
                        indexReplayScript++;
                        currentStateReplay = StateReplay.Performance;
                        Story.hasAskedForReplay = false;
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
                        actors[Story.idActorForReplay - 1].SetupForReplay();
                    }

                    // REPLAY PERFORMANCE
                    if (!actors[Story.idActorForReplay - 1].trapdoorCover.IsGoingUpSlow() && !trapdoorCoverDown)
                    {
                        actors[Story.idActorForReplay - 1].PerformReplay();
                        hasPerformedReplay = true;
                    }

                    // ON EXIT STATE
                    if (hasPerformedReplay && !actors[Story.idActorForReplay - 1].IsPlayingReplay())
                    {
                        actors[Story.idActorForReplay - 1].trapdoorCover.GoDownFast();
                        trapdoorCoverDown = true;
                        hasPerformedReplay = false;
                    }

                    if (trapdoorCoverDown && !actors[Story.idActorForReplay - 1].trapdoorCover.IsGoingDownFast())
                    {
                        currentStateReplay = StateReplay.Continue;
                        indexReplayScript++;
                        trapdoorCoverDown = false;
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
        //actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
    }
}
