using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    public static ArrayList rotationsRightArm = new ArrayList();
    public static ArrayList rotationsRightForeArm = new ArrayList();
    public static ArrayList rotationsRightHand = new ArrayList();

    public static int numActions = 0;

    public static void DNNRewardFunction()
    {
        //ActorAgent[] actors = EnvironmentStatus.getActors();

        //for (int i = 0; i < EnvironmentStatus.NUM_ACTORS; i++)
        //{
        //    for (int j = 0; j < Human.numActions; j++)
        //    {
        //        actors[i].AddReward(calculateReward((Quaternion)actors[i].performedRotationsRightArm[j], (Quaternion)rotationsRightArm[j]));
        //        actors[i].AddReward(calculateReward((Quaternion)actors[i].performedRotationsRightForeArm[j], (Quaternion)rotationsRightForeArm[j]));
        //        actors[i].AddReward(calculateReward((Quaternion)actors[i].performedRotationsRightHand[j], (Quaternion)rotationsRightHand[j]));
        //    }
        //}
    }

    private static float calculateReward(Quaternion v1, Quaternion v2)
    {
        float pointMaxDistX = Math.Abs(v1.x) + 1;
        float pointMaxDistY = Math.Abs(v1.y) + 1;
        float pointMaxDistZ = Math.Abs(v1.z) + 1;
        float pointMaxDistW = Math.Abs(v1.w - 0.5f) + 0.5f;
        float maxDist = (float)Math.Sqrt(Math.Pow(pointMaxDistX, 2) + Math.Pow(pointMaxDistY, 2) + Math.Pow(pointMaxDistZ, 2) + Math.Pow(pointMaxDistW, 2));
        float reward = 1 - (2 / maxDist) * (float)Math.Sqrt(Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2) + Math.Pow(v1.z - v2.z, 2) + Math.Pow(v1.w - v2.w, 2));

        Debug.Log(reward);

        return reward;
    }
}
