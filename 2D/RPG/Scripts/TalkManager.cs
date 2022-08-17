using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int,string[]> talkData; // 키-값의 형태로 있기 때문에 키 - 대화 데이터의 형태로 넣는다.
    // 문자열 배열로 넣은 이유는 한 대상마다 여러 대사가 있기 때문이다. (중요!)
    Dictionary<int, Sprite> portraitData; // 초상화 데이터

    public Sprite[] portraitArr;


    // Start is called before the first frame update
    void Awake()
    {
        talkData = new Dictionary<int, string[]>(); // 초기화
        portraitData = new Dictionary<int, Sprite>(); // 초기화
        GenerateData();
    }

    // Update is called once per frame
    void GenerateData() // 데이터 세팅 작업!
    {
        talkData.Add(1000,new string[] {"안녕!:2", "이곳은 풍경이 참 좋은 것 같아.:1"}); // 구분자와 함께 초상화 인덱스를 추가할 것
        talkData.Add(2000, new string[] { "안녕하담!:0", "깃발을 찾아보람!:0"});
        talkData.Add(100, new string[] { ".... 안내 게시판..", "어서오세요! 이 곳에서는 자유롭게 움직일 수 있습니다!","점프키를 통해서 깃발을 찾아보세요!" });
        talkData.Add(200, new string[] { ".... 당신에게 드리는 꿀팁!..", "너무 조급하게 점프하지 마세요!" });
        talkData.Add(300, new string[] { ".... 그대로 가면 됩니다! ..", "믿으세요!" });
        talkData.Add(400, new string[] { ".... 가시에 맞아 떠오른 상태에서 점프를 한다면? ..", "세상에, 이거 버그 아니에요?", "가시에 맞음은 추진력을 얻기 위함이지!"});
        talkData.Add(500, new string[] { ".... 갈길이 멀어요! ..", "어떻게 점프해야 할 지 알죠?"});


        //QuestTalk
        talkData.Add(10 + 1000, new string[] { "안녕~!:0", "날 좀 도와주지 않겠어?:2","내가 집에서 깃발을 가져왔는데, 바람에 날아가버렸어:3","찾아주지 않을래?:2" });
        talkData.Add(11 + 2000, new string[] { "안녕하담~!:0", "뭘 찾으러 왔냠?:0", "깃발을 봤냐고 물어보는거냠?:0", "절대 내가 가져가지 않았담!:0","내 부탁을 들어 준다면 깃발의 행방을 알려주겠담!:0","아까 닭꼬치를 사먹으려고 가져왔던 동전을 잃어버렸담!:0","동전을 찾아달람!:0" });

        talkData.Add(20 + 3000, new string[] { "동전을 찾았다! 돌정령에게 돌아 가 보자" });

        talkData.Add(21 + 2000, new string[] { "고마워!:0","깃발은 오른쪽으로 가 보면 있을거야!:0" });


        portraitData.Add(1000 + 0, portraitArr[0]); // idle
        portraitData.Add(1000 + 1, portraitArr[1]); // smile
        portraitData.Add(1000 + 2, portraitArr[2]); // talk
        portraitData.Add(1000 + 3, portraitArr[3]); // angry
        portraitData.Add(2000 + 0, portraitArr[4]); // Dol_Face

    }

    public string GetTalk(int id, int talkIndex) // talkIndex - 몇 번째 문장인지!
    {

        if (!talkData.ContainsKey(id))
        {// 해당 퀘스트 진행 순서 대사가 없을 때, 앞으로 돌아가서 재탐색을 한다. -> 퀘스트 맨 처음 대사를 가지고 온다.

            if(!talkData.ContainsKey(id - id % 10)) // 퀘스트 대화가 없을 때!
            {
                // 퀘스트 맨 처음 대사마저 없을 때, 기본 대사를 가지고 오면 된다!
                // 퀘스트 id만을 제거 해 주면 된다!
                /*
                if (talkIndex == talkData[id - id % 100].Length)
                {
                    return null;
                }
                else
                {
                    return talkData[id - id % 100][talkIndex];
                    // 100을 나눈 나머지를 빼 주면.. Quest index만을 제외한 대사가 나오게 된다.
                }
                */
                //재귀함수 사용

                return GetTalk(id - id % 100, talkIndex);

            }
            else
            { // 퀘스트 기본 대사가 있으면 기본 대사를 가져 올 것!
                /*
                if (talkIndex == talkData[id - id % 10].Length)
                {
                    return null;
                }
                else
                {
                    return talkData[id - id % 10][talkIndex];
                    // 예를 들어 1021번째 id를 가져야 하는데 없다면 1021 - (1021%10 = 1) = 1020번째 대사를 가져오게 되는 것이다.....
                }
                */
                return GetTalk(id - id % 10, talkIndex);
            }
            
            // 이렇게 대사를 예외처리하는 것의 전제는 퀘스트 Index가 10 단위로 올라가며, 각 퀘스트 내에서 대사들은 1씩 올라간다는 것이다.
            // NPC 및 오브젝트들은 100단위로 증가시켜야 한다.(최소 100단위)
            // 아까 100,101,102,103... 으로 했을 때 왜 항상 100으로 설정한 놈의 대사만 나왔냐 하면
            // 두 번째 if문에서 id - id%10의 결과는 10의 단위만 살아남게 된다. 즉, 111이 들어가면 110이 나오게 되는 것이다.
            // 그런데 그 안쪽에 있는 놈은 id - id%100이다. 즉, 100단위만 살아남는 것이다.
            // 예를 들어 121이 들어갔으면 121 - (121%100 = 21) = 100이 되는 것이다.
            // 따라서, 아까 오브젝트 id를 101,102,103.. 으로 한 것은 다 100으로 통일되어 나오게 됐던 것이다.
            // 그래서 NPC나 Object들은 최소단위를 100으로, 그리고 퀘스트는 10의 단위로 왔다갔다 하는게 한 퀘스트 세트로 설정 한 것이다.
            // 만약 퀘스트의 왔다갔다 하는 단위를 바꾸려면 위에서 id%10에서 10을 다른 숫자로 바꾸면 될 듯 한데... 이건 뭐 긴 스토리라도 여러 개의 세부 퀘스트로 쪼개면 될 일이다.

            // 여러 서브 퀘스트들을 만들기 위해서는.... 방법을 찾아 보자



        }
        if(talkIndex == talkData[id].Length) 
        {
            // 예를 들어 길이가 2라면 index가 1까지일 때만 대화 내용이 있기 때문에 1일때 까지만 내용을 주고, 인덱스 초과 시!(길이랑 인덱스 값이랑 같을 때!)
            // 이제 그 뭐냐 null을 주는 것이다.
            return null;
        }
        else
        {
            return talkData[id][talkIndex]; // talkData[id] 에서 string 배열이 나오게 되며, 뒤에 있는 index를 통하여 대사가 나올 것이다.
        }
        
    }

    public Sprite GetPortrait(int id,int portraitIndex)
    {

        return portraitData[id+portraitIndex];
    }

}
