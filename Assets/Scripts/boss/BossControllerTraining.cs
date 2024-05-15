using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControllerTraining : MonoBehaviour
{
    public static BossControllerTraining instance;

    public GameObject bossPrefab; // Prefab for the boss itself
    private GameObject bossInstance;
    public GameObject[] skillPrefabs; // Array of skill prefabs
    private Vector3 centerPosition;
    public bool isBossGenerated = false;
    private Transform bossTransform;
    private LineRenderer lineRenderer;
    private GameObject bossInstanceStatic;

    private IEnumerator WaitForTemplateAndInstantiate()
    {
        // 等待直到 EnemyGenerator 的实例和 EnemyParentTemplate 都不为 null
        while (EnemyGenerator.instance == null || EnemyGenerator.instance.EnemyParentTemplate == null)
        {
            yield return new WaitForSeconds(0.1f); // 每次检查间隔0.1秒，防止过度消耗性能
        }

        // 当条件满足后，进行实例化
        // bossInstanceStatic = Instantiate(EnemyGenerator.instance.EnemyParentTemplate);

        bossInstanceStatic = EnemyGenerator.instance.EnemyParentTemplate;
        Debug.Log("EnemyParentTemplate has been instantiated.");
    }

    private void Awake()
    {
        instance = this;
        // 启动协程
        StartCoroutine(WaitForTemplateAndInstantiate());
    }

    void Update()
    {
        if (GameManager.instance.curStatus == Status.Training && !isBossGenerated)
        {
            DeployBoss();
            isBossGenerated = true;
        }
        else if (GameManager.instance.curStatus != Status.Training)
        {
            isBossGenerated = false;
        }
    }

    void DeployBoss()
    {
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

        // Deploy the boss with data and scale
        GameObject boss = DefaultBossData(centerPosition);
        boss.transform.localScale = new Vector3(5, 5, 5); // Scale the boss to 5x5x5 units
    }

    public GameObject temp;
    GameObject DefaultBossData(Vector3 check_point)
    {
        temp = Instantiate(bossPrefab, check_point - new Vector3(0, 3, 0), Quaternion.identity);
    
        // if (bossInstanceStatic != null)
        // {
        //     temp.transform.SetParent(bossInstanceStatic.transform, false);
        // }
        // else
        // {
        //     Debug.LogError("bossInstanceStatic is not instantiated or not in the scene ");
        // }

        // Set boss's data
        EnemyData bossData = new EnemyData();
        bossData.MaxHealth = 100000;
        bossData.CurHealth = 100000;
        bossData.Attack = 1;
        bossData.Exp = 1000;

        BossEnemyController.instance.enemyData = bossData;
        temp.GetComponent<EnemyController>().enemyData = bossData;
        return temp;
    }



   
}