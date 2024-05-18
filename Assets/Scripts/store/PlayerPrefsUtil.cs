using UnityEngine;
using System.Collections.Generic;

public static class PlayerPrefsUtil
{

    public static List<string> GetAllKeys()
    {
         
        List<string> keys = new List<string>();
        int counter = 0;
        bool hasNextKey = true;

        while (hasNextKey)
        {
            string key = counter.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                 
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

    public static void PrintPref(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
             
        }
        else
        {
            Debug.LogWarning("Key " + key + " doesn't exist!");
        }
    }

    public static void DeleteAllPrefs()
    {
        PlayerPrefs.DeleteAll();
         
    }

    public static void DeletePref(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
             
        }
        else
        {
            Debug.LogWarning("Key " + key + " doesn't exist!");
        }
    }
}