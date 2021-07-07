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
        "You are a movie director who \n\nis emerging among the \n\nmost famous directors.",
        "You have just finished writing with \n\nQuentin Tarantino \n\nthe screenplay for \n\n“ABCD”.",
        "Now it’s time \n\nto create the casting.",
        "You are searching for a new actor \n\nfor the role of X, \n\none of the main characters.",
        "This character is particular \n\nbecause appears with a mask, \n\nhe/she never speaks and \n\nhe/she’s always seated on a chair.",
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
