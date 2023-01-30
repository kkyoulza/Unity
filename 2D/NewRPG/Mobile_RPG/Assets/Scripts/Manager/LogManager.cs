using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter Ŭ���� ����� ���� ���ӽ����̽� �߰�

[System.Serializable]
public class LogRecord
{
    private int[] huntingCount = new int[10]; // �� ���͵��� ��ġ �������� �����ϱ� ���� int �迭 (��ü �������� �ش� �迭 ���ҵ��� �ջ��ϸ� �ȴ�.)

    // ����Ʈ ����
    public List<int[]> huntList = new List<int[]>(); // � ���͸� ��ƾ� �ϴ��� ����Ʈ�� ���� [����Ʈ �ڵ�, ���� �ڵ�, ��ƾ� �ϴ� ������, ���� ���� ������,�Ϸ� �� npc �ڵ�]
    public List<int[]> gainList = new List<int[]>(); // � �������� ���� �ϴ��� ����Ʈ�� ���� [����Ʈ �ڵ�, ������ �ڵ�, ���� �ϴ� ����, ���� ������ �ִ� ����,�Ϸ� �� npc �ڵ�]

    public int[,] questComplete = new int[1000,2]; // ����Ʈ �Ϸ� ���� ��� ( 0 - ���� ��, 1 - ���� ��, 2 - �Ϸ� ) + ����Ʈ �Ϸ� ���� ����
    // ���߿� �뷮�� �ø� ��, ������ �Ű� �־�� �Ѵ�..? DB�� �����ϸ� ���ϱ� �ϰڴ�..

    // npc �ڵ忡�� ���� ȣ�� �� �� ���� npc���� ȣ���ؼ� �ش�Ǵ� ����Ʈ �ڵ带 ������ ��� �Ѵ�..?


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
