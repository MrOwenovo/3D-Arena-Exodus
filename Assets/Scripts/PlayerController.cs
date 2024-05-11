using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.isNewGame)
        {
            SetData(DefaultPlayerData());
        }

        animator = GetComponent<Animator>();


        StartCoroutine(SPCheck());
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


            if (Input.GetMouseButtonDown(0) && canAttack)
            {
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
        check_att_time += Time.deltaTime;
        if (check_att_time >= attackCD)
        {
            canAttack = true;
            check_att_time = 0;
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