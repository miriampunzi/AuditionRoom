using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempClickUI : MonoBehaviour
{
    public void Press(bool isPositive)
    {
        if (isPositive)
            Story.wasYesPressed = true;
        else
            Story.wasNoPressed = true;
    }

    public void Vote(int id)
    {
        switch (Story.currentState)
        {
            // the user clicked a trapdoor button to ask for replay of a particular actor
            case Story.State.Replay:
                Story.idActorForReplay = id;
                Story.hasAskedForReplay = true;

                Story.bestActorVoted = -1;
                Story.hasVoted = false;
                break;

            // the user clicked a trapdoor button to vote a particular actor
            case Story.State.Voting:
                Story.bestActorVoted = id;
                Story.hasVoted = true;

                // after voting, the rewards are given to the actors
                Human.DNNRewardFunction();

                Story.idActorForReplay = -1;
                Story.hasAskedForReplay = false;
                break;
        }
    }
}
