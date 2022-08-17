using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData // 오브젝트에 넣지 않고 스크립트 자체로 실행하기 위해서 MonoBehavior를 지운다.
{
    public string questName;
    public int[] npcId; // 퀘스트와 관련 있는 NPCID를 저장

    public QuestData(string name,int[] npc)
    {
        this.questName = name;
        this.npcId = npc;
    }

}
