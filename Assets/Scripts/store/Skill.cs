using UnityEngine;

[System.Serializable]
public class Skill
{
    public string name;
    public int cost;
    public bool isOwned;
    public Sprite icon;

    public Skill(string name, int cost, Sprite icon)
    {
        this.name = name;
        this.cost = cost;
        this.isOwned = false;
        this.icon = icon;
    }
}