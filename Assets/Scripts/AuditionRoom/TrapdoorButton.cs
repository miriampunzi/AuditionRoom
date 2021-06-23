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

    public UnityEvent onPressed;
    public UnityEvent onReleased;

    public int id = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
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
        EnvironmentStatus.bestActorVoted = id;
        EnvironmentStatus.hasVoted = true;

        // TODO to divide in different files
        EnvironmentStatus.idActorForReplay = id;
        EnvironmentStatus.hasAskedForReplay = true;
        //onPressed.Invoke();
    }

    private void Released()
    {
        isPressed = false;
        //onReleased.Invoke();
    }
}
