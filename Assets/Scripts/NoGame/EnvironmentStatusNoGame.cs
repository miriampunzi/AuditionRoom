using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStatusNoGame : MonoBehaviour
{
    public static bool feedbackProvided;
    public static int idBestActor = -1;

    public static int NUM_ACTORS = 2;

    public static bool wasYesPressed = false;
    public static bool wasNoPressed = false;

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
}
