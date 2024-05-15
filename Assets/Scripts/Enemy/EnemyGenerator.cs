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
    public GameObject EnemyParentTemplate;
    public GameObject EnemyParentTraining;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
   
    public void DeployEnemies(int skillLevel, Vector3 bossPosition)
    {
        // Calculate the start and end points of the parabola
        Vector3 startPoint = bossPosition + new Vector3(0, 7, 0); // Start point above the boss
        Vector3 endPoint = bossPosition + new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6)); // End point around the boss

        GameObject enemyPrefab;
        EnemyData enemyData;

        switch (skillLevel)
        {
            case 0:
                enemyData = DefaultSlimeData(startPoint, endPoint);
                break;
            case 1:
                enemyData = DefaultTurtleData(startPoint, endPoint);
                break;
            case 2:
                enemyData = DefaultBoxData(startPoint, endPoint);
                break;
            case 3:
                enemyData = DefaultETData(startPoint, endPoint);
                break;
            default:
                return; // Handle unexpected skill level
        }

        // Instantiate enemies and set data
    }

   

    EnemyData DefaultSlimeData(Vector3 startPoint, Vector3 endPoint)
    {
        // Instantiate SlimePrefab at the startPoint
        GameObject temp = Instantiate(SlimePrefab, startPoint, Quaternion.identity);
        temp.transform.parent = EnemyParent.transform;

        // Add Rigidbody component to the enemy
        Rigidbody rb = temp.AddComponent<Rigidbody>();
        rb.useGravity = true; // Enable gravity for the enemy to follow a parabolic path

        // Calculate velocity vector for the parabolic trajectory
        Vector3 direction = endPoint - startPoint;
        float height = Mathf.Abs(direction.y); // Height of the parabola
        direction.y = 0; // Ignore vertical direction for horizontal velocity
        float distance = direction.magnitude; // Distance between start and end points
        float horizontalSpeed = distance / 2; // Adjust speed as needed
        float verticalSpeed = Mathf.Sqrt(2 * Physics.gravity.magnitude * height); // Calculate vertical velocity based on gravity and height

        Vector3 horizontalVelocity = direction.normalized * horizontalSpeed; // Horizontal velocity
        Vector3 verticalVelocity = Vector3.up * verticalSpeed; // Vertical velocity

        // Set initial velocity for the enemy Rigidbody
        rb.velocity = horizontalVelocity + verticalVelocity;

        // Set enemy data and return
        EnemyData slimeData = new EnemyData();
        slimeData.MaxHealth = 100;
        slimeData.CurHealth = slimeData.MaxHealth;
        slimeData.Attack = 10;
        slimeData.Exp = 20;

        temp.GetComponent<EnemyController>().enemyData = slimeData;
        return slimeData;
    }
    EnemyData DefaultTurtleData(Vector3 startPoint, Vector3 endPoint)
{
    GameObject temp = Instantiate(TurtlePrefab, startPoint, Quaternion.identity);
    temp.transform.parent = EnemyParent.transform;

    Rigidbody rb = temp.AddComponent<Rigidbody>();
    rb.useGravity = true;

    Vector3 direction = endPoint - startPoint;
    float height = Mathf.Abs(direction.y);
    direction.y = 0;
    float distance = direction.magnitude;
    float horizontalSpeed = distance / 2;
    float verticalSpeed = Mathf.Sqrt(2 * Physics.gravity.magnitude * height);

    Vector3 horizontalVelocity = direction.normalized * horizontalSpeed;
    Vector3 verticalVelocity = Vector3.up * verticalSpeed;

    rb.velocity = horizontalVelocity + verticalVelocity;

    EnemyData turtleData = new EnemyData();
    turtleData.MaxHealth = 3000;
    turtleData.CurHealth = turtleData.MaxHealth;
    turtleData.Attack = 20;
    turtleData.Exp = 35;

    temp.GetComponent<EnemyController>().enemyData = turtleData;
    return turtleData;
}

EnemyData DefaultBoxData(Vector3 startPoint, Vector3 endPoint)
{
    GameObject temp = Instantiate(BoxPrefab, startPoint, Quaternion.identity);
    temp.transform.parent = EnemyParent.transform;

    Rigidbody rb = temp.AddComponent<Rigidbody>();
    rb.useGravity = true;

    Vector3 direction = endPoint - startPoint;
    float height = Mathf.Abs(direction.y);
    direction.y = 0;
    float distance = direction.magnitude;
    float horizontalSpeed = distance / 2;
    float verticalSpeed = Mathf.Sqrt(2 * Physics.gravity.magnitude * height);

    Vector3 horizontalVelocity = direction.normalized * horizontalSpeed;
    Vector3 verticalVelocity = Vector3.up * verticalSpeed;

    rb.velocity = horizontalVelocity + verticalVelocity;

    EnemyData boxData = new EnemyData();
    boxData.MaxHealth = 200;
    boxData.CurHealth = boxData.MaxHealth;
    boxData.Attack = 20;
    boxData.Exp = 60;

    temp.GetComponent<EnemyController>().enemyData = boxData;
    return boxData;
}

EnemyData DefaultETData(Vector3 startPoint, Vector3 endPoint)
{
    GameObject temp = Instantiate(ETPrefab, startPoint, Quaternion.identity);
    temp.transform.parent = EnemyParent.transform;

    Rigidbody rb = temp.AddComponent<Rigidbody>();
    rb.useGravity = true;

    Vector3 direction = endPoint - startPoint;
    float height = Mathf.Abs(direction.y);
    direction.y = 0;
    float distance = direction.magnitude;
    float horizontalSpeed = distance / 2;
    float verticalSpeed = Mathf.Sqrt(2 * Physics.gravity.magnitude * height);

    Vector3 horizontalVelocity = direction.normalized * horizontalSpeed;
    Vector3 verticalVelocity = Vector3.up * verticalSpeed;

    rb.velocity = horizontalVelocity + verticalVelocity;

    EnemyData etData = new EnemyData();
    etData.MaxHealth = 300;
    etData.CurHealth = etData.MaxHealth;
    etData.Attack = 20;
    etData.Exp = 100;

    temp.GetComponent<EnemyController>().enemyData = etData;
    return etData;
}

  

 
}
