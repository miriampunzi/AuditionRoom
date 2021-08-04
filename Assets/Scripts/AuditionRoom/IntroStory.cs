using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class IntroStory : MonoBehaviour
{
    [SerializeField] private GameObject ViveCameraRigPrefab;
    [SerializeField] private GameObject collidersViveCameraRigPrefab;

    private ArrayList contextScript = new ArrayList()
    {
        "You are a movie director who \n\nis emerging among the \n\nmost famous directors",
        "You are searching for a new actor \n\nof one of the main characters",
        "This character is particular \n\nbecause appears with a mask, \n\never speaks and \n\always seated on a chair",
    };

    private int position = 0;
    TextMeshPro scriptTextMesh;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("ViveCameraRig") == null)
        {
            Instantiate(ViveCameraRigPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            GameObject VRCamera = GameObject.Find("Camera");
            Camera camera = VRCamera.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
        }
        
        if (GameObject.FindGameObjectWithTag("ViveColliders") == null)
        {
            Instantiate(ViveCameraRigPrefab, new Vector3(0, 0, 0), Quaternion.identity);
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
