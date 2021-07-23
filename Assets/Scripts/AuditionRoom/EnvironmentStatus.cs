using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStatus : MonoBehaviour
{
    public static List<Actor> allActors;
    public static List<ActorMonoBehavior> allActorMonoBehaviors;
    public static int ALL_ACTORS = 6;
    public static List<TrapdoorCover> trapdoorCovers;

    static List<bool> avatarTypes = new List<bool>()
    {
        true,       // human
        true,       // human
        false,      // virtual
        false,      // virtual
        false,      // virtual
    };

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

    private static void ShuffleAvatarTypes()
    {
        int n = avatarTypes.Count;

        for (int i = avatarTypes.Count - 1; i > 1; i--)
        {
            int rnd = Random.Range(0, i + 1);

            bool value = avatarTypes[rnd];
            avatarTypes[rnd] = avatarTypes[i];
            avatarTypes[i] = value;
        }
    }

    public static void PlaceActors()
    {
        // select actors
        List<int> numbersAllActors = new List<int>();
        for (int i = 0; i < ALL_ACTORS; i++)
            numbersAllActors.Add(i + 1);

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
            allActors = getActors();
            allActorMonoBehaviors = getActorsMonoBehavior();
        }

        // decide who is human and who is virtual
        ShuffleAvatarTypes();

        // place actors
        foreach (Actor actor in allActors)
        {
            // TODO CREA VETTORE 
            if (actor.numActor == selectedActors[0])
            {
                actor.transform.position = new Vector3(-2.4f, -2.5f, 0.41f);
                actor.tag = "Actor";
                actor.idActor = 1;
                actor.trapdoorCover = trapdoorCovers[0];
                actor.isHuman = avatarTypes[0];
            }
            else if (actor.numActor == selectedActors[1])
            {
                actor.transform.position = new Vector3(-1.43f, -2.5f, 2.53f);
                actor.tag = "Actor";
                actor.idActor = 2;
                actor.trapdoorCover = trapdoorCovers[1];
                actor.isHuman = avatarTypes[1];
            }
            else if (actor.numActor == selectedActors[2])
            {
                actor.transform.position = new Vector3(0.12f, -2.5f, 0.38f);
                actor.tag = "Actor";
                actor.idActor = 3;
                actor.trapdoorCover = trapdoorCovers[2];
                actor.isHuman = avatarTypes[2];
            }
            else if (actor.numActor == selectedActors[3])
            {
                actor.transform.position = new Vector3(1.59f, -2.5f, 2.44f);
                actor.tag = "Actor";
                actor.idActor = 4;
                actor.trapdoorCover = trapdoorCovers[3];
                actor.isHuman = avatarTypes[3];
            }
            else if (actor.numActor == selectedActors[4])
            {
                actor.transform.position = new Vector3(2.62f, -2.5f, 0.43f);
                actor.tag = "Actor";
                actor.idActor = 5;
                actor.trapdoorCover = trapdoorCovers[4];
                actor.isHuman = avatarTypes[4];
            }
            else
            {
                actor.transform.position = new Vector3(-15, -3, 4);
                actor.tag = "Untagged";
                actor.idActor = -1;
                actor.trapdoorCover = null;
            }
        }

        //TODO MODIFICA
        foreach (ActorMonoBehavior actor in allActorMonoBehaviors)
        {
            if (actor.numActor == selectedActors[0])
            {
                actor.idActor = 1;
            }
            else if (actor.numActor == selectedActors[1])
            {
                actor.idActor = 2;
            }
            else if (actor.numActor == selectedActors[2])
            {
                actor.idActor = 3;
            }
            else if (actor.numActor == selectedActors[3])
            {
                actor.idActor = 4;
            }
            else if (actor.numActor == selectedActors[4])
            {
                actor.idActor = 5;
            }
            else
            {
                actor.idActor = -1;
            }
        }
    }
}
