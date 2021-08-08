using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;

public class EnvironmentStatus : MonoBehaviour
{
    public static bool isGame = false;
    public static bool isFirstRound = true;

    // environment objects
    public static List<Actor> allActors;
    public static int NUM_ALL_ACTORS = 9;
    public static List<TrapdoorCover> trapdoorCovers;

    // who is virtual and who is human
    static List<bool> avatarTypesInGame = new List<bool>()
    {
        true,       // human
        true,       // human
        false,      // virtual
        false,      // virtual
        false,      // virtual
    };

    static List<bool> avatarTypesInNoGame = new List<bool>()
    {
        true,       // human
        false,      // virtual
    };

    public static int NUM_FEMALE_ANIMATIONS = 9;
    public static int NUM_MALE_ANIMATIONS = 10;
    public static List<AnimatorController> femaleAnimations = new List<AnimatorController>();
    public static List<AnimatorController> maleAnimations = new List<AnimatorController>();
    public static List<string> femaleAnimationNames = new List<string>();
    public static List<string> maleAnimationNames = new List<string>();

    // number of performing actors
    public const int NUM_PERFORMING_ACTORS_GAME = 5;
    public const int NUM_PERFORMING_ACTORS_NO_GAME = 2;

    public static List<Actor> performingActors = new List<Actor>();

    // movements performed by the avatar to copy 
    public static ArrayList rotationsRightArm = new ArrayList();
    public static ArrayList rotationsRightForeArm = new ArrayList();
    public static ArrayList rotationsRightHand = new ArrayList();

    public static ArrayList rotationsLeftArm = new ArrayList();
    public static ArrayList rotationsLeftForeArm = new ArrayList();
    public static ArrayList rotationsLeftHand = new ArrayList();

    public static ArrayList rotationsHead = new ArrayList();

    public static ArrayList rotationsChest = new ArrayList();

    // number of actions performed by the avatar to copy
    public static int numActions = 0;

    // has the player already provided the feedback?
    public static bool feedbackProvided = false;

    // id of the best actor voted. -1 if nobody was voted
    public static int idBestActor = -1;

    // variable to count the number of episodes. It's incremented after every EndEpisode() call
    public static int numEpisode = 1;

    // retrieve all actors from environment
    public static List<Actor> GetActors()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        List<Actor> actors = new List<Actor>();

        foreach (GameObject actor in gameObjects)
        {
            if (actor.CompareTag("Actor"))
                actors.Add(actor.GetComponent<Actor>());
        }

        // sort actors by their id
        actors.Sort(new ActorComparer());
        return actors;
    }

    // retrieve all trapdoor covers from environment
    public static List<TrapdoorCover> GetTrapdoorCovers()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        List<TrapdoorCover> trapdoorCovers = new List<TrapdoorCover>();
        foreach (GameObject trapdoorCover in gameObjects)
        {
            if (trapdoorCover.CompareTag("TrapdoorCover"))
                trapdoorCovers.Add(trapdoorCover.GetComponent<TrapdoorCover>());
        }

        // sort actors by their id
        trapdoorCovers.Sort(new TrapdoorComparer());
        return trapdoorCovers;
    }

    // shuffle virtual and human avatar to change order in performances
    private static void ShuffleAvatarTypes(List<bool> avatarTypes)
    {
        for (int i = avatarTypes.Count - 1; i > 1; i--)
        {
            int rnd = Random.Range(0, i + 1);
            bool value = avatarTypes[rnd];
            avatarTypes[rnd] = avatarTypes[i];
            avatarTypes[i] = value;
        }
    }

    // load animations from resource folder
    public static void LoadAnimations()
    {
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

    // shuffle female and male animations
    public static void ShuffleAnimations()
    {
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

    // executed only on first play
    public static void Init()
    {
        allActors = GetActors();

        foreach (Actor actor in allActors)
        {
            actor.initialPosition = actor.transform.localPosition;
            actor.initialRotation = actor.transform.localRotation;
        }

        if (isGame)
            trapdoorCovers = GetTrapdoorCovers();

        LoadAnimations();
    }

    // setup round
    public static void SetupRound()
    {
        // reset actors data & positions
        foreach (Actor actor in allActors)
        {
            actor.idPerformance = -1;
            actor.trapdoorCover = null;
            actor.transform.localPosition = actor.initialPosition;
        }

        // delete previous selection of actors
        performingActors.Clear();

        // decide who is human and who is virtual
        if (isGame)
            ShuffleAvatarTypes(avatarTypesInGame);
        else
            ShuffleAvatarTypes(avatarTypesInNoGame);

        // select actors
        List<int> numbersAllActors = new List<int>();
        for (int i = 0; i < NUM_ALL_ACTORS; i++)
            numbersAllActors.Add(i + 1);

        if (isGame)
        {
            for (int i = 0; i < NUM_PERFORMING_ACTORS_GAME; i++)
            {
                int randomNum = Random.Range(0, numbersAllActors.Count);
                performingActors.Add(allActors[numbersAllActors[randomNum] - 1]);
                numbersAllActors.RemoveAt(randomNum);
            }
        }
        else
        {
            for (int i = 0; i < NUM_PERFORMING_ACTORS_NO_GAME; i++)
            {
                int randomNum = Random.Range(0, numbersAllActors.Count);
                performingActors.Add(allActors[numbersAllActors[randomNum] - 1]);
                numbersAllActors.RemoveAt(randomNum);
            }
        }

        // place actors
        for (int i = 0; i < performingActors.Count; i++)
        {
            performingActors[i].idPerformance = (i + 1);
            
            if (performingActors[i].gender == 0)
                performingActors[i].SetAnimation(femaleAnimations[i], femaleAnimationNames[i]);
            else
                performingActors[i].SetAnimation(maleAnimations[i], maleAnimationNames[i]);

            // TODO this works only with NUM_PERFORMING_ACTORS_GAME = 5
            if (isGame)
            {
                performingActors[i].trapdoorCover = trapdoorCovers[i];
                performingActors[i].isHuman = avatarTypesInGame[i];

                switch (i)
                {
                    case 0:
                        performingActors[i].transform.position = new Vector3(-2.4f, -2.5f, 0.41f);
                        break;

                    case 1:
                        performingActors[i].transform.position = new Vector3(-1.43f, -2.5f, 2.53f);
                        break;

                    case 2:
                        performingActors[i].transform.position = new Vector3(0.12f, -2.5f, 0.38f);
                        break;

                    case 3:
                        performingActors[i].transform.position = new Vector3(1.59f, -2.5f, 2.44f);
                        break;

                    case 4:
                        performingActors[i].transform.position = new Vector3(2.62f, -2.5f, 0.43f);
                        break;
                }
            }
            else
            {
                performingActors[i].isHuman = avatarTypesInNoGame[i];

                if (!isFirstRound)
                {
                    performingActors[i].rightArmAgent.EndEpisode();
                    performingActors[i].leftArmAgent.EndEpisode();
                    performingActors[i].headChestAgent.EndEpisode();
                }

                switch (i)
                {
                    case 0:
                        performingActors[i].transform.position = new Vector3(-1.2f, 0, 0);
                        break;

                    case 1:
                        performingActors[i].transform.position = new Vector3(1.2f, 0, 0);
                        break;
                }
            }
        }

        isFirstRound = false;
    }

    
}
