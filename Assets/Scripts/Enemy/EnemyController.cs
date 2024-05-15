using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public GameObject BarHolderPrefab;
    public Transform BarPoint;
    public NavMeshAgent agent;


    public GameObject healthBar;
    public Transform main_cam;


    public EnemyData enemyData;
    //public float curHP = 100;
    //public float maxHP = 100;

    public bool isGetHit = false;
    public float timer = 0;
    public AudioSource GetHitAS;
    public GameObject staticPrefab;
    public GameObject bleedPrefab;

    public bool isDead = false;
    public float deadTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        main_cam = Camera.main.transform;
        healthBar = Instantiate(BarHolderPrefab, UIManager.instance.enemyBarsUI.transform);
        animator = GetComponent<Animator>();
        GetHitAS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.curStatus == Status.Game && !isDead)
        {
            if (healthBar != null)
            {
                healthBar.transform.position = BarPoint.position;
                healthBar.transform.LookAt(main_cam.position);
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
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(GameManager.instance.Player.transform.position);
            }
            else
            {
                agent.Warp(transform.position);
            }

            // speed up
            timer += Time.deltaTime;
            if (timer > 6)
            {
                agent.speed += 0.5f;
                animator.SetFloat("Run", agent.speed);
                timer = 0;
            }

            CheckDeath();
            healthBar.transform.GetChild(0).GetComponent<Image>().fillAmount =
                enemyData.CurHealth / enemyData.MaxHealth;
            enemyData.Location = transform.position;
        }
        else if (isDead)
        {
            deadTimer += Time.deltaTime;
            if (deadTimer > 1)
            {
                Destroy(gameObject);
                Destroy(healthBar);
            }
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
    public void TakeDamage(int damage)
    {
        Debug.Log("受到了+"+damage);
        Debug.Log("蓄力攻击");

        GetHitAS.Play();
        GetDamage(damage);
        isGetHit = true;
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
                isGetHit = true;
                SkillFunctions.instance.isChargingAttack = false;
            }
            else
            {
                Debug.Log("普通攻击");
                GetHitAS.Play();
                GetDamage();
                isGetHit = true;
            }
           
        }
        if (other.tag == "laser")
        {
            Debug.Log("激光攻击");

            GetHitAS.Play();
            GetDamage(0, 4);
            isGetHit = true;
        }
        if (other.tag == "bleed"  )
        {
            GetHitAS.Play();
            GetDamage(0,4);
            isGetHit = true;
            
        }
        if (other.tag == "sword"  )
        {
            GetHitAS.Play();
            GetDamage(5,0);
            isGetHit = true;
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
                SkillFunctions.instance.isChargingAttack = false;
            }
            else
            {
                Debug.Log("普通攻击");
                GetHitAS.Play();
                GetDamage();
                isGetHit = true;
            }
            SkillFunctions.instance.chargeProgress = 0f;
            SkillFunctions.instance.chargingParticles.Stop();
            SkillFunctions.instance.chargingParticles.Clear();
        }
        
        if (other.tag == "laser"  )
        {
            Debug.Log("激光攻击");

            GetHitAS.Play();
            GetDamage();
            isGetHit = true;
        }
        if (other.tag == "bleed"  )
        {
            GetHitAS.Play();
            GetDamage(0,4);
            isGetHit = true;
            
        }
        if (other.tag == "sword"  )
        {
            GetHitAS.Play();
            GetDamage(5,0);
            isGetHit = true;
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
        GetDamage(0,4);
        Debug.Log("Bomb has been destroyed after " + delay + " seconds.");
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
        enemyData = data;
    }
}