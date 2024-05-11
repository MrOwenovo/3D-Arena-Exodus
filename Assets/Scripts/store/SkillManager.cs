using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public List<Skill> ownedSkills;
    public List<Skill> availableSkills;
    public int coins = 100;  // Assume player starts with 100 coins

    void Awake()
    {
        instance = this;
        LoadSkills();
    }

    public void BuySkill(Skill skill)
    {
        if (coins >= skill.cost && !skill.isOwned)
        {
            skill.isOwned = true;
            coins -= skill.cost;
            ownedSkills.Add(skill);
            SaveSkills();
        }
    }

    void LoadSkills()
    {
        // Load skills and coins from PlayerPrefs (or any other method you prefer)
    }

    void SaveSkills()
    {
        // Save skills and coins to PlayerPrefs (or any other method you prefer)
    }
}