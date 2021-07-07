using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStatus : MonoBehaviour
{
    public static bool feedbackProvided = false;
    public static int numEpisode = 1;

    public const int NUM_ACTORS = 5;
    public const float ARM_LENGTH = 0.62f;

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

    public static List<ActorMonoBehavior> getActorsMonoBehavior()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        List<ActorMonoBehavior> actors = new List<ActorMonoBehavior>();
        int i = 0;
        foreach (GameObject actor in gameObjects)
        {
            if (actor.CompareTag("Actor"))
            {
                actors.Add(actor.GetComponent<ActorMonoBehavior>());
                i++;
            }
        }

        actors.Sort(new ActorMonoBehaviorComparer());
        return actors;
    }
}
