using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Status
{
    Menu,Game,Pause,Over
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // This is the map object
    public GameObject map;
    // This is the prefab list
    public GameObject[] prefabList;
    // This is the map size
    public int[] mapSize = new int[3] { 36, 6, 36 };
    // This is the map array
    public int[,,] mapArray;
    // This is the sea width
    public int seaWidth = 3;
    // This is the sea height
    public float seaHeight = 0.5f;
    // This is the sea power
    public int seaPower = 2;
    // This is the wave speed
    public float waveSpeed = 0.5f;
    // This is min land height
    public int minLandHeight = 2;
    // This is the smoothness of the map
    public int smoothness = 20;



    //-----------------------------------------------
    // This is the player prefab
    public GameObject playerPrefab;
    // This is the player init position
    public Vector3 playerInitPosition = new Vector3(20, 4, 20);
    public Status curStatus = Status.Menu;


    [Header("Enemy Generate Peroid")]
    public float wait_time;

    [HideInInspector]
    public GameObject Player;

    public GameUIData uiData;
    public GameObject MenuEnv;


    public int Level = 0;
    public bool isNewGame = true;

    
    public float[] experienceToNextLevel;

    public Vector2 startPoint;
    public Vector2 endPoint;
    public Vector2 controlPoint;

    bool isGenerateMap = false;
    

    private void Awake()
    {
        instance = this;
    }

    // Return the perlin noise value which is between 2 and prefabList.Length
    // The smoothness is the smoothness of the map
    //int GetPerlinNoise(int x, int z){
    //    return Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, z / smoothness) * (prefabList.Length - 2)) + 2;
    //}


    void GenerateMapArray(){
        // Temp variables
        int tempHeight;
        // Init the map array
        mapArray = new int[mapSize[0], mapSize[1], mapSize[2]];
        // Generate the map array
        for(int i = 0; i < mapSize[0]; i++){
            for(int k = 0; k < mapSize[2]; k++){
                // If the map block is in the sea, the map height will be 1
                if(i < seaWidth || i >= mapSize[0] - seaWidth || k < seaWidth || k >= mapSize[2] - seaWidth){
                    // Fill the map block with the sea
                    mapArray[i, 0, k] = 0;
                    // Fill the rest map block with the air
                    for(int j = 1; j < mapSize[1]; j++)
                        mapArray[i, j, k] = 1;
                }
                else
                {
                    // Generate the map height
                    //tempHeight = Random.Range(minLandHeight, mapSize[1]);
                    tempHeight = minLandHeight;
                    // Fill the map block with the ground
                    for (int j = 0; j < tempHeight; j++)
                        mapArray[i, j, k] = 2;
                    // Fill the rest map block with the air
                    for(int j = tempHeight; j < mapSize[1]; j++)
                        if(j == tempHeight)
                        {
                            if((i==5 && k % 4 == 0)||(i==35&& k%4 ==0)|| (k == 5 && i % 4 == 0) || (k == 35 && i % 4 == 0))
                            {
                                mapArray[i, j, k] = 3;
                            }
                            else
                            {
                                mapArray[i, j, k] = 1;
                            }
                        }
                        else
                        {
                            mapArray[i, j, k] = 1;
                        }
                        
                }
            }
        }
    }


    float GetSinValue(float x, float z){
        return seaHeight * Mathf.Sin(x / seaPower) * Mathf.Sin(z / seaPower);
    }


    void InitSeaCube(){
        for(int i = 0; i < mapSize[0]; i++){
            for(int k = 0; k < mapSize[2]; k++){
                // If the map block is the sea
                if(i < seaWidth || i >= mapSize[0] - seaWidth || k < seaWidth || k >= mapSize[2] - seaWidth){
                    // Init the height of the sea cube with the sin value
                    MapGenerator.mapBlockArray[i, 0, k].transform.position = new Vector3(i, GetSinValue(i, k), k);
                    // Set the movement direction of the sea cube
                    if (GetSinValue(i, k) < GetSinValue(i + 1, k + 1))
                        EventAdder.AddEvent(MapGenerator.mapBlockArray[i, 0, k], "SeaMovement", new string[3] {"moveUp", "seaHeight", "waveSpeed"}, new object[3] {1, seaHeight, waveSpeed});
                    else
                        EventAdder.AddEvent(MapGenerator.mapBlockArray[i, 0, k], "SeaMovement", new string[3] {"moveUp", "seaHeight", "waveSpeed"}, new object[3] {-1, seaHeight, waveSpeed});
                }
            }
        }
    }

    // Generate the player
    void GeneratePlayer()
    {
        Player = Instantiate(playerPrefab, playerInitPosition, Quaternion.identity);
        Player.name = "Player";
        Player.transform.tag = "Player";
        PlayerMovement.maxJumpTimes = 2;
    }

    private IEnumerator BakeNavMesh()
    {
        yield return new WaitForSeconds(0.2f);
        NavMeshSurface navMeshSurface = GetComponent<NavMeshSurface>();
        if(navMeshSurface == null)
        {
            navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        }
        navMeshSurface.BuildNavMesh();
    }

    private IEnumerator GeneratEnemies(float wati_time)
    {
        while(true && GameManager.instance.curStatus == Status.Game)
        {
            yield return new WaitForSeconds(wati_time);
            EnemyGenerator.instance.CreateEnemy();
        }
    }
    void Start()
    {
        experienceToNextLevel = new float[100];
        for(int i = 0; i < 100; i++)
        {
            float t = (float)(i / 100f);
            int thisExp = (int)BezierCurve(t);
            experienceToNextLevel[i] = thisExp;
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        if(curStatus == Status.Game)
        {
            uiData.SurvivalTime += Time.deltaTime;
            int levelCode = (int)uiData.SurvivalTime / 60;
            
            if(levelCode == Level&& Level !=5 )
            {
                AudioManager.instance.SwitchBGM(levelCode);
                Level += 1;
            }

        }
        else if(curStatus != Status.Pause)
        {
            Level = 0;
        }

        if(Level == 5 && EnemyGenerator.instance.EnemyParent.transform.childCount==0)
        {
            UIManager.instance.WINtext.enabled = true;
            UIManager.instance.LOSEText.enabled = false;
            GameWin();
        }
    }

    

    private float BezierCurve(float t)
    {
        Vector2 subA = Vector2.Lerp(startPoint, controlPoint, t);
        Vector2 subB = Vector2.Lerp(controlPoint,endPoint, t);
        return Vector2.Lerp(subA, subB, t).y;
    }

    public void StartGame()
    {
        isNewGame = true;
        EnemyGenerator.instance.EnemyParent = new GameObject("EnemyParent");
        Time.timeScale = 1;
        uiData = new GameUIData();
        uiData.SurvivalTime = 0;
        uiData.EnemyKillNum = 0;
        SwitchGameStatus(Status.Game);
        if (!isGenerateMap) {
            GenerateMapArray();
            MapGenerator.GenerateMap(map, mapSize, mapArray, prefabList);
            InitSeaCube();
            isGenerateMap = true;
        }
        
        StartCoroutine(BakeNavMesh());
        GeneratePlayer();


        // IEnumerator enu = GeneratEnemies(wait_time);
        // StartCoroutine(enu);
        BossController.instance.isBossGenerated = false;
    }

    public void StartFromLoad()
    {
        isNewGame = false;
        EnemyGenerator.instance.EnemyParent = new GameObject("EnemyParent");
        Time.timeScale = 1;
        SwitchGameStatus(Status.Game);
        if (!isGenerateMap)
        {
            GenerateMapArray();
            MapGenerator.GenerateMap(map, mapSize, mapArray, prefabList);
            InitSeaCube();
            isGenerateMap = true;
        }
        StartCoroutine(BakeNavMesh());
        GeneratePlayer();

        Data[] allDatas = SaveManager.instance.LoadAllData();

        PlayerController.instance.SetData((PlayerData)allDatas[0]);

        for(int i = 1; i < allDatas.Length; i++)
        {
            EnemyData enemydata = (EnemyData)allDatas[i];

            if (enemydata.MaxHealth == 100)
            {
                GameObject temp = Instantiate(EnemyGenerator.instance.SlimePrefab, enemydata.Location, Quaternion.identity);
                temp.GetComponent<EnemyController>().SetData(enemydata);
                temp.transform.parent = EnemyGenerator.instance.EnemyParent.transform;
                
            }else if(enemydata.MaxHealth == 160)
            {
                GameObject temp = Instantiate(EnemyGenerator.instance.TurtlePrefab, enemydata.Location, Quaternion.identity);
                temp.GetComponent<EnemyController>().SetData(enemydata);
                temp.transform.parent = EnemyGenerator.instance.EnemyParent.transform;
                
            }
            else if (enemydata.MaxHealth == 200)
            {
                GameObject temp = Instantiate(EnemyGenerator.instance.BoxPrefab, enemydata.Location, Quaternion.identity);
                temp.GetComponent<EnemyController>().SetData(enemydata);
                temp.transform.parent = EnemyGenerator.instance.EnemyParent.transform;
                
            }
            else if (enemydata.MaxHealth == 300)
            {
                GameObject temp = Instantiate(EnemyGenerator.instance.ETPrefab, enemydata.Location,Quaternion.identity);
                temp.GetComponent<EnemyController>().SetData(enemydata);
                temp.transform.parent = EnemyGenerator.instance.EnemyParent.transform;
                
            }

        }

        uiData = SaveManager.instance.LoadUIData();

        IEnumerator enu = GeneratEnemies(wait_time);
        StartCoroutine(enu);


    }

    public void SaveData()
    {
        Debug.Log("Save data");
        List<EnemyData> enemyDatas = new List<EnemyData>();
        GameObject enemyParent = EnemyGenerator.instance.EnemyParent;
        for(int i = 0; i < enemyParent.transform.childCount; i++)
        {
            enemyDatas.Add(enemyParent.transform.GetChild(i).GetComponent<EnemyController>().enemyData);

        }
        SaveManager.instance.SaveAllData(PlayerController.instance.playerData, enemyDatas);
        SaveManager.instance.SaveUIData(uiData);
    }
    public void PauseGame()
    {
        if(curStatus == Status.Game)
        {
            SwitchGameStatus(Status.Pause);
            Time.timeScale = 0;
        }else if(curStatus == Status.Pause)
        {
            SwitchGameStatus(Status.Game);
            Time.timeScale = 1;
        }
    }
    public void GameOver()
    {
        SwitchGameStatus(Status.Over);
        Debug.Log("GameOver");
        StopAllCoroutines();
        GameObject tempEP = EnemyGenerator.instance.EnemyParent;
        for(int i = 0; i < tempEP.transform.childCount; i++)
        {
            tempEP.transform.GetChild(i).GetComponent<Animator>().SetTrigger("Lose");
            tempEP.transform.GetChild(i).GetComponent<NavMeshAgent>().speed = 0;
        }
    }

    //TO DO
    private void GameWin()
    {
        SwitchGameStatus(Status.Over);
        Debug.Log("Win");
        StopAllCoroutines();
        Time.timeScale = 0;
    }
    public void BackToMenu()
    {
        SwitchGameStatus(Status.Menu);
        // TO DO
        AudioManager.instance.SwitchBGM(-1);
        StopAllCoroutines();
        Time.timeScale = 1;
        Destroy(Player);
        Destroy(EnemyGenerator.instance.EnemyParent);
        Transform enemyBars = UIManager.instance.enemyBarsUI.transform;
        for(int i = 0;i < enemyBars.childCount; i++)
        {
            Destroy(enemyBars.GetChild(i).gameObject);
        }
    }

    public void ReStart()
    {
        Destroy(Player);
        Destroy(EnemyGenerator.instance.EnemyParent);
        Transform enemyBars = UIManager.instance.enemyBarsUI.transform;
        for (int i = 0; i < enemyBars.childCount; i++)
        {
            Destroy(enemyBars.GetChild(i).gameObject);
        }

        StartGame();
    }

    public void SwitchGameStatus(Status nextStatus)
    {
        switch (nextStatus)
        {
            case Status.Menu:
                MenuEnv.SetActive(true);
                map.SetActive(false);
                Camera.main.transform.position = new Vector3(20, 6, -2);
                Camera.main.transform.rotation = Quaternion.Euler(20, 0, 0);

                UIManager.instance.MenuCanvas.enabled = true;
                UIManager.instance.GameCanvas.enabled = false;
                UIManager.instance.PauseCanvas.enabled = false;
                UIManager.instance.OverCanvas.enabled = false;
                break;
            case Status.Game:
                MenuEnv.SetActive(false);
                map.SetActive(true);
                UIManager.instance.MenuCanvas.enabled = false;
                UIManager.instance.GameCanvas.enabled = true;
                UIManager.instance.PauseCanvas.enabled = false;
                UIManager.instance.OverCanvas.enabled = false;
                break;
            case Status.Pause:
                UIManager.instance.MenuCanvas.enabled = false;
                UIManager.instance.GameCanvas.enabled = false;
                UIManager.instance.PauseCanvas.enabled = true;
                UIManager.instance.OverCanvas.enabled = false;
                break;
            case Status.Over:
                UIManager.instance.MenuCanvas.enabled = false;
                UIManager.instance.GameCanvas.enabled = false;
                UIManager.instance.PauseCanvas.enabled = false;
                UIManager.instance.OverCanvas.enabled = true;
                break;
        }
        curStatus = nextStatus;
    }

    public void Quit()
    {
        Application.Quit();
    }


}
