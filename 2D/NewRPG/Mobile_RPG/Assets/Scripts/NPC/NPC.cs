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
    public int cntQuestCode; // ���� ����Ʈ �ڵ�

    public Dictionary<int, string[]> dicitonaryInfo = new Dictionary<int, string[]>(); // ��ȭ ���� - ��ȭ Bundle

    public int setDialogType; // � ��Ȳ�� ��ȭ �����̸� ���� �� ���ΰ�?
    public int cntDialogCode; // ���� ��ȭ�� ������ ���� �Ǿ��°�? (�̰��� ��ȭ�� ���� ��, ������ 0���� ���� �� �־�� �Ѵ�.)

    public TextMeshPro NPCName;

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

        NPCName.text = this.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTalkNPC()
    {
        ReadFile(Application.dataPath + "/test.csv");
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

        // �б� ���� üũ


        NameTxt.text = dicitonaryInfo[cntDialogCode][4];
        DialogTxt.text = dicitonaryInfo[cntDialogCode][5];
        cntDialogCode++;
    }

    public void ReadFile(string filePath)
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

                if (data_values[1] == npcCode.ToString() && data_values[3] == setDialogType.ToString())  // NPC �ڵ�� �̸� ���� �� ��Ȳ ��ȣ�� ���� ��
                {
                    // �ʿ� �������� ��ųʸ��� ����
                    dicitonaryInfo.Add(int.Parse(data_values[2]), data_values); // ��ȭ ���� �߰�(�� �ٷ� ����!)
                }
            }
            reader.Close();
        }
        else
            value = "������ �����ϴ�.";

        Debug.Log(dicitonaryInfo.Count);

        return;
    }

    void SetQuest()
    {
        // ��ȭ ���� �Ľ�
        string value = dicitonaryInfo[cntDialogCode][5];
        var data_values = value.Split(':');

        int[] imsiArr = new int[5];

        for(int i = 0; i < data_values.Length; i++)
        {
            LogManager playerLogManager = GameObject.FindGameObjectWithTag("Player").GetComponent<LogManager>();
            var values_info = data_values[i].Split('.');
            if(values_info[0] == "h")
            {
                // ���� ��� �̼��̶��

                imsiArr[0] = int.Parse(values_info[3]); // ����Ʈ �ڵ�
                imsiArr[1] = int.Parse(values_info[1]); // ��ƾ� �� ���� �ڵ�
                imsiArr[2] = int.Parse(values_info[2]); // ��ƾ� �� ���� ������
                imsiArr[3] = 0; // ���� ���� ������ (����Ʈ�� �޴� �������� ������ 0���� �ʱ�ȭ �� ��� �Ѵ�.)
                imsiArr[4] = int.Parse(values_info[4]); // ����Ʈ�� �Ϸ� �� npc �ڵ�

                playerLogManager.playerLog.huntList.Add(imsiArr);

                Debug.Log(playerLogManager.playerLog.huntList[0][0]);
                Debug.Log(playerLogManager.playerLog.huntList[0][1]);
                Debug.Log(playerLogManager.playerLog.huntList[0][2]);
                Debug.Log(playerLogManager.playerLog.huntList[0][3]);
                Debug.Log(playerLogManager.playerLog.huntList[0][4]);

            }
            else if(values_info[i][0] == 'g')
            {
                // ȹ�� �̼��̶��

                imsiArr[0] = int.Parse(values_info[3]); // ����Ʈ �ڵ�
                imsiArr[1] = int.Parse(values_info[1]); // ȹ���ؾ� �� ������ �ڵ�
                imsiArr[2] = int.Parse(values_info[2]); // ȹ���ؾ� �� ������ ����
                imsiArr[3] = 0; // ���� ���� ������ ���� -> ����Ʈ �������̶�� 0���� �ʱ�ȭ�� ������ ������ �ִ� �������̶�� �������� ���� �;� �Ѵ�.
                                // �̰� PlayerItem �ڵ忡�� ������ �ڵ带 ������ ������ ��� �� �ִ� �Լ��� ������ �� �� ����. (���߿� ����!)
                imsiArr[4] = int.Parse(values_info[4]); // ����Ʈ�� �Ϸ� �� npc �ڵ�

                playerLogManager.playerLog.gainList.Add(imsiArr);

            }
        }

        

        
    }

}
