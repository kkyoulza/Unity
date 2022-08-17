using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // Start is called before the first frame update

    public TalkManager talkManager; // ��ũ��Ʈ�� ������ ä��!
    public GameObject talkPanel;
    public Image portraitImg;

    public QuestManager questManager;
    public GameObject MenuSet;
    public GameObject StateSet; // ���� â

    public GameObject player;

    public Text NoticeQuest;
    private string QuestState;

    public Text PowerTxt; // power ���� �ؽ�Ʈ
    public Text AccTxt; // Acc ���� �ؽ�Ʈ
    public Text PointTxt; // ���� ����Ʈ �ؽ�Ʈ

    public Text MaxAtk;
    public Text MinAtk;

    CharactorStats StatOption;

    public Text talkText;
    public int talkIndex;
    public GameObject scanObject; // �÷��̾ ������ ������Ʈ
    public bool isAction; // ���� ����� ����

    private void Start()
    {
        GameLoad();
        QuestState = "���� ���� ����Ʈ : " + questManager.CheckQuest();
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
            if (MenuSet.activeSelf) // �̹� ���� ������ ���Բ�!
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
        // �� NPC�� �־��� ObjectDate��� ���� ������Ʈȭ �Ǿ���
        // �� ��ü�� �ִ� ������Ʈ�� ������ ��!
        // TalkManager�� ���������� ������ �ϴ� ���̱⿡ �ϳ��� ���������� ObjectData�� ������ �� �����Ѵ�. ���� �������� �޶��� �� �ֱ⿡
        // �巡�� �� ������� �������� �ʰ� GetComponent�� ���Ͽ� �ڵ带 ���ؼ� �����ϴ� ���̴�.
        // TalkManager�� �ڵ�� ������ �� �ֱ� �ϴ�. TalkingManager ���꺤Ʈ�� ������ �ڿ� �ű⼭ GetComponent... �����ϳ�
        Talk(objData.id, objData.isNPC);

        talkPanel.SetActive(isAction); // ���º����� ���� ���°� ���ϱ⿡ ����ϰ� ����� �־���

    }

    void Talk(int id,bool isNPC)
    {
        int questTalkIndex = questManager.GetQuestTalkIndex(id);
        string talkData = talkManager.GetTalk(id+questTalkIndex, talkIndex); // ���⼭ �ε����� �����Ѵ�.(����Ʈ ����, �ʻ�ȭ ��)

        if(talkData == null) // ��ȭ�� ������ ��!
        {
            isAction = false;
            talkIndex = 0;
            QuestState = "���� ���� ����Ʈ : " + questManager.CheckQuest(id);
            NoticeQuest.text = QuestState;
            return; // ���⼭ �Լ��� ������ ����(void�Լ����� ���� �����ϴ� ����)
        }

        if (isNPC)
        {
            talkText.text = talkData.Split(':')[0]; // split�� �ϸ� ���ڿ��� �迭�� �Ǳ⿡ �ε����� ���� ���� �־�� �Ѵ�!

            portraitImg.sprite = talkManager.GetPortrait(id,int.Parse(talkData.Split(':')[1]));
            portraitImg.color = new Color(1, 1, 1, 1);
        }
        else
        {
            talkText.text = talkData;

            portraitImg.color = new Color(1, 1, 1, 0);
        }

        isAction = true; // ��ȭ�� ����� ���� true!
        talkIndex++;
    }

    public void GameSave()
    {
        // �ʼ����� �������� ��������! (�÷��̾��� ��ġ, ����Ʈ ���� ��Ȳ ��)
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetInt("QuestId", questManager.questId);
        PlayerPrefs.SetInt("QuestActionIndex", questManager.questActionIndex);
        PlayerPrefs.Save();

        // �������� ���� ���ÿ��� �÷��̾� ���ÿ� �ִ� ȸ�� �̸��� ���� �̸����� ������Ʈ���� ����ǰ� �ȴ�.

        MenuSet.SetActive(false);
    }

    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("PlayerX")) // Ű�� �����ϴ°�???? �ƴ϶�� Load�� ���� ����(����ó��)
            return;


        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        int questId = PlayerPrefs.GetInt("QuestId");
        int questActionIndex = PlayerPrefs.GetInt("QuestActionIndex");


        player.transform.position = new Vector3(x, y, 0);
        questManager.questId = questId;
        questManager.questActionIndex = questActionIndex;
        questManager.ControlObject(); // ������Ʈ�� �ٽ� ����! -> ����Ʈ ���൵�� ���� ������Ʈ�� ���� ���ε� �����Ǳ� ������ �ε忡�� �ѹ� �� ì�� �ش�.

    }

    public void GameExit()
    {
        Application.Quit();
    }

}
