using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // UI Bar
    public GameObject HPBar;
    public GameObject MPBar;
    public GameObject EXPBar;

    // Bar 길이 정보
    RectTransform HPRect;
    RectTransform MPRect;
    RectTransform EXPRect;

    // 수치 정보
    public int ExpBarLength; // 경험치 바 가로 길이
    float expRate; // 경험치 비율

    StatInformation stat; // 플레이어 스탯

    // 텍스트 모음
    public Text ExpText;
    public Text HPText;
    public Text MPText;
    public Text LVText;

    // Start is called before the first frame update
    void Awake()
    {
        HPRect = HPBar.GetComponent<RectTransform>();
        MPRect = MPBar.GetComponent<RectTransform>();
        EXPRect = EXPBar.GetComponent<RectTransform>();
    }

    private void Start()
    {
        stat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().playerStat;
    }

    // Update is called once per frame
    void Update()
    {
        SetEXPBar();
        SetHPMP();
        SetLV();
    }

    void SetEXPBar()
    {
        expRate = ((float)stat.playerCntExperience / (float)stat.playerMaxExperience);
        EXPRect.sizeDelta = new Vector2(ExpBarLength * expRate, 50);
        ExpText.text = stat.playerCntExperience + " ( " + string.Format("{0:0.00}", expRate * 100) + "% ) / " + stat.playerMaxExperience;
    }

    void SetHPMP()
    {
        HPRect.sizeDelta = new Vector2(250 * (stat.playerCntHP / stat.playerMaxHP), 60);
        MPRect.sizeDelta = new Vector2(250 * (stat.playerCntMP / stat.playerMaxMP), 60);

        HPText.text = stat.playerCntHP + " / " + stat.playerMaxHP;
        MPText.text = stat.playerCntMP + " / " + stat.playerMaxMP;

    }

    void SetLV()
    {
        LVText.text = "LV " + stat.playerLevel;
    }
}
