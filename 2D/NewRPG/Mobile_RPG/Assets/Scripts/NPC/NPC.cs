using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    // NPC Info
    public int npcCode; // npc ������ �ڵ�
    public int cntQuestCode = -1; // ���� ����Ʈ �ڵ�

    public Dictionary<int, string[]> dicitonaryInfo = new Dictionary<int, string[]>(); // ��ȭ ���� - ��ȭ Bundle

    public int setDialogType; // � ��Ȳ�� ��ȭ �����̸� ���� �� ���ΰ�?
    public int cntDialogCode; // ���� ��ȭ�� ������ ���� �Ǿ��°�? (�̰��� ��ȭ�� ���� ��, ������ 0���� ���� �� �־�� �Ѵ�.)

    public TextMeshPro NPCName;

    LogManager playerLogManager;

    // UI Info
    public GameObject DialogPanel; // ��ȭ â
    public GameObject NextBtn; // ���� ��ư
    public GameObject OKBtn; // ���� ��ư
    public GameObject CancelBtn; // ��� ��ư

    public Text NameTxt; // ��ȭ ��ü �̸�
    public Text DialogTxt; // ��ȭ �ؽ�Ʈ

    

    // Start is called before the first frame update
    void Awake()
    {
        DialogPanel = GameObject.FindGameObjectWithTag("UIDialog").transform.GetChild(0).gameObject;
        NextBtn = DialogPanel.transform.GetChild(1).gameObject;
        NameTxt = DialogPanel.transform.GetChild(0).gameObject.GetComponent<Text>();
        DialogTxt = DialogPanel.transform.GetChild(2).gameObject.GetComponent<Text>();
        OKBtn = DialogPanel.transform.GetChild(3).gameObject;
        CancelBtn = DialogPanel.transform.GetChild(4).gameObject;

        CancelBtn.GetComponent<Button>().onClick.AddListener(CancelDialog);
        NextBtn.GetComponent<Button>().onClick.AddListener(ShowDialogUI);

        playerLogManager = GameObject.FindGameObjectWithTag("Player").GetComponent<LogManager>();
        NPCName.text = this.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTalkNPC()
    {
        if(cntQuestCode != -1)
        {
            CheckQuest();
            return;
        }
        ReadFile(setDialogType);
        ShowDialogUI();
    }

    void CancelDialog()
    {
        DialogPanel.SetActive(false);
        Time.timeScale = 1.0f;
        dicitonaryInfo.Clear(); // �ٷ� Ŭ��� �� �־�� �ٸ� ��Ȳ�� ��ȭ or �ٸ� NPC���� ��ȭ���� ������ �� ���� �� �ִ�.
        cntDialogCode = 0; // �ε����� 0���� �ʱ�ȭ �ϰ� �����Ѵ�.
    }

    void ShowDialogUI() // �̰��� NextBtn������ ��� ��
    {
        if (!DialogPanel.activeSelf)
            DialogPanel.SetActive(true);

        if (cntDialogCode >= dicitonaryInfo.Count) // index�� ��ųʸ� ũ��� �������� �Ǹ�
        {
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear(); // �ٷ� Ŭ��� �� �־�� �ٸ� ��Ȳ�� ��ȭ or �ٸ� NPC���� ��ȭ���� ������ �� ���� �� �ִ�.
            cntDialogCode = 0; // �ε����� 0���� �ʱ�ȭ �ϰ� �����Ѵ�.
            return;
        }

        // ����Ʈ ���� üũ
        if(dicitonaryInfo[cntDialogCode][0] == "1")
        {
            // ����Ʈ���..? -> ����Ʈ�� �׻� �������� �ش�!
            SetQuest(); // ����Ʈ ���� �ο�
            cntDialogCode = 0;
            setDialogType++;
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear(); // �ٷ� Ŭ��� �� �־�� �ٸ� ��Ȳ�� ��ȭ or �ٸ� NPC���� ��ȭ���� ������ �� ���� �� �ִ�.
            return;
        }
        else if(dicitonaryInfo[cntDialogCode][0] == "2")
        {
            // ���� �ο�
            GetReward();
            cntDialogCode = 0;
            setDialogType += 2;
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear();
            return;
        }

        // �б� ���� üũ


        NameTxt.text = dicitonaryInfo[cntDialogCode][4];
        DialogTxt.text = dicitonaryInfo[cntDialogCode][5];
        cntDialogCode++;
    }

    public void ReadFile(int inputType)
    {
        TextAsset csvFile;

        csvFile = Resources.Load("CSV/test") as TextAsset;

        StringReader reader = new StringReader(csvFile.text);

        while (reader.Peek() > -1)
        {
            string value = reader.ReadLine();
            var data_values = value.Split(',');

            if (data_values[1] == npcCode.ToString() && data_values[3] == inputType.ToString())  // NPC �ڵ�� �̸� ���� �� ��Ȳ ��ȣ�� ���� ��
            {
                // �ʿ� �������� ��ųʸ��� ����
                dicitonaryInfo.Add(int.Parse(data_values[2]), data_values); // ��ȭ ���� �߰�(�� �ٷ� ����!)
            }

        }

    }

    void CheckQuest()
    {
        // ����Ʈ �Ϸ� ���θ� ������ �Լ�

        int count = 0;

        // ��� Ȯ��
        for(int i=0;i< playerLogManager.playerLog.huntList.Count; i++)
        {
            if (playerLogManager.playerLog.huntList[i][4] == npcCode
                && playerLogManager.playerLog.huntList[i][0] == cntQuestCode
                && playerLogManager.playerLog.huntList[i][2] == playerLogManager.playerLog.huntList[i][3])
            {
                // �Ϸ��� NPC �ڵ尡 ��ġ + ���� �� ����Ʈ �ڵ�� ���� �� + n��° ��� ������ �������� ��
                count++;
            }
        }

        for(int i=0;i<playerLogManager.playerLog.gainList.Count; i++)
        {
            if (playerLogManager.playerLog.gainList[i][4] == npcCode
                && playerLogManager.playerLog.gainList[i][0] == cntQuestCode
                && playerLogManager.playerLog.gainList[i][2] == playerLogManager.playerLog.gainList[i][3])
            {
                // �Ϸ��� NPC �ڵ尡 ��ġ + ���� �� ����Ʈ �ڵ�� ���� �� + Ư�� �������� ��� ������ �������� ��
                count++;
            }
        }
        
        if(playerLogManager.playerLog.questComplete[cntQuestCode, 1] == count)
        {
            ReadFile(setDialogType);
            ShowDialogUI();
        }
        else
        {
            // ������ �������� �ʾ��� ��
            ReadFile(setDialogType+1);
            ShowDialogUI();
        }

    }
    
    void GetReward()
    {
        string value = dicitonaryInfo[cntDialogCode][5];
        var data_values = value.Split(':');

        int[] imsiArr = new int[3];

        for (int i = 0; i < data_values.Length; i++)
        {
            // ���� ����

            var values_info = data_values[i].Split('.');

            if (values_info[0] == "e")
            {
                // ����ġ ����

                playerLogManager.gameObject.GetComponent<PlayerStats>().playerStat.playerCntExperience += int.Parse(values_info[2]);

            }
            else if (values_info[0] == "g")
            {
                // ��� ����

                playerLogManager.gameObject.GetComponent<PlayerItem>().cntGold += int.Parse(values_info[2]);

            }else if(values_info[0] == "i")
            {
                // ������ ����



            }
        }
    }

    void SetQuest()
    {
        // ��ȭ ���� �Ľ�
        string value = dicitonaryInfo[cntDialogCode][5];
        var data_values = value.Split(':');

        int[] imsiArr = new int[5];

        for(int i = 0; i < data_values.Length; i++)
        {
            var values_info = data_values[i].Split('.');

            if (playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] != 0)
            {
                break; // �� ���� ��ȭ���� �� ������ ����Ʈ�� �־����� ������ ���� ����Ʈ �ڵ� �ڸ��� �ִ� ���� 0(����Ʈ ���� ��)�� �ƴϸ� ����Ʈ�޴� ���� ������.(�����ġ)
            }

            if(values_info[0] == "h")
            {
                // ���� ��� �̼��̶��

                imsiArr[0] = int.Parse(values_info[3]); // ����Ʈ �ڵ�
                imsiArr[1] = int.Parse(values_info[1]); // ��ƾ� �� ���� �ڵ�
                imsiArr[2] = int.Parse(values_info[2]); // ��ƾ� �� ���� ������
                imsiArr[3] = 0; // ���� ���� ������ (����Ʈ�� �޴� �������� ������ 0���� �ʱ�ȭ �� ��� �Ѵ�.)
                imsiArr[4] = int.Parse(values_info[4]); // ����Ʈ�� �Ϸ� �� npc �ڵ�

                playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] = 1;
                playerLogManager.playerLog.questComplete[int.Parse(values_info[3]), 1]++; // ����Ʈ ���� ī��Ʈ �߰�
                cntQuestCode = int.Parse(values_info[3]);
                playerLogManager.playerLog.huntList.Add(imsiArr);

            }
            else if(values_info[0] == "g")
            {
                // ȹ�� �̼��̶��

                imsiArr[0] = int.Parse(values_info[3]); // ����Ʈ �ڵ�
                imsiArr[1] = int.Parse(values_info[1]); // ȹ���ؾ� �� ������ �ڵ�
                imsiArr[2] = int.Parse(values_info[2]); // ȹ���ؾ� �� ������ ����
                imsiArr[3] = 0; // ���� ���� ������ ���� -> ����Ʈ �������̶�� 0���� �ʱ�ȭ�� ������ ������ �ִ� �������̶�� �������� ���� �;� �Ѵ�.
                                // �̰� PlayerItem �ڵ忡�� ������ �ڵ带 ������ ������ ��� �� �ִ� �Լ��� ������ �� �� ����. (���߿� ����!)
                imsiArr[4] = int.Parse(values_info[4]); // ����Ʈ�� �Ϸ� �� npc �ڵ�

                playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] = 1;
                cntQuestCode = int.Parse(values_info[3]);
                playerLogManager.playerLog.gainList.Add(imsiArr);

            }
        }
      
    }

}
