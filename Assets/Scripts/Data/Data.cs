using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    public float[] current_position = new float[3];

    public Vector3 Location
    {
        get { return new Vector3(current_position[0], current_position[1], current_position[2]); }
        set { current_position[0] = value.x; current_position[1] = value.y; current_position[2] = value.z; }
    }

    public float current_health;

    public float CurHealth
    {
        get { return current_health; }
        set { current_health = value; }
    }
}
