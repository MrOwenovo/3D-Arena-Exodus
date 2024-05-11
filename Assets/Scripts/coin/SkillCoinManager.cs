using UnityEngine;

namespace coin
{
    public class SkillCoinManager : MonoBehaviour
    {
        public GameObject skillCoinPrefab;
        private GameObject currentSkillCoin;
        private float timer;
        private float spawnInterval = 60; // Interval to spawn skill coins
        private Vector3 centerPosition;
        private bool isCoinGenerated = false;
        void Start()
        {
        }

        void Update()
        {
            if (GameManager.instance.curStatus == Status.Game)
            {
                if (GameManager.instance.curStatus == Status.Game && !isCoinGenerated)
                {
                    GameObject centerBlock = GameObject.Find("MapBlock_20_1_20");
                    if (centerBlock != null)
                    {
                        centerPosition = centerBlock.transform.position;
                    }
                    else
                    {
                        Debug.LogError("Center block 'MapBlock_20_1_20' not found. Using default center.");
                        centerPosition = new Vector3(20, 1, 20); // Default if center block not found
                    }

                    isCoinGenerated = true;
                }

                timer += Time.deltaTime;
                if (timer >= spawnInterval && currentSkillCoin == null)
                {
                    Vector3 spawnPosition =
                        centerPosition + new Vector3(Random.Range(-10, 11), 0.25f, Random.Range(-10, 11));
                    currentSkillCoin = Instantiate(skillCoinPrefab, spawnPosition, Quaternion.identity);
                    timer = 0; // Reset timer after spawning
                }
            }
        }
    }
}