using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;
    private LoadingCube loadingCube;

    private List<RightArmAgent> rightArmAgents = new List<RightArmAgent>();
    private List<LeftArmAgent> leftArmAgents = new List<LeftArmAgent>();
    private List<HeadChestAgent> headChestAgents = new List<HeadChestAgent>();        

    private bool isLoading = false;

    private enum StateLoading
    {
        First,
        Second,
        Third
    }

    private ArrayList loadingScript;

    private StateLoading currentStateLoading = StateLoading.First;
    private int indexLoadingScript = 0;

    public LoadingStateMachine()
    {
        scriptTextMesh = GameObject.FindGameObjectWithTag("Script").GetComponent<TextMeshPro>();
        loadingCube = GameObject.FindGameObjectWithTag("LoadingCube").GetComponent<LoadingCube>();
    }

    public void SetScript(ArrayList loadingScript)
    {
        this.loadingScript = loadingScript;
    }

    public void Execute()
    {
        // Learning in background underground
        for (int i = 0; i < EnvironmentStatus.performingActors.Count; i++)
        {
            EnvironmentStatus.performingActors[i].rightArmAgent.isForPerformance = false;
            EnvironmentStatus.performingActors[i].leftArmAgent.isForPerformance = false;
            EnvironmentStatus.performingActors[i].headChestAgent.isForPerformance = false;

            EnvironmentStatus.performingActors[i].rightArmAgent.LearnInBackground();
            EnvironmentStatus.performingActors[i].leftArmAgent.LearnInBackground();
            EnvironmentStatus.performingActors[i].headChestAgent.LearnInBackground();
        }

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

                        for (int i = 0; i < EnvironmentStatus.performingActors.Count; i++)
                        {
                            EnvironmentStatus.performingActors[i].rightArmAgent.EndEpisode();
                            EnvironmentStatus.performingActors[i].leftArmAgent.EndEpisode();
                            EnvironmentStatus.performingActors[i].headChestAgent.EndEpisode();
                        }

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
