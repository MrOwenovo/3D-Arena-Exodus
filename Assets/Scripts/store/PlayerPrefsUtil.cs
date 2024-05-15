using UnityEngine;
using System.Collections.Generic;

public static class PlayerPrefsUtil
{


    /// <summary>
    /// 获取所有PlayerPrefs键
    /// </summary>
    /// <returns>所有键的列表</returns>
    public static List<string> GetAllKeys()
    {
        Debug.Log("获取全部键");
        List<string> keys = new List<string>();
        int counter = 0;
        bool hasNextKey = true;

        while (hasNextKey)
        {
            string key = counter.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                Debug.Log(key);
                PrintPref(key);
                keys.Add(key);
            }
            else
            {
                hasNextKey = false;
            }

            counter++;
        }

        return keys;
    }

    /// <summary>
    /// 输出指定键的PlayerPrefs值
    /// </summary>
    /// <param name="key">PlayerPrefs键</param>
    public static void PrintPref(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            Debug.Log(key + " : " + PlayerPrefs.GetString(key));
        }
        else
        {
            Debug.LogWarning("Key " + key + " doesn't exist!");
        }
    }

    /// <summary>
    /// 删除所有PlayerPrefs键值对
    /// </summary>
    public static void DeleteAllPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs deleted!!!!!!!!!!!");
    }

    /// <summary>
    /// 删除指定的PlayerPrefs键值对
    /// </summary>
    /// <param name="key">要删除的PlayerPrefs键</param>
    public static void DeletePref(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            Debug.Log("Key " + key + " deleted!");
        }
        else
        {
            Debug.LogWarning("Key " + key + " doesn't exist!");
        }
    }
}