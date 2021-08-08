using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VotingStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;

    private enum StateVoting
    {
        ActorsAppear,
        Choosing,
        Confirm,
        Win,
        Lose,
        Continue
    }

    private ArrayList votingScript = new ArrayList()
    {
        "Now it’s time to vote",
        "Which was the best actor?",
        "Are you sure you want to vote this actor?",
        "Congratulations actor num $!",
        "Bye bye the others!",
        "Do you want to do another round?"
    };

    private StateVoting currentStateVoting = StateVoting.ActorsAppear;
    private int indexVotingScript = 0;

    private static bool hasStartedPlayingWin = false;

    public VotingStateMachine()
    {
        scriptTextMesh = GameObject.FindGameObjectWithTag("Script").GetComponent<TextMeshPro>();
    }

    public void Execute()
    {
        // update text script
        if (indexVotingScript < votingScript.Count)
        {
            if (currentStateVoting == StateVoting.Win)
            {
                string modifiedText = ((string)votingScript[indexVotingScript]).Replace("$", "" + (Story.bestActorVoted));
                scriptTextMesh.text = modifiedText;
            }
            else
                scriptTextMesh.text = (string)votingScript[indexVotingScript];
        }

        switch (currentStateVoting)
        {
            case StateVoting.ActorsAppear:
                // BEGIN
                if (!Story.trapdoorCoverUp)
                {
                    for (int i = 0; i < EnvironmentStatus.NUM_PERFORMING_ACTORS_GAME; i++)
                    {
                        EnvironmentStatus.performingActors[i].rightArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[i].leftArmAgent.SetupForReplay();
                        EnvironmentStatus.performingActors[i].headChestAgent.SetupForReplay();

                        EnvironmentStatus.performingActors[i].transform.position = new Vector3(
                            EnvironmentStatus.performingActors[i].transform.position.x,
                            EnvironmentStatus.performingActors[i].transform.position.y + 0.5f,
                            EnvironmentStatus.performingActors[i].transform.position.z);
                        EnvironmentStatus.performingActors[i].trapdoorCover.GoUpSlow();
                    }
                    Story.trapdoorCoverUp = true;
                }

                // TIME EXPIRED
                if (Story.trapdoorCoverUp && !EnvironmentStatus.performingActors[EnvironmentStatus.NUM_PERFORMING_ACTORS_GAME - 1].trapdoorCover.IsGoingUpSlow())
                {
                    Story.trapdoorCoverUp = true;
                    indexVotingScript++;
                    currentStateVoting = StateVoting.Choosing;
                }

                break;

            case StateVoting.Choosing:
                if (Story.hasVoted)
                {
                    indexVotingScript++;
                    currentStateVoting = StateVoting.Confirm;
                    Story.hasVoted = false;
                }

                break;

            case StateVoting.Confirm:
                // NO
                if (!Story.wasYesPressed && Story.wasNoPressed)
                {
                    indexVotingScript--;
                    currentStateVoting = StateVoting.Choosing;
                    Story.wasNoPressed = false;
                }

                // YES
                else if (Story.wasYesPressed && !Story.wasNoPressed)
                {
                    indexVotingScript++;
                    currentStateVoting = StateVoting.Win;
                    Story.wasYesPressed = false;
                }

                break;

            case StateVoting.Win:
                // BEGIN
                if (!hasStartedPlayingWin)
                {
                    for (int i = 0; i < EnvironmentStatus.NUM_PERFORMING_ACTORS_GAME; i++)
                    {
                        if (Story.bestActorVoted == EnvironmentStatus.performingActors[i].idPerformance)
                            EnvironmentStatus.performingActors[i].PlayVictory();
                    }

                    hasStartedPlayingWin = true;
                }

                // TIME EXPIRED
                if (hasStartedPlayingWin && !EnvironmentStatus.performingActors[Story.bestActorVoted - 1].IsPlayingWinning())
                {
                    indexVotingScript++;
                    currentStateVoting = StateVoting.Lose;
                    hasStartedPlayingWin = false;
                }

                break;

            case StateVoting.Lose:
                // BEGIN
                if (!Story.hasGoneDownFast)
                {
                    for (int i = 0; i < EnvironmentStatus.NUM_PERFORMING_ACTORS_GAME; i++)
                        if (EnvironmentStatus.performingActors[i].idPerformance != Story.bestActorVoted)
                            EnvironmentStatus.performingActors[i].trapdoorCover.GoDownFast();
                    Story.hasGoneDownFast = true;
                }

                // TIME EXPIRED
                if (Story.hasGoneDownFast && !EnvironmentStatus.performingActors[0].trapdoorCover.IsGoingDownFast())
                {
                    Story.hasGoneDownFast = false;

                    for (int i = 0; i < EnvironmentStatus.NUM_PERFORMING_ACTORS_GAME; i++)
                    {
                        //actors[i].EndEpisode();
                        EnvironmentStatus.performingActors[i].rightArmAgent.EndEpisode();
                        EnvironmentStatus.performingActors[i].leftArmAgent.EndEpisode();
                        EnvironmentStatus.performingActors[i].headChestAgent.EndEpisode();
                    }

                    indexVotingScript++;
                    currentStateVoting = StateVoting.Continue;
                }

                break;

            case StateVoting.Continue:
                // YES
                if (Story.wasYesPressed && !Story.wasNoPressed)
                {
                    for (int i = 0; i < EnvironmentStatus.performingActors.Count; i++)
                        EnvironmentStatus.performingActors[i].PlayIdle();

                    Story.wasYesPressed = false;

                    for (int i = 0; i < EnvironmentStatus.NUM_PERFORMING_ACTORS_GAME; i++)
                    {
                        EnvironmentStatus.performingActors[i].transform.position = new Vector3(
                            EnvironmentStatus.performingActors[i].transform.position.x,
                            EnvironmentStatus.performingActors[i].transform.position.y + 0.5f,
                            EnvironmentStatus.performingActors[i].transform.position.z);
                        EnvironmentStatus.performingActors[i].trapdoorCover.GoDownFast();
                    }

                    Story.NextState();
                }
                // NO
                else if (!Story.wasYesPressed && Story.wasNoPressed)
                {
                    Story.wasNoPressed = false;

                    scriptTextMesh.text = "Thanks to the actors you chose, the algorithm predicts your movie will be a success!!";
                    indexVotingScript++;

                    for (int i = 0; i < EnvironmentStatus.NUM_PERFORMING_ACTORS_GAME; i++)
                    {
                        EnvironmentStatus.performingActors[i].transform.position = new Vector3(
                            EnvironmentStatus.performingActors[i].transform.position.x,
                            EnvironmentStatus.performingActors[i].transform.position.y + 0.5f,
                            EnvironmentStatus.performingActors[i].transform.position.z);
                        EnvironmentStatus.performingActors[i].trapdoorCover.GoDownFast();
                    }
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
        currentStateVoting = StateVoting.ActorsAppear;
        indexVotingScript = 0;
        hasStartedPlayingWin = false;

        //actors = EnvironmentStatus.GetActors();
    }
}
