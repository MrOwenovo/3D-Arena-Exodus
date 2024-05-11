using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossEnemyController : EnemyController 
{
    


    // Start is called before the first frame update
    void Start()
    {
        base.main_cam = Camera.main.transform;
        healthBar = Instantiate(BarHolderPrefab, UIManager.instance.enemyBarsUI.transform);
        animator = GetComponent<Animator>();
        GetHitAS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if(GameManager.instance.curStatus == Status.Game && !isDead)
        {
            if(healthBar != null)
            {
                healthBar.transform.position = BarPoint.position;
                healthBar.transform.LookAt(main_cam.position);

            }
            if(!PlayerController.instance.isAttacking)
            {
                isGetHit = false;
            }
            if(gameObject.GetComponent<NavMeshAgent>()== null) { 
                gameObject.AddComponent<NavMeshAgent>();
            }
            agent = gameObject.GetComponent<NavMeshAgent>();
            // if (agent.isOnNavMesh)
            // {
            //     agent.SetDestination(GameManager.instance.Player.transform.position);
            // }
            // else
            // {
            //     agent.Warp(transform.position);
            // }

            // speed up
            timer += Time.deltaTime;
            if(timer > 6) {
                agent.speed += 0.5f;
                animator.SetFloat("Run",agent.speed);
                timer = 0;
            }

            CheckDeath();
            healthBar.transform.GetChild(0).GetComponent<Image>().fillAmount = enemyData.CurHealth / enemyData.MaxHealth;
            enemyData.Location = transform.position;

        }else if (isDead)
        {
            deadTimer += Time.deltaTime;
            if(deadTimer > 1)
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

    private void OnTriggerEnter(Collider other)
    {
       
        if(other.tag == "Weapon" && !isGetHit && PlayerController.instance.isAttacking)
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
        if(enemyData.CurHealth > 0)
        {
            if(PlayerController.instance.isCritical)
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
