using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex; // ����Ʈ ������ ���ϴ� ��!
    public GameObject[] questObject; // ����Ʈ�� �ʿ��� ������Ʈ���� ��� �� ��!

    Dictionary<int, QuestData> questList;

    // Start is called before the first frame update
    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    // Update is called once per frame
    void GenerateData()
    {
        questList.Add(10, new QuestData("����� ������ ��!", new int[] { 1000,2000 }));
        questList.Add(20, new QuestData("�������� ��Ź", new int[] { 3000, 2000 }));
        questList.Add(30, new QuestData("Ŭ����!", new int[] { 0 }));
    }

    public int GetQuestTalkIndex(int id)
    {
        return questId + questActionIndex; // ����Ʈ ��ȣ + ��ȭ ���� = ����Ʈ ��ȭ ID
    }

    public string CheckQuest(int id)
    {
        
        if(id == questList[questId].npcId[questActionIndex]) // NPC�� id�� �޾Ƽ� id�� ���� ���� ���� ����Ʈ�� ������ ������ �´� NPC�� ��, ���� ������ ����ǰ� �Ѵ�.(��ȭ�� id�� �ø����ν�!)
        {
            // ��ȭ�� ������ �� ���൵�� �ϳ��� �ø��� ����! -> ��ȭ�� ���� ���°� Manager���� ����!
            questActionIndex++;
        }

        // Control Quest Object -> ����Ʈ�� ���õ� Object�� ������ �����ϴ� �۾�!
        ControlObject();

        if (questActionIndex == questList[questId].npcId.Length)
        {
            NextQuest(); // ��ȭ ������ ���� �������� �� ���� ����Ʈ��!
        }

        return questList[questId].questName;

    }

    public string CheckQuest() // �� ó���� id ������ ���� �ʰ��� �ʱ� ����Ʈ�� �̸��� ����� �� �ְ� �ϱ� ����
    {
        // �����ε��� Ȱ���Ͽ� ���� �Լ� �̸��� �Ű����� ���̷� �����Ͽ� �����ϰ� �Ǵ� ���̴�.
        return questList[questId].questName;

    }

    void NextQuest()
    {
        questId += 10;
        questActionIndex = 0;
    }

    public void ControlObject() // ����Ʈ�� ���õ� ������Ʈ�� ����
    {
        switch (questId)
        {
            case 10:
                if(questActionIndex == 2)
                {
                    questObject[0].SetActive(true); // ã�ƴ޶�� ��ȭ�� ������ �ٷ� ������ �����Բ�!
                }
                break;
            case 20:
                if (questActionIndex == 1)
                {
                    questObject[0].SetActive(false); // ���� ����Ʈ ���ο��� ������ ã���� ��! ����.
                }
                else if(questActionIndex == 0){
                    questObject[0].SetActive(true); // Load��Ȳ������ �� ��簡 ������ ���� ������ ����ٴ� ����� �ߵ����� ���� �� �ִ�.
                    // ���� ��ȭ�� ������ �� ���Ŀ� ���� Ų ����, �� questId�� 20�̸鼭, ActionIndex�� 0�϶��� �ߵ��� �Ǿ�� �Ѵ�.
                }
                break;
        }
    }


}
