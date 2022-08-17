using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex; // 퀘스트 순서를 정하는 것!
    public GameObject[] questObject; // 퀘스트에 필요한 오브젝트들이 들어 갈 것!

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
        questList.Add(10, new QuestData("깃발을 가져와 줘!", new int[] { 1000,2000 }));
        questList.Add(20, new QuestData("돌정령의 부탁", new int[] { 3000, 2000 }));
        questList.Add(30, new QuestData("클리어!", new int[] { 0 }));
    }

    public int GetQuestTalkIndex(int id)
    {
        return questId + questActionIndex; // 퀘스트 번호 + 대화 순서 = 퀘스트 대화 ID
    }

    public string CheckQuest(int id)
    {
        
        if(id == questList[questId].npcId[questActionIndex]) // NPC의 id를 받아서 id가 현재 진행 중인 퀘스트의 적절한 순서에 맞는 NPC일 때, 다음 순서로 진행되게 한다.(대화의 id를 올림으로써!)
        {
            // 대화가 끝났을 때 진행도를 하나씩 올리기 위함! -> 대화가 끝이 나는건 Manager에서 관리!
            questActionIndex++;
        }

        // Control Quest Object -> 퀘스트와 관련된 Object가 있으면 세팅하는 작업!
        ControlObject();

        if (questActionIndex == questList[questId].npcId.Length)
        {
            NextQuest(); // 대화 순서가 끝에 도달했을 때 다음 퀘스트로!
        }

        return questList[questId].questName;

    }

    public string CheckQuest() // 맨 처음에 id 정보를 받지 않고서도 초기 퀘스트의 이름을 출력할 수 있게 하기 위함
    {
        // 오버로딩을 활용하여 같은 함수 이름을 매개변수 차이로 구분하여 참조하게 되는 것이다.
        return questList[questId].questName;

    }

    void NextQuest()
    {
        questId += 10;
        questActionIndex = 0;
    }

    public void ControlObject() // 퀘스트와 관련된 오브젝트들 관리
    {
        switch (questId)
        {
            case 10:
                if(questActionIndex == 2)
                {
                    questObject[0].SetActive(true); // 찾아달라는 대화가 끝나고 바로 동전이 나오게끔!
                }
                break;
            case 20:
                if (questActionIndex == 1)
                {
                    questObject[0].SetActive(false); // 다음 퀘스트 라인에서 동전을 찾았을 때! 끈다.
                }
                else if(questActionIndex == 0){
                    questObject[0].SetActive(true); // Load상황에서는 앞 대사가 끝나자 마자 동전이 생긴다는 기능이 발동되지 않을 수 있다.
                    // 따라서 대화가 끝나고 난 직후에 껐다 킨 상태, 즉 questId가 20이면서, ActionIndex가 0일때도 발동이 되어야 한다.
                }
                break;
        }
    }


}
