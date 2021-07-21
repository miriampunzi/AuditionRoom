using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VotingStateMachineNoGame : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private List<Actor> actors;

    private enum StateVoting
    {
        Choosing,
        Continue
    }

    private ArrayList votingScript = new ArrayList()
    {
        "Which was the best actor?",
        "Do you want to do another round?"
    };

    private StateVoting currentStateVoting = StateVoting.Choosing;
    private int indexVotingScript = 0;

    private static bool hasStartedPlayingWin = false;

    public VotingStateMachineNoGame(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
        actors = EnvironmentStatus.getActors();
    }

    public void Execute()
    {
        // update text script
        if (indexVotingScript < votingScript.Count)
        {
            scriptTextMesh.text = (string)votingScript[indexVotingScript];
        }

        switch (currentStateVoting)
        {
            case StateVoting.Choosing:
                if (StoryNoGame.hasVoted)
                {
                    indexVotingScript++;
                    currentStateVoting = StateVoting.Continue;
                    StoryNoGame.hasVoted = false;
                }

                break;

            case StateVoting.Continue:
                // YES
                if (StoryNoGame.wasYesPressed && !StoryNoGame.wasNoPressed)
                {
                    StoryNoGame.wasYesPressed = false;

                    for (int i = 0; i < EnvironmentStatusNoGame.NUM_ACTORS; i++)
                    {
                        actors[i].EndEpisode();
                    }

                    StoryNoGame.NextState();                    
                }
                // NO
                else if (!StoryNoGame.wasYesPressed && StoryNoGame.wasNoPressed)
                {
                    StoryNoGame.wasNoPressed = false;

                    for (int i = 0; i < EnvironmentStatusNoGame.NUM_ACTORS; i++)
                    {
                        actors[i].EndEpisode();
                    }

                    scriptTextMesh.text = "Bye-bye!";
                    indexVotingScript++;
                }

                break;

            default:
                scriptTextMesh.color = Color.red;
                scriptTextMesh.text = "YOU SHOULDN'T ENTER HERE";

                break;
        }
    }

    public void ResetStateMachine()
    {
        currentStateVoting = StateVoting.Choosing;
        indexVotingScript = 0;
        hasStartedPlayingWin = false;

        actors = EnvironmentStatusNoGame.getActors();
    }
}
