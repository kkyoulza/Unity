using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData // ������Ʈ�� ���� �ʰ� ��ũ��Ʈ ��ü�� �����ϱ� ���ؼ� MonoBehavior�� �����.
{
    public string questName;
    public int[] npcId; // ����Ʈ�� ���� �ִ� NPCID�� ����

    public QuestData(string name,int[] npc)
    {
        this.questName = name;
        this.npcId = npc;
    }

}
