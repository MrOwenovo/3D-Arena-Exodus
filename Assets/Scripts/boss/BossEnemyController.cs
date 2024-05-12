using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossEnemyController : EnemyController
{
    public GameObject[] skillPrefabs;
    private LineRenderer lineRenderer;
    public float maxRadius = 5f;
    public float expandRate = 1f;
    private float radius = 0.0f;
    private bool isAttacking = false;


    // Start is called before the first frame update
    void Start()
    {
        // agent.enabled = false; // 禁用NavMeshAgent以固定Boss位置
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };
        lineRenderer.positionCount = 0;

        StartCoroutine(SkillDeploymentRoutine());

        base.main_cam = Camera.main.transform;
        base.healthBar = Instantiate(BarHolderPrefab, UIManager.instance.enemyBarsUI.transform);
        base.animator = GetComponent<Animator>();
        GetHitAS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.curStatus == Status.Game && !isDead)
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
            base.healthBar.transform.GetChild(0).GetComponent<Image>().fillAmount =
                enemyData.CurHealth / enemyData.MaxHealth;
            enemyData.Location = transform.position;
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
        DeploySkill(0);
        yield return new WaitForSeconds(1);
        DeploySkill(0);
        yield return new WaitForSeconds(1);
        DeploySkill(0);
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
                
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    void ExpandCircle()
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon" && !isGetHit && PlayerController.instance.isAttacking)
        {
            GetHitAS.Play();
            GetDamage();
            isGetHit = true;
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
            GetHitAS.Play();
            GetDamage();
            isGetHit = true;
        }

        if (!PlayerController.instance.isInvincible && other.tag == "Player")
        {
            PlayerController.instance.GetHurt(enemyData.Attack);
            PlayerController.instance.isInvincible = true;
        }
    }

    public void GetDamage()
    {
        if (enemyData.CurHealth > 0)
        {
            if (PlayerController.instance.isCritical)
            {
                enemyData.CurHealth -= PlayerController.instance.playerData.Attack * 2;
            }
            else
            {
                enemyData.CurHealth -= PlayerController.instance.playerData.Attack;
            }
        }
    }

    public void SetData(EnemyData data)
    {
        //transform.position = data.Location;
        enemyData = data;
    }
}