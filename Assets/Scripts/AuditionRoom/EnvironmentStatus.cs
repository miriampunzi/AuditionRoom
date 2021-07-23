using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStatus : MonoBehaviour
{
    public static List<Actor> allActors;
    public static List<ActorMonoBehavior> allActorMonoBehaviors;
    public static int ALL_ACTORS = 6;
    public static List<TrapdoorCover> trapdoorCovers;

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

    // retrieve all actors monobehavior from environment
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

    // retrieve all trapdoor covers from environment
    public static List<TrapdoorCover> getTrapdoorCovers()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        List<TrapdoorCover> trapdoorCovers = new List<TrapdoorCover>();
        int i = 0;
        foreach (GameObject trapdoorCover in gameObjects)
        {
            if (trapdoorCover.CompareTag("TrapdoorCover"))
            {
                trapdoorCovers.Add(trapdoorCover.GetComponent<TrapdoorCover>());
                i++;
            }
        }

        // sort actors by their id
        trapdoorCovers.Sort(new TrapdoorComparer());
        return trapdoorCovers;
    }

    public static void PlaceActors()
    {
        List<int> numbersAllActors = new List<int>();
        for (int i = 0; i < ALL_ACTORS; i++)
            numbersAllActors.Add(i);

        List<int> selectedActors = new List<int>();
        for (int i = 0; i < NUM_ACTORS; i++)
        {
            int randomNum = Random.Range(0, numbersAllActors.Count);
            selectedActors.Add(numbersAllActors[randomNum]);
            numbersAllActors.RemoveAt(randomNum);
        }

        if (trapdoorCovers == null)
        {
            trapdoorCovers = getTrapdoorCovers();

            Debug.Log(trapdoorCovers.Count);
            allActors = getActors();
            allActorMonoBehaviors = getActorsMonoBehavior();
        }

        foreach (Actor actor in allActors)
        {
            for (int i = 0; i < selectedActors.Count; i++)
            {
                if (actor.numActor == selectedActors[i])
                {
                    switch (i)
                    {
                        case 0:
                            actor.transform.position = new Vector3(-2.4f, -2.5f, 0.41f);
                            actor.tag = "Actor";
                            actor.idActor = 1;
                            actor.trapdoorCover = trapdoorCovers[i];
                            break;

                        case 1:
                            actor.transform.position = new Vector3(-1.43f, -2.5f, 2.53f);
                            actor.tag = "Actor";
                            actor.idActor = 2;
                            actor.trapdoorCover = trapdoorCovers[i];
                            break;

                        case 2:
                            actor.transform.position = new Vector3(0.12f, -2.5f, 0.38f);
                            actor.tag = "Actor";
                            actor.idActor = 3;
                            actor.trapdoorCover = trapdoorCovers[i];
                            break;

                        case 3:
                            actor.transform.position = new Vector3(1.59f, -2.5f, 2.44f);
                            actor.tag = "Actor";
                            actor.idActor = 4;
                            actor.trapdoorCover = trapdoorCovers[i];
                            break;

                        case 4:
                            actor.transform.position = new Vector3(2.62f, -2.5f, 0.43f);
                            actor.tag = "Actor";
                            actor.idActor = 5;
                            actor.trapdoorCover = trapdoorCovers[i];
                            break;

                        default:
                            actor.transform.position = new Vector3(-15, -3, 4);
                            actor.tag = "Untagged";
                            actor.idActor = -1;
                            actor.trapdoorCover = null;
                            break;
                    }

                    break;
                }
            }
            
        }
    }
}
