using System.Collections;
using UnityEngine;

public class ChargingParticlesController : MonoBehaviour
{
    public static ChargingParticlesController instance;
    public GameObject player;  
    public float radius = 2f;  
    public int numberOfParticles = 50; 
    public float particleSpeed = 0.5f; 
    public ParticleSystem chargingParticlesPrefab;  

    private ParticleSystem[] chargingParticles;  
    public bool isStart = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (isStart)
        {
            chargingParticles = new ParticleSystem[numberOfParticles]; 
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
            UpdateParticlePositions();
        }
    }

    public void PlayParticles()
    {
        isStart = true;
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
        for (int i = 0; i < numberOfParticles; i++)
        {
            float angle = i * 2 * Mathf.PI / numberOfParticles;  
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;  
            Vector3 targetPosition = player.transform.position + offset;  
            if (chargingParticles != null)
            {
                if (chargingParticles[i].transform != null)
                {
                    chargingParticles[i].transform.position = Vector3.MoveTowards(
                        chargingParticles[i].transform.position, targetPosition,
                        particleSpeed * Time.deltaTime);  
                    chargingParticles[i].transform.LookAt(player.transform);  
                }
            }
        }
    }
}