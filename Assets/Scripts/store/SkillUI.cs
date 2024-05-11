using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    public GameObject skillPanelPrefab;
    public Transform ownedSkillsPanel;
    public Transform availableSkillsPanel;

    void Start()
    {
        PopulateSkillPanels();
    }

    void PopulateSkillPanels()
    {
        foreach (var skill in SkillManager.instance.ownedSkills)
        {
            CreateSkillPanel(skill, ownedSkillsPanel);
        }
        foreach (var skill in SkillManager.instance.availableSkills)
        {
            CreateSkillPanel(skill, availableSkillsPanel);
        }
    }

    void CreateSkillPanel(Skill skill, Transform parentPanel)
    {
        GameObject panel = Instantiate(skillPanelPrefab, parentPanel);
        panel.GetComponentInChildren<Text>().text = skill.name + " - Cost: " + skill.cost;
        panel.GetComponentInChildren<Image>().sprite = skill.icon;
        if (!skill.isOwned)
        {
            panel.GetComponentInChildren<Button>().onClick.AddListener(() => SkillManager.instance.BuySkill(skill));
        }
    }
}