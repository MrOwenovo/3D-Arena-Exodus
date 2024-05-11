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

    public Text WINtext;
    public Text LOSEText;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
