using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class EnemyData : Data
{
    public float max_health;

    public float MaxHealth
    {
        get { return max_health; }
        set { max_health = value; }
    }

    public int attack;

    public int Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    public float exp;
    public float Exp
    {
        get { return exp; }
        set { exp = value; }
    }

}
