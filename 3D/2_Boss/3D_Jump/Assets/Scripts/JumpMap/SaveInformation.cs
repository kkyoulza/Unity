using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter Ŭ���� ����� ���� ���ӽ����̽� �߰�

[System.Serializable]
public class Info{

    public int firstScore, secondScore, thirdScore;
    public int totalScore;

    public int cntScore;

    public int fallCountone, fallCounttwo, fallCountthree; // �������� 1,2,3���� ������ Ƚ��
    public int totalFallCount; // �� �������� ���� ������ Ƚ��


    public Info(int cnt, int total)
    {
        this.cntScore = cnt;
        this.totalFallCount = total;
    }

}

public class SaveInformation : MonoBehaviour
{
    Info info = new Info(0, 0); // �� ó���� 0���� ����

    int currentStage = 1; // ���� ���������� ��Ÿ����.

    // Start is called before the first frame update

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // ���� �ٲ� ������� �ʰ��Ѵ�.
        Debug.Log("SaveBase");
    }

    void Start()
    {
        Debug.Log("New Stage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SumScore()
    {
        // ���� ������ ���� �������� ������ ���Ѵ�.
        info.totalScore += info.cntScore;
        Debug.Log("���� �ջ� �Ϸ�, �� ���� : " + info.totalScore);

    }

    public void addCntScore(int add)
    {
        info.cntScore += add;

        switch (currentStage)
        {
            case 1:
                info.firstScore += add;
                break;
            case 2:
                info.secondScore += add;
                break;
            case 3:
                info.thirdScore += add;
                break;

        }
    }

    public int GetStage()
    {
        return currentStage;
    }

    public int GetCntScore()
    {
        return info.cntScore;
    }

    public int GetTotalScore()
    {
        return info.totalScore;
    }

    public void clearCntScore()
    {
        // �� ���������� �̵����� ��, ���� ������ �ʱ�ȭ �Ѵ�.
        info.cntScore = 0;
    }

    public void stageUp()
    {
        if (currentStage == 3) // 3���������� Ŭ�����ϸ�
            currentStage = 1; // 1�� �ʱ�ȭ �ϰ�
        else // �׷��� ������
            currentStage++; // ���������� ���Ѵ�.
    }

    public void SaveInfoToFile()
    {

        string fileName = "jumpScoreInfo";
        string path = Application.dataPath + "/" + fileName + ".dat";

        FileStream fs = new FileStream(path, FileMode.Create); // ���� ��� ����
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fs, info); // ����ȭ �Ͽ� ����

        Debug.Log("���� ���� �Ϸ�");

        fs.Close();


    }

    public void LoadInfoFile()
    {
        string fileName = "jumpScoreInfo";
        string path = Application.dataPath + "/" + fileName + ".dat";

        if (File.Exists(path))
        {
            // ���� ������ �����ϸ�

            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            Info info = formatter.Deserialize(fs) as Info; // �� ����ȭ ��, Ŭ���� ���¿� �´� ��ü�� �ٽ� ����

            Debug.Log("���� �� ���� ���� : " + info.cntScore);
            Debug.Log("���� �� ���� ���� : " + info.totalScore);

            fs.Close();
        }
        else
        {
            // ������ �������� ������
            Debug.Log("������ �������� ����");
        }
    }


}
