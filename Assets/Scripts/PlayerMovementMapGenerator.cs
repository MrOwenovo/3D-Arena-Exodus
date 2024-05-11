using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementMapGenerator : MonoBehaviour
{
    // This is the map object
    public GameObject map;
    // This is the prefab list
    public GameObject[] prefabList;
    // This is the map size
    public int[] mapSize = new int[3] { 50, 2, 50 };
    // This is the map array
    public int[,,] mapArray;
    


    // Generate the map array
    void GenerateMapArray(){
        // Init the map array
        mapArray = new int[mapSize[0], mapSize[1], mapSize[2]];
        // Generate the map array
        for(int i = 0; i < mapSize[0]; i++){
            for(int k = 0; k < mapSize[2]; k++){
                for(int j = 0; j < mapSize[1]; j++)
                    mapArray[i, j, k] = 0;
            }
        }
    }





    void Start()
    {
        GenerateMapArray();
        MapGenerator.GenerateMap(map, mapSize, mapArray, prefabList);
        
    }
}
