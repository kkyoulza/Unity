using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Managing : MonoBehaviour
{
    GameObject player; // �÷��̾� ������Ʈ
    GameObject saveObject; // ���� ���� ������Ʈ

    SaveInformation saveInfo; // ���� ���� �ڵ�
    Vector3 startPos; // ���� ������(���̺� ����Ʈ�� �Ա� �� ������ ��ġ)

    public Text noticeText;
    public Text scoreText;
    public GameObject fallPanel; // �������� �� ������ �ǳ�
    public GameObject panel; // �ǳ�

    int score; // UI ������ ������ ��, ��� ���� ������ �ҷ��� �����ϴ� ����

    float onTime = 0f;
    float delTime = 3.0f;
    float onTime2 = 0f; // �������� ���� �˸� �ð�
    float delTime2 = 2.0f; // �������� ���� �˸� �ִ� ����

    bool isOn = false;
    bool fallNoticeOn = false; // �������� ���� �˸��� �� �ִ� ����?

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
        // �÷��̾ Ÿ������ �̵���Ű�� �Լ�
        player.transform.position = target;
    }

    public void addScore(int num)
    {
        switch (num)
        {
            case 0: // silver
                score = int.Parse(scoreText.text);
                score++;
                saveInfo.addCntScore(1); // ���� ���� ������Ʈ�� 1�� �߰�
                scoreText.text = score.ToString();
                break;
            case 1: // gold
                score = int.Parse(scoreText.text);
                score += 10;
                saveInfo.addCntScore(10); // ���� ���� ������Ʈ ����
                scoreText.text = score.ToString();
                break;
        }
    }


    public void ShowNotices(int num)
    {
        switch (num)
        {
            case 1:
                panel.SetActive(true); // ù ��°�� ���� ��ȭ�� X
                if (!isOn) // UI�� ����� ���¿��� UI ���� ��
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
                if (!isOn) // UI�� ����� ���¿��� UI ���� ��
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "������ ������ ������ �ö󰩴ϴ�!\n ������ �ִ��� ���� �����鼭 ���� �������� ���� �ſ�!";
                break;
            case 3:
                panel.SetActive(true);
                if (!isOn) // UI�� ����� ���¿��� UI ���� ��
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "�� ���̴� ������ ������ ���� �� �־��!\n ������ �ޱ� ���ؼ��� ����̰� ���ƾ߰���?";
                break;
            case 4:
                panel.SetActive(true);
                if (!isOn) // UI�� ����� ���¿��� UI ���� ��
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "��� ���� ����� ������ ���̺� ����Ʈ����!\n �ٴڿ� �������� ���̺� ����Ʈ�� �����Ѵ�ϴ�!";
                break;
            case 5:
                panel.SetActive(true);
                if (!isOn) // UI�� ����� ���¿��� UI ���� ��
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "���濡 �����̴� �Ķ� �� ������ ���̳���?\n ƨ�� ������ �ʰ� �����ϼ���!";
                break;
            case 6:
                panel.SetActive(true);
                if (!isOn) // UI�� ����� ���¿��� UI ���� ��
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "�տ� ����� ������ ���̳���?\n ��! ��! Ƣ��鼭 �� �ָ� �ϴ� ���� �ö� ����!";
                break;
            case 7:
                panel.SetActive(true);
                if (!isOn) // UI�� ����� ���¿��� UI ���� ��
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "�տ� ����� ������ ũ�Ⱑ �پ��ٰ� �þ��ٰ� �ϳ׿�!\n ũ�Ⱑ Ŀ���⸦ ��ٷȴٰ� ���� ���� ��õ�ؿ�!";
                break;

        }
    }

    public void ShowFallNotice()
    {
        fallPanel.SetActive(true);
        if (!fallNoticeOn) // UI�� ����� ���¿��� UI ���� ��
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
        noticeText.text = "������, ���� �� �߿� �� ���� �����ϼ���!\n �ʿ� ���� �ڼ��� ������ ���÷��� ����ǥ�� ������ ���ּ���!";
    }
    public void ShowInfoStage2Plus()
    {
        panel.SetActive(true);
        if (!isOn) // UI�� ����� ���¿��� UI ���� ��
        {
            isOn = true;
        }
        else
        {
            onTime = 0f;
        }
        noticeText.text = "������ �� : ���� ���� ��ֹ����� �հ� ���� �ڽ�\n ���� �� : ������ ��Ʈ���� �䱸�Ǵ� �ڽ�\n �� �� �����ϸ� �ٲ��� ���ϴ� ����!";
    }

    public void StageClearUI()
    {
        panel.SetActive(true);
        noticeText.text = "�������� Ŭ����!\n ���� : " + saveInfo.GetCntScore()+"\n ���� ���� : "+saveInfo.GetTotalScore();
    }

    public void StageClearGainGoldUI()
    {
        panel.SetActive(true);
        noticeText.text = "�� �������� Ŭ���� �Ϸ�!\n ȹ�� ����� 10�� ��ŭ ��带 ȹ���մϴ�!\n ȹ�� ��� : "+saveInfo.info.totalScore * 10+"G";
    }


}
