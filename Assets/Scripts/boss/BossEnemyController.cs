using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossEnemyController : EnemyController
{
    public static BossEnemyController instance;
    public GameObject[] skillPrefabs;
    public GameObject circlePrefab;
    private LineRenderer lineRenderer;
    public float maxRadius = 5f;
    public float expandRate = 1f;
    private float radius = 0.0f;
    private bool isAttacking = false;
    private new GameObject healthBar;
    Vector3 centerPosition;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // agent.enabled = false; // 禁用NavMeshAgent以固定Boss位置
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };
        lineRenderer.positionCount = 0;

       

        
        GameObject centerBlock = GameObject.Find("MapBlock_20_1_20");
        if (centerBlock != null)
        {
            centerPosition = centerBlock.transform.position;
        }
        else
        {
            Debug.LogError("Center block 'MapBlock_20_1_20' not found. Using a default position.");
            centerPosition = new Vector3(20, 1, 20); // Default if center block not found
        }
        base.main_cam = Camera.main.transform;

        // 实例化血条
        healthBar = Instantiate(BarHolderPrefab, centerPosition + Vector3.up * 2 + new Vector3(0, 6, 0), Quaternion.identity, UIManager.instance.enemyBarsUI.transform);
        healthBar.transform.localScale = new Vector3(4, 4, 4); // Scale the boss to 5x5x5 units

        animator = GetComponent<Animator>();
        GetHitAS = GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        // 将血条位置设置为主摄像机朝向的方向
        if (Camera.main != null)
        {
            if (healthBar!=null)
            {
                healthBar.transform.LookAt(healthBar.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
                
            }
        }
    }

    private bool isEnd = true;

    // Update is called once per frame
    void Update()
    {
        if ((GameManager.instance.curStatus == Status.Game)&&isEnd)
        {
            isEnd = false;
            StartCoroutine(SkillDeploymentRoutine());
        }
        if ((GameManager.instance.curStatus == Status.Game||GameManager.instance.curStatus == Status.Training) && !isDead)
        {
            if (!isDead)
            {
                LookAtPlayer();
                if (isAttacking)
                {
                    ExpandCircle();
                }
            }

            if (base.healthBar != null)
            {
                base.healthBar.transform.position = BarPoint.position;
                base.healthBar.transform.LookAt(main_cam.position);
            }

            if (!PlayerController.instance.isAttacking)
            {
                isGetHit = false;
            }

            if (gameObject.GetComponent<NavMeshAgent>() == null)
            {
                gameObject.AddComponent<NavMeshAgent>();
            }

            agent = gameObject.GetComponent<NavMeshAgent>();


            // speed up
            timer += Time.deltaTime;
            if (timer > 6)
            {
                agent.speed += 0.5f;
                animator.SetFloat("Run", agent.speed);
                timer = 0;
            }

            CheckDeath();
            this.healthBar.transform.GetChild(0).GetComponent<Image>().fillAmount =
                enemyData.CurHealth / enemyData.MaxHealth;
            Debug.Log("设置血量:"+enemyData.CurHealth / enemyData.MaxHealth);
            Debug.Log(this.healthBar.transform.GetChild(0).GetComponent<Image>().fillAmount);
            enemyData.Location = transform.position;
            UIManager.instance.BossHealth.text = "BossHealth"+enemyData.CurHealth;
        }
        else if (isDead)
        {
            deadTimer += Time.deltaTime;
            if (deadTimer > 1)
            {
                Destroy(gameObject);
                Destroy(base.healthBar);
            }
        }
        
        if (isStatic && isBleed)
        {
            // 销毁staticPrefab和bleedPrefab
            if (staticPrefab != null)
            {
                Destroy(staticPrefab);
                staticPrefab = null;
            }

            if (bleedPrefab != null)
            {
                Destroy(bleedPrefab);
                bleedPrefab = null;
            }

            // 生成爆炸效果
            GameObject effect = new GameObject("ExplosionEffect");
            effect.transform.position = transform.position; // 假设是在当前对象的位置
            ParticleSystem particleSystem = effect.AddComponent<ParticleSystem>();
            ConfigureParticleEffect(particleSystem);
            Destroy(effect, 2.0f); // 2秒后销毁效果

            Debug.Log("元素反应！");
            // 调用GetDamage方法扣除Boss血量
            GetDamage(0, 10);

            // 重置状态
            isStatic = false;
            isBleed = false;
        }
    }
    void ConfigureParticleEffect(ParticleSystem particleSystem)
    {
        var main = particleSystem.main;
        main.startColor = Color.red; // 爆炸效果为红色
        main.startLifetime = 0.5f; // 生命周期
        main.startSpeed = 10f; // 爆炸的速度
        main.startSize = 2f; // 粒子大小

        var emission = particleSystem.emission;
        emission.rateOverTime = 100; // 每秒产生100个粒子

        particleSystem.Play(); // 开始播放粒子系统
    }
    void LookAtPlayer()
    {
        Vector3 direction = PlayerController.instance.transform.position - transform.position;
        direction.y = 0; // Keep the boss level
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }

    IEnumerator SkillDeploymentRoutine()
    {
        yield return new WaitForSeconds(1);
        DeploySkill(0);
        yield return new WaitForSeconds(1);
        DeploySkill(1);
        yield return new WaitForSeconds(1);
        DeploySkill(2);
        yield return new WaitForSeconds(1);
        DeploySkill(3);

        yield return new WaitForSeconds(5);
        isEnd = true;
        ;

        
    }

    void DeploySkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:
                new WaitForSeconds(Random.Range(1, 5));
                var range = Random.Range(0, 4);
                Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
                EnemyGenerator.instance.DeployEnemies(range, spawnPosition);
                break;
            case 1:
                StartCoroutine(ActivateShockwave(transform.position));
                break;
            case 2:
                if (GameManager.instance.difficulty==2)
                {
                    new WaitForSeconds(Random.Range(3, 5));
                    var range2 = Random.Range(0, 4);
                    Vector3 spawnPosition2 = transform.position + new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
                    EnemyGenerator.instance.DeployEnemies(range2, spawnPosition2);
                }
                break;
            case 3:
                if (GameManager.instance.difficulty == 2)
                {
                    Debug.Log("Boss使用技能: ");

                    // 获取Boss和玩家的位置
                    Vector3 bossPosition = this.transform.position;  // 假设这个脚本附加在Boss对象上
                    Vector3 playerPosition = GameManager.instance.Player.transform.position;

                    // 实例化激光预制体并设置位置和方向
                    GameObject laser = Instantiate(PlayerController.instance.laserPrefab, bossPosition, Quaternion.identity);
                    laser.tag = "bossAttack";
                    laser.transform.LookAt(playerPosition);  // 使激光朝向玩家

                    // 设置激光的比例和LineRenderer组件
                    laser.transform.localScale = new Vector3(4, 4, 4);
                    LineRenderer lineRenderer = laser.GetComponent<LineRenderer>();
                    if (lineRenderer != null)
                    {
                        lineRenderer.enabled = true;
                        lineRenderer.startWidth = 0.1f;  // 根据需要调整线的宽度
                        lineRenderer.endWidth = 0.1f;
                        lineRenderer.SetPositions(new Vector3[] { bossPosition, playerPosition });  // 设置激光的起始和结束位置
                    }

                    Destroy(laser, 1.0f); // 1秒后销毁激光
                }
                else
                {
                    Debug.Log("Boss技能未激活，因为难度不是2");
                }

                break;
            case 4:
                break;
        }
    }
    IEnumerator ActivateShockwave(Vector3 origin)
    {
        Debug.Log("释放!!");
        GameObject shockwave = new GameObject("Shockwave");
        // shockwave.layer = LayerMask.NameToLayer("Shockwave"); // 将冲击波设置为特殊层

        shockwave.transform.position = origin;
        shockwave.transform.rotation = Quaternion.Euler(0, 0, 0);
        shockwave.AddComponent<SphereCollider>().isTrigger = true;

        // 创建空心圆环
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.transform.parent = shockwave.transform;
        ring.transform.localPosition = Vector3.zero;
        ring.transform.localRotation = Quaternion.identity;
        ring.transform.localScale = new Vector3(0.1f, 0.05f, 0.1f); // 初始缩放比例

        GameObject hole = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hole.transform.parent = ring.transform;
        hole.transform.localPosition = Vector3.up * 0.025f; // 将球体移动到圆环上方
        hole.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f); // 调整球体大小

        Renderer ringRenderer = ring.GetComponent<Renderer>();
        ringRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
        ringRenderer.sharedMaterial.color = Color.red;
        ringRenderer.sharedMaterial.SetColor("_EmissionColor", Color.red);

        Renderer holeRenderer = hole.GetComponent<Renderer>();
        holeRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
        holeRenderer.sharedMaterial.color = Color.black; // 使用黑色材质来创建空心效果

        float maxScale = 3.0f;
        float speed = 5.0f;

        while (ring.transform.localScale.x < maxScale)
        {
            ring.transform.localScale += Vector3.one * speed * Time.deltaTime;
            hole.transform.localScale += Vector3.one * speed * Time.deltaTime; // 同步缩放球体
            yield return null;
        }

        Destroy(shockwave, 1.0f);
    }    void ExpandCircle()
    {
        radius += Time.deltaTime * expandRate;
        if (radius > maxRadius)
        {
            isAttacking = false;
            lineRenderer.positionCount = 0;
            return;
        }

        DrawCircle(radius);
        CheckPlayerCollision(radius);
    }

    void DrawCircle(float radius)
    {
        Debug.Log("画圈");
        int segments = 360;
        lineRenderer.positionCount = segments + 1;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * i;
            points[i] = new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius) + transform.position;
        }

        lineRenderer.SetPositions(points);
    }

    void CheckPlayerCollision(float currentRadius)
    {
        if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) <= currentRadius)
        {
            PlayerController.instance.GetHurt(enemyData.Attack); // Apply damage based on boss attack
            isAttacking = false; // Stop attack after hitting the player
            lineRenderer.positionCount = 0; // Clear the circle
        }
    }


    private void CheckDeath()
    {
        if (enemyData.CurHealth <= 0)
        {
            GameManager.instance.uiData.EnemyKillNum++;
            PlayerController.instance.GetExp(enemyData.Exp);
            animator.SetTrigger("Die");
            agent.speed = 0;
            isDead = true;
            //Destroy(gameObject);
            //Destroy(healthBar);
            GameManager.instance.GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon" && !isGetHit && PlayerController.instance.isAttacking)
        {
            if ( SkillFunctions.instance.isChargingAttack)
            {
                Debug.Log("蓄力攻击");

                GetHitAS.Play();
                GetDamage(0, 4);
                Debug.Log("boss掉血");
                isGetHit = true;
                SkillFunctions.instance.isChargingAttack = false;
            }
            else
            {
                Debug.Log("普通攻击");
                GetHitAS.Play();
                GetDamage();
                Debug.Log("boss掉血");

                isGetHit = true;
            }
           
        }
        if (other.tag == "laser"  )
        {
            Debug.Log("激光攻击");
            Debug.Log("boss血量: "+enemyData.CurHealth);

            GetHitAS.Play();
            GetDamage(0,0.05f);
            isGetHit = true;
            
            staticPrefab = Instantiate(base.staticPrefab, centerPosition+new Vector3(0,8,0), Quaternion.identity);
            staticPrefab.transform.localScale = new Vector3(3, 3, 3);
            isStatic = true;
    

            StartCoroutine(RotateAndDestroyObject(staticPrefab, 3f,"isStatic"));
        }
        if (other.tag == "bleed"  )
        {
            Debug.Log("aoe攻击");
            Debug.Log("boss血量: "+enemyData.CurHealth);
            GetHitAS.Play();
            GetDamage(0,0.1f);
            isGetHit = true;
            
            bleedPrefab = Instantiate(base.bleedPrefab, centerPosition+new Vector3(0,8,0), Quaternion.identity);
            bleedPrefab.transform.localScale = new Vector3(3, 3, 3);
            isBleed = true;
    

            StartCoroutine(RotateAndDestroyObject(bleedPrefab, 3f,"isBleed"));

        }
        if (other.CompareTag("bomb"))
        {
            other.transform.parent = transform;  // Make the bomb a child of the enemy
            other.transform.localPosition = Vector3.zero;  // Set the local position to zero to align it with the enemy's position
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;  // Disable physics to stick the bomb

            StartCoroutine(DestroyBombAfterDelay(other.gameObject, 3.0f));  // Start the coroutine to destroy the bomb after 3 seconds
            Debug.Log("Bomb has attached to the enemy.");
        }
        if (!PlayerController.instance.isInvincible && other.tag == "Player")
        {
            PlayerController.instance.GetHurt(enemyData.Attack);
            PlayerController.instance.isInvincible = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" && !isGetHit && PlayerController.instance.isAttacking)
        {
            
            if ( SkillFunctions.instance.isChargingAttack)
            {
                Debug.Log("蓄力攻击");

                GetHitAS.Play();
                GetDamage(0, 4);
                isGetHit = true;
                Debug.Log("boss掉血");

                SkillFunctions.instance.isChargingAttack = false;
            }
            else
            {
                Debug.Log("普通攻击");
                GetHitAS.Play();
                GetDamage();
                Debug.Log("boss掉血");

                isGetHit = true;
            }
            SkillFunctions.instance.chargeProgress = 0f;
            SkillFunctions.instance.chargingParticles.Stop();
            SkillFunctions.instance.chargingParticles.Clear();
        }
        if (other.tag == "laser"  )
        {
            Debug.Log("激光攻击");
            Debug.Log("boss血量: "+enemyData.CurHealth);

            GetHitAS.Play();
            GetDamage(0,0.05f);
            isGetHit = true;
            
            staticPrefab = Instantiate(base.staticPrefab, centerPosition+new Vector3(0,8,0), Quaternion.identity);
            staticPrefab.transform.localScale = new Vector3(3, 3, 3);
            isStatic = true;
    

            StartCoroutine(RotateAndDestroyObject(staticPrefab, 3f,"isStatic"));
        }
        if (other.tag == "sword"  )
        {
            GetHitAS.Play();
            GetDamage(0,0.1f);
            isGetHit = true;
        }
        if (other.tag == "bleed")
        {
            Debug.Log("aoe攻击");
            Debug.Log("boss血量: "+enemyData.CurHealth);
            GetHitAS.Play();
            GetDamage(0,0.1f);
            isGetHit = true;
            
            bleedPrefab = Instantiate(base.bleedPrefab, centerPosition+new Vector3(0,8,0), Quaternion.identity);
            bleedPrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            isBleed = true;
    

            StartCoroutine(RotateAndDestroyObject(bleedPrefab, 3f,"isBleed"));
        }
        if (other.CompareTag("bomb"))
        {
            other.transform.parent = transform;  // Make the bomb a child of the enemy
            other.transform.localPosition = Vector3.zero;  // Set the local position to zero to align it with the enemy's position
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;  // Disable physics to stick the bomb

            StartCoroutine(DestroyBombAfterDelay(other.gameObject, 3.0f));  // Start the coroutine to destroy the bomb after 3 seconds
            Debug.Log("Bomb has attached to the enemy.");
        }
        if (!PlayerController.instance.isInvincible && other.tag == "Player")
        {
            PlayerController.instance.GetHurt(enemyData.Attack);
            PlayerController.instance.isInvincible = true;
        }
    }
    IEnumerator DestroyBombAfterDelay(GameObject bomb, float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        Destroy(bomb);  // Destroy the bomb object
        GetDamage(0,0.2f);
        Debug.Log("Bomb has been destroyed after " + delay + " seconds.");
    }
    private GameObject staticPrefab;
    private GameObject bleedPrefab;
    private bool isStatic = false;
    private bool isBleed = false;
    IEnumerator RotateAndDestroyObject(GameObject obj, float duration,string stateName)
    {
        float time = 0;
        while (time < duration)
        {
            // 每帧旋转一定角度
            obj.transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime); // 调整旋转速度和轴向
            time += Time.deltaTime;
            yield return null;
        }

        if (stateName=="isState")
        {
            isStatic = false;
        }else if (stateName=="isBleed")
        {
            isBleed = false;
        }
        

        // 摧毁对象
        Destroy(obj);
    }
    

    public void GetDamage(int damage = 0, float rate = 0)
    {
        if (enemyData.CurHealth > 0)
        {
            if ((damage == 0 && rate == 0))
            {
                if (PlayerController.instance.isCritical)
                {
                    Debug.Log("攻击力: "+PlayerController.instance.playerData.Attack* 2);
                    enemyData.CurHealth -= PlayerController.instance.playerData.Attack * 2;
                }
                else
                {
                    Debug.Log("攻击力: "+PlayerController.instance.playerData.Attack);

                    enemyData.CurHealth -= PlayerController.instance.playerData.Attack;
                }
            }

            if (damage != null && damage > 0)
            {
                Debug.Log("攻击力: "+PlayerController.instance.playerData.Attack+ damage);

                enemyData.CurHealth -= PlayerController.instance.playerData.Attack + damage;
            }

            if (rate != null && rate > 0)
            {
                Debug.Log("攻击力: "+PlayerController.instance.playerData.Attack* rate);

                enemyData.CurHealth -= PlayerController.instance.playerData.Attack * rate;
            }
        }
    }

    public void SetData(EnemyData data)
    {
        //transform.position = data.Location;
        this.enemyData = data;
    }
}