using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public Transform topOfLever;

    [SerializeField] private float forwardBackwardTilt = 0;

    private bool trapdoorsOpen = false;

    void Update()
    {
        forwardBackwardTilt = topOfLever.rotation.eulerAngles.x;
        if (forwardBackwardTilt < 355 && forwardBackwardTilt > 290)
        {
            forwardBackwardTilt = Math.Abs(forwardBackwardTilt - 360);

            if (!trapdoorsOpen)
            {
                GameObject trapdoorsCovers = GameObject.Find("TrapdoorsCovers");
                trapdoorsCovers.transform.position = new Vector3(trapdoorsCovers.transform.position.x, trapdoorsCovers.transform.position.y - 3, trapdoorsCovers.transform.position.z);
                trapdoorsOpen = true;
            }

            Debug.Log("Backward");
        }
        else if (forwardBackwardTilt > 5 && forwardBackwardTilt < 74)
        {
            Debug.Log("Forward");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            transform.LookAt(other.transform.position, transform.up);
        }
    }
}
