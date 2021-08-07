using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordingStateMachineNoGame : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;

    public RecordingStateMachineNoGame(TextMeshPro scriptTextMesh)
    {
        this.scriptTextMesh = scriptTextMesh;
    }

    private enum StateRecording
    {
        Question,
        Ready,
        Performance,
        Continue
    }

    private ArrayList script = new ArrayList()
    {
        "Before starting, show an example of joy. Are you ready?\n",
        "Press X when you want to start",
        "Recording...\nPress X when you want to finish",
        "Do you want to redo your recording?"
    };

    private StateRecording currentStateRecording = StateRecording.Question;
    private int indexInScript = 0;

    public void Execute()
    {
        // update text script
        if (indexInScript < script.Count)
        {
            scriptTextMesh.text = (string)script[indexInScript];

            switch (currentStateRecording)
            {
                case StateRecording.Question:
                    // YES
                    if (StoryNoGame.wasYesPressed && !StoryNoGame.wasNoPressed)
                    {
                        indexInScript++;
                        currentStateRecording = StateRecording.Ready;

                        RecordMovement.Init();

                        StoryNoGame.wasYesPressed = false;
                        StoryNoGame.wasNoPressed = false;
                    }

                    break;

                case StateRecording.Ready:
                    // X PRESSING
                    //if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))

                    if (Input.GetKeyDown("x"))
                    {
                        indexInScript++;
                        currentStateRecording = StateRecording.Performance;
                    }

                    StoryNoGame.wasYesPressed = false;
                    StoryNoGame.wasNoPressed = false;

                    break;

                case StateRecording.Performance:
                    // TODO TRACK MOVEMENTS
                    RecordMovement.Record();

                    // X PRESSING
                    //if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))

                    if (Input.GetKeyDown("x"))
                    {
                        indexInScript++;
                        currentStateRecording = StateRecording.Continue;
                    }

                    StoryNoGame.wasYesPressed = false;
                    StoryNoGame.wasNoPressed = false;

                    break;

                case StateRecording.Continue:
                    // YES
                    if (StoryNoGame.wasYesPressed && !StoryNoGame.wasNoPressed)
                    {
                        RecordMovement.ResetMotions();

                        indexInScript = 1;
                        currentStateRecording = StateRecording.Ready;

                        StoryNoGame.wasYesPressed = false;
                        StoryNoGame.wasNoPressed = false;
                    }
                    // NO
                    else if (!StoryNoGame.wasYesPressed && StoryNoGame.wasNoPressed)
                    {
                        RecordMovement.SaveToFile();

                        StoryNoGame.wasNoPressed = false;
                        StoryNoGame.wasYesPressed = false;

                        StoryNoGame.NextState();
                    }

                    break;
            }
        }
    }

    public void ResetStateMachine()
    {
        indexInScript = 0;
        currentStateRecording = StateRecording.Question;
    }
}
