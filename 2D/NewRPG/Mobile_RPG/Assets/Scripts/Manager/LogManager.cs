using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

[System.Serializable]
public class LogRecord
{
    private int[] huntingCount = new int[10]; // 각 몬스터들의 퇴치 마릿수를 저장하기 위한 int 배열 (전체 마릿수는 해당 배열 원소들을 합산하면 된다.)

    // 퀘스트 관련
    public List<int[]> huntList = new List<int[]>(); // 어떤 몬스터를 잡아야 하는지 리스트로 저장 [퀘스트 코드, 몬스터 코드, 잡아야 하는 마릿수, 현재 잡은 마릿수,완료 할 npc 코드]
    public List<int[]> gainList = new List<int[]>(); // 어떤 아이템을 얻어야 하는지 리스트로 저장 [퀘스트 코드, 아이템 코드, 얻어야 하는 개수, 현재 가지고 있는 개수,완료 할 npc 코드]

    public int[,] questComplete = new int[1000,2]; // 퀘스트 완료 여부 기록 ( 0 - 시작 전, 1 - 진행 중, 2 - 완료 ) + 퀘스트 완료 조건 개수
    // 나중에 용량을 늘릴 때, 정보를 옮겨 주어야 한다..? DB로 관리하면 편하긴 하겠다..

    // npc 코드에서 씬이 호출 될 때 마다 npc들을 호출해서 해당되는 퀘스트 코드를 세팅해 줘야 한다..?


    public LogRecord()
    {
        
    }

    void CheckQuest(int index)
    {
        for(int i = 0; i < huntList.Count; i++)
        {
            if(huntList[i][1] == index && huntList[i][3] < huntList[i][2])
            {
                huntList[i][3]++;
            }
        }
    }

    public void addMonsterCount(int index)
    {
        CheckQuest(index);
        this.huntingCount[index]++;
    }

    public int returnMonsterCount(int index)
    {
        return this.huntingCount[index];
    }

}

public class LogManager : MonoBehaviour
{
    public LogRecord playerLog = new LogRecord();

    // Start is called before the first frame update
    void Start()
    {
        string test = "g.1.2:h.2.3";
        var testArr = test.Split(':');

        for(int i = 0; i < testArr.Length; i++)
        {
            Debug.Log(testArr[i]);
            var doubleSplit = testArr[i].Split('.');
            for(int j = 0; j < doubleSplit.Length; j++)
            {
                Debug.Log(doubleSplit[j]);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
