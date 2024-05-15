using UnityEngine;

[System.Serializable]
public class Skill
{
    public string name;
    public int cost;
    public bool isOwned;
    public Sprite icon;  // Direct reference to the Sprite
    public bool isFree;

    public Skill(string name, int cost, Sprite icon, bool isFree = false)
    {
        this.name = name;
        this.cost = cost;
        this.isOwned = isFree;
        this.icon = icon;
        this.isFree = isFree;
    }
}