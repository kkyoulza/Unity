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

    // Bar ���� ����
    RectTransform HPRect;
    RectTransform MPRect;
    RectTransform EXPRect;

    // ��ġ ����
    public int ExpBarLength; // ����ġ �� ���� ����
    float expRate; // ����ġ ����

    StatInformation stat; // �÷��̾� ����
    GameObject player;

    // �ؽ�Ʈ ����
    public Text ExpText;
    public Text HPText;
    public Text MPText;
    public Text LVText;

    public Text MonsterCount;
    public Text QuestCnt;

    public Text DialogNameTxt;
    public Text DialogTxt;

    public int setDialogType; // � ��ȭ ������ ��ȭ �������� ���� ���� �� ������ üũ�ϴ� ��
    public Dictionary<int, string> dialogDictionary = new Dictionary<int, string>(); // ��ȭ ������ ���� ��ȭ ���� ����
    public Dictionary<int, string> nameDictionary = new Dictionary<int, string>(); // ��ȭ ������ ���� �̸� ����
    public int cntDialogCode; // ���� ��ȭ�� ������ ���� �Ǿ��°�?

    // ���� Prefab
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

                if(data_values[3] == setDialogType.ToString()) // ��ųʸ��� ����
                {
                    dialogDictionary.Add(int.Parse(data_values[2]), data_values[5]);
                    nameDictionary.Add(int.Parse(data_values[2]), data_values[4]);
                }
            }
            reader.Close();
        }
        else
            value = "������ �����ϴ�.";

        Debug.Log(nameDictionary.Count);

        return;
    }

    public void showDialog()
    {
        // ��ųʸ��� ����� ��ȭ ������ ����ϴ� �޼���

        if (cntDialogCode >= nameDictionary.Count) // index�� ��ųʸ� ũ��� �������� �Ǹ�
        {
            cntDialogCode = 0; // �ε����� 0���� �ʱ�ȭ �ϰ� �����Ѵ�.
            return;
        }
            
        DialogNameTxt.text = nameDictionary[cntDialogCode];
        DialogTxt.text = dialogDictionary[cntDialogCode];
        cntDialogCode++;
    }

}
