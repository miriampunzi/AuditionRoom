using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapdoorButton : MonoBehaviour
{
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float deadZone = 0.025f;

    private bool isPressed;
    private Vector3 startPos;
    private ConfigurableJoint joint;

    public int id = 0;

    void Start()
    {
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        // the player has just pressed the button
        if (!isPressed && GetValue() + threshold >= 1)
        {
            Pressed();
        }
        // the player has just released the button
        if (isPressed && GetValue() - threshold <= 0)
            Released();
    }

    private float GetValue()
    {
        // get percentage
        var value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;

        if (Math.Abs(value) < deadZone)
            value = 0;

        return Mathf.Clamp(value, -1, 1);
    }

    private void Pressed()
    {
        isPressed = true;

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

    private void Released()
    {
        isPressed = false;
    }
}
