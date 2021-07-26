using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class CastingStory : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private ArrayList actorsScript = new ArrayList()
    {
        "These are the actors \n\nat the casting auditions you organized",
        "Pay attention: not all of them are professionals!",
        "You have to choose the best one \n\nperforming the emotion of joy",
        "Are you ready?"
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
