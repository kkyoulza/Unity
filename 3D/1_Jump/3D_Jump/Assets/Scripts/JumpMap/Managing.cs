using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Managing : MonoBehaviour
{
    GameObject player;
    Vector3 startPos;

    public Text noticeText;
    public Text scoreText;
    public GameObject fallPanel; // 떨어졌을 때 나오는 판넬
    public GameObject panel; // 판넬

    int score;

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
        startPos = player.transform.position;
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
                scoreText.text = score.ToString();
                break;
            case 1: // gold
                score = int.Parse(scoreText.text);
                score += 10;
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


}
