using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStatus : MonoBehaviour
{
    public static bool feedbackProvided = false;
    public static int numEpisode = 1;

    public const int NUM_ACTORS = 5;
    public const float ARM_LENGTH = 0.62f;

    public static bool wasYesPressed = false;
    public static bool wasNoPressed = false;
    public static bool isVotingTime = true;

    public static Actor[] getActors()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        Actor[] actors = new Actor[NUM_ACTORS];
        int i = 0;
        foreach (GameObject actor in gameObjects)
        {
            if (actor.CompareTag("Actor"))
            {   // layer Actor2 
                actors[i] = actor.GetComponent<Actor>();
                i++;
            }
        }

        return actors;
    }

    public static void TrapdoorsUpSlow()
    {
        GameObject trapdoorsCovers = GameObject.Find("TrapdoorsCovers");
        trapdoorsCovers.transform.position = new Vector3(trapdoorsCovers.transform.position.x, trapdoorsCovers.transform.position.y - 3, trapdoorsCovers.transform.position.z);

    }

    public static void TrapdoorsDownFast()
    {

    }
}
