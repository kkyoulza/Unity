using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInformation : MonoBehaviour
{

    int currentStage = 1; // ���� ���������� ��Ÿ����.

    int firstScore, secondScore, thirdScore; // �������� 1,2,3 ���ھ�
    int totalScore; // �� �������� ���� ���ھ�

    int cntScore;

    int fallCountone, fallCounttwo, fallCountthree; // �������� 1,2,3���� ������ Ƚ��
    int totalFallCount; // �� �������� ���� ������ Ƚ��

    // Start is called before the first frame update

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // ���� �ٲ� ������� �ʰ��Ѵ�.
        Debug.Log("SaveBase");
    }

    void Start()
    {
        cntScore = 0;
        totalScore = 0; // �� ó���� 0���� ����
        Debug.Log("New Stage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SumScore()
    {
        // ���� ������ ���� �������� ������ ���Ѵ�.
        totalScore += cntScore;
        Debug.Log("���� �ջ� �Ϸ�, �� ���� : " + totalScore);

    }

    public void addCntScore(int add)
    {
        cntScore += add;

        switch (currentStage)
        {
            case 1:
                firstScore += add;
                break;
            case 2:
                secondScore += add;
                break;
            case 3:
                thirdScore += add;
                break;

        }
    }

    public int GetStage()
    {
        return currentStage;
    }

    public int GetCntScore()
    {
        return cntScore;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    public void clearCntScore()
    {
        // �� ���������� �̵����� ��, ���� ������ �ʱ�ȭ �Ѵ�.
        cntScore = 0;
    }

    public void stageUp()
    {
        if (currentStage == 3) // 3���������� Ŭ�����ϸ�
            currentStage = 1; // 1�� �ʱ�ȭ �ϰ�
        else // �׷��� ������
            currentStage++; // ���������� ���Ѵ�.
    }


}
