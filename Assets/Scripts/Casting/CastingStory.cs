using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class CastingStory : MonoBehaviour
{
    private ArrayList actorsScript = new ArrayList()
    {
        "These are the actors who showed up \nat the casting auditions you organized. \nPay attention: not all of them are professionals!",
        "You have to choose the best one and \nget rid of the other actors that are \ntrying to invade your auditions!",
        "Choose the best one performing the \nemotion of joy with only body language.",
        "But before, show them an example of \nwhat is for you a good interpretation of joy \nperformed only with the upper part of your body.",
        "Are you ready?"
    };

    private int position = 0;
    TextMeshPro scriptTextMesh;

    private void Start()
    {
        scriptTextMesh = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        scriptTextMesh.text = (string)actorsScript[position];
                
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            position++;
        }

        if (position == actorsScript.Count)
        {
            SceneManager.LoadScene("AuditionRoom");
        }
    }
}
