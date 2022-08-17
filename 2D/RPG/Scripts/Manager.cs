using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // Start is called before the first frame update

    public TalkManager talkManager; // 스크립트를 변수로 채택!
    public GameObject talkPanel;
    public Image portraitImg;

    public QuestManager questManager;
    public GameObject MenuSet;
    public GameObject StateSet; // 스탯 창

    public GameObject player;

    public Text NoticeQuest;
    private string QuestState;

    public Text PowerTxt; // power 스탯 텍스트
    public Text AccTxt; // Acc 스탯 텍스트
    public Text PointTxt; // 남은 포인트 텍스트

    public Text MaxAtk;
    public Text MinAtk;

    CharactorStats StatOption;

    public Text talkText;
    public int talkIndex;
    public GameObject scanObject; // 플레이어가 조사한 오브젝트
    public bool isAction; // 상태 저장용 변수

    private void Start()
    {
        GameLoad();
        QuestState = "진행 중인 퀘스트 : " + questManager.CheckQuest();
        NoticeQuest.text = QuestState;

        StatOption = player.GetComponent<CharactorStats>();

        StatOption.SetPoint(10);
        StatOption.SetPower(5);
        StatOption.SetAcc(40);

        PowerTxt.text = StatOption.GetPower().ToString();
        PointTxt.text = StatOption.GetPoint().ToString();

        AccTxt.text = StatOption.GetAcc().ToString();

        MaxAtk.text = StatOption.CalAndGetMaxAtk().ToString();
        MinAtk.text = StatOption.CalAndGetMinAtk().ToString();

    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (MenuSet.activeSelf) // 이미 켜져 있으면 끄게끔!
            {
                MenuSet.SetActive(false);
            }
            else
            {
                MenuSet.SetActive(true);
            }
            
        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            if(StateSet.activeSelf)
            {
                StateSet.SetActive(false);
            }
            else
            {
                StateSet.SetActive(true);
            }
        }


        PowerTxt.text = StatOption.GetPower().ToString();
        PointTxt.text = StatOption.GetPoint().ToString();
        AccTxt.text = StatOption.GetAcc().ToString();

        MaxAtk.text = StatOption.CalAndGetMaxAtk().ToString();
        MinAtk.text = StatOption.CalAndGetMinAtk().ToString();

    }

    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        ObjectData objData = scanObj.GetComponent<ObjectData>();
        // 각 NPC에 넣었던 ObjectDate라는 것이 컴포넌트화 되었다
        // 각 객체에 있는 컴포넌트를 가져올 때!
        // TalkManager는 통합적으로 관리를 하는 것이기에 하나만 존재하지만 ObjectData는 각각에 다 존재한다. 따라서 각각마다 달라질 수 있기에
        // 드래그 앤 드랍으로 지정하지 않고 GetComponent를 통하여 코드를 통해서 지정하는 것이다.
        // TalkManager도 코드로 가져올 수 있긴 하다. TalkingManager 오브벡트를 가져온 뒤에 거기서 GetComponent... 복잡하네
        Talk(objData.id, objData.isNPC);

        talkPanel.SetActive(isAction); // 상태변수에 따라서 상태가 변하기에 깔끔하게 만들어 주었음

    }

    void Talk(int id,bool isNPC)
    {
        int questTalkIndex = questManager.GetQuestTalkIndex(id);
        string talkData = talkManager.GetTalk(id+questTalkIndex, talkIndex); // 여기서 인덱스를 관리한다.(퀘스트 여부, 초상화 등)

        if(talkData == null) // 대화가 끝났을 때!
        {
            isAction = false;
            talkIndex = 0;
            QuestState = "진행 중인 퀘스트 : " + questManager.CheckQuest(id);
            NoticeQuest.text = QuestState;
            return; // 여기서 함수의 진행이 끝남(void함수에서 강제 종료하는 역할)
        }

        if (isNPC)
        {
            talkText.text = talkData.Split(':')[0]; // split을 하면 문자열의 배열이 되기에 인덱스도 같이 적어 주어야 한다!

            portraitImg.sprite = talkManager.GetPortrait(id,int.Parse(talkData.Split(':')[1]));
            portraitImg.color = new Color(1, 1, 1, 1);
        }
        else
        {
            talkText.text = talkData;

            portraitImg.color = new Color(1, 1, 1, 0);
        }

        isAction = true; // 대화가 진행될 때는 true!
        talkIndex++;
    }

    public void GameSave()
    {
        // 필수적인 정보들을 저장하자! (플레이어의 위치, 퀘스트 진행 상황 등)
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetInt("QuestId", questManager.questId);
        PlayerPrefs.SetInt("QuestActionIndex", questManager.questActionIndex);
        PlayerPrefs.Save();

        // 정보들은 빌드 세팅에서 플레이어 세팅에 있는 회사 이름과 게임 이름으로 레지스트리에 저장되게 된다.

        MenuSet.SetActive(false);
    }

    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("PlayerX")) // 키가 존재하는가???? 아니라면 Load를 하지 마라(예외처리)
            return;


        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        int questId = PlayerPrefs.GetInt("QuestId");
        int questActionIndex = PlayerPrefs.GetInt("QuestActionIndex");


        player.transform.position = new Vector3(x, y, 0);
        questManager.questId = questId;
        questManager.questActionIndex = questActionIndex;
        questManager.ControlObject(); // 오브젝트를 다시 설정! -> 퀘스트 진행도에 따라서 오브젝트의 생성 여부도 결정되기 때문에 로드에서 한번 더 챙겨 준다.

    }

    public void GameExit()
    {
        Application.Quit();
    }

}
