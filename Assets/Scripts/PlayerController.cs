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
    private Dictionary<string, float> skillCooldowns = new Dictionary<string, float>();
    public bool canAttack = true;

    //Player status
    bool isRun;
    public bool isCritical;
    public bool isAttacking = false;

    public bool isInvincible = false;


    public GameObject indicatorPrefab;
    public GameObject laserPrefab;
    public GameObject bleedPrefab;
    public GameObject bombPrefab;
    public GameObject swordPrefab;

    private GameObject currentIndicator;
    private Vector3 targetPosition;
    private bool movingToTarget = false;

    public PlayerData playerData;


    public float curSP;

    public AudioSource WeaponAS;
    public ParticleSystem ps;

    public Skill currentSkill;
    public int selectedSkillIndex = 0;
    public List<Skill> equippedSkills = new List<Skill>();
    public float chargeTime = 2f;
    private float currentChargeTime = 0f;

    public Dictionary<string, bool> skillCanAttack = new Dictionary<string, bool>(5);
    public Dictionary<string, bool> skillCanAttackIsStart = new Dictionary<string, bool>(5);
    
    // 设置技能冷却时间
    public void SetSkillCooldown(string skillName, float cooldownTime)
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

    // 获取技能冷却时间
    public float GetSkillCooldown(string skillName)
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
        if (SkillUI.instance.skillContainers[0].transform.Find("skillPanel(Clone)/SkillImage/name")!=null)
        {
         
        var text = SkillUI.instance.skillContainers[0].transform.Find("skillPanel(Clone)/SkillImage/name")
            .GetComponent<Text>();
        while (!flag)
        {
            text = SkillUI.instance.skillContainers[0].transform.Find("skillPanel(Clone)/SkillImage/name")
                .GetComponent<Text>();
            if (text != null)
            {
                flag = true;
            }
        }

        foreach (GameObject container in SkillUI.instance.skillContainers)
        {
            var find = container.transform.Find("skillPanel(Clone)/SkillImage/name");
            if (find != null)
            {
                var subtext = find.GetComponent<Text>();
                Debug.Log("!!!2 " + subtext.text);
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
            currentSkill = equippedSkills[0]; // 默认选中第一个技能
           
        }
    }

    public void HandleSkillSelection()
    {
        if (GameManager.instance.curStatus == Status.Game)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && equippedSkills.Count > 0)
            {
                setImageActive(0);
                Debug.Log("设置为1");
                currentSkill = equippedSkills[0];
                selectedSkillIndex = 0;

                Debug.Log(equippedSkills[0].name);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && equippedSkills.Count > 1)
            {
                setImageActive(1);

                Debug.Log("设置为2");
                currentSkill = equippedSkills[1];
                selectedSkillIndex = 1;

                Debug.Log(equippedSkills[1].name);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && equippedSkills.Count > 2)
            {
                setImageActive(2);

                Debug.Log("设置为3");
                currentSkill = equippedSkills[2];
                selectedSkillIndex = 2;

                Debug.Log(equippedSkills[2].name);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4) && equippedSkills.Count > 3)
            {
                setImageActive(3);

                Debug.Log("设置为4");
                currentSkill = equippedSkills[3];
                selectedSkillIndex = 3;

                Debug.Log(equippedSkills[3].name);
            }
        }else if (GameManager.instance.curStatus == Status.Training)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) )
            {
                setImageActive(0);
                Debug.Log("设置为1");
                currentSkill = equippedSkills[0];
                selectedSkillIndex = 0;

                Debug.Log(equippedSkills[0].name);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                setImageActive(1);

                Debug.Log("设置为2");
                currentSkill = equippedSkills[1];
                selectedSkillIndex = 1;

                Debug.Log(equippedSkills[1].name);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                setImageActive(2);

                Debug.Log("设置为3");
                currentSkill = equippedSkills[2];
                selectedSkillIndex = 2;

                Debug.Log(equippedSkills[2].name);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4) )
            {
                setImageActive(3);

                Debug.Log("设置为4");
                currentSkill = equippedSkills[3];
                selectedSkillIndex = 3;

                Debug.Log(equippedSkills[3].name);
            }
        }
        
    }

    void setImageActive(int index)
    {
        clearImageActive();
        Debug.Log("开始设置");
        var imageGameObject = SkillUI.instance.skillContainers[index].transform
            .Find("skillPanel(Clone)/SkillImage/Image").gameObject;
        if (imageGameObject != null)
        {
            imageGameObject.SetActive(true); // 激活 GameObject
        }
        else
        {
            Debug.LogError("Image GameObject not found.");
        }
    }

    void clearImageActive()
    {
        for (int i = 0; i < 4; i++)
        {
            if (SkillUI.instance.skillContainers[i].transform.Find("skillPanel(Clone)/SkillImage/Image") != null)
            {
                var imageGameObject = SkillUI.instance.skillContainers[i].transform
                    .Find("skillPanel(Clone)/SkillImage/Image").gameObject;
                if (imageGameObject != null)
                {
                    imageGameObject.SetActive(false); // 激活 GameObject
                }
                else
                {
                    Debug.LogError("Image GameObject not found.");
                }
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }

    IEnumerator SPCheck()
    {
        while (true && (GameManager.instance.curStatus == Status.Game||GameManager.instance.curStatus == Status.Training))
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
        if ((GameManager.instance.curStatus == Status.Game||GameManager.instance.curStatus == Status.Training))
        {
            HandleSkillSelection();
            SkillFunctions.instance.HandleSkillUsage(currentSkill);

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

            if (playerData.ShieldValue <= 0)
            {
                // 停止护盾特效
                if (SkillFunctions.instance.shieldParticles != null)
                {
                    SkillFunctions.instance.shieldParticles.Stop();
                }
            }

            var skillCanAttackCopy = new Dictionary<string, bool>(skillCanAttack);
            var skillCanAttackIsStartCopy = new Dictionary<string, bool>(skillCanAttackIsStart);
          
            foreach (var canAttack in skillCanAttackCopy)
            {

                if (!canAttack.Value&& !skillCanAttackIsStartCopy[canAttack.Key])
                {
                    Debug.Log("!SAD:  "+canAttack.Key);
                    Debug.Log("!SAD:  "+canAttack.Value);
                    // skillCanAttack.Add(canAttack.Key+"isStart",true);

                    switch (canAttack.Key)
                    {
                        
                        case "Shield awarded":
                            SkillFunctions.instance.AttackCoolTime(canAttack.Key, attackCD + 5);
                            break;
                        case "Add maximum health":
                            SkillFunctions.instance.AttackCoolTime(canAttack.Key, attackCD + 7);
                            break;
                        case "Atk Up":
                            SkillFunctions.instance.AttackCoolTime(canAttack.Key, attackCD + 3);
                            break;
                        case "lasers":
                            SkillFunctions.instance.AttackCoolTime(canAttack.Key, attackCD + 9);
                            break; 
                        case "bleed":
                            SkillFunctions.instance.AttackCoolTime(canAttack.Key, attackCD + 5);
                            break; 
                        case "bomb":
                            SkillFunctions.instance.AttackCoolTime(canAttack.Key, attackCD + 3);
                            break;
                        case "Sword throwing":
                            SkillFunctions.instance.AttackCoolTime(canAttack.Key, attackCD );
                            break;
                        default:
                            SkillFunctions.instance.AttackCoolTime(canAttack.Key, attackCD);
                            break;
                    }
                }
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
        if (other.tag == "bossAttack"  )
        {
            Debug.Log("激光攻击");
            GetHurt(2);
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
        if (playerData.ShieldValue > 0)
        {
            playerData.ShieldValue -= damage;
            UIManager.instance.Sheildbar.fillAmount = PlayerController.instance.playerData.ShieldValue / 50;
            UIManager.instance.ShieldValue.text =
                PlayerController.instance.playerData.ShieldValue.ToString() + " / " + 50;

            if (playerData.ShieldValue < 0)
            {
                playerData.ShieldValue = 0;
                UIManager.instance.Sheildbar.fillAmount = PlayerController.instance.playerData.ShieldValue / 50;
                UIManager.instance.ShieldValue.text =
                    PlayerController.instance.playerData.ShieldValue.ToString() + " / " + 50;
                ChargingParticlesController.instance.destory();
            }
        }
        else
        {
            playerData.CurHealth -= damage;
            isInvincible = true;
        }
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