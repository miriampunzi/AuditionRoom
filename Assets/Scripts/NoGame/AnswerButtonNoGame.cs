using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerButtonNoGame : MonoBehaviour
{
    [SerializeField] private bool isPositive;

    public void Pressed()
    {
        if (isPositive)
            StoryNoGame.wasYesPressed = true;
        else
            StoryNoGame.wasNoPressed = true;
    }
}
