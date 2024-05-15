using System.Collections;
using UnityEngine;

public class ChargingParticlesController : MonoBehaviour
{
    public static ChargingParticlesController instance;
    public GameObject player; // 玩家对象
    public float radius = 2f; // 环绕半径
    public int numberOfParticles = 50; // 粒子数量
    public float particleSpeed = 0.5f; // 粒子移动速度
    public ParticleSystem chargingParticlesPrefab; // 充能粒子预制体

    private ParticleSystem[] chargingParticles; // 充能粒子数组
    public bool isStart = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (isStart)
        {
            // 实例化充能粒子并设置位置、速度等属性
            chargingParticles = new ParticleSystem[numberOfParticles]; // 初始化充能粒子数组
            for (int i = 0; i < numberOfParticles; i++)
            {
                chargingParticles[i] = Instantiate(chargingParticlesPrefab, transform.position, Quaternion.identity);
                chargingParticles[i].transform.parent = transform;
                float angle = i * Mathf.PI * 2 / numberOfParticles;
                Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                chargingParticles[i].transform.position = player.transform.position + offset;
                chargingParticles[i].transform.LookAt(player.transform);
                // chargingParticles[i].startColor = Color.yellow;
            }
        }
    }

    void Update()
    {
        if (isStart)
        {
            // 更新充能粒子位置
            UpdateParticlePositions();
        }
    }

    public void PlayParticles()
    {
        isStart = true;
        // 启动所有充能粒子
        if (chargingParticles != null)
        {
            foreach (ParticleSystem particle in chargingParticles)
            {
                if (particle != null)
                {
                    particle.Play();
                }
            }
        }
    }

    public void destory()
    {
        isStart = false;
        // 启动所有充能粒子
        if (chargingParticles != null)
        {
            foreach (ParticleSystem particle in chargingParticles)
            {
                if (particle != null)
                {
                    particle.Stop();
                    particle.Clear();
                }
            }
        }
    }

    void UpdateParticlePositions()
    {
        // 更新所有充能粒子的位置，形成一个圆形排列
        for (int i = 0; i < numberOfParticles; i++)
        {
            float angle = i * 2 * Mathf.PI / numberOfParticles; // 计算角度，确保粒子均匀分布在圆周上
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius; // 计算粒子位置偏移量
            Vector3 targetPosition = player.transform.position + offset; // 计算粒子目标位置
            if (chargingParticles != null)
            {
                if (chargingParticles[i].transform != null)
                {
                    chargingParticles[i].transform.position = Vector3.MoveTowards(
                        chargingParticles[i].transform.position, targetPosition,
                        particleSpeed * Time.deltaTime); // 将粒子移动到目标位置
                    chargingParticles[i].transform.LookAt(player.transform); // 使粒子朝向玩家
                }
            }
        }
    }
}