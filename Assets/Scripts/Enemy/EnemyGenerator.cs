using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public static EnemyGenerator instance;

    public GameObject SlimePrefab;
    public GameObject TurtlePrefab;
    public GameObject BoxPrefab;
    public GameObject ETPrefab;
    public GameObject EnemyParent;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
   
    public void DeployEnemies(int skillLevel, Vector3 bossPosition)
    {
        // Adjusted to spawn enemies around the boss, not randomly across the map
        Vector3 spawnPoint = bossPosition + new Vector3(Random.Range(-5, 6), 7, Random.Range(-5, 6));

        GameObject enemyPrefab;
        EnemyData enemyData;

        switch (skillLevel)
        {
            case 0:
                DefaultSlimeData(spawnPoint);
                // enemyPrefab = SlimePrefab;
                // enemyData = new EnemyData { MaxHealth = 100, Attack = 10, Exp = 20 };
                break;
            case 1:
                DefaultTurtleData(spawnPoint);

                // enemyPrefab = TurtlePrefab;
                // enemyData = new EnemyData { MaxHealth = 160, Attack = 20, Exp = 35 };
                break;
            case 2:
                DefaultBoxData(spawnPoint);

                // enemyPrefab = BoxPrefab;
                // enemyData = new EnemyData { MaxHealth = 200, Attack = 20, Exp = 60 };
                break;
            case 3:
                DefaultETData(spawnPoint);

                // enemyPrefab = ETPrefab;
                // enemyData = new EnemyData { MaxHealth = 300, Attack = 20, Exp = 100 };
                break;
            default:
                return; // Handle unexpected skill level
        }

       
    }

   
    public void CreateEnemy()
    {
        if(GameManager.instance.curStatus == Status.Game)
        {
            while (true)
            {
                int generatX = Random.Range(0 + GameManager.instance.seaWidth, GameManager.instance.mapSize[0] - GameManager.instance.seaWidth);
                int generatZ = Random.Range(0 + GameManager.instance.seaWidth, GameManager.instance.mapSize[2] - GameManager.instance.seaWidth);
                Vector3 check_Point = new Vector3(generatX, 7, generatZ);
                RaycastHit hit;
                if (Physics.Raycast(check_Point, Vector3.down, out hit, 9))
                {
                    if (hit.collider.tag == "Ground")
                    {
                        if(GameManager.instance.Level == 1)
                        {
                            DefaultSlimeData(check_Point);
                            
                        }else if(GameManager.instance.Level == 2)
                        {
                            DefaultTurtleData(check_Point);
                        }
                        else if (GameManager.instance.Level == 3)
                        {
                            DefaultBoxData(check_Point);
                        }
                        else if (GameManager.instance.Level == 4)
                        {
                            DefaultETData(check_Point);
                        }
                        else
                        {
                            break;
                        }

                        break;

                    }
                }
            }
        }


    }

    EnemyData DefaultSlimeData(Vector3 check_point)
    {
        GameObject temp = Instantiate(SlimePrefab);
        temp.transform.position = check_point - new Vector3(0, 3, 0);
        temp.transform.parent = EnemyParent.transform;
        

        EnemyData slimeData = new EnemyData();
        slimeData.MaxHealth = 100;
        slimeData.CurHealth = slimeData.MaxHealth;
        slimeData.Attack = 10;
        slimeData.Exp = 20;

        temp.GetComponent<EnemyController>().enemyData = slimeData;
        return slimeData;
    }

    EnemyData DefaultTurtleData(Vector3 check_point)
    {
        GameObject temp = Instantiate(TurtlePrefab);
        temp.transform.position = check_point - new Vector3(0, 3, 0);
        temp.transform.parent = EnemyParent.transform;

        EnemyData turtleData = new EnemyData();
        turtleData.MaxHealth = 160;
        turtleData.CurHealth = turtleData.MaxHealth;
        turtleData.Attack = 20;
        turtleData.Exp = 35;

        temp.GetComponent<EnemyController>().enemyData = turtleData;
        return turtleData;
    }
    EnemyData DefaultBoxData(Vector3 check_point)
    {
        GameObject temp = Instantiate(BoxPrefab);
        temp.transform.position = check_point - new Vector3(0, 3, 0);
        temp.transform.parent = EnemyParent.transform;

        EnemyData turtleData = new EnemyData();
        turtleData.MaxHealth = 200;
        turtleData.CurHealth = turtleData.MaxHealth;
        turtleData.Attack = 20;
        turtleData.Exp = 60;

        temp.GetComponent<EnemyController>().enemyData = turtleData;
        return turtleData;
    }
    EnemyData DefaultETData(Vector3 check_point)
    {
        GameObject temp = Instantiate(ETPrefab);
        temp.transform.position = check_point - new Vector3(0, 3, 0);
        temp.transform.parent = EnemyParent.transform;

        EnemyData turtleData = new EnemyData();
        turtleData.MaxHealth = 300;
        turtleData.CurHealth = turtleData.MaxHealth;
        turtleData.Attack = 20;
        turtleData.Exp = 100;

        temp.GetComponent<EnemyController>().enemyData = turtleData;
        return turtleData;
    }
}
