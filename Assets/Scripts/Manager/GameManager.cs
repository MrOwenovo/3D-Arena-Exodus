using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum Status
{
    Menu,Game,Pause,Over,Training
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
    public Button changeDifficulty;
    public Button openSkills;
    public Text changeDifficultyText;



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

    public GameObject guideBook;

    bool isGenerateMap = false;

    public void openGuid()
    {
        guideBook.SetActive(true);
    }
    public void closeGuid()
    {
        guideBook.SetActive(false);
    }
    private void Awake()
    {
        instance = this;
        Screen.SetResolution(1920, 1080, true);
    }

    // Return the perlin noise value which is between 2 and prefabList.Length
    // The smoothness is the smoothness of the map
    //int GetPerlinNoise(int x, int z){
    //    return Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, z / smoothness) * (prefabList.Length - 2)) + 2;
    //}
    public int difficulty = 1;
    public void changeDif()
    {
        if (difficulty == 1)
        {
            difficulty = 2;
            changeDifficultyText.text = "changeToEasy";
            BossEnemyController.instance.enemyData.MaxHealth += 2000;
            BossEnemyController.instance.enemyData.CurHealth += 2000;
            UIManager.instance.BossHealth.text ="BossHealth: "+ BossEnemyController.instance.enemyData.CurHealth;
        }else if (difficulty == 2)
        {
            difficulty = 1;
            changeDifficultyText.text = "changeToDifficult";
            BossEnemyController.instance.enemyData.MaxHealth -= 2000;
            BossEnemyController.instance.enemyData.CurHealth -= 2000;
            UIManager.instance.BossHealth.text ="BossHealth: "+ BossEnemyController.instance.enemyData.CurHealth;


            if (BossEnemyController.instance.enemyData.CurHealth < 0)
            {
                BossEnemyController.instance.enemyData.CurHealth = 0;
                UIManager.instance.BossHealth.text ="BossHealth: "+ BossEnemyController.instance.enemyData.CurHealth;

            }

        }
        
    }

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

    public void DestroyAllObjectsWithName(string objectName)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == objectName)
            {
                Destroy(obj);
            }
        }
    }
    // private IEnumerator DelayedLoadContainerState(float delay)
    // {
    //     // yield return new WaitForSeconds(delay);
    //     SkillUI.instance.LoadContainerState();
    // }

  public void BackToMenu()
{
    if (SkillUI.instance != null)
    {
        if (SkillUI.instance.trainingSkillName != null && SkillUI.instance.trainingSkillName.Count > 0 && SkillUI.instance.trainingSkillName[0] != null)
        {
            SkillUI.instance.trainingSkillName = new List<string>(4);
        }
    }

  
    
    DestroyAllObjectsWithName("skillPanel(Clone)");

    // Start coroutine to delay LoadContainerState
    // StartCoroutine(DelayedLoadContainerState(1f)); // 1 second delay, adjust as needed
    // yield return new WaitForSeconds(delay);
    SkillUI.instance.LoadContainerState();

    // if (SkillUI.instance != null)
    // {
    //     SkillUI.instance.LoadContainerState();
    // }

    if (PlayerController.instance != null)
    {
        PlayerController.instance.DelayedUpdateEquippedSkills(1f);
    }

    DestroyAllObjectsWithName("Sword Variant(Clone)");
    DestroyAllObjectsWithName("PolygonPowerupShield(Clone)");
    DestroyAllObjectsWithName("PolygonPowerupSpeed(Clone)");
    DestroyAllObjectsWithName("PolyBoneRibCageGory Variant(Clone)");
    DestroyAllObjectsWithName("Shockwave");

    if (EnemyGenerator.instance != null && EnemyGenerator.instance.EnemyParent != null && GameManager.instance.frontStatus == Status.Game)
    {
        EnemyGenerator.instance.destroyParent();
    }

    if (GameManager.instance != null)
    {
        GameManager.instance.openSkills.gameObject.SetActive(false);
        GameManager.instance.SwitchGameStatus(Status.Menu);
    }

    if (AudioManager.instance != null)
    {
        AudioManager.instance.SwitchBGM(-1);
    }

    StopAllCoroutines();
    Time.timeScale = 1;

    if (GameManager.instance != null && GameManager.instance.Player != null)
    {
        Destroy(GameManager.instance.Player);
    }

    if (BossControllerTraining.instance != null && BossControllerTraining.instance.temp != null)
    {
        Destroy(BossControllerTraining.instance.temp);
    }

    if (UIManager.instance != null)
    {
        Transform enemyBars = UIManager.instance.enemyBarsUI.transform;
        for (int i = 0; i < enemyBars.childCount; i++)
        {
            Destroy(enemyBars.GetChild(i).gameObject);
        }

        SkillUI.instance.SetActiveRecursively(SkillUI.instance.storePanel, false);
    }

    if (ObstacleGenerator.instance != null)
    {
        ObstacleGenerator.instance.mapOccupied = new bool[ObstacleGenerator.instance.mapSize, ObstacleGenerator.instance.mapSize];
        foreach (GameObject obstacle in ObstacleGenerator.instance.generatedObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
        ObstacleGenerator.instance.generatedObstacles.Clear();
    }

    if (SkillCoinManager.instance != null && SkillCoinManager.instance.currentSkillCoin != null)
    {
        Destroy(SkillCoinManager.instance.currentSkillCoin);
    }
}


    // Generate the player
    void GeneratePlayer()
    {
        Player = Instantiate(playerPrefab, new Vector3(25, 6, 25), Quaternion.identity);
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
            // EnemyGenerator.instance.CreateEnemy();
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
             ;
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

    
//1
    private float BezierCurve(float t)
    {
        Vector2 subA = Vector2.Lerp(startPoint, controlPoint, t);
        Vector2 subB = Vector2.Lerp(controlPoint,endPoint, t);
        return Vector2.Lerp(subA, subB, t).y;
    }

    public void StartGame()
    {
      

        SkillUI.instance.onceLoadData = null;
        ObstacleGenerator.instance.isObstaclesGenerated = false;
        for (int i = 0; i < 4; i++)
        {
            if (SkillUI.instance != null && SkillUI.instance.skillContainers != null && i < SkillUI.instance.skillContainers.Length)
            {
                GameObject skillContainer = SkillUI.instance.skillContainers[i];
                if (skillContainer != null)
                {
                    Transform imageTransform = skillContainer.transform.Find("skillPanel(Clone)/SkillImage/Image");
                    if (imageTransform != null)
                    {
                        GameObject imageGameObject = imageTransform.gameObject;
                        imageGameObject.SetActive(false); // Activate GameObject
                    }
                    else
                    {
                        Debug.LogWarning($"Image transform not found in skill container {i}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Skill container {i} is null");
                }
            }
            else
            {
                Debug.LogWarning($"SkillUI.instance or skillContainers is null, or index {i} is out of bounds");
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (SkillUI.instance != null && SkillUI.instance.skillContainers != null &&
                i < SkillUI.instance.skillContainers.Length)
            {
                GameObject skillContainer = SkillUI.instance.skillContainers[i];
                if (skillContainer != null)
                {
                    Transform skillPanelTransform = skillContainer.transform.Find("skillPanel(Clone)/SkillImage/cover");
                    if (skillPanelTransform != null)
                    {
                        Image coverImage = skillPanelTransform.GetComponent<Image>();
                        if (coverImage != null)
                        {
                            coverImage.fillAmount = 0;
                        }
                        else
                        {
                            Debug.LogWarning($"Cover image component not found in skill container {i}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Skill panel transform not found in skill container {i}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Skill container {i} is null");
                }
            }
            else
            {
                Debug.LogWarning($"SkillUI.instance or skillContainers is null, or index {i} is out of bounds");
            }
        }
        if (BossEnemyController.instance!=null)
        {
            BossEnemyController.instance.bleedPrefab.gameObject.SetActive(true);
        }

        if (BossEnemyController.instance != null)
        {
            BossEnemyController.instance.staticPrefab.gameObject.SetActive(true);
        }
        frontStatus = Status.Game;
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
        ToggleSkillStoreVisibility();
        // SkillUI.instance.storePanel.SetActive(true);

        // IEnumerator enu = GeneratEnemies(wait_time);
        // StartCoroutine(enu);
        BossController.instance.isBossGenerated = false;
    }
    public void StartTrainingGame()
    {
        SkillUI.instance.onceLoadData = null;

        BossControllerTraining.instance.isBossGenerated = false;
        for (int i = 0; i < 4; i++)
        {
            if (SkillUI.instance != null && SkillUI.instance.skillContainers != null && i < SkillUI.instance.skillContainers.Length)
            {
                GameObject skillContainer = SkillUI.instance.skillContainers[i];
                if (skillContainer != null)
                {
                    Transform imageTransform = skillContainer.transform.Find("skillPanel(Clone)/SkillImage/Image");
                    if (imageTransform != null)
                    {
                        GameObject imageGameObject = imageTransform.gameObject;
                        imageGameObject.SetActive(false); // Activate GameObject
                    }
                    else
                    {
                        Debug.LogWarning($"Image transform not found in skill container {i}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Skill container {i} is null");
                }
            }
            else
            {
                Debug.LogWarning($"SkillUI.instance or skillContainers is null, or index {i} is out of bounds");
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (SkillUI.instance != null && SkillUI.instance.skillContainers != null &&
                i < SkillUI.instance.skillContainers.Length)
            {
                GameObject skillContainer = SkillUI.instance.skillContainers[i];
                if (skillContainer != null)
                {
                    Transform skillPanelTransform = skillContainer.transform.Find("skillPanel(Clone)/SkillImage/cover");
                    if (skillPanelTransform != null)
                    {
                        Image coverImage = skillPanelTransform.GetComponent<Image>();
                        if (coverImage != null)
                        {
                            coverImage.fillAmount = 0;
                        }
                        else
                        {
                            Debug.LogWarning($"Cover image component not found in skill container {i}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Skill panel transform not found in skill container {i}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Skill container {i} is null");
                }
            }
            else
            {
                Debug.LogWarning($"SkillUI.instance or skillContainers is null, or index {i} is out of bounds");
            }
        }

        if (BossEnemyController.instance!=null)
        {
            BossEnemyController.instance.bleedPrefab.gameObject.SetActive(true);
        }
        
        if (BossEnemyController.instance != null)
        {
            BossEnemyController.instance.staticPrefab.gameObject.SetActive(true);
        }
        frontStatus = Status.Training;
        openSkills.gameObject.SetActive(true);
        isNewGame = true;
        // EnemyGenerator.instance.EnemyParent = new GameObject("EnemyParent");
        Time.timeScale = 1;
        uiData = new GameUIData();
        uiData.SurvivalTime = 0;
        uiData.EnemyKillNum = 0;
        SwitchGameStatus(Status.Training);
        if (!isGenerateMap) {
            GenerateMapArray();
            MapGenerator.GenerateMap(map, mapSize, mapArray, prefabList);
            InitSeaCube();
            isGenerateMap = true;
        }
        SkillUI.instance.SetActiveRecursivelyStep2(SkillUI.instance.storePanel, !SkillUI.instance.storePanelTraining.activeSelf);

        StartCoroutine(BakeNavMesh());
        GeneratePlayer();
        ToggleSkillStoreVisibility();
        // SkillUI.instance.storePanel.SetActive(true);

        // IEnumerator enu = GeneratEnemies(wait_time);
        // StartCoroutine(enu);
        BossController.instance.isBossGenerated = false;
    }
    public void ToggleSkillStoreVisibility()
    {
        // 首先，设定 storePanel 下除指定 containers 外的所有子对象为不活跃
        foreach (Transform child in SkillUI.instance.storePanel.transform)
        {
            // 仅当 child 不是 skillContainers 数组中的一个元素时，才设其为不活跃
            bool shouldStayActive = false;

            foreach (GameObject container in SkillUI.instance.skillContainers)
            {
                if (child.gameObject == container)
                {
                    shouldStayActive = true;
                    break;
                }
            }
            if (!shouldStayActive)
            {
                SetActiveRecursive(child.gameObject, false);
            }
            else
            {
                foreach (Transform subChild in child)
                {
                    if (subChild.name == "skillPanel(Clone)")
                    {
                        SetActiveRecursive(subChild.gameObject, true);  
                    }
                }
            }
        }

        SkillUI.instance.storePanel.SetActive(true);
        SkillUI.instance.containerContainer.SetActive(true);
        foreach (GameObject container in SkillUI.instance.skillContainers)
        {
            container.SetActive(true); 
        }
    }

    void SetActiveRecursive(GameObject obj, bool active)
    {
        if (obj.name != "Image")
        {
            obj.SetActive(active);
            foreach (Transform child in obj.transform)
            {
                SetActiveRecursive(child.gameObject, active);
            }
        }
    }


    public void StartFromLoad()
    {
        frontStatus = Status.Game;
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
        ToggleSkillStoreVisibility();

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

        // IEnumerator enu = GeneratEnemies(wait_time);
        // StartCoroutine(enu);


    }

    public void SaveData()
    {
         ;
        List<EnemyData> enemyDatas = new List<EnemyData>();
        GameObject enemyParent = EnemyGenerator.instance.EnemyParent;
        for(int i = 0; i < enemyParent.transform.childCount; i++)
        {
            enemyDatas.Add(enemyParent.transform.GetChild(i).GetComponent<EnemyController>().enemyData);

        }
        SaveManager.instance.SaveAllData(PlayerController.instance.playerData, enemyDatas);
        SaveManager.instance.SaveUIData(uiData);
    }

    private Status currentStatus = Status.Game;

    public void ResumeGame()
    {
        MenuEnv.SetActive(false);
        map.SetActive(true);
        UIManager.instance.MenuCanvas.enabled = false;
        UIManager.instance.GameCanvas.enabled = true;
        UIManager.instance.PauseCanvas.enabled = false;
        UIManager.instance.OverCanvas.enabled = false;
        if (frontStatus==Status.Game)
        {
            curStatus = Status.Game;
        }else if (frontStatus==Status.Training)
        {
            currentStatus = Status.Training;
        }
        Time.timeScale = 1;
        // SwitchGameStatus(Status.Game);
        // currentStatus = Status.Game;
        // curStatus = Status.Game;
    }

    public Status frontStatus = Status.Game;
    public void PauseGame()
    {
         
        if(currentStatus == Status.Game)
        {
            SwitchGameStatus(Status.Pause);
            currentStatus = Status.Game;
            Time.timeScale = 0;
        }else if(currentStatus == Status.Training)
        {
            SwitchGameStatus(Status.Pause);
            currentStatus = Status.Training;
            Time.timeScale = 1;
        }else if(currentStatus == Status.Pause&& currentStatus == Status.Game)
        {
             
            SwitchGameStatus(Status.Game);
            Time.timeScale = 1;
        }else if(currentStatus == Status.Pause&& currentStatus == Status.Training)
        {
             

            SwitchGameStatus(Status.Training);
            Time.timeScale = 1;
        }
    }
    public void GameOver()
    {
        frontStatus = currentStatus;
        SwitchGameStatus(Status.Over);
         ;
        StopAllCoroutines();

        GameObject tempEP = EnemyGenerator.instance.EnemyParent;
        if (tempEP != null)  
        {
            for(int i = 0; i < tempEP.transform.childCount; i++)
            {
                GameObject child = tempEP.transform.GetChild(i).gameObject;
                if (child != null) 
                {
                    Animator animator = child.GetComponent<Animator>();
                    if (animator != null) 
                    {
                        animator.SetTrigger("Lose");
                    }

                    NavMeshAgent agent = child.GetComponent<NavMeshAgent>();
                    if (agent != null)  
                    {
                        agent.speed = 0;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("EnemyParent is not found. It might have been destroyed.");
        }
    }


    //TO DO
    public void GameWin()
    {
        SwitchGameStatus(Status.Over);
         ;
        StopAllCoroutines();
        Time.timeScale = 0;
    }

    public void clearStore()
    {
        PlayerPrefsUtil.DeleteAllPrefs();

    }
    

    public void ReStart()
    {
        Destroy(Player);
        // Destroy(EnemyGenerator.instance.EnemyParent);
        // Destroy(EnemyGenerator.instance.EnemyParentTraining);
        EnemyGenerator.instance.EnemyParent.gameObject.SetActive(false);
        EnemyGenerator.instance.EnemyParentTraining.gameObject.SetActive(false);
        Transform enemyBars = UIManager.instance.enemyBarsUI.transform;
        for (int i = 0; i < enemyBars.childCount; i++)
        {
            Destroy(enemyBars.GetChild(i).gameObject);
        }

        StartGame();
    }
//  qads
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
                curStatus = Status.Menu;
                break;
            case Status.Game:
                MenuEnv.SetActive(false);
                map.SetActive(true);
                UIManager.instance.MenuCanvas.enabled = false;
                UIManager.instance.GameCanvas.enabled = true;
                UIManager.instance.PauseCanvas.enabled = false;
                UIManager.instance.OverCanvas.enabled = false;
                curStatus = Status.Game;
                break;
            case Status.Training:
                MenuEnv.SetActive(false);
                map.SetActive(true);
                UIManager.instance.MenuCanvas.enabled = false;
                UIManager.instance.GameCanvas.enabled = true;
                UIManager.instance.PauseCanvas.enabled = false;
                UIManager.instance.OverCanvas.enabled = false;
                curStatus = Status.Training;
                break;
            case Status.Pause:
                UIManager.instance.MenuCanvas.enabled = false;
                UIManager.instance.GameCanvas.enabled = false;
                UIManager.instance.PauseCanvas.enabled = true;
                UIManager.instance.OverCanvas.enabled = false;
                curStatus = Status.Pause;
                 ;
                break;
            // 
            case Status.Over:
                UIManager.instance.MenuCanvas.enabled = false;
                UIManager.instance.GameCanvas.enabled = false;
                UIManager.instance.PauseCanvas.enabled = false;
                UIManager.instance.OverCanvas.enabled = true;
                UIManager.instance.OverCanvas.gameObject.SetActive(true);
                curStatus = Status.Over;
                break;
        }
        curStatus = nextStatus;
    }

    public void Quit()
    {
        Application.Quit();
    }


}
