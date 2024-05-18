using UnityEngine;
using System.Collections;

public class SkillCoinManager : MonoBehaviour
{
    public static SkillCoinManager instance;

    public GameObject skillCoinPrefab;  // Prefab for the Skill Coin
    public Transform bossTransform;     // Reference to the boss's transform
    public float spawnRadius = 15.0f;   // Radius around the boss within which coins can appear
    public GameObject currentSkillCoin = null;

    private void Awake()
    {
        instance = this;
    }

    
    private void Start()
    {
        StartCoroutine(SpawnSkillCoinRoutine());
    }

    IEnumerator SpawnSkillCoinRoutine()
    {
        while (true)
        {
            if (currentSkillCoin == null)
            {
                yield return new WaitForSeconds(Random.Range(10, 20)); // Wait between 10 to 30 seconds
                SpawnSkillCoin();
            }
            yield return null;
        }
    }

    void SpawnSkillCoin()
    {
        if ((GameManager.instance.curStatus == Status.Game))
        {
            Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection.y = 0;  

            Vector3 spawnLocation = bossTransform.position + randomDirection + new Vector3(0, 0.5f, 0);
            currentSkillCoin = Instantiate(skillCoinPrefab, spawnLocation, Quaternion.identity);
 
        }
    }


    public void CoinCollected()
    {
       
        if (currentSkillCoin != null)
        {
            UIManager.instance.ShowCoinCollectedAnimation(currentSkillCoin.transform.position); // Displays the collection animation
            Destroy(currentSkillCoin);
            currentSkillCoin = null;
            SaveCoinData();
        }
    }


    void SaveCoinData()
    {
        // Saves the incremented count of collected coins
        int coins = PlayerPrefs.GetInt("SkillCoinsCollected", 0) + 1;
        PlayerPrefs.SetInt("SkillCoinsCollected", coins);
        PlayerPrefs.Save();
        UIManager.instance.UpdateCoinsDisplay(coins);
    }
}