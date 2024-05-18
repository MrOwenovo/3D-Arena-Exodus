using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    public static SkillUI instance;

    public GameObject skillPanelPrefab; // Prefab for displaying a skill
    public Transform ownedSkillsPanel; // Parent panel for owned skills
    public Transform availableSkillsPanel; // Parent panel for skills that can be purchased
    public Transform availableSkillsPanelTraining; // Parent panel for skills that can be purchased
    public GameObject backgroundPanel; // Parent panel for skills that can be purchased
    public GameObject storePanel; // The entire store panel
    public GameObject storePanelTraining; // The entire store panel

    public Button clearButton;
    public Button clearButtonTrain;
    public Button OpenTrain;

    public GameObject[] skillContainers;
    public GameObject[] skillContainersTraining;
    public GameObject containerContainer; // The entire store pane l

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
        // PlayerPrefsUtil.DeleteAllPrefs();
        
        PopulateSkillPanels();
        storePanel.SetActive(false); // Ensure the store is hidden at game start
        clearButton.onClick.AddListener(ClearSkillContainers);
        clearButtonTrain.onClick.AddListener(ClearSkillContainersTrain);
        LoadContainerState(); // Load containers state at start
        if ((GameManager.instance.curStatus == Status.Training))
        {
            clearButtonTrain.gameObject.SetActive(true);
            OpenTrain.gameObject.SetActive(true);
            
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.PauseGame();
        }
    }

    public void ToggleSkillStore()
    {
        SkillUI.instance.SaveContainerState();

        bool isActive = !storePanel.activeSelf; // Determine the new active state based on the current state
        SetActiveRecursively(storePanel, isActive);
    }
    public void OpenSkillStore()
    {
        bool isActiveTraining = !storePanelTraining.activeSelf; // Determine the new active state based on the current state
        bool isActive = !storePanel.activeSelf; // Determine the new active state based on the current state
        SetActiveRecursively(storePanelTraining, isActiveTraining);

    }

// Recursive method to set the active state of a GameObject and all of its children, except those named "Image"
    public void SetActiveRecursively(GameObject obj, bool state)
    {
        if (obj.name != "Image")
        {
            obj.SetActive(state);
        }

        foreach (Transform child in obj.transform)
        {
            SetActiveRecursively(child.gameObject, state); // Apply the same to all children
        }
    } 
    public void SetActiveRecursivelyStep2(GameObject obj, bool state)
    {
        if (obj.name == "skillContainer1"||obj.name == "skillContainer2"||obj.name == "skillContainer3"||obj.name == "skillContainer4")
        {
            obj.SetActive(state);
        }

        foreach (Transform child in obj.transform)
        {
            SetActiveRecursively(child.gameObject, state); // Apply the same to all children
        }
    }


    void PopulateSkillPanels()
    {
         ;
        
            // Clear existing panels to prevent duplication
            foreach (Transform child in ownedSkillsPanel) Destroy(child.gameObject);
            foreach (Transform child in availableSkillsPanel) Destroy(child.gameObject);

            // Populate the owned and available skill panels
            foreach (var skill in SkillManager.instance.ownedSkills)
            {
                 ;

                foreach (var ownedSkill in SkillManager.instance.ownedSkills)
                {
                     
                }

                 ;
                CreateSkillPanel(skill, ownedSkillsPanel, false);
            }

            foreach (var skill in SkillManager.instance.availableSkills)
            {
                foreach (var ownedSkill in SkillManager.instance.availableSkills)
                {
                     
                }

                CreateSkillPanel(skill, availableSkillsPanel, true);
            }
       //
             ;

            // Populate the owned and available skill panels
            foreach (var skill in SkillManager.instance.ownedSkills)
            {
                CreateSkillPanel(skill, availableSkillsPanelTraining, false);
            }

            foreach (var skill in SkillManager.instance.availableSkills)
            {
                CreateSkillPanel(skill, availableSkillsPanelTraining, false);
            }
       
    }

    GameObject CreateSkillPanel(Skill skill, Transform parentPanel, bool isPurchasable)
    {
        GameObject panel = Instantiate(skillPanelPrefab, parentPanel);
        Text nameText = panel.transform.Find("SkillImage/name").GetComponent<Text>();
        Image skillImage = panel.transform.Find("SkillImage").GetComponent<Image>();
        nameText.text = skill.name;

         ;
        if (skill.icon != null)
        {
            var skillByName = SkillManager.instance.GetSkillByName(skill.name);
             
             
            skillImage.sprite = skillByName.icon;
        }
        
        else
        {
            Debug.LogError("Sprite is missing for skill: " + skill.name);
        }

        Button button = skillImage.gameObject.AddComponent<Button>();
        if (isPurchasable)
        {
            panel.transform.Find("SkillImage/cost").GetComponent<Text>().text = skill.cost.ToString();
            button.onClick.AddListener(() => { SkillManager.instance.BuySkill(skill); });
        }
        else
        {
            panel.transform.Find("SkillImage/cost").GetComponent<Text>().text = "";
            button.onClick.AddListener(() => AddSkillToContainer(skill));
        }

        return panel;
    }


    public void SaveContainerState()
    {
             ;
            SkillContainerData[] containerData = new SkillContainerData[skillContainers.Length];
            for (int i = 0; i < skillContainers.Length; i++)
            {
                var container = skillContainers[i];
                if (container != null)
                {
                    var skillPanel = container.transform.Find("skillPanel(Clone)");
                    if (skillPanel != null) 
                    {
                         ;
                        var skillImage = skillPanel.Find("SkillImage");
                        if (skillImage != null)
                        {
                            var text = skillImage.Find("name").GetComponent<Text>(); 
                            if (text != null)
                            {
                                var skillName = text.text;
                                 
                                 
                                containerData[i] = new SkillContainerData { skillName = skillName };
                                 
                            }
                        }
                        else
                        {
                             

                            containerData[i] = new SkillContainerData { skillName = "" };
                        }
                    }
                }
                else
                {
                     
                    containerData[i] = new SkillContainerData { skillName = "" };
                }
            }
            if (!(GameManager.instance.curStatus == Status.Training))
            {

            string jsonData = JsonUtility.ToJson(new SkillContainerDataArray { data = containerData });


            if (onceLoadData!=null)
            {
                PlayerPrefs.SetString("SkillContainerData", onceLoadData);
                PlayerPrefs.Save();
                return;
            }
            Debug.Log(jsonData);

            PlayerPrefs.SetString("SkillContainerData", jsonData);
            PlayerPrefs.Save();
            string jsonData2 = PlayerPrefs.GetString("SkillContainerData", "");
             
        }
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

    public string onceLoadData ;
    public void LoadContainerState()
    {
        string jsonData = PlayerPrefs.GetString("SkillContainerData", "");
        onceLoadData = jsonData;
        Debug.Log(jsonData);
        if (!string.IsNullOrEmpty(jsonData))
        {
            SkillContainerDataArray containerDataArray = JsonUtility.FromJson<SkillContainerDataArray>(jsonData);
            Debug.Log(containerDataArray);

            for (int i = 0; i < 4; i++)
            {
                DestroyObjectsWithNameInContainers("skillPanel(Clone)");

                // skillContainers[i] = new GameObject("SkillContainer" + (i + 1));
            }
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
                            if (skill != null && PlayerController.instance != null)
                            {
                                if (!PlayerController.instance.skillCanAttack.ContainsKey(skill.name))
                                {
                                    PlayerController.instance.skillCanAttack.Add(skill.name, true);
                                }
                                else
                                {
                                    Debug.LogWarning($"Skill {skill.name} already exists in skillCanAttack dictionary");
                                }

                                if (!PlayerController.instance.skillCanAttackIsStart.ContainsKey(skill.name))
                                {
                                    PlayerController.instance.skillCanAttackIsStart.Add(skill.name, false);
                                }
                                else
                                {
                                    Debug.LogWarning($"Skill {skill.name} already exists in skillCanAttackIsStart dictionary");
                                }
                            }
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
                if (skillPanel != null)  
                {
                    var skillImage = skillPanel.Find("SkillImage");
                    if (skillImage != null)
                    {
                        var text = skillImage.Find("name").GetComponent<Text>();  
                        if (text != null && text.text == skill.name)
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
        foreach (GameObject container in skillContainers)
        {
            if (container != null)
            {
                Transform skillPanelTransform = container.transform.Find("skillPanel(Clone)/SkillImage/name");
                if (skillPanelTransform != null)
                {
                    Text textComponent = skillPanelTransform.gameObject.GetComponent<Text>();
                    if (textComponent != null)
                    {

                        string text = textComponent.text;
                        if ((container.transform.childCount == 0 || string.IsNullOrEmpty(text)) && !IsSkillAlreadyAdded(skill))
                        {
                            InstantiateSkillInContainer(skill, container);
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Text component not found in {skillPanelTransform.gameObject.name}");
                    }
                }
                else
                {
                    InstantiateSkillInContainer(skill, container);
                    SetActiveRecursively(container, true);
                
                    if (PlayerController.instance != null)
                    {

                        PlayerController.instance.equippedSkills.Add(skill);
                        PlayerController.instance.UpdateEquippedSkills();
                    }
                    else
                    {
                        Debug.LogWarning("PlayerController.instance is null");
                    }
                    break;
                }
            }
            else
            {
                Debug.LogWarning("Skill container is null");
            }
        }
    }


    public List<string> trainingSkillName = new List<string>(4);

    void InstantiateSkillInContainer(Skill skill, GameObject container)
    {

        // if (skill.icon == null || skill.icon.texture == null) {
        // }
        GameObject skillObj = Instantiate(skillPanelPrefab, container.transform);
        RectTransform rectTransform = skillObj.GetComponent<RectTransform>();
        rectTransform.SetParent(container.transform, false);

        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
        rectTransform.pivot = new Vector2(1, 0);

        rectTransform.anchoredPosition = new Vector2(-10, 10);  
        Image image = skillObj.transform.Find("SkillImage").GetComponent<Image>();
        if (image != null)
        {
            image.sprite = skill.icon;
        }

        skillObj.transform.Find("SkillImage/name").GetComponent<Text>().text = skill.name;
        // skillObj.transform.Find("SkillImage").GetComponent<Image>().sprite = skill.icon;
        GameObject skillImage = skillObj.transform.Find("SkillImage").gameObject;
        skillImage.SetActive(true); // Activate the SkillImage if it was inactive

        if (!skillImage.activeSelf)
        {
            skillImage.SetActive(true); // Activate the SkillImage if it was inactive
        }

        if (!(GameManager.instance.curStatus == Status.Training))
        {
            
            SaveContainerState(); // Save state after clearing
            return;
        }
        PlayerController.instance.DelayedUpdateEquippedSkills(1f);
        trainingSkillName.Add(skill.name);
        // Optionally, add a click event to remove from container or other functionality
        // DestroyObjectsWithNameInContainers("skillPanel(Clone)");
        // LoadContainerState();
    }
    public void DestroyObjectsWithNameInContainers(string objectName)
    {
        foreach (GameObject container in skillContainers)
        {
            if (container != null)
            {
                Transform[] allChildren = container.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child.gameObject.name == objectName)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            else
            {
                Debug.LogWarning("One of the skill containers is null");
            }
        }
    }
    void ClearSkillContainers()
    {
        // foreach (GameObject container in skillContainers)
        // {
        //     foreach (Transform child in container.transform)
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }
        DestroyObjectsWithNameInContainers("skillPanel(Clone)");
        SaveContainerState();

        // for (int i = 0; i < skillContainers.Length; i++)
        // {
        //     if (skillContainers[i]!=null)
        //     {
        //         skillContainers[i] = new GameObject("SkillContainer" + (i + 1));
        //     }
        // }
        // SaveContainerState(); // Save state after clearing
        // // SkillUI.instance.LoadContainerState();
        // LoadContainerState();
    }void ClearSkillContainersTrain()
    {
        // foreach (GameObject container in skillContainers)
        // {
        //     foreach (Transform child in container.transform)
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }
        DestroyObjectsWithNameInContainers("skillPanel(Clone)");
        PlayerController.instance.DelayedUpdateEquippedSkills(1f);

        // for (int i = 0; i < skillContainers.Length; i++)
        // {
        //     if (skillContainers[i]!=null)
        //     {
        //         skillContainers[i] = new GameObject("SkillContainer" + (i + 1));
        //     }
        // }
        // SaveContainerState(); // Save state after clearing
        // // SkillUI.instance.LoadContainerState();
        // LoadContainerState();
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