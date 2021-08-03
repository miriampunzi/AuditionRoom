using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStatusNoGame : MonoBehaviour
{
    public static bool feedbackProvided;
    public static int idBestActor = -1;

    public static int NUM_ACTORS = 2;
    public static int ALL_ACTORS = 6;

    public static List<Actor> allActors;
    public static List<ActorMonoBehavior> allActorMonoBehaviors;

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
                //Debug.Log("Actor " + actors[i].numActor + ": " + actors[i].transform.position + " " + actors[i].initialPosition);

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

    public static void PlaceActors()
    {
        int numActor1 = Random.Range(1, ALL_ACTORS + 1);
        int numActor2;

        do
        {
            numActor2 = Random.Range(1, ALL_ACTORS + 1);
        }
        while (numActor1 == numActor2);

        if (allActors == null)
        {
            allActors = getActors();
            allActorMonoBehaviors = getActorsMonoBehavior();

            //Debug.Log(allActors.Count + " " + allActorMonoBehaviors.Count);

            foreach (Actor actor in allActors)
                actor.initialPosition = actor.transform.position;
        }

        

        foreach (Actor actor in allActors)
        {
            if (actor.numActor == numActor1)
            {
                actor.transform.position = new Vector3(-1.2f, 0, 0);
                actor.tag = "Actor";
                actor.idActor = 1;
            }
            else if (actor.numActor == numActor2)
            {
                actor.transform.position = new Vector3(1.2f, 0, 0);
                actor.tag = "Actor";
                actor.idActor = 2;
            }
            else
            {
                //Debug.Log("Actor " + actor.numActor + ": " + actor.transform.position + " " + actor.initialPosition);
                actor.transform.position = actor.initialPosition;              
                actor.tag = "Untagged";
                actor.idActor = -1;
            }
        }
    }
}
