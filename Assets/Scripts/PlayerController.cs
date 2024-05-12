using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : PlayerMovement
{
    public static PlayerController instance;

    public LayerMask GroundLayer;

    public Animator animator;

    public float invincibleTime;
    float check_iv_time = 0;
    public float attackCD;
    float check_att_time = 0;
    bool canAttack = true;

    //Player status
    bool isRun;
    public bool isCritical;
    public bool isAttacking = false;

    public bool isInvincible = false;


    public GameObject indicatorPrefab;
    private GameObject currentIndicator;
    private Vector3 targetPosition;
    private bool movingToTarget = false;

    public PlayerData playerData;


    public float curSP;

    public AudioSource WeaponAS;
    public ParticleSystem ps;

    public Skill currentSkill;
    private int selectedSkillIndex = 0;
    private List<Skill> equippedSkills = new List<Skill>();

    
    void Start()
    {
        if (GameManager.instance.isNewGame)
        {
            SetData(DefaultPlayerData());
        }

        animator = GetComponent<Animator>();


        StartCoroutine(SPCheck());
        
        StartCoroutine(DelayedUpdateEquippedSkills(1.0f)); 
        Debug.Log("初始化");
    }
    IEnumerator DelayedUpdateEquippedSkills(float delay)
    {
        yield return new WaitForSeconds(delay); // 等待指定的延迟时间
        UpdateEquippedSkills(); // 执行 UpdateEquippedSkills 方法
    }
 
    void UpdateEquippedSkills()
    {
        equippedSkills.Clear();
        var flag = false;
        var text = SkillUI.instance.skillContainers[0].transform.Find("skillPanel(Clone)/SkillImage/name").GetComponent<Text>();
        while (!flag){
            text =  SkillUI.instance.skillContainers[0].transform.Find("skillPanel(Clone)/SkillImage/name").GetComponent<Text>();
            if (text != null)
            {
                flag = true;
            }
        }
        foreach (GameObject container in SkillUI.instance.skillContainers)
        {
            var find = container.transform.Find("skillPanel(Clone)/SkillImage/name");
            if (find!=null)
            {
                var subtext = find.GetComponent<Text>();
                Debug.Log("!!!2 "+subtext.text);
                if (subtext != null)
                {
                    Skill skill = SkillManager.instance.GetSkillByName(subtext.text);
                    if (skill != null)
                    {
                        equippedSkills.Add(skill);
                    }
                }
            }
            else
            {
                return;
            }
            
        }
        if (equippedSkills.Count > 0)
            currentSkill = equippedSkills[0];  // 默认选中第一个技能
    }

    void HandleSkillSelection()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1) && equippedSkills.Count > 0)
        {
            setImageActive(0);
            Debug.Log("设置为1");
            currentSkill = equippedSkills[0];
            Debug.Log(equippedSkills[0].name);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && equippedSkills.Count > 1)
        {
            setImageActive(1);

            Debug.Log("设置为2");
            currentSkill = equippedSkills[1];
            Debug.Log(equippedSkills[1].name);

        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && equippedSkills.Count > 2)
        {
            setImageActive(2);

            Debug.Log("设置为3");
            currentSkill = equippedSkills[2];
            Debug.Log(equippedSkills[2].name);

        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && equippedSkills.Count > 3)
        {
            setImageActive(3);

            Debug.Log("设置为4");
            currentSkill = equippedSkills[3];
            Debug.Log(equippedSkills[3].name);
        }
    }

    void setImageActive(int index)
    {
        clearImageActive();
        Debug.Log("开始设置");
        var imageGameObject = SkillUI.instance.skillContainers[index].transform.Find("skillPanel(Clone)/SkillImage/Image").gameObject;
        if (imageGameObject != null) {
            imageGameObject.SetActive(true);  // 激活 GameObject
        } else {
            Debug.LogError("Image GameObject not found.");
        }
    }

    void clearImageActive()
    {
        for(int i =0;i<4;i++)
        {
            var imageGameObject = SkillUI.instance.skillContainers[i].transform.Find("skillPanel(Clone)/SkillImage/Image").gameObject;
            if (imageGameObject != null) {
                imageGameObject.SetActive(false);  // 激活 GameObject
            } else {
                Debug.LogError("Image GameObject not found.");
            }
        }
    }

    void HandleSkillUsage()
    {
        if (currentSkill.name == "attack")
        {
            if (Input.GetMouseButtonDown(0) && canAttack)
            {
                Debug.Log("使用技能: " + currentSkill.name);
                canAttack = false;
                int att_value = Random.Range(0, 100);
                WeaponAS.Play();
                if (att_value <= playerData.CriticalRate)
                {
                    isCritical = true;
                }
                else
                {
                    isCritical = false;
                }

                animator.SetBool("Critical", isCritical);
                animator.SetTrigger("Attack");
                
                GameObject skillContainer = SkillUI.instance.skillContainers[0]; // 假设攻击技能容器的下标为0
                // AddGlowEffect(skillContainer);
            }
            // 这里添加实际的技能使用代码，例如播放动画、生成效果等
        }
    }
    
    private void Awake()
    {
        instance = this;
    }

    IEnumerator SPCheck()
    {
        while (true && GameManager.instance.curStatus == Status.Game)
        {
            if (isRun)
            {
                if (curSP > 0)
                {
                    curSP -= 10;
                }
            }
            else
            {
                if (curSP < 100)
                {
                    curSP += 10;
                }
            }

            yield return new WaitForSeconds(1);
        }
    }



    PlayerData DefaultPlayerData()
    {
        PlayerData pData = new PlayerData();
        pData.Location = GameManager.instance.playerInitPosition;
        pData.MaxHealth = 100;
        pData.CurHealth = pData.MaxHealth;
        pData.Level = 1;
        pData.MaxSp = 100;
        pData.Attack = 30;
        pData.CriticalRate = 30;
        pData.JumpTime = 1;
        pData.MoveSpeed = 5;

        pData.CurExp = 0;
        pData.MaxExp = GameManager.instance.experienceToNextLevel[pData.Level - 1];
        return pData;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.curStatus == Status.Game)
        {
            HandleSkillSelection();
            HandleSkillUsage();
            
            bool isMove = MovePlayer(GroundLayer);
            animator.SetBool("Walk", isMove);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (curSP > 0)
                {
                    moveSpeed = 8;
                    isRun = true;
                    animator.SetBool("Run", isRun);
                }
                else
                {
                    moveSpeed = 5;
                    isRun = false;
                    animator.SetBool("Run", isRun);
                }
            }


            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                moveSpeed = 5;
                isRun = false;
                animator.SetBool("Run", isRun);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f, GroundLayer))
                {
                    targetPosition = hit.point + new Vector3(0, 0.3f, 0);
                    movingToTarget = true;
                    ShowIndicator(targetPosition);
                }
            }

            if (movingToTarget)
            {
                MoveToLocation();
            }


            

            if (isInvincible)
            {
                InvincibleTime();
            }

            if (!canAttack)
            {
                AttackCoolTime();
            }

            CheckDeath();
            CheckLevelUp();
            playerData.Location = transform.position;
        }
    }

    void MoveToLocation()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
        }
        else
        {
            movingToTarget = false;
            if (currentIndicator != null)
            {
                Destroy(currentIndicator);
            }
        }
    }

    void ShowIndicator(Vector3 position)
    {
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }

        currentIndicator = Instantiate(indicatorPrefab, position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SkillCoin"))
        {
            Debug.Log("Collected a skill coin.");
            SkillCoinManager.instance.CoinCollected();
        }
    }


    private void AttackCoolTime()
    {
        

        if (!canAttack)
        {
            check_att_time += Time.deltaTime;
            float cooldownRatio = check_att_time / attackCD;
            SkillUI.instance.skillContainers[selectedSkillIndex].transform.Find("skillPanel(Clone)/SkillImage/cover").GetComponent<Image>().fillAmount = 1 - cooldownRatio;

            if (check_att_time >= attackCD)
            {
                canAttack = true;
                check_att_time = 0;
                SkillUI.instance.skillContainers[selectedSkillIndex].transform.Find("skillPanel(Clone)/SkillImage/cover").GetComponent<Image>().fillAmount = 0; // 完全填充表示冷却完成
            }
        }
    }



    private void InvincibleTime()
    {
        check_iv_time += Time.deltaTime;
        if (check_iv_time >= invincibleTime)
        {
            isInvincible = false;
            check_iv_time = 0;
        }
    }

    private void CheckLevelUp()
    {
        if (playerData.CurExp >= playerData.MaxExp)
        {
            ps.Play();
            playerData.CurExp = 0;
            playerData.Level += 1;
            playerData.MaxExp = GameManager.instance.experienceToNextLevel[playerData.Level - 1];

            playerData.MaxHealth += 10;
            playerData.CurHealth = playerData.MaxHealth;

            playerData.Attack += 5;
        }
    }

    private void CheckDeath()
    {
        if (playerData.CurHealth <= 0)
        {
            animator.SetTrigger("Die");
            UIManager.instance.WINtext.enabled = false;
            UIManager.instance.LOSEText.enabled = true;
            GameManager.instance.GameOver();
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void GetHurt(int damage)
    {
        playerData.CurHealth -= damage;
        isInvincible = true;
    }

    public void GetExp(float exp)
    {
        playerData.CurExp += exp;
    }

    public void StartAttacking()
    {
        isAttacking = true;
    }

    public void EndAttacking()
    {
        isAttacking = false;
    }

    public void SetData(PlayerData data)
    {
        playerData = data;
        curSP = data.MaxSp;
        maxJumpTimes = data.JumpTime;
        transform.position = data.Location;
    }
}