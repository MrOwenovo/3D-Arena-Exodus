using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public List<Skill> ownedSkills = new List<Skill>();
    public List<Skill> availableSkills = new List<Skill>();
    public List<Skill> equippedSkills = new List<Skill>(new Skill[4]);  // 4 skill slots
    private int coins = 0;  // 假设玩家开始时有100个硬币
    public Text coinsText;  // 显示硬币数量的UI组件

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            PopulateSkills();

            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateCoinDisplay();
    }

   void PopulateSkills()
   {
       LoadSkills();  // 尝试从 PlayerPrefs 加载技能列表
   
       if (ownedSkills == null || availableSkills == null || (ownedSkills.Count == 0 && availableSkills.Count == 0))
       {
           Debug.Log("从头开始");
           // 如果本地没有保存，从头开始加载技能
           GameObject skillsContainer = GameObject.Find("Skills");
           ownedSkills = new List<Skill>();
           availableSkills = new List<Skill>();
           PopulateSkillsRecursive(skillsContainer.transform, availableSkills);
       }
   
       // 同步技能状态到 UI 或其他系统
       // UpdateUIWithSkills();
   }

   void PopulateSkillsRecursive(Transform parent, List<Skill> targetList)
   {
       foreach (Transform skillTransform in parent)
       {
           SpriteRenderer renderer = skillTransform.GetComponent<SpriteRenderer>();
           if (renderer != null)
           {
               bool isFree = skillTransform.parent.name == "free";
               Skill skill = new Skill(skillTransform.name, GetCostFromName(skillTransform.name), renderer.sprite, isFree);
               if (skill.isFree)
               {
                   skill.isOwned = true;
                   ownedSkills.Add(skill);
               }
               else
               {
                   targetList.Add(skill);
               }
           }
           PopulateSkillsRecursive(skillTransform, targetList);
       }
   }



    
   
   public void SaveSkills()
   {
       string ownedSkillsJson = JsonUtility.ToJson(new SkillListWrapper { Skills = ownedSkills });
       string availableSkillsJson = JsonUtility.ToJson(new SkillListWrapper { Skills = availableSkills });

       PlayerPrefs.SetString("OwnedSkills", ownedSkillsJson);
       PlayerPrefs.SetString("AvailableSkills", availableSkillsJson);
       PlayerPrefs.SetInt("SkillCoinsCollected", coins);
       PlayerPrefs.Save();
   }

   public void LoadSkills()
   {
       Debug.Log("Loading skills...");
       string ownedSkillsJson = PlayerPrefs.GetString("OwnedSkills", "");
       string availableSkillsJson = PlayerPrefs.GetString("AvailableSkills", "");

       if (!string.IsNullOrEmpty(ownedSkillsJson))
       {
           ownedSkills = JsonUtility.FromJson<SkillListWrapper>(ownedSkillsJson).Skills;
           Debug.Log("!!!!??");
           Debug.Log(ownedSkillsJson);
       }

       if (!string.IsNullOrEmpty(availableSkillsJson))
       {
           availableSkills = JsonUtility.FromJson<SkillListWrapper>(availableSkillsJson).Skills;
           Debug.Log("!!!!??");
           Debug.Log(availableSkillsJson);
       }

       coins = PlayerPrefs.GetInt("SkillCoinsCollected", 0);
       // ReassociateSprites();  // 重新关联 Sprite
   }


    [System.Serializable]
    public class SkillListWrapper
    {
        public List<Skill> Skills;
    }
    int GetCostFromName(string name)
    {
        // Check specific names and assign costs accordingly
        if (name == "bomb" || name == "Sword throwing")
        {
            return 3; // Cost for 'bomb' and 'Sword throwing'
        }
        else if (name == "bleed" || name == "lasers")
        {
            return 6; // Cost for 'bleed' and 'lasers'
        }
        else
        {
            return 2; // Default cost for all other names
        }
    }

    public Skill GetSkillByName(string skillName)
    {
        // 首先在拥有的技能列表中查找
        foreach (var skill in ownedSkills)
        {
            if (skill.name.Equals(skillName))
                return skill;
        }

        // 如果未找到，在可购买的技能列表中查找
        foreach (var skill in availableSkills)
        {
            if (skill.name.Equals(skillName))
                return skill;
        }

        // 如果还未找到，检查装备的技能
        foreach (var skill in equippedSkills)
        {
            if (skill != null && skill.name.Equals(skillName))
                return skill;
        }

        // 如果都没有找到，返回 null
        return null;
    }

    public bool BuySkill(Skill skill)
    {
        Debug.Log("Attempting to buy skill: " + skill.name);
        if (coins >= skill.cost && !skill.isOwned)
        {
            skill.isOwned = true;
            coins -= skill.cost;
            ownedSkills.Add(skill);
            availableSkills.Remove(skill);
            SaveSkills();
            SaveCoins();
            UpdateCoinDisplay();
            SkillUI.instance.UpdateSkillPanels();
            MessageManager.instance.ShowMessage("Skill purchased successfully: " + skill.name);
            Debug.Log("Skill purchased successfully.");
            return true;
        }
        else
        {
            MessageManager.instance.ShowMessage("Insufficient coins to purchase skill: " + skill.name);
            Debug.Log("Failed to purchase skill: insufficient coins.");
            return false;
        }
    }


    void UpdateCoinDisplay()
    {
        coinsText.text = "Coins: " + coins;
    }

    void LoadCoins()
    {
        // Load skills and coins from PlayerPrefs or another storage method
        coins = PlayerPrefs.GetInt("SkillCoinsCollected", 0)+10;  // Default to 100 coins if nothing is saved
    }

    void SaveCoins()
    {
        // Save skills and coins to PlayerPrefs or another storage method
        PlayerPrefs.SetInt("SkillCoinsCollected", coins);
        PlayerPrefs.Save();
    }
    
}

