using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Managing : MonoBehaviour
{
    GameObject player; // 플레이어 오브젝트
    GameObject saveObject; // 정보 저장 오브젝트

    SaveInformation saveInfo; // 정보 저장 코드
    Vector3 startPos; // 시작 포지션(세이브 포인트를 먹기 전 리스폰 위치)

    public Text noticeText;
    public Text scoreText;
    public GameObject fallPanel; // 떨어졌을 때 나오는 판넬
    public GameObject panel; // 판넬

    int score; // UI 점수를 갱신할 때, 잠시 현재 점수를 불러와 저장하는 변수

    float onTime = 0f;
    float delTime = 3.0f;
    float onTime2 = 0f; // 떨어졌을 때의 알림 시간
    float delTime2 = 2.0f; // 떨어졌을 때의 알림 최대 지속

    bool isOn = false;
    bool fallNoticeOn = false; // 떨어졌을 때의 알림이 떠 있는 상태?

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        saveObject = GameObject.FindGameObjectWithTag("information");

        saveInfo = saveObject.GetComponent<SaveInformation>();
        startPos = player.transform.position;

        if(saveInfo.GetStage() == 1)
        {
            delTime = 3.0f;
        }
        else if(saveInfo.GetStage() == 2)
        {
            delTime = 5.0f;
            ShowInfoStage2();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            onTime += Time.deltaTime;
            if(onTime > delTime)
            {
                panel.SetActive(false);
                isOn = false;
                onTime = 0f;
            }

        }

        if (fallNoticeOn)
        {
            onTime2 += Time.deltaTime;
            if (onTime2 > delTime2)
            {
                fallPanel.SetActive(false);
                fallNoticeOn = false;
                onTime2 = 0f;
            }
        }
    }

    public void MoveToTarget(Vector3 target)
    {
        // 플레이어를 타겟으로 이동시키는 함수
        player.transform.position = target;
    }

    public void addScore(int num)
    {
        switch (num)
        {
            case 0: // silver
                score = int.Parse(scoreText.text);
                score++;
                saveInfo.addCntScore(1); // 점수 관리 오브젝트에 1점 추가
                scoreText.text = score.ToString();
                break;
            case 1: // gold
                score = int.Parse(scoreText.text);
                score += 10;
                saveInfo.addCntScore(10); // 점수 관리 오브젝트 갱신
                scoreText.text = score.ToString();
                break;
        }
    }


    public void ShowNotices(int num)
    {
        switch (num)
        {
            case 1:
                panel.SetActive(true); // 첫 번째는 내용 변화가 X
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }

                break;
            case 2:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "동전을 먹으면 점수가 올라갑니다!\n 동전을 최대한 많이 먹으면서 골인 지점까지 가면 돼요!";
                break;
            case 3:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "안 보이는 곳에도 동전이 있을 수 있어요!\n 만점을 받기 위해서는 눈썰미가 좋아야겠죠?";
                break;
            case 4:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "방금 먹은 노란색 꼬깔은 세이브 포인트에요!\n 바닥에 떨어지면 세이브 포인트로 복귀한답니다!";
                break;
            case 5:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "전방에 움직이는 파란 색 발판이 보이나요?\n 튕겨 나가지 않게 조심하세요!";
                break;
            case 6:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "앞에 보라색 발판이 보이나요?\n 통! 통! 튀기면서 저 멀리 하늘 위로 올라가 봐요!";
                break;
            case 7:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "앞에 노란색 발판은 크기가 줄었다가 늘었다가 하네요!\n 크기가 커지기를 기다렸다가 가는 것을 추천해요!";
                break;

        }
    }

    public void ShowFallNotice()
    {
        fallPanel.SetActive(true);
        if (!fallNoticeOn) // UI가 사라진 상태에서 UI 생성 시
        {
            fallNoticeOn = true;
        }
        else
        {
            onTime2 = 0f;
        }
    }

    public void ShowInfoStage2()
    {
        isOn = true;
        panel.SetActive(true);
        noticeText.text = "오른쪽, 왼쪽 길 중에 한 곳을 선택하세요!\n 맵에 대한 자세한 설명을 보시려면 물음표에 가까이 가주세요!";
    }
    public void ShowInfoStage2Plus()
    {
        panel.SetActive(true);
        if (!isOn) // UI가 사라진 상태에서 UI 생성 시
        {
            isOn = true;
        }
        else
        {
            onTime = 0f;
        }
        noticeText.text = "오른쪽 길 : 여러 가지 장애물들을 뚫고 가는 코스\n 왼쪽 길 : 세심한 컨트롤이 요구되는 코스\n 한 번 선택하면 바꾸지 못하니 주의!";
    }

    public void StageClearUI()
    {
        panel.SetActive(true);
        noticeText.text = "스테이지 클리어!\n 점수 : " + saveInfo.GetCntScore()+"\n 누적 점수 : "+saveInfo.GetTotalScore();
    }

    public void StageClearGainGoldUI()
    {
        panel.SetActive(true);
        noticeText.text = "전 스테이지 클리어 완료!\n 획득 골드의 10배 만큼 골드를 획득합니다!\n 획득 골드 : "+saveInfo.info.totalScore * 10+"G";
    }


}
