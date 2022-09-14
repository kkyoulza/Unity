using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInformation : MonoBehaviour
{

    int currentStage = 1; // 현재 스테이지를 나타낸다.

    int firstScore, secondScore, thirdScore; // 스테이지 1,2,3 스코어
    int totalScore; // 전 스테이지 통합 스코어

    int cntScore;

    int fallCountone, fallCounttwo, fallCountthree; // 스테이지 1,2,3에서 떨어진 횟수
    int totalFallCount; // 전 스테이지 통합 떨어진 횟수

    // Start is called before the first frame update

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 사라지지 않게한다.
        Debug.Log("SaveBase");
    }

    void Start()
    {
        cntScore = 0;
        totalScore = 0; // 맨 처음에 0으로 시작
        Debug.Log("New Stage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SumScore()
    {
        // 누적 점수에 현재 스테이지 점수를 더한다.
        totalScore += cntScore;
        Debug.Log("점수 합산 완료, 총 점수 : " + totalScore);

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
        // 새 스테이지로 이동했을 때, 현재 점수를 초기화 한다.
        cntScore = 0;
    }

    public void stageUp()
    {
        if (currentStage == 3) // 3스테이지를 클리어하면
            currentStage = 1; // 1로 초기화 하고
        else // 그렇지 않으면
            currentStage++; // 스테이지를 더한다.
    }


}
