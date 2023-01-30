using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    // NPC Info
    public int npcCode; // npc 고유의 코드
    public int cntQuestCode = -1; // 현재 퀘스트 코드

    public Dictionary<int, string[]> dicitonaryInfo = new Dictionary<int, string[]>(); // 대화 순서 - 대화 Bundle

    public int setDialogType; // 어떤 상황의 대화 뭉탱이를 가져 올 것인가?
    public int cntDialogCode; // 현재 대화가 어디까지 진행 되었는가? (이것은 대화를 끝낼 때, 무조건 0으로 세팅 해 주어야 한다.)

    public TextMeshPro NPCName;

    LogManager playerLogManager;

    // UI Info
    public GameObject DialogPanel; // 대화 창
    public GameObject NextBtn; // 다음 버튼
    public GameObject OKBtn; // 수락 버튼
    public GameObject CancelBtn; // 취소 버튼

    public Text NameTxt; // 대화 주체 이름
    public Text DialogTxt; // 대화 텍스트

    

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
        dicitonaryInfo.Clear(); // 바로 클리어를 해 주어야 다른 상황의 대화 or 다른 NPC와의 대화에서 파일을 잘 읽을 수 있다.
        cntDialogCode = 0; // 인덱스를 0으로 초기화 하고 리턴한다.
    }

    void ShowDialogUI() // 이것은 NextBtn에서도 사용 됨
    {
        if (!DialogPanel.activeSelf)
            DialogPanel.SetActive(true);

        if (cntDialogCode >= dicitonaryInfo.Count) // index가 딕셔너리 크기랑 같아지게 되면
        {
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear(); // 바로 클리어를 해 주어야 다른 상황의 대화 or 다른 NPC와의 대화에서 파일을 잘 읽을 수 있다.
            cntDialogCode = 0; // 인덱스를 0으로 초기화 하고 리턴한다.
            return;
        }

        // 퀘스트 여부 체크
        if(dicitonaryInfo[cntDialogCode][0] == "1")
        {
            // 퀘스트라면..? -> 퀘스트는 항상 마지막에 준다!
            SetQuest(); // 퀘스트 정보 부여
            cntDialogCode = 0;
            setDialogType++;
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear(); // 바로 클리어를 해 주어야 다른 상황의 대화 or 다른 NPC와의 대화에서 파일을 잘 읽을 수 있다.
            return;
        }
        else if(dicitonaryInfo[cntDialogCode][0] == "2")
        {
            // 보상 부여
            GetReward();
            cntDialogCode = 0;
            setDialogType += 2;
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear();
            return;
        }

        // 분기 여부 체크


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

            if (data_values[1] == npcCode.ToString() && data_values[3] == inputType.ToString())  // NPC 코드와 미리 세팅 된 상황 번호가 같을 때
            {
                // 필요 정보만을 딕셔너리에 저장
                dicitonaryInfo.Add(int.Parse(data_values[2]), data_values); // 대화 번들 추가(한 줄로 전부!)
            }

        }

    }

    void CheckQuest()
    {
        // 퀘스트 완료 여부를 따지는 함수

        int count = 0;

        // 사냥 확인
        for(int i=0;i< playerLogManager.playerLog.huntList.Count; i++)
        {
            if (playerLogManager.playerLog.huntList[i][4] == npcCode
                && playerLogManager.playerLog.huntList[i][0] == cntQuestCode
                && playerLogManager.playerLog.huntList[i][2] == playerLogManager.playerLog.huntList[i][3])
            {
                // 완료할 NPC 코드가 일치 + 설정 된 퀘스트 코드랑 맞을 때 + n번째 사냥 조건을 만족했을 때
                count++;
            }
        }

        for(int i=0;i<playerLogManager.playerLog.gainList.Count; i++)
        {
            if (playerLogManager.playerLog.gainList[i][4] == npcCode
                && playerLogManager.playerLog.gainList[i][0] == cntQuestCode
                && playerLogManager.playerLog.gainList[i][2] == playerLogManager.playerLog.gainList[i][3])
            {
                // 완료할 NPC 코드가 일치 + 설정 된 퀘스트 코드랑 맞을 때 + 특정 아이템을 얻는 조건을 만족했을 때
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
            // 조건을 만족하지 않았을 때
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
            // 보상 종류

            var values_info = data_values[i].Split('.');

            if (values_info[0] == "e")
            {
                // 경험치 보상

                playerLogManager.gameObject.GetComponent<PlayerStats>().playerStat.playerCntExperience += int.Parse(values_info[2]);

            }
            else if (values_info[0] == "g")
            {
                // 골드 보상

                playerLogManager.gameObject.GetComponent<PlayerItem>().cntGold += int.Parse(values_info[2]);

            }else if(values_info[0] == "i")
            {
                // 아이템 증가



            }
        }
    }

    void SetQuest()
    {
        // 대화 내용 파싱
        string value = dicitonaryInfo[cntDialogCode][5];
        var data_values = value.Split(':');

        int[] imsiArr = new int[5];

        for(int i = 0; i < data_values.Length; i++)
        {
            var values_info = data_values[i].Split('.');

            if (playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] != 0)
            {
                break; // 한 개의 대화에서 한 종류의 퀘스트만 주어지기 때문에 만약 퀘스트 코드 자리에 있는 값이 0(퀘스트 수행 전)이 아니면 퀘스트받는 것을 끝낸다.(방어장치)
            }

            if(values_info[0] == "h")
            {
                // 만약 사냥 미션이라면

                imsiArr[0] = int.Parse(values_info[3]); // 퀘스트 코드
                imsiArr[1] = int.Parse(values_info[1]); // 잡아야 할 몬스터 코드
                imsiArr[2] = int.Parse(values_info[2]); // 잡아야 할 몬스터 마릿수
                imsiArr[3] = 0; // 현재 잡은 마릿수 (퀘스트를 받는 시점에는 무조건 0으로 초기화 해 줘야 한다.)
                imsiArr[4] = int.Parse(values_info[4]); // 퀘스트를 완료 할 npc 코드

                playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] = 1;
                playerLogManager.playerLog.questComplete[int.Parse(values_info[3]), 1]++; // 퀘스트 조건 카운트 추가
                cntQuestCode = int.Parse(values_info[3]);
                playerLogManager.playerLog.huntList.Add(imsiArr);

            }
            else if(values_info[0] == "g")
            {
                // 획득 미션이라면

                imsiArr[0] = int.Parse(values_info[3]); // 퀘스트 코드
                imsiArr[1] = int.Parse(values_info[1]); // 획득해야 할 아이템 코드
                imsiArr[2] = int.Parse(values_info[2]); // 획득해야 할 아이템 개수
                imsiArr[3] = 0; // 현재 얻은 아이템 개수 -> 퀘스트 아이템이라면 0으로 초기화가 맞지만 기존에 있던 아이템이라면 아이템을 가져 와야 한다.
                                // 이건 PlayerItem 코드에서 아이템 코드를 넣으면 개수를 출력 해 주는 함수를 만들어야 할 것 같다. (나중에 수정!)
                imsiArr[4] = int.Parse(values_info[4]); // 퀘스트를 완료 할 npc 코드

                playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] = 1;
                cntQuestCode = int.Parse(values_info[3]);
                playerLogManager.playerLog.gainList.Add(imsiArr);

            }
        }
      
    }

}
