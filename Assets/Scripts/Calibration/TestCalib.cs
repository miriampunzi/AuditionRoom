using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCalib : MonoBehaviour
{

    [Header("Tracking References")]

    public Transform spine;
    public Transform lLeg;
    public Transform rLeg;

    public Transform head;

    public TextMesh textMesh;


    //---------Arms
    public Transform lController;
    public Transform rController;

    [Header("Model References")]
    public Transform lHand;
    public Transform rHand;

    public Transform lFoot;
    [Header("Params")]
    public bool changeRig = true;
    public float groundHeight = 2.1f;
    //1.80 = 0.95

    private Transform cameraRig;

    private float scaleMlp;

    private VRIK ik;

    private int nbIn = 0;

    private void Start()
    {
        ik = GetComponent<VRIK>();
        cameraRig = transform.parent;
        Debug.Assert(cameraRig.gameObject.name != "[CameraRig]");
    }

    public void TriggerIn()
    {
        nbIn++;
        TryBeginCalibration();
    }

    public void TriggerOut()
    {
        nbIn--;
        TryBeginCalibration();
    }
    private void TryBeginCalibration()
    {
        if (nbIn != 2)
            return;
        StartCoroutine(Timing());
    }

    void CalibrateSize()
    {
        scaleMlp = (head.position.y -  lLeg.position.y) / 2.00f;
        float sizeF = (ik.solver.spine.headTarget.position.y - ik.references.root.position.y) / (ik.references.head.position.y - ik.references.root.position.y);
        ik.references.root.localScale *= sizeF * scaleMlp;
    }

    void CalibrateTrackerRotation()
    {
        spine.LookAt(spine.position + new Vector3(1, 0, 0));

        lLeg.LookAt(lLeg.position + new Vector3(1, 0, 0));

        rLeg.LookAt(rLeg.position + new Vector3(1, 0, 0));

    }

    void CalibrateArmsLength()
    {
        float dist = Vector3.Distance(lController.position, lHand.position);
        ik.solver.leftArm.armLengthMlp = 1.0f + (dist / 6.0f);
        ik.solver.rightArm.armLengthMlp = 1.0f + (dist /6.0f);
    }

    void CalibrateSpine()
    {
        //[!] depend aussi de la taille de l'avatar donc a changer si changement avatar
        float dist = head.position.y - spine.position.y;
        ik.references.spine.localScale = Vector3.one * (dist + 0.1f);
    }

    void CalibrateLegs()
    {
        //[!] depend aussi de la taille de l'avatar donc a changer si changement avatar

        float dist = spine.position.y - lLeg.position.y;

        //Todo calculer
        ik.references.leftThigh.localScale = Vector3.one * dist;
        ik.references.rightThigh.localScale = Vector3.one * dist;
    }


    void OldCalibrateLegs()
    {
        //0.95 avk size
        scaleMlp = 0.95f;


        //Todo calculer
        ik.references.leftThigh.localScale *= scaleMlp;
        ik.references.rightThigh.localScale *= scaleMlp;
    }

    void ChangeCameraRigHeight() {
        Debug.Log("GroundHeight : " + groundHeight);
        Debug.Log("lfootPosY : " + lFoot.position.y);
        float dist = groundHeight - lFoot.position.y;
        var pos = cameraRig.localPosition;
        pos.y += dist;
        cameraRig.localPosition = pos;

    }

    IEnumerator Timing()
    {

        yield return new WaitForSeconds(2);
        textMesh.text = "\n Calibration : dans 3";
        yield return new WaitForSeconds(2);
        textMesh.text = "\n Calibration : dans 2";
        yield return new WaitForSeconds(2);
        textMesh.text = "\n Calibration : dans 1";
        yield return new WaitForSeconds(2);

        //---------Calibration des tailles

        //CalibrateSize();
        
        CalibrateLegs();
        CalibrateSpine();
        //-------- Calibration des rotations

        CalibrateTrackerRotation();
        CalibrateArmsLength();

        if (changeRig)
            ChangeCameraRigHeight();

    }

}
