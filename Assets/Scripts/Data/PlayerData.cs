using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData : Data
{
    public float max_health;

    public float MaxHealth
    {
        get { return max_health; }
        set { max_health = value; }
    }
    public float max_shield;

    public float MaxShield
    {
        get { return max_shield; }
        set { max_shield = value; }
    }

    public float max_sp;
    public float MaxSp
    {
        get { return max_sp; }
        set { max_sp = value; }
    }

    public float move_speed;

    public float MoveSpeed
    {
        get { return move_speed; }
        set { move_speed = value; }
    }

    public int jump_time;
    public int JumpTime
    {
        get { return jump_time; }
        set
        {
            jump_time = value;
        }
    }

    public int attack;
    public int Attack
    {
        get { return attack; }
        set
        {
            attack = value;
        }
    }
    public int shieldValue;
    public int ShieldValue
    {
        get { return shieldValue; }
        set
        {
            shieldValue = value;
        }
    }

    public float critical_rate;
    public float CriticalRate
    {
        get { return critical_rate; }
        set
        {
            critical_rate = value;
        }
    }

    public float cur_exp;
    public float CurExp
    {
        get { return cur_exp; }
        set
        {
            cur_exp = value;
        }
    }

    public float max_exp;
    public float MaxExp
    {
        get { return max_exp; }
        set
        {
            max_exp = value;
        }
    }

    public int level;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
        }   
    }
}
