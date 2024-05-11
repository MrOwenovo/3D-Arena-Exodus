using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameUIData 
{
    // Start is called before the first frame update
    public float survivalTime;

    public float SurvivalTime {
        get {  return survivalTime; } set {  survivalTime = value; }
    }

    public int enemyKilledNum;
    public int EnemyKillNum
    {
        get { return enemyKilledNum; }
        set { enemyKilledNum = value; }
    }
}
