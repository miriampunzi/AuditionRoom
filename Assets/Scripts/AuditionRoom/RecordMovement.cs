using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordMovement : MonoBehaviour
{
    private bool hasRecorded = false;   // has the avatar to copy finished the performance?
    private static bool hasStartedPlaying = false;

    public static Transform avatarToCopy;
    static Animator animatorAvatarToCopy;

    static Transform avatarToCopyRightArm;
    static Transform avatarToCopyRightForeArm;
    static Transform avatarToCopyRightHand;

    static Transform avatarToCopyLeftArm;
    static Transform avatarToCopyLeftForeArm;
    static Transform avatarToCopyLeftHand;

    static Transform avatarToCopyHead;
    static Transform avatarToCopyChest;

    public static void Init()
    {
        avatarToCopy = GameObject.FindGameObjectWithTag("AvatarToCopy").transform;
        animatorAvatarToCopy = avatarToCopy.GetComponent<Animator>();

        avatarToCopyRightArm = GameObject.FindGameObjectWithTag("RightArmAvatarToCopy").transform;
        avatarToCopyRightForeArm = GameObject.FindGameObjectWithTag("RightForeArmAvatarToCopy").transform;
        avatarToCopyRightHand = GameObject.FindGameObjectWithTag("RightHandAvatarToCopy").transform;

        avatarToCopyLeftArm = GameObject.FindGameObjectWithTag("LeftArmAvatarToCopy").transform;
        avatarToCopyLeftForeArm = GameObject.FindGameObjectWithTag("LeftForeArmAvatarToCopy").transform;
        avatarToCopyLeftHand = GameObject.FindGameObjectWithTag("LeftHandAvatarToCopy").transform;

        avatarToCopyHead = GameObject.FindGameObjectWithTag("HeadAvatarToCopy").transform;

        avatarToCopyChest = GameObject.FindGameObjectWithTag("ChestAvatarToCopy").transform;
    }


    public static void Record()
    {
        if (!hasStartedPlaying)
        {
            ResetMotions();
            //animatorAvatarToCopy.Play("Standing Greeting", -1, 0f);
            hasStartedPlaying = true;
        }

        Debug.Log("Recording...");

        // saving rotations performed by avatar to copy
        EnvironmentStatus.rotationsRightArm.Add(avatarToCopyRightArm.localRotation);
        EnvironmentStatus.rotationsRightForeArm.Add(avatarToCopyRightForeArm.localRotation);
        EnvironmentStatus.rotationsRightHand.Add(avatarToCopyRightHand.localRotation);

        EnvironmentStatus.rotationsLeftArm.Add(avatarToCopyLeftArm.localRotation);
        EnvironmentStatus.rotationsLeftForeArm.Add(avatarToCopyLeftForeArm.localRotation);
        EnvironmentStatus.rotationsLeftHand.Add(avatarToCopyLeftHand.localRotation);

        EnvironmentStatus.rotationsHead.Add(avatarToCopyHead.localRotation);

        EnvironmentStatus.rotationsChest.Add(avatarToCopyChest.localRotation);

        EnvironmentStatus.numActions++;
    }

    public static void ResetMotions()
    {
        EnvironmentStatus.rotationsRightArm.Clear();
        EnvironmentStatus.rotationsRightForeArm.Clear();
        EnvironmentStatus.rotationsRightHand.Clear();

        EnvironmentStatus.rotationsLeftArm.Clear();
        EnvironmentStatus.rotationsLeftForeArm.Clear();
        EnvironmentStatus.rotationsLeftHand.Clear();

        EnvironmentStatus.rotationsHead.Clear();

        EnvironmentStatus.rotationsChest.Clear();

        EnvironmentStatus.numActions = 0;

        hasStartedPlaying = false;
    }

    public static void SaveToFile()
    {
        // WRITE RIGHT ARM ROTATIONS IN FILE
        using (StreamWriter theWriter = new StreamWriter("RightArm.csv"))
        {
            theWriter.WriteLine("rightarmx;rightarmy;rightarmz;rightarmw;rightforearmx;rightforearmy;rightforearmz;rightforearmw;righthandx;righthandy;righthandz;righthandw");
            for (int i = 0; i < EnvironmentStatus.numActions; i++)
            {
                Quaternion qra = (Quaternion)EnvironmentStatus.rotationsRightArm[i];
                Quaternion qrfa = (Quaternion)EnvironmentStatus.rotationsRightForeArm[i];
                Quaternion qrh = (Quaternion)EnvironmentStatus.rotationsRightHand[i];
                theWriter.WriteLine(qra.x + ";" + qra.y + ";" + qra.z + ";" + qra.w + ";" + qrfa.x + ";" + qrfa.y + ";" + qrfa.z + ";" + qrfa.w + ";" + qrh.x + ";" + qrh.y + ";" + qrh.z + ";" + qrh.w);
            }
        }

        // WRITE LEFT ARM ROTATIONS IN FILE
        using (StreamWriter theWriter = new StreamWriter("LeftArm.csv"))
        {
            theWriter.WriteLine("leftarmx;leftarmy;leftarmz;leftarmw;leftforearmx;leftforearmy;leftforearmz;leftforearmw;lefthandx;lefthandy;lefthandz;lefthandw");
            for (int i = 0; i < EnvironmentStatus.numActions; i++)
            {
                Quaternion qla = (Quaternion)EnvironmentStatus.rotationsLeftArm[i];
                Quaternion qlfa = (Quaternion)EnvironmentStatus.rotationsLeftForeArm[i];
                Quaternion qlh = (Quaternion)EnvironmentStatus.rotationsLeftHand[i];
                theWriter.WriteLine(qla.x + ";" + qla.y + ";" + qla.z + ";" + qla.w + ";" + qlfa.x + ";" + qlfa.y + ";" + qlfa.z + ";" + qlfa.w + ";" + qlh.x + ";" + qlh.y + ";" + qlh.z + ";" + qlh.w);
            }
        }

        // WRITE HEAD ROTATIONS IN FILE
        using (StreamWriter theWriter = new StreamWriter("Head.csv"))
        {
            theWriter.WriteLine("headx;heady;headz;headw");
            for (int i = 0; i < EnvironmentStatus.numActions; i++)
            {
                Quaternion qh = (Quaternion)EnvironmentStatus.rotationsHead[i];
                theWriter.WriteLine(qh.x + ";" + qh.y + ";" + qh.z + ";" + qh.w);
            }
        }

        // WRITE CHEST ROTATIONS IN FILE
        using (StreamWriter theWriter = new StreamWriter("Chest.csv"))
        {
            theWriter.WriteLine("chestx;chesty;chestz;chestw");
            for (int i = 0; i < EnvironmentStatus.numActions; i++)
            {
                Quaternion ch = (Quaternion)EnvironmentStatus.rotationsChest[i];
                theWriter.WriteLine(ch.x + ";" + ch.y + ";" + ch.z + ";" + ch.w);
            }
        }

        Debug.Log("Saved to file...");
    }
}
