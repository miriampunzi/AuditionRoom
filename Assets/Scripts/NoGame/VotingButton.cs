using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VotingButton : MonoBehaviour
{
    public void VoteActor (int num)
    {
        switch (StoryNoGame.currentState)
        {
            // the user clicked a trapdoor button to ask for replay of a particular actor
            case StoryNoGame.State.Replay:
                StoryNoGame.idActorForReplay = num;
                StoryNoGame.hasAskedForReplay = true;

                StoryNoGame.bestActorVoted = -1;
                StoryNoGame.hasVoted = false;

                Debug.Log("Replay for avatar " + num);
                break;

            // the user clicked a trapdoor button to vote a particular actor
            case StoryNoGame.State.Voting:
                StoryNoGame.bestActorVoted = num;
                StoryNoGame.hasVoted = true;

                // after voting, the rewards are given to the actors
                //Human.DNNRewardFunction();

                StoryNoGame.idActorForReplay = -1;
                StoryNoGame.hasAskedForReplay = false;

                Debug.Log("Voted for avatar " + num);
                break;
        }
    }
}
