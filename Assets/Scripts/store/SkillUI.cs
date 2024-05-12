﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    public static SkillUI instance;

    public GameObject skillPanelPrefab; // Prefab for displaying a skill
    public Transform ownedSkillsPanel; // Parent panel for owned skills
    public Transform availableSkillsPanel; // Parent panel for skills that can be purchased
    public GameObject backgroundPanel; // Parent panel for skills that can be purchased
    public GameObject storePanel; // The entire store panel
    
    public Button clearButton; 

    public GameObject[] skillContainers;
    public GameObject containerContainer; // The entire store panel

    public ScrollRect ownedScrollRect;
    public ScrollRect availableScrollRect;

    private List<GameObject> ownedSkillPanels = new List<GameObject>();
    private List<GameObject> availableSkillPanels = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PopulateSkillPanels();
        storePanel.SetActive(false); // Ensure the store is hidden at game start
        clearButton.onClick.AddListener(ClearSkillContainers);
        LoadContainerState(); // Load containers state at start
    }

    public void ToggleSkillStore() {
        bool isActive = !storePanel.activeSelf;  // Determine the new active state based on the current state
        SetActiveRecursively(storePanel, isActive);
    }

// Recursive method to set the active state of a GameObject and all of its children, except those named "Image"
public void SetActiveRecursively(GameObject obj, bool state) {
        if (obj.name != "Image") {
            obj.SetActive(state);
        }
        foreach (Transform child in obj.transform) {
            SetActiveRecursively(child.gameObject, state);  // Apply the same to all children
        }
    }



    void PopulateSkillPanels()
    {
        // Clear existing panels to prevent duplication
        foreach (Transform child in ownedSkillsPanel) Destroy(child.gameObject);
        foreach (Transform child in availableSkillsPanel) Destroy(child.gameObject);

        // Populate the owned and available skill panels
        foreach (var skill in SkillManager.instance.ownedSkills)
        {
            foreach (var ownedSkill in SkillManager.instance.ownedSkills)
            {
                Debug.Log(ownedSkill.name + ", " + ownedSkill.cost + ", " + ownedSkill.isOwned + ", " +
                          ownedSkill.isFree + ", " + ownedSkill.icon);
            }

            CreateSkillPanel(skill, ownedSkillsPanel, false);
        }

        foreach (var skill in SkillManager.instance.availableSkills)
        {
            foreach (var ownedSkill in SkillManager.instance.availableSkills)
            {
                Debug.Log(ownedSkill.name + ", " + ownedSkill.cost + ", " + ownedSkill.isOwned + ", " +
                          ownedSkill.isFree + ", " + ownedSkill.icon);
            }

            CreateSkillPanel(skill, availableSkillsPanel, true);
        }
    }

    GameObject CreateSkillPanel(Skill skill, Transform parentPanel, bool isPurchasable)
    {
        GameObject panel = Instantiate(skillPanelPrefab, parentPanel);
        // 设置技能名称和成本
        panel.transform.Find("SkillImage/name").GetComponent<Text>().text = skill.name;
        // 设置技能图标
        panel.transform.Find("SkillImage").GetComponent<Image>().sprite = skill.icon;
        Image skillImage = panel.transform.Find("SkillImage").GetComponent<Image>();
        skillImage.gameObject.AddComponent<Button>(); // Add Button component
        Button button = skillImage.GetComponent<Button>();

        if (isPurchasable)
        {
            panel.transform.Find("SkillImage/cost").GetComponent<Text>().text = skill.cost.ToString();

            button.onClick.AddListener(() =>
            {
                SkillManager.instance.BuySkill(skill);
                Debug.Log("购买技能");
            });


        }
        else
        {
            Debug.Log("添加免费技能监听");
            button.onClick.AddListener(() => AddSkillToContainer(skill));

            panel.transform.Find("SkillImage/cost").GetComponent<Text>().text = "";

          
        }

        return panel;
    }
    void SaveContainerState()
    {
        Debug.Log("开始保存");
        SkillContainerData[] containerData = new SkillContainerData[skillContainers.Length];
        for (int i = 0; i < skillContainers.Length; i++)
        {
            var container = skillContainers[i];
            var skillPanel = container.transform.Find("skillPanel(Clone)");
            if (skillPanel != null) // 确保skillPanel存在
            {
                Debug.Log("找到skillPanel");
                var skillImage = skillPanel.Find("SkillImage");
                if (skillImage != null)
                {
                    var text = skillImage.Find("name").GetComponent<Text>(); // 假设子对象的名称是 "Name"
                    if (text != null)
                    {
                        var skillName = text.text;
                        Debug.Log(skillName);
                        Debug.Log("找到后添加"+i);
                        containerData[i] = new SkillContainerData { skillName = skillName };
                        Debug.Log(containerData[i].skillName);
                    }
                }
                else
                {
                    Debug.Log("无法添加"+i);

                    // 如果Image组件或sprite为空，则记录空字符串或null
                    containerData[i] = new SkillContainerData { skillName = "" };
                }
            }
            else
            {
                Debug.Log("没找到panel"+i);
                // 如果没有找到skillPanel，则记录空字符串或null
                containerData[i] = new SkillContainerData { skillName = "" };
            }
        }

        Debug.Log(containerData[0].skillName);
        Debug.Log(containerData[1].skillName);
        Debug.Log(containerData[2].skillName);
        Debug.Log(containerData[3].skillName);
        Debug.Log(JsonUtility.ToJson(containerData));
        // 将技能容器数据序列化为JSON字符串并保存
        string jsonData = JsonUtility.ToJson(new SkillContainerDataArray { data = containerData });
        Debug.Log("JSON Data: " + jsonData);
        Debug.Log("保存技能+"+jsonData);
        Debug.Log(containerData);
        PlayerPrefs.SetString("SkillContainerData", jsonData);
        PlayerPrefs.Save();
        string jsonData2 = PlayerPrefs.GetString("SkillContainerData", "");
        Debug.Log("!!!!+"+jsonData2);
    }

    [System.Serializable]
    public class SkillContainerData
    {
        public string skillName;
    }

    [System.Serializable]
    public class SkillContainerDataArray
    {
        public SkillContainerData[] data;
    }

    void LoadContainerState()
    {
        string jsonData = PlayerPrefs.GetString("SkillContainerData", "");
        Debug.Log("Loaded JSON Data : " + jsonData);
        if (!string.IsNullOrEmpty(jsonData))
        {
            SkillContainerDataArray containerDataArray = JsonUtility.FromJson<SkillContainerDataArray>(jsonData);
            if (containerDataArray != null && containerDataArray.data != null)
            {
                for (int i = 0; i < containerDataArray.data.Length; i++)
                {
                    var data = containerDataArray.data[i];
                    if (!string.IsNullOrEmpty(data.skillName))
                    {
                        Skill skill = SkillManager.instance.GetSkillByName(data.skillName);
                        if (skill != null && i < skillContainers.Length)
                        {
                            InstantiateSkillInContainer(skill, skillContainers[i]);
                        }
                    }
                }
            }
        }
    }

    bool IsSkillAlreadyAdded(Skill skill)
    {
        foreach (GameObject container in skillContainers)
        {
            if (container.transform.childCount > 0)
            {
                var skillPanel = container.transform.Find("skillPanel(Clone)");
                if (skillPanel != null) // 确保skillPanel存在
                {
                    var skillImage = skillPanel.Find("SkillImage");
                    if (skillImage != null)
                    {
                        var text = skillImage.Find("name").GetComponent<Text>(); // 假设子对象的名称是 "Name"
                        if (text != null&&text.text== skill.name)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    void AddSkillToContainer(Skill skill)
    {
        Debug.Log("监听");
        foreach (GameObject container in skillContainers)
        {
            if (container.transform.childCount == 0 && !IsSkillAlreadyAdded(skill))
            {
                Debug.Log("添加技能");
                InstantiateSkillInContainer(skill, container);
                break;
            }
            
        }
    }
    void InstantiateSkillInContainer(Skill skill, GameObject container)
    {
        GameObject skillObj = Instantiate(skillPanelPrefab, container.transform);
        RectTransform rectTransform = skillObj.GetComponent<RectTransform>();
        rectTransform.SetParent(container.transform, false);

        // 设置为常规缩放和位置调整
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        // 将锚点和轴心设置在父对象的右下角
        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
        rectTransform.pivot = new Vector2(1, 0);

        // 调整位置偏移量
        rectTransform.anchoredPosition = new Vector2(-10, 10);  // 可

        skillObj.transform.Find("SkillImage/name").GetComponent<Text>().text = skill.name;
        skillObj.transform.Find("SkillImage").GetComponent<Image>().sprite = skill.icon;
        GameObject skillImage = skillObj.transform.Find("SkillImage").gameObject;
        skillImage.SetActive(true); // Activate the SkillImage if it was inactive

        if (!skillImage.activeSelf)
        {
            skillImage.SetActive(true); // Activate the SkillImage if it was inactive
        }

        // Optionally, add a click event to remove from container or other functionality
        SaveContainerState(); // Save state after adding skill
    }
   
    void ClearSkillContainers()
    {
        foreach (GameObject container in skillContainers)
        {
            foreach (Transform child in container.transform)
            {
                Destroy(child.gameObject);
            }
        }
        SaveContainerState(); // Save state after clearing
    }
    
    public void UpdateSkillPanels()
    {
        ClearSkillPanels();
        PopulateSkillPanels();
    }
    private void ClearSkillPanels()
    {
        foreach (Transform child in ownedSkillsPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in availableSkillsPanel)
        {
            Destroy(child.gameObject);
        }
    }
}

  
   