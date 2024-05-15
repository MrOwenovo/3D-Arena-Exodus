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
            return 0f; // 如果找不到技能名称，则返回默认值
        }
    }
}

// 在其他地方使用示例：
public class ExampleUsage
{
    private SkillCooldownManager cooldownManager = new SkillCooldownManager();

    void Start()
    {
        // 设置技能冷却时间
        cooldownManager.SetCooldown("Shield awarded", 10f);
        cooldownManager.SetCooldown("Fireball", 15f);
        // 其他技能...
    }

    void Update()
    {
        // 获取技能冷却时间并进行逻辑处理
        float shieldCooldown = cooldownManager.GetCooldown("Shield awarded");
        float fireballCooldown = cooldownManager.GetCooldown("Fireball");
        // 其他技能...
    }
}