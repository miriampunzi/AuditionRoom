using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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

    public static int NUM_FEMALE_ANIMATIONS = 9;
    public static int NUM_MALE_ANIMATIONS = 10;
    public static List<AnimatorController> femaleAnimations = new List<AnimatorController>();
    public static List<AnimatorController> maleAnimations = new List<AnimatorController>();
    public static List<string> femaleAnimationNames = new List<string>();
    public static List<string> maleAnimationNames = new List<string>();

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

    public static void RetrieveAnimations()
    {
        if (femaleAnimations.Count == 0 && maleAnimations.Count == 0)
        {
            femaleAnimations.Clear();
            maleAnimations.Clear();

            for (int i = 0; i < NUM_FEMALE_ANIMATIONS; i++)
            {
                femaleAnimations.Add((AnimatorController)Resources.Load("RecordedAnimations/" + "FemaleControllerJoy" + (i + 1)));
                femaleAnimationNames.Add("FemaleJoy" + (i + 1));
            }

            for (int i = 0; i < NUM_MALE_ANIMATIONS; i++)
            { 
                maleAnimations.Add((AnimatorController)Resources.Load("RecordedAnimations/" + "MaleControllerJoy" + (i + 1)));
                maleAnimationNames.Add("MaleJoy" + (i + 1));
            }
        }

        // Shuffle female animator controllers
        for (int i = femaleAnimations.Count - 1; i > 1; i--)
        {
            int rnd = Random.Range(0, i + 1);
            AnimatorController value = femaleAnimations[rnd];
            femaleAnimations[rnd] = femaleAnimations[i];
            femaleAnimations[i] = value;

            string valueString = femaleAnimationNames[rnd];
            femaleAnimationNames[rnd] = femaleAnimationNames[i];
            femaleAnimationNames[i] = valueString;
        }

        // Shuffle male animator controllers
        for (int i = maleAnimations.Count - 1; i > 1; i--)
        {
            int rnd = Random.Range(0, i + 1);
            AnimatorController value = maleAnimations[rnd];
            maleAnimations[rnd] = maleAnimations[i];
            maleAnimations[i] = value;

            string valueString = maleAnimationNames[rnd];
            maleAnimationNames[rnd] = maleAnimationNames[i];
            maleAnimationNames[i] = valueString;
        }
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

        int numHuman = Random.Range(1, 3);

        foreach (Actor actor in allActors)
        {
            if (actor.numActor == numActor1)
            {
                actor.transform.position = new Vector3(-1.2f, 0, 0);
                actor.tag = "Actor";
                actor.idActor = 1;
                if (numHuman == actor.idActor)
                    actor.isHuman = true;
                else
                    actor.isHuman = false;

                actor.rightArmAgent.EndEpisode();
                actor.leftArmAgent.EndEpisode();
                actor.headChestAgent.EndEpisode();
            }
            else if (actor.numActor == numActor2)
            {
                actor.transform.position = new Vector3(1.2f, 0, 0);
                actor.tag = "Actor";
                actor.idActor = 2;
                if (numHuman == actor.idActor)
                    actor.isHuman = true;
                else
                    actor.isHuman = false;

                actor.rightArmAgent.EndEpisode();
                actor.leftArmAgent.EndEpisode();
                actor.headChestAgent.EndEpisode();
            }
            else
            {
                //Debug.Log("Actor " + actor.numActor + ": " + actor.transform.position + " " + actor.initialPosition);
                actor.transform.position = actor.initialPosition;              
                actor.tag = "Untagged";
                actor.idActor = -1;
            }
        }

        RetrieveAnimations();

        foreach (ActorMonoBehavior actor in allActorMonoBehaviors)
        {
            if (actor.numActor == numActor1)
            {
                actor.idActor = 1;
                if (actor.gender == 0)
                    actor.SetAnimation(femaleAnimations[0], femaleAnimationNames[0]);
                else
                    actor.SetAnimation(maleAnimations[0], maleAnimationNames[0]);
            }
            else if (actor.numActor == numActor2)
            {
                actor.idActor = 2;
                if (actor.gender == 0)
                    actor.SetAnimation(femaleAnimations[1], femaleAnimationNames[1]);
                else
                    actor.SetAnimation(maleAnimations[1], maleAnimationNames[1]);
            }
            else
            {
                actor.idActor = -1;
            }
        }
    }
}
