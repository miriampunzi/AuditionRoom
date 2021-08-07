using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordingStateMachine : MonoBehaviour
{
    private TextMeshPro scriptTextMesh;

    public RecordingStateMachine(TextMeshPro scriptTextMesh)
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
        "Before, show an example of joy! \nYou will have to click the trigger button of the controller to start and stop the recordings. \n\nAre you ready?",
        "Press the trigger button when you want to start",
        "Recording...\nPress the trigger button when you want to finish",
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
                    if (Story.wasYesPressed && !Story.wasNoPressed)
                    {
                        indexInScript++;
                        currentStateRecording = StateRecording.Ready;

                        RecordMovement.Init();

                        Story.wasYesPressed = false;
                        Story.wasNoPressed = false;
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

                    Story.wasYesPressed = false;
                    Story.wasNoPressed = false;

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

                    Story.wasYesPressed = false;
                    Story.wasNoPressed = false;

                    break;

                case StateRecording.Continue:
                    // YES
                    if (Story.wasYesPressed && !Story.wasNoPressed)
                    {
                        RecordMovement.ResetMotions();

                        indexInScript = 1;
                        currentStateRecording = StateRecording.Ready;

                        Story.wasYesPressed = false;
                        Story.wasNoPressed = false;
                    }
                    // NO
                    else if (!Story.wasYesPressed && Story.wasNoPressed)
                    {
                        RecordMovement.SaveToFile();

                        Story.wasNoPressed = false;
                        Story.wasYesPressed = false;

                        Story.NextState();
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
