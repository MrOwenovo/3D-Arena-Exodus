using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    private void Awake()
    {
        instance = this;
    }
    string path;

    void Start()
    {
        path = Application.persistentDataPath + "/saves/";
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveAllData(PlayerData playerData,List<EnemyData> enemyDatas)
    {
        StreamWriter sw = new StreamWriter(path + "GameSave.data");
        string jsData = JsonUtility.ToJson(playerData);

        foreach (EnemyData data in enemyDatas)
        {
            jsData += "|" + JsonUtility.ToJson(data);
        }
        sw.Write(jsData);
        sw.Close();

    }

    public void SaveUIData(GameUIData uiData)
    {
        StreamWriter sw = new StreamWriter(path + "GameUISave.data");
        string jsData = JsonUtility.ToJson(uiData);
        sw.Write(jsData);
        sw.Close();

    }
    public GameUIData LoadUIData()
    {
        string fileName = "GameUISave.data";
        if (File.Exists(path + fileName))
        {
            StreamReader sr = new StreamReader(path + fileName);
            string dataStr = sr.ReadToEnd();


            GameUIData uiData = new GameUIData();
            JsonUtility.FromJsonOverwrite(dataStr, uiData);
            return uiData;


        }
        else
        {
            Debug.LogError("Save flie not exist");
            return null;
        }
    }

    public Data[] LoadAllData()
    {
        string fileName = "GameSave.data";
        if(File.Exists(path + fileName))
        {
            StreamReader sr = new StreamReader(path + fileName);
            string dataStr = sr.ReadToEnd();
            string[] allDataStr = dataStr.Split('|');
            Data[] allDataOjt = new Data[allDataStr.Length];
            for(int i = 0; i < allDataStr.Length; i++)
            {
                if (i == 0)
                {
                    PlayerData playerData = new PlayerData();
                    JsonUtility.FromJsonOverwrite(allDataStr[i], playerData);
                    allDataOjt[i] = playerData;
                }
                else
                {
                    EnemyData slimeData = new EnemyData();
                    JsonUtility.FromJsonOverwrite(allDataStr[i], slimeData);
                    allDataOjt[i] = slimeData;
                }
            }
            return allDataOjt;


        }
        else
        {
            Debug.LogError("Save flie not exist");
            return null;
        }
    }
}
