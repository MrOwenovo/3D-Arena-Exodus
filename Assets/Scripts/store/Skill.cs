using UnityEngine;

[System.Serializable]
public class Skill
{
    public string name;
    public int cost;
    public bool isOwned;
    public Sprite icon;
    public bool isFree;// Free skills are automatically owned

    public Skill(string name, int cost, Sprite icon, bool isFree = false)
    {
        this.name = name;
        this.cost = cost;
        this.isOwned = false;
        this.icon = icon;
        this.isFree = isFree;
    }
}