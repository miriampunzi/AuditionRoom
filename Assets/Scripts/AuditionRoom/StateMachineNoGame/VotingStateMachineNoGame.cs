using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VotingStateMachineNoGame : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;

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

    public VotingStateMachineNoGame()
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
        if (indexVotingScript < votingScript.Count)
        {
            scriptTextMesh.text = (string)votingScript[indexVotingScript];
        }

        switch (currentStateVoting)
        {
            case StateVoting.Choosing:
                if (Story.hasVoted)
                {
                    indexVotingScript++;
                    currentStateVoting = StateVoting.Continue;
                    Story.hasVoted = false;
                }

                break;

            case StateVoting.Continue:
                // YES
                if (Story.wasYesPressed && !Story.wasNoPressed)
                {
                    Story.wasYesPressed = false;

                    for (int i = 0; i < EnvironmentStatus.performingActors.Count; i++)
                    {
                        EnvironmentStatus.performingActors[i].rightArmAgent.EndEpisode();
                        EnvironmentStatus.performingActors[i].leftArmAgent.EndEpisode();
                        EnvironmentStatus.performingActors[i].headChestAgent.EndEpisode();
                    }

                    Story.NextState();                    
                }
                // NO
                else if (!Story.wasYesPressed && Story.wasNoPressed)
                {
                    Story.wasNoPressed = false;

                    for (int i = 0; i < EnvironmentStatus.performingActors.Count; i++)
                    {
                        EnvironmentStatus.performingActors[i].rightArmAgent.EndEpisode();
                        EnvironmentStatus.performingActors[i].leftArmAgent.EndEpisode();
                        EnvironmentStatus.performingActors[i].headChestAgent.EndEpisode();
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
    }
}
