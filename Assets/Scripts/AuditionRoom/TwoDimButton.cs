using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimButton : MonoBehaviour
{
    public static void OnClickYes()
    {
        Story.wasYesPressed = true;
    }

    public static void OnClickNo()
    {
        Story.wasNoPressed = true;
    }
}
