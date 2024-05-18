using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public static BossController instance;

    public GameObject bossPrefab; // Prefab for the boss itself
    public GameObject bossPrefab2; // Prefab for the boss itself
    public GameObject bossPrefab3; // Prefab for the boss itself
    public GameObject bossPrefab4; // Prefab for the boss itself
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
             ;
            DeployBoss();
            isBossGenerated = true;
        }
        else if (GameManager.instance.curStatus != Status.Game&&GameManager.instance.curStatus != Status.Pause
                 )
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
        GameObject modelPrefab = GetRandomModelPrefab();

        GameObject temp = Instantiate(modelPrefab, check_point - new Vector3(0, 3, 0), Quaternion.identity);
        temp.transform.parent = EnemyGenerator.instance.EnemyParent.transform;

        MeshFilter bossMeshFilter = bossPrefab.GetComponentInChildren<MeshFilter>();
        if (bossMeshFilter != null)
        {
            MeshFilter instanceMeshFilter = temp.GetComponentInChildren<MeshFilter>();
            if (instanceMeshFilter != null)
            {
                instanceMeshFilter.sharedMesh = bossMeshFilter.sharedMesh;
            }
        }

        // Set boss's data
        EnemyData bossData = new EnemyData();
        bossData.MaxHealth = 3000;  // Example health
        bossData.CurHealth = 3000;
        bossData.Attack = 50;       // Example attack power
        bossData.Exp = 1000;        // Example experience value

        BossEnemyController.instance.enemyData = bossData;
        temp.GetComponent<EnemyController>().enemyData = bossData;
        return temp;
    }

    GameObject GetRandomModelPrefab()
    {
        int randomIndex = Random.Range(0, 4);
        switch (randomIndex)
        {
            case 0:
                return bossPrefab;  
            case 1:
                return bossPrefab2;
            case 2:
                return bossPrefab3;
            case 3:
                return bossPrefab4;
            default:
                return bossPrefab;  
        }
    }


   
}