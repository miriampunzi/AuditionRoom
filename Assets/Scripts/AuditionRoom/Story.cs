using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR;

public class Story : MonoBehaviour
{
    private ArrayList instructionsScript = new ArrayList()
    {
        "Now it’s the turn of the actor 1. Are you ready to see the performance?",
        "Performance...",
        "Do you want to see a replay?",
        "Did you like the performance?"
    };

    private int positionInIstructions = 0;
    private int indexActor = 3;
    TextMeshPro scriptTextMesh;

    private bool trapdoorCoverUp = false;
    private Actor[] actors;

    private void Start()
    {
        scriptTextMesh = GetComponent<TextMeshPro>();
        actors = EnvironmentStatus.getActors();
    }

    void Update()
    {
        if (indexActor < EnvironmentStatus.NUM_ACTORS)
        {
            if (positionInIstructions < instructionsScript.Count)
                scriptTextMesh.text = (string)instructionsScript[positionInIstructions];

            switch (positionInIstructions)
            {
                case 0:
                    if (!trapdoorCoverUp)
                    {
                        actors[indexActor].trapdoorCover.GoUpSlow();
                        trapdoorCoverUp = true;
                    }

                    if (EnvironmentStatus.wasYesPressed)
                    {
                        positionInIstructions++;
                        actors[indexActor].PlayAnimation();
                        EnvironmentStatus.wasYesPressed = false;
                    }

                    break;

                case 1:
                    if (!actors[indexActor].IsPlayingAnimation())
                    {
                        positionInIstructions++;
                    }
                    break;

                case 2: // REPLAY
                    if (EnvironmentStatus.wasYesPressed && !EnvironmentStatus.wasNoPressed)
                    {
                        positionInIstructions -= 2;
                        EnvironmentStatus.wasYesPressed = false;
                    }
                    else if (!EnvironmentStatus.wasYesPressed && EnvironmentStatus.wasNoPressed)
                    {
                        EnvironmentStatus.wasNoPressed = false;
                        positionInIstructions++;
                    }

                    break;

                case 3: // DID YOU LIKE THE PERFORMANCE

                    if (EnvironmentStatus.wasYesPressed && !EnvironmentStatus.wasNoPressed)
                    {
                        scriptTextMesh.text = "Great, see him/her later!";
                        EnvironmentStatus.wasYesPressed = false;
                        positionInIstructions++;
                        actors[indexActor].trapdoorCover.GoDownFast();
                    }
                    else if (!EnvironmentStatus.wasYesPressed && EnvironmentStatus.wasNoPressed)
                    {
                        scriptTextMesh.text = "BYE-BYE!!";
                        EnvironmentStatus.wasNoPressed = false;
                        positionInIstructions++;
                        actors[indexActor].trapdoorCover.GoDownFast();
                    }

                    break;

                default:
                    Debug.Log("entrato");
                    positionInIstructions = 0;
                    indexActor++;
                    break;
            }
        }
    }
}
