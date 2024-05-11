using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;


    public GameObject enemyBarsUI;
    public Canvas MenuCanvas;
    public Canvas GameCanvas;
    public Canvas PauseCanvas;
    public Canvas OverCanvas;

    public Image SPbar;
    public Image HPbar;
    public Image Exbar;

    public Text TimeValue;
    public Text HPValue;
    public Text SPValue;
    public Text EXPValue;

    public Text EnemyKilled;
    public Text PlayerLevel;
    public Text PlayerCoin;

    public Text WINtext;
    public Text LOSEText;

    private void Awake()
    {
        instance = this;
        UpdateCoinsDisplay(PlayerPrefs.GetInt("SkillCoinsCollected", 0));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void ShowCoinCollectedAnimation(Vector3 position)
    {
        GameObject effect = new GameObject("CoinCollectEffect");
        effect.transform.position = position;
        ParticleSystem particleSystem = effect.AddComponent<ParticleSystem>();
        ConfigureParticleEffect(particleSystem);
        Destroy(effect, 2.0f);  // Adjust time as needed for the duration of the effect
    }
    void ConfigureParticleEffect(ParticleSystem ps)
    {
        // 停止粒子系统并清除现有粒子，确保可以安全修改配置
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // 配置粒子系统
        ParticleSystem.MainModule main = ps.main;
        main.startColor = Color.yellow;
        main.startSize = 0.2f;
        main.startLifetime = 1.0f;
        main.duration = 1.0f;  // 现在可以安全地设置持续时间
        main.loop = false;

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 0;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0.0f, 50);
        emission.SetBursts(new[] { burst });

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        // 重新开始播放粒子系统
        ps.Play();
    }

    public void UpdateCoinsDisplay(int coins)
    {
        PlayerCoin.text = "Coins: " + coins.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.curStatus == Status.Game)
        {
            SPbar.fillAmount = PlayerController.instance.curSP / PlayerController.instance.playerData.MaxSp;
            SPValue.text = PlayerController.instance.curSP.ToString()+" / "+  PlayerController.instance.playerData.MaxSp.ToString();
            HPbar.fillAmount = PlayerController.instance.playerData.CurHealth / PlayerController.instance.playerData.MaxHealth;
            HPValue.text = PlayerController.instance.playerData.CurHealth.ToString()+ " / " + PlayerController.instance.playerData.MaxHealth.ToString();
            Exbar.fillAmount = PlayerController.instance.playerData.CurExp / PlayerController.instance.playerData.MaxExp;
            EXPValue.text = PlayerController.instance.playerData.CurExp.ToString()+ " / "+ PlayerController.instance.playerData.MaxExp.ToString();

            int minute = (int)GameManager.instance.uiData.SurvivalTime / 60;
            int second = (int)GameManager.instance.uiData.SurvivalTime % 60;

            TimeValue.text = minute.ToString("00")+" : " + second.ToString("00");
            EnemyKilled.text = "Enemy Killed : "+ GameManager.instance.uiData.EnemyKillNum.ToString("000");
            PlayerLevel.text = "Level : " + PlayerController.instance.playerData.level.ToString("00");



        }
        
    }
}
