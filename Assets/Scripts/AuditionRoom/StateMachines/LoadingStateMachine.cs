using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private LoadingCube loadingCube;

    private bool isLoading = false;

    private enum StateLoading
    {
        First,
        Second,
        Third
    }

    private ArrayList loadingScript = new ArrayList()
    {
        "Processing movement...",
        "Actors are learning...",
        "Almost done..."
    };

    private StateLoading currentStateLoading = StateLoading.First;
    private int indexLoadingScript = 0;

    public LoadingStateMachine()
    {
        scriptTextMesh = GameObject.FindGameObjectWithTag("Script").GetComponent<TextMeshPro>();
        loadingCube = GameObject.FindGameObjectWithTag("LoadingCube").GetComponent<LoadingCube>();
    }
    public void Execute()
    {
        // update text script
        if (indexLoadingScript < loadingScript.Count)
        {
            scriptTextMesh.text = (string)loadingScript[indexLoadingScript];

            switch (currentStateLoading)
            {
                case StateLoading.First:

                    if (!isLoading)
                    {
                        loadingCube.Show();
                        loadingCube.Move();
                        isLoading = true;
                    }

                    if (!loadingCube.IsMoving())
                    {
                        currentStateLoading = StateLoading.Second;
                        indexLoadingScript++;
                        isLoading = false;
                    }

                    break;

                case StateLoading.Second:

                    if (!isLoading)
                    {
                        loadingCube.Show();
                        loadingCube.Move();
                        isLoading = true;
                    }

                    if (!loadingCube.IsMoving())
                    {
                        currentStateLoading = StateLoading.Third;
                        indexLoadingScript++;
                        isLoading = false;
                    }

                    break;

                case StateLoading.Third:

                    if (!isLoading)
                    {
                        loadingCube.Show();
                        loadingCube.Move();
                        isLoading = true;
                    }

                    if (!loadingCube.IsMoving())
                    {
                        loadingCube.Hide();
                        isLoading = false;
                        Story.NextState();
                    }

                    break;
            }
        }   


        
    }

    public void ResetStateMachine()
    {
        currentStateLoading = StateLoading.First;
        indexLoadingScript = 0;
        isLoading = false;
    }
}
