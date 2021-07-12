using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStatus : MonoBehaviour
{
    // movements performed by the avatar to copy 
    public static ArrayList rotationsRightArm = new ArrayList();
    public static ArrayList rotationsRightForeArm = new ArrayList();
    public static ArrayList rotationsRightHand = new ArrayList();

    public static ArrayList rotationsLeftArm = new ArrayList();
    public static ArrayList rotationsLeftForeArm = new ArrayList();
    public static ArrayList rotationsLeftHand = new ArrayList();

    // number of actions performed by the avatar to copy
    public static int numActions = 0;

    // has the player already provided the feedback?
    public static bool feedbackProvided = false;

    // id of the best actor voted. -1 if nobody was voted
    public static int idBestActor = -1;

    // variable to count the number of episodes. It's incremented after every EndEpisode() call
    public static int numEpisode = 1;

    // number of actors in the environment
    public const int NUM_ACTORS = 5;

    // retrieve all actors from environment
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

        // sort actors by their id
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
