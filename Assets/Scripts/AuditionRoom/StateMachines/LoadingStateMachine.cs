using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private LoadingCube loadingCube;

    private bool isLoading = false;

    public LoadingStateMachine()
    {
        scriptTextMesh = GameObject.FindGameObjectWithTag("Script").GetComponent<TextMeshPro>();
        loadingCube = GameObject.FindGameObjectWithTag("LoadingCube").GetComponent<LoadingCube>();
    }
    public void Execute()
    {
        if (!isLoading)
        {
            loadingCube.Show();
            scriptTextMesh.text = "Loading...";
            loadingCube.Move();
            isLoading = true;
        }

        if (!loadingCube.IsMoving())
        {
            loadingCube.Hide();
            Story.NextState();
        }
    }

    public void ResetStateMachine()
    {

    }
}
