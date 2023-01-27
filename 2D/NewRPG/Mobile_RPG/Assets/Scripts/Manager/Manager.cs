using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

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
    GameObject player;

    // 텍스트 모음
    public Text ExpText;
    public Text HPText;
    public Text MPText;
    public Text LVText;

    public Text MonsterCount;
    public Text QuestCnt;

    public Text DialogNameTxt;
    public Text DialogTxt;

    public int setDialogType; // 어떤 대화 종류의 대화 시퀀스를 전부 가져 올 것인지 체크하는 것
    public Dictionary<int, string> dialogDictionary = new Dictionary<int, string>(); // 대화 순서에 따른 대화 내용 저장
    public Dictionary<int, string> nameDictionary = new Dictionary<int, string>(); // 대화 순서에 따른 이름 저장
    public int cntDialogCode; // 현재 대화가 어디까지 진행 되었는가?

    // 몬스터 Prefab
    public GameObject testMonster;

    // Start is called before the first frame update
    void Awake()
    {
        HPRect = HPBar.GetComponent<RectTransform>();
        MPRect = MPBar.GetComponent<RectTransform>();
        EXPRect = EXPBar.GetComponent<RectTransform>();
        ReadFile(Application.dataPath + "/test.csv");
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        stat = player.GetComponent<PlayerStats>().playerStat;
    }

    // Update is called once per frame
    void Update()
    {
        SetEXPBar();
        SetHPMP();
        SetLV();
        MonsterCount.text = player.GetComponent<LogManager>().playerLog.returnMonsterCount(1).ToString();
        if(player.GetComponent<LogManager>().playerLog.huntList.Count != 0)
            QuestCnt.text = player.GetComponent<LogManager>().playerLog.huntList[0][3] + "/" + player.GetComponent<LogManager>().playerLog.huntList[0][2];
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

    public void TestDialog()
    {
        Time.timeScale = 0;

    }

    void ReadFile(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        string value = "";

        if (fileInfo.Exists)
        {
            bool isEnd = false;
            StreamReader reader = new StreamReader(filePath);
            while (!isEnd)
            {
                value = reader.ReadLine();
                if (value == null)
                {
                    isEnd = true;
                    break;
                }
                var data_values = value.Split(',');

                if(data_values[3] == setDialogType.ToString()) // 딕셔너리에 저장
                {
                    dialogDictionary.Add(int.Parse(data_values[2]), data_values[5]);
                    nameDictionary.Add(int.Parse(data_values[2]), data_values[4]);
                }
            }
            reader.Close();
        }
        else
            value = "파일이 없습니다.";

        Debug.Log(nameDictionary.Count);

        return;
    }

    public void showDialog()
    {
        // 딕셔너리에 저장된 대화 내용을 출력하는 메서드

        if (cntDialogCode >= nameDictionary.Count) // index가 딕셔너리 크기랑 같아지게 되면
        {
            cntDialogCode = 0; // 인덱스를 0으로 초기화 하고 리턴한다.
            return;
        }
            
        DialogNameTxt.text = nameDictionary[cntDialogCode];
        DialogTxt.text = dialogDictionary[cntDialogCode];
        cntDialogCode++;
    }

}
