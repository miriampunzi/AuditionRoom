using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class CastingStory : MonoBehaviour
{
    [SerializeField] private GameObject ViveCameraRigPrefab;
    [SerializeField] private GameObject collidersViveCameraRigPrefab;

    private ArrayList actorsScript = new ArrayList()
    {
        "These are the actors \n\nat the casting auditions you organized",
        "Pay attention: \n\nnot all of them \n\nare professionals!",
        "You have to choose the best one \n\nperforming the emotion of joy",
        "Are you ready?"
    };

    private int position = 0;
    TextMeshPro scriptTextMesh;

    public static bool wasNextPressed = false;

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
            Instantiate(collidersViveCameraRigPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }

        scriptTextMesh = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        scriptTextMesh.text = (string)actorsScript[position];
                
        if (Input.GetKeyDown("x") || ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            position++;
            wasNextPressed = false;
        }

        if (position == actorsScript.Count)
        {
            scriptTextMesh.text = "";
        }
    }
}
