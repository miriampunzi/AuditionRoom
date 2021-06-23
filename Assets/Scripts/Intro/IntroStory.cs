using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class IntroStory : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private ArrayList contextScript = new ArrayList()
    {
        "You are a movie director who \nis emerging among the \nmost famous directors.",
        "You have just finished writing with \nQuentin Tarantino the screenplay for “ABCD”.",
        "Now it’s time to create the casting. \nYou are searching for a new actor for the role of X, \none of the main characters of the movie.",
        "This character is particular because appears with a mask, \nhe/she never speaks and \nhe/she’s always seated on a chair.",
    };

    private int position = 0;
    TextMeshPro scriptTextMesh;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            GameObject VRCamera = GameObject.Find("VRCamera");
            Camera camera = VRCamera.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

        scriptTextMesh = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        scriptTextMesh.text = (string) contextScript[position];
                
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            position++;
        }

        if (position == contextScript.Count)
        {
            SceneManager.LoadScene("Casting");
        }
    }
}
