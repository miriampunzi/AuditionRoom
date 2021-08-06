using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VotingStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private List<Actor> actors;
    private List<ActorMonoBehavior> actorsMonoBehavior;

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

    public VotingStateMachine(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
        actors = EnvironmentStatus.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
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
                    for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                    {
                        //actors[i].SetupForReplay();
                        actors[i].rightArmAgent.SetupForReplay();
                        actors[i].leftArmAgent.SetupForReplay();
                        actors[i].headChestAgent.SetupForReplay();

                        actors[i].transform.position = new Vector3(actors[i].transform.position.x, actors[i].transform.position.y + 0.5f, actors[i].transform.position.z);
                        actors[i].trapdoorCover.GoUpSlow();
                    }
                    Story.trapdoorCoverUp = true;
                }

                // TIME EXPIRED
                if (Story.trapdoorCoverUp && !actors[EnvironmentStatus.NUM_ACTORS - 1].trapdoorCover.IsGoingUpSlow())
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
                    for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                    {
                        if (Story.bestActorVoted == actors[i].idActor)
                        {
                            actorsMonoBehavior[i].PlayVictory();
                        }
                        else
                        {
                            actorsMonoBehavior[i].PlayDefeat();
                        }
                    }

                    hasStartedPlayingWin = true;
                }

                // TIME EXPIRED
                if (hasStartedPlayingWin && !actorsMonoBehavior[Story.bestActorVoted - 1].IsPlayingWinning())
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
                    for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                        if (actors[i].idActor != Story.bestActorVoted)
                            actors[i].trapdoorCover.GoDownFast();
                    Story.hasGoneDownFast = true;
                }

                // TIME EXPIRED
                if (Story.hasGoneDownFast && !actors[0].trapdoorCover.IsGoingDownFast())
                {
                    Story.hasGoneDownFast = false;

                    for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                    {
                        //actors[i].EndEpisode();
                        actors[i].rightArmAgent.EndEpisode();
                        actors[i].leftArmAgent.EndEpisode();
                        actors[i].headChestAgent.EndEpisode();
                    }

                    indexVotingScript++;
                    currentStateVoting = StateVoting.Continue;
                }

                break;

            case StateVoting.Continue:
                // YES
                if (Story.wasYesPressed && !Story.wasNoPressed)
                {
                    Story.wasYesPressed = false;

                    for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                    {
                        actors[i].transform.position = new Vector3(actors[i].transform.position.x, actors[i].transform.position.y + 0.5f, actors[i].transform.position.z);
                        actors[i].trapdoorCover.GoDownFast();
                    }

                    Story.NextState();
                }
                // NO
                else if (!Story.wasYesPressed && Story.wasNoPressed)
                {
                    Story.wasNoPressed = false;

                    scriptTextMesh.text = "Bye-bye!";
                    indexVotingScript++;

                    for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
                    {
                        actors[i].transform.position = new Vector3(actors[i].transform.position.x, actors[i].transform.position.y + 0.5f, actors[i].transform.position.z);
                        actors[i].trapdoorCover.GoDownFast();
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

        actors = EnvironmentStatus.getActors();
        actorsMonoBehavior = EnvironmentStatus.getActorsMonoBehavior();
    }
}
