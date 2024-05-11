using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public static BossController instance;

    public GameObject bossPrefab; // Prefab for the boss itself
    private GameObject bossInstance;
    public GameObject[] skillPrefabs; // Array of skill prefabs
    private Vector3 centerPosition;
    public bool isBossGenerated = false;
    private Transform bossTransform;
    private LineRenderer lineRenderer;
    
    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if (GameManager.instance.curStatus == Status.Game && !isBossGenerated)
        {
            DeployBoss();
            isBossGenerated = true;
        }
        else if (GameManager.instance.curStatus != Status.Game)
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

    GameObject DefaultBossData(Vector3 check_point)
    {
        GameObject temp = Instantiate(bossPrefab, check_point - new Vector3(0, 3, 0), Quaternion.identity);
        temp.transform.parent = EnemyGenerator.instance.EnemyParent.transform;

        // Set boss's data
        EnemyData bossData = new EnemyData();
        bossData.MaxHealth = 1000;  // Example health
        bossData.CurHealth = bossData.MaxHealth;
        bossData.Attack = 50;       // Example attack power
        bossData.Exp = 1000;        // Example experience value

        temp.GetComponent<EnemyController>().enemyData = bossData;
        return temp;
    }


   
}