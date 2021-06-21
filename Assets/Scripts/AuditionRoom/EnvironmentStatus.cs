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
    public static bool hasVoted = false;
    public static int bestActorVoted = -1;

    public static List<Actor> getActors()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        List<Actor> actors = new List<Actor>();
        int i = 0;
        foreach (GameObject actor in gameObjects)
        {
            if (actor.CompareTag("Actor"))
            {
                actors.Add(actor.GetComponent<Actor>());
                i++;
            }
        }

        actors.Sort(new ActorComparer());
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
