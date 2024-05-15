// SkillManager.cs

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SkillFunctions : MonoBehaviour
{
    
    // Variables for charging
    private bool isCharging = false;
    public bool isChargingAttack = false;
    public float chargeProgress = 0f;
    public ParticleSystem chargingParticles;
    public ParticleSystem shieldParticles;
    public ParticleSystem laserParticles;
    public bool startCharging=false;
    
    public float laserDuration = 1f; // 激光持续时间
    public float laserDamage = 10f; // 激光伤害

    private bool isFiring = false; // 是否正在发射激光
    private float currentDuration = 0f;
    
    public static SkillFunctions instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(DelayedStartCharging(1.0f));
        chargeProgress = 0f;

    }

    IEnumerator DelayedStartCharging(float delay)
    {
        yield return new WaitForSeconds(delay);
        startCharging = true;
        if (!isChargingAttack)
        {
            chargeProgress = 0f;
            chargingParticles.Stop();
            chargingParticles.Clear();
        }
    }


    void Update()
    {
        if (isCharging&&(GameManager.instance.curStatus == Status.Game||GameManager.instance.curStatus == Status.Training)&&startCharging)
        {
            UpdateChargeProgress();
        }
        PlayerController.instance.HandleSkillSelection();

        
    }
    void UpdateChargeProgress()
    {
        Debug.Log("更新chargePrograss");
        chargeProgress += Time.deltaTime;
        // Adjust particle emission rate based on charge progress
        var emission = chargingParticles.emission;
        emission.rateOverTime = chargeProgress * 10; // Adjust multiplier as needed
    }

    // Method to start charging when the left mouse button is pressed
    public void StartCharging()
    {
        Debug.Log("开始蓄力");
        isChargingAttack = false;
        isCharging = true;
      
        // Start the particle system
        chargingParticles.Play();
    }

    // Method to release the charged attack when the left mouse button is released
    public void ReleaseChargedAttack()
    {
        isCharging = false;
       
        // Release the attack with appropriate effects based on charge progress
        Debug.Log("Charged attack released with progress: " + chargeProgress);
        if (chargeProgress > 1)
        {
        int att_value = Random.Range(0, 100);
        PlayerController.instance.WeaponAS.Play();
        if (att_value <= PlayerController.instance.playerData.CriticalRate)
        {
            PlayerController.instance.isCritical = true;
        }
        else
        {
            PlayerController.instance.isCritical = false;
        }
        PlayerController.instance.animator.SetBool("Critical", PlayerController.instance.isCritical);
        PlayerController.instance.animator.SetTrigger("Attack");
        
            isChargingAttack = true;
        }
        chargeProgress = 0f;
        chargingParticles.Stop();
        chargingParticles.Clear();
    }

    public void HandleSkillUsage(Skill currentSkill)
    {
        if (!PlayerController.instance.skillCanAttack.ContainsKey(currentSkill.name))
        {
            PlayerController.instance.skillCanAttack.Add(currentSkill.name, true);
        }
        if (!PlayerController.instance.skillCanAttackIsStart.ContainsKey(currentSkill.name))
        {
            PlayerController.instance.skillCanAttackIsStart.Add(currentSkill.name, false);
        }
        if (currentSkill.name == "attack")
        {
            if (Input.GetMouseButtonDown(0) && PlayerController.instance.skillCanAttack[currentSkill.name])
            {
                StartCharging();
                Debug.Log("使用技能: " + currentSkill.name);
                PlayerController.instance.skillCanAttack[currentSkill.name] = false;
                int att_value = Random.Range(0, 100);
                PlayerController.instance.WeaponAS.Play();
                if (att_value <= PlayerController.instance.playerData.CriticalRate)
                {
                    PlayerController.instance.isCritical = true;
                }
                else
                {
                    PlayerController.instance.isCritical = false;
                }

                PlayerController.instance.animator.SetBool("Critical", PlayerController.instance.isCritical);
                PlayerController.instance.animator.SetTrigger("Attack");

                //
               
            }
            else if (Input.GetMouseButtonUp(0) && isCharging)
            {
                ReleaseChargedAttack();
            }
            // chargeProgress = 0f;
            // chargingParticles.Stop();
            // chargingParticles.Clear();
        }

        if (currentSkill.name == "Shield awarded")
        {
            if (Input.GetMouseButtonDown(0) && PlayerController.instance.skillCanAttack[currentSkill.name])
            {
                if (PlayerController.instance.curSP >= 50) // 检查是否有足够的法力值
                {
                    // 扣除法力值
                    PlayerController.instance.curSP -= 50;

                    // 给玩家套上护盾
                    PlayerController.instance.playerData.ShieldValue = 50;
                    Debug.Log("增加护盾，护盾值: "+PlayerController.instance.playerData.ShieldValue);
                    // UIManager.instance.shield.text = PlayerController.instance.playerData.ShieldValue.ToString();
                    UIManager.instance.Sheildbar.fillAmount = PlayerController.instance.playerData.ShieldValue / 50;
                    UIManager.instance.ShieldValue.text = PlayerController.instance.playerData.ShieldValue.ToString()+ " / " + 50;
                    // 生成环绕用户的特效
                    if (shieldParticles != null)
                    {
                        shieldParticles.Play();
                        ChargingParticlesController.instance.PlayParticles();
                    }
                    
                    Debug.Log("使用技能: " + currentSkill.name);
                   
                    PlayerController.instance.skillCanAttack[currentSkill.name] = false;
                }
                else
                {
                    Debug.Log("法力值不足，无法释放护盾技能。");
                }
            }
        }
        if (currentSkill.name == "Add maximum health")
        {
            if (Input.GetMouseButtonDown(0) && PlayerController.instance.skillCanAttack[currentSkill.name])
            {
                if (PlayerController.instance.curSP >= 70) // 检查是否有足够的法力值
                {
                    // 扣除法力值
                    PlayerController.instance.curSP -= 70;

                    // 给玩家套上护盾
                    PlayerController.instance.playerData.MaxHealth +=20;
                    PlayerController.instance.playerData.max_health +=20;
                    PlayerController.instance.playerData.current_health +=20;
                    Debug.Log("增加血量，血量值: "+ PlayerController.instance.playerData.MaxHealth);
                    // UIManager.instance.Sheildbar.fillAmount = PlayerController.instance.playerData.ShieldValue / 50;
                    // UIManager.instance.ShieldValue.text = PlayerController.instance.playerData.ShieldValue.ToString()+ " / " + 50;
                    // 生成环绕用户的特效
                    Debug.Log("使用技能: " + currentSkill.name);
                   
                    PlayerController.instance.skillCanAttack[currentSkill.name] = false;
                }
                else
                {
                    Debug.Log("法力值不足，无法释放护盾技能。");
                }
            }
        }
        if (currentSkill.name == "Atk Up")
        {
            if (Input.GetMouseButtonDown(0) && PlayerController.instance.skillCanAttack[currentSkill.name])
            {
                if (PlayerController.instance.curSP >= 40) // 检查是否有足够的法力值
                {
                    // 扣除法力值
                    PlayerController.instance.curSP -= 40;

                    // 给玩家套上护盾
                    PlayerController.instance.playerData.attack +=50;
                    Debug.Log("增加攻击，攻击值: "+ PlayerController.instance.playerData.attack);
                    // UIManager.instance.Sheildbar.fillAmount = PlayerController.instance.playerData.ShieldValue / 50;
                    // UIManager.instance.ShieldValue.text = PlayerController.instance.playerData.ShieldValue.ToString()+ " / " + 50;
                    // 生成环绕用户的特效
                    Debug.Log("使用技能: " + currentSkill.name);
                   
                    PlayerController.instance.skillCanAttack[currentSkill.name] = false;
                }
                else
                {
                    Debug.Log("法力值不足，无法释放攻击                                                      技能。");
                }
            }
        } 
        if (currentSkill.name == "lasers")
        {
            if (Input.GetMouseButtonDown(0) && PlayerController.instance.skillCanAttack[currentSkill.name])
{
    if (PlayerController.instance.curSP >= 40) // 检查是否有足够的法力值
    {
        // 扣除法力值
        PlayerController.instance.curSP -= 40;
        Debug.Log("使用技能: " + currentSkill.name);
        PlayerController.instance.skillCanAttack[currentSkill.name] = false;

        // 发射射线，获取鼠标点击的位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, PlayerController.instance.GroundLayer))
        {
            Vector3 targetPosition = hit.point + new Vector3(0, 0.3f, 0);
            Vector3 direction = targetPosition - PlayerController.instance.transform.position;

            // 实例化激光预制体并设置位置和方向
            GameObject laser = Instantiate(PlayerController.instance.laserPrefab, PlayerController.instance.transform.position + new Vector3(5, 5, 5), Quaternion.identity);
            laser.transform.localScale = new Vector3(4, 4, 4);
            laser.transform.LookAt(targetPosition);

            // 获取激光预制体上的 LineRenderer 组件
            LineRenderer lineRenderer = laser.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, PlayerController.instance.transform.position);
                lineRenderer.SetPosition(1, targetPosition);
            }

            else
            {
                Debug.Log("没有射击到任何目标");
            }

            Destroy(laser, 1.0f); // 1秒后销毁激光
        }
        else
        {
            Debug.Log("没有点击到地面");
        }
    }
    else
    {
        Debug.Log("法力值不足，无法释放攻击技能。");
    }
}
        }
        if (currentSkill.name == "bleed")
        {
            if (Input.GetMouseButtonDown(0) && PlayerController.instance.skillCanAttack[currentSkill.name])
            {
                if (PlayerController.instance.curSP >= 40) // 检查是否有足够的法力值
                {
                    // 扣除法力值
                    PlayerController.instance.curSP -= 40;
                    Debug.Log("使用技能: " + currentSkill.name);
                    PlayerController.instance.skillCanAttack[currentSkill.name] = false;

                    // 发射射线，获取鼠标点击的位置
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100f, PlayerController.instance.GroundLayer))
                    {
                        // 实例化bleed预制体并设置位置
                        GameObject bleedEffect = Instantiate(PlayerController.instance.bleedPrefab, hit.point + Vector3.down * 5f, Quaternion.identity);

                        // 开始协程来控制动画和消失
                        StartCoroutine(AnimateBleedEffect(bleedEffect));
                    }
                    else
                    {
                        Debug.Log("没有点击到地面");
                    }
                }
                else
                {
                    Debug.Log("法力值不足，无法释放攻击技能。");
                }
            }
        }

        if (currentSkill.name == "bomb")
        {
            if (Input.GetMouseButtonDown(0) && PlayerController.instance.skillCanAttack[currentSkill.name])
            {
                if (PlayerController.instance.curSP >= 40) // Check if there's enough mana
                {
                    // Deduct mana
                    PlayerController.instance.curSP -= 40;
                    Debug.Log("Using skill: " + currentSkill.name);
                    PlayerController.instance.skillCanAttack[currentSkill.name] = false;

                    // Cast a ray from the camera to the mouse position
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100f, PlayerController.instance.GroundLayer))
                    {
                        // Instantiate the bomb prefab at the hit point
                        GameObject bomb = Instantiate(PlayerController.instance.bombPrefab,
                            hit.point + new Vector3(0, 0.3f, 0), Quaternion.identity);
                        bombs.Add(bomb); // Add the bomb to the list for tracking
                        bomb.transform.localPosition +=
                            new Vector3(0, 0.3f, 0); // Adjust the position slightly above the ground


                        Debug.Log("Bomb placed at: " + hit.point);
                    }
                    else
                    {
                        Debug.Log("No ground clicked, cannot place bomb.");
                    }
                }
                else
                {
                    Debug.Log("Insufficient mana to cast skill.");
                }
            }
        }

        if (currentSkill.name == "Sword throwing")
        {
            if (Input.GetMouseButtonDown(0) && PlayerController.instance.skillCanAttack[currentSkill.name])
            {
                if (PlayerController.instance.curSP >= 40) // Check if there's enough mana
                {
                    PlayerController.instance.curSP -= 40; // Deduct mana
                    Debug.Log("Using skill: " + currentSkill.name);
                    PlayerController.instance.skillCanAttack[currentSkill.name] = false;

                    // Cast a ray from the camera to the mouse position to determine the direction
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    Vector3 targetPosition;
                    if (Physics.Raycast(ray, out hit, 100f, PlayerController.instance.GroundLayer))
                    {
                        targetPosition = new Vector3(hit.point.x, PlayerController.instance.transform.position.y,
                            hit.point.z); // Adjust height to player's y
                    }
                    else
                    {
                        // If no ground was hit, simply use a point in the direction of the ray
                        Vector3 horizontalPoint = ray.origin + ray.direction * 100f;
                        targetPosition = new Vector3(horizontalPoint.x, PlayerController.instance.transform.position.y,
                            horizontalPoint.z); // Keep it at player's height
                    }

                    // Calculate initial position in front of the player to avoid pushing
                    Vector3 initialPosition = PlayerController.instance.transform.position +
                                              PlayerController.instance.transform.forward * 2;

                    // Instantiate the sword prefab at a position in front of the player
                    GameObject sword = Instantiate(PlayerController.instance.swordPrefab, initialPosition,
                        Quaternion.identity);
                    sword.transform.localScale = new Vector3(5f, 5f, 5f);
                    sword.transform.LookAt(targetPosition);

                    // Optionally add a Rigidbody and apply a force to "throw" the sword
                    Rigidbody rb = sword.AddComponent<Rigidbody>();
                    rb.useGravity = false; // Assume you want the sword to fly straight
                    Vector3 direction = (targetPosition - sword.transform.position).normalized;
                    rb.AddForce(direction * 500f); // Adjust force as necessary

                    // Destroy the sword after some time to clean up
                    Destroy(sword, 1.0f);
                }
                else
                {
                    Debug.Log("Insufficient mana to cast the skill.");
                }
            }

        }
    }
    public List<GameObject> bombs = new List<GameObject>();
    IEnumerator AnimateBleedEffect(GameObject bleedEffect)
    {
        float elapsedTime = 0f;
        float duration = 1f; // 动画持续时间

        // 慢慢上升到地面
        while (elapsedTime < duration)
        {
            bleedEffect.transform.position = Vector3.Lerp(bleedEffect.transform.position+ Vector3.up * 0.15f, bleedEffect.transform.position + Vector3.up * 0.055f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终位置精确
        bleedEffect.transform.position = bleedEffect.transform.position + Vector3.up * 0.1f;

        // 延迟销毁
        yield return new WaitForSeconds(0.6f);
        Destroy(bleedEffect);
    }
    private IEnumerator CoolDownCoroutine(string currentSkillName, float attackCD)
    {
        int index = PlayerController.instance.selectedSkillIndex;
        PlayerController.instance.skillCanAttackIsStart[currentSkillName]= true;
        float cooldownTime = 0f;
        PlayerController.instance.skillCanAttack[currentSkillName] = false;

        while (cooldownTime < attackCD)
        {
            cooldownTime += Time.deltaTime;
            float cooldownRatio = cooldownTime / attackCD;
            SkillUI.instance.skillContainers[index].transform
                .Find("skillPanel(Clone)/SkillImage/cover").GetComponent<Image>().fillAmount = 1 - cooldownRatio;

            yield return null;
        }
        PlayerController.instance.skillCanAttack[currentSkillName] = true;
        // PlayerController.instance.SetSkillCooldown(currentSkillName, 0);

        SkillUI.instance.skillContainers[PlayerController.instance.selectedSkillIndex].transform
            .Find("skillPanel(Clone)/SkillImage/cover").GetComponent<Image>().fillAmount = 0; // 完全填充表示冷却完成
        Debug.Log("!END:  "+currentSkillName);
        PlayerController.instance.skillCanAttackIsStart[currentSkillName]= false;


    }

    public void AttackCoolTime(string currentSkillName, float attackCD)
    {
        StartCoroutine(CoolDownCoroutine(currentSkillName, attackCD));
    }
}