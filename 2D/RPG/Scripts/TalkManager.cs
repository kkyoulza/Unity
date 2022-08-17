using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int,string[]> talkData; // Ű-���� ���·� �ֱ� ������ Ű - ��ȭ �������� ���·� �ִ´�.
    // ���ڿ� �迭�� ���� ������ �� ��󸶴� ���� ��簡 �ֱ� �����̴�. (�߿�!)
    Dictionary<int, Sprite> portraitData; // �ʻ�ȭ ������

    public Sprite[] portraitArr;


    // Start is called before the first frame update
    void Awake()
    {
        talkData = new Dictionary<int, string[]>(); // �ʱ�ȭ
        portraitData = new Dictionary<int, Sprite>(); // �ʱ�ȭ
        GenerateData();
    }

    // Update is called once per frame
    void GenerateData() // ������ ���� �۾�!
    {
        talkData.Add(1000,new string[] {"�ȳ�!:2", "�̰��� ǳ���� �� ���� �� ����.:1"}); // �����ڿ� �Բ� �ʻ�ȭ �ε����� �߰��� ��
        talkData.Add(2000, new string[] { "�ȳ��ϴ�!:0", "����� ã�ƺ���!:0"});
        talkData.Add(100, new string[] { ".... �ȳ� �Խ���..", "�������! �� �������� �����Ӱ� ������ �� �ֽ��ϴ�!","����Ű�� ���ؼ� ����� ã�ƺ�����!" });
        talkData.Add(200, new string[] { ".... ��ſ��� �帮�� ����!..", "�ʹ� �����ϰ� �������� ������!" });
        talkData.Add(300, new string[] { ".... �״�� ���� �˴ϴ�! ..", "��������!" });
        talkData.Add(400, new string[] { ".... ���ÿ� �¾� ������ ���¿��� ������ �Ѵٸ�? ..", "����, �̰� ���� �ƴϿ���?", "���ÿ� ������ �������� ��� ��������!"});
        talkData.Add(500, new string[] { ".... ������ �־��! ..", "��� �����ؾ� �� �� ����?"});


        //QuestTalk
        talkData.Add(10 + 1000, new string[] { "�ȳ�~!:0", "�� �� �������� �ʰھ�?:2","���� ������ ����� �����Դµ�, �ٶ��� ���ư����Ⱦ�:3","ã������ ������?:2" });
        talkData.Add(11 + 2000, new string[] { "�ȳ��ϴ�~!:0", "�� ã���� �Գ�?:0", "����� �óİ� ����°ų�?:0", "���� ���� �������� �ʾҴ�!:0","�� ��Ź�� ��� �شٸ� ����� ����� �˷��ְڴ�!:0","�Ʊ� �߲�ġ�� ��������� �����Դ� ������ �Ҿ���ȴ�!:0","������ ã�ƴ޶�!:0" });

        talkData.Add(20 + 3000, new string[] { "������ ã�Ҵ�! �����ɿ��� ���� �� ����" });

        talkData.Add(21 + 2000, new string[] { "����!:0","����� ���������� �� ���� �����ž�!:0" });


        portraitData.Add(1000 + 0, portraitArr[0]); // idle
        portraitData.Add(1000 + 1, portraitArr[1]); // smile
        portraitData.Add(1000 + 2, portraitArr[2]); // talk
        portraitData.Add(1000 + 3, portraitArr[3]); // angry
        portraitData.Add(2000 + 0, portraitArr[4]); // Dol_Face

    }

    public string GetTalk(int id, int talkIndex) // talkIndex - �� ��° ��������!
    {

        if (!talkData.ContainsKey(id))
        {// �ش� ����Ʈ ���� ���� ��簡 ���� ��, ������ ���ư��� ��Ž���� �Ѵ�. -> ����Ʈ �� ó�� ��縦 ������ �´�.

            if(!talkData.ContainsKey(id - id % 10)) // ����Ʈ ��ȭ�� ���� ��!
            {
                // ����Ʈ �� ó�� ��縶�� ���� ��, �⺻ ��縦 ������ ���� �ȴ�!
                // ����Ʈ id���� ���� �� �ָ� �ȴ�!
                /*
                if (talkIndex == talkData[id - id % 100].Length)
                {
                    return null;
                }
                else
                {
                    return talkData[id - id % 100][talkIndex];
                    // 100�� ���� �������� �� �ָ�.. Quest index���� ������ ��簡 ������ �ȴ�.
                }
                */
                //����Լ� ���

                return GetTalk(id - id % 100, talkIndex);

            }
            else
            { // ����Ʈ �⺻ ��簡 ������ �⺻ ��縦 ���� �� ��!
                /*
                if (talkIndex == talkData[id - id % 10].Length)
                {
                    return null;
                }
                else
                {
                    return talkData[id - id % 10][talkIndex];
                    // ���� ��� 1021��° id�� ������ �ϴµ� ���ٸ� 1021 - (1021%10 = 1) = 1020��° ��縦 �������� �Ǵ� ���̴�.....
                }
                */
                return GetTalk(id - id % 10, talkIndex);
            }
            
            // �̷��� ��縦 ����ó���ϴ� ���� ������ ����Ʈ Index�� 10 ������ �ö󰡸�, �� ����Ʈ ������ ������ 1�� �ö󰣴ٴ� ���̴�.
            // NPC �� ������Ʈ���� 100������ �������Ѿ� �Ѵ�.(�ּ� 100����)
            // �Ʊ� 100,101,102,103... ���� ���� �� �� �׻� 100���� ������ ���� ��縸 ���Գ� �ϸ�
            // �� ��° if������ id - id%10�� ����� 10�� ������ ��Ƴ��� �ȴ�. ��, 111�� ���� 110�� ������ �Ǵ� ���̴�.
            // �׷��� �� ���ʿ� �ִ� ���� id - id%100�̴�. ��, 100������ ��Ƴ��� ���̴�.
            // ���� ��� 121�� ������ 121 - (121%100 = 21) = 100�� �Ǵ� ���̴�.
            // ����, �Ʊ� ������Ʈ id�� 101,102,103.. ���� �� ���� �� 100���� ���ϵǾ� ������ �ƴ� ���̴�.
            // �׷��� NPC�� Object���� �ּҴ����� 100����, �׸��� ����Ʈ�� 10�� ������ �Դٰ��� �ϴ°� �� ����Ʈ ��Ʈ�� ���� �� ���̴�.
            // ���� ����Ʈ�� �Դٰ��� �ϴ� ������ �ٲٷ��� ������ id%10���� 10�� �ٸ� ���ڷ� �ٲٸ� �� �� �ѵ�... �̰� �� �� ���丮�� ���� ���� ���� ����Ʈ�� �ɰ��� �� ���̴�.

            // ���� ���� ����Ʈ���� ����� ���ؼ���.... ����� ã�� ����



        }
        if(talkIndex == talkData[id].Length) 
        {
            // ���� ��� ���̰� 2��� index�� 1������ ���� ��ȭ ������ �ֱ� ������ 1�϶� ������ ������ �ְ�, �ε��� �ʰ� ��!(���̶� �ε��� ���̶� ���� ��!)
            // ���� �� ���� null�� �ִ� ���̴�.
            return null;
        }
        else
        {
            return talkData[id][talkIndex]; // talkData[id] ���� string �迭�� ������ �Ǹ�, �ڿ� �ִ� index�� ���Ͽ� ��簡 ���� ���̴�.
        }
        
    }

    public Sprite GetPortrait(int id,int portraitIndex)
    {

        return portraitData[id+portraitIndex];
    }

}
