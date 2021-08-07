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

    [SerializeField] private GameObject presentationMask;

    private ArrayList contextScript = new ArrayList()
    {
        "Hello!",
        "You are an emerging movie director \n\n who organized a casting \n\nfor your new movie",
        "So, you are looking for an actor \n\nwho plays the role of \n\none of the main characters",
        "This character is particular \n\nbecause appears with a mask, \n\nnever speaks and \n\nis always sitting on a chair",
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
            Instantiate(ViveCameraRigPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }

        scriptTextMesh = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        scriptTextMesh.text = (string) contextScript[position];
                
        if (Input.GetKeyDown("x") || wasNextPressed)
        {
            position++;
            wasNextPressed = false;
        }

        if (position == 3)
        {
            presentationMask.active = true;
        }

        if (position == contextScript.Count)
        {
            SceneManager.LoadScene("Casting");
        }
    }
}
