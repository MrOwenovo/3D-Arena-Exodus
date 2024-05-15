using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleGenerator : MonoBehaviour
{
    public static  ObstacleGenerator insatance;
    public int mapSize = 31;
    public bool[,] mapOccupied;
    private Vector3 centerPosition;
    private bool isObstaclesGenerated = false;
    public List<GameObject> generatedObstacles = new List<GameObject>(1000);
    public void Awake()
    {
        insatance = this;
    }

    void Start()
    {
        mapOccupied = new bool[mapSize, mapSize];
        
    }

    void Update()
    {
        if ((GameManager.instance.curStatus == Status.Game) && !isObstaclesGenerated)
        {
            GameObject centerBlock = GameObject.Find("MapBlock_20_1_20");
            if (centerBlock != null)
            {
                centerPosition = centerBlock.transform.position;
            }
            else
            {
                Debug.LogError("Center block 'MapBlock_20_1_20' not found. Using default center.");
                centerPosition = new Vector3(mapSize / 2, 1, mapSize / 2); // Default center if block not found
            }
            GenerateObstacles();
            PlaceCenterBlock();
            isObstaclesGenerated = true; 
        }
        else if (!(GameManager.instance.curStatus == Status.Game||GameManager.instance.curStatus == Status.Training))
        {
            isObstaclesGenerated = false; 
        }
    }

    void GenerateObstacles()
    {
        int obstaclesNeeded = 20;
        int tallObstaclesNeeded = 5;
        int currentObstacles = 0;
        int currentTallObstacles = 0;

        while (currentObstacles < obstaclesNeeded)
        {
            float x = centerPosition.x + Random.Range(-10, 11); // Adjust range to fit your map if needed
            float z = centerPosition.z + Random.Range(-10, 11); // Adjust range to fit your map if needed

            if (x >= 0 && x < mapSize && z >= 0 && z < mapSize && !mapOccupied[(int)x, (int)z]) // Check bounds
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                generatedObstacles.Add(cube);
                bool isTall = currentTallObstacles < tallObstaclesNeeded;
                float height = isTall ? 2f : 1f;
                
                cube.transform.position = new Vector3(x, height / 2+1.5f, z); // Ensure the base of the block is above the ground
                cube.transform.localScale = new Vector3(1, height, 1);

                mapOccupied[(int)x, (int)z] = true;
                currentObstacles++;
                if (isTall)
                {
                    currentTallObstacles++;
                }
            }
        }
    }

    void PlaceCenterBlock()
    {
        // This can remain unchanged or be removed if no longer needed
        // GameObject centerBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // centerBlock.transform.position = centerPosition + new Vector3(0, 0.5f, 0);
        // centerBlock.transform.localScale = new Vector3(1, 1, 1);
        // centerBlock.GetComponent<Renderer>().material.color = Color.red;
    }
}
