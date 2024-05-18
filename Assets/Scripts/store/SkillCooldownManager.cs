using System.Collections.Generic;

public class SkillCooldownManager
{
    private Dictionary<string, float> skillCooldowns = new Dictionary<string, float>();

    public void SetCooldown(string skillName, float cooldownTime)
    {
        if (skillCooldowns.ContainsKey(skillName))
        {
            skillCooldowns[skillName] = cooldownTime;
        }
        else
        {
            skillCooldowns.Add(skillName, cooldownTime);
        }
    }

    public float GetCooldown(string skillName)
    {
        if (skillCooldowns.ContainsKey(skillName))
        {
            return skillCooldowns[skillName];
        }
        else
        {
            return 0f;  
        }
    }
}

public class ExampleUsage
{
    private SkillCooldownManager cooldownManager = new SkillCooldownManager();

    void Start()
    {
        cooldownManager.SetCooldown("Shield awarded", 10f);
        cooldownManager.SetCooldown("Fireball", 15f);
    }

    void Update()
    {
        float shieldCooldown = cooldownManager.GetCooldown("Shield awarded");
        float fireballCooldown = cooldownManager.GetCooldown("Fireball");
    }
}