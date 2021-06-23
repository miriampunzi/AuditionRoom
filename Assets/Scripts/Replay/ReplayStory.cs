using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayStory : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private enum StateReplay
    {
        Question,
        Performance,
        Continue
    }

    private ArrayList replayScript = new ArrayList()
    {
        "Which actor do you want to ask for a replay",
        "Performance...",
        "Do you want to see other replays?"
    };

    private StateReplay currentStateReplay;
    private int indexReplayScript = 0;

    private List<Actor> actors;
    TextMeshPro scriptTextMesh;

    private bool trapdoorCoverUp = false;
    private bool hasStartedPlaying = false;
    private bool trapdoorCoverDown = false;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Instantiate(playerPrefab, new Vector3(0, 0, -3.8f), Quaternion.identity);

            GameObject VRCamera = GameObject.Find("VRCamera");
            Camera camera = VRCamera.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

        scriptTextMesh = GetComponent<TextMeshPro>();
        actors = EnvironmentStatus.getActors();
        currentStateReplay = StateReplay.Question;
    }

    private void Update()
    {
        if (indexReplayScript < replayScript.Count)
        {
            scriptTextMesh.text = (string)replayScript[indexReplayScript];

            switch (currentStateReplay)
            {
                case StateReplay.Question:
                    if (EnvironmentStatus.hasAskedForReplay)
                    {
                        indexReplayScript++;
                        currentStateReplay = StateReplay.Performance;
                        EnvironmentStatus.hasAskedForReplay = false;
                    }

                    break;

                case StateReplay.Performance:
                    if (!trapdoorCoverUp)
                    {
                        actors[EnvironmentStatus.idActorForReplay - 1].transform.position = new Vector3(
                            actors[EnvironmentStatus.idActorForReplay - 1].transform.position.x,
                            actors[EnvironmentStatus.idActorForReplay - 1].transform.position.y + 0.1f,
                            actors[EnvironmentStatus.idActorForReplay - 1].transform.position.z);
                        actors[EnvironmentStatus.idActorForReplay - 1].trapdoorCover.GoUpSlow();
                        trapdoorCoverUp = true;
                    }

                    if (!actors[EnvironmentStatus.idActorForReplay - 1].trapdoorCover.IsGoingUpSlow() && !hasStartedPlaying && !trapdoorCoverDown)
                    {
                        actors[EnvironmentStatus.idActorForReplay - 1].PlayAnimation();
                        hasStartedPlaying = true;
                    }

                    if (hasStartedPlaying && !actors[EnvironmentStatus.idActorForReplay - 1].IsPlayingAnimation())
                    {
                        hasStartedPlaying = false;
                        actors[EnvironmentStatus.idActorForReplay - 1].trapdoorCover.GoDownSlow();
                        trapdoorCoverDown = true;
                    }

                    if (trapdoorCoverDown && !actors[EnvironmentStatus.idActorForReplay - 1].trapdoorCover.IsGoingDownSlow())
                    {
                        currentStateReplay = StateReplay.Continue;
                        indexReplayScript++;
                        trapdoorCoverDown = false;
                    }

                    break;

                case StateReplay.Continue:
                    // YES
                    if (EnvironmentStatus.wasYesPressed && !EnvironmentStatus.wasNoPressed)
                    {
                        indexReplayScript = 0;
                        currentStateReplay = StateReplay.Question;
                        EnvironmentStatus.wasYesPressed = false;

                        trapdoorCoverUp = false;
                        hasStartedPlaying = false;
                        trapdoorCoverDown = false;
                    }
                    // NO
                    else if (!EnvironmentStatus.wasYesPressed && EnvironmentStatus.wasNoPressed)
                    {
                        EnvironmentStatus.wasNoPressed = false;
                        SceneManager.LoadScene("AuditionRoom");
                    }

                    break;
            }
        }
    }
}
