using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Managing : MonoBehaviour
{
    GameObject player;
    Vector3 startPos;

    public Text noticeText;
    public GameObject panel; // �ǳ�
    float onTime = 0f;
    float delTime = 3.0f;
    bool isOn;

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
    }

    public void MoveToTarget(Vector3 target)
    {
        // �÷��̾ Ÿ������ �̵���Ű�� �Լ�
        player.transform.position = target;
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
                noticeText.text = "�ٴڿ� �������� ó�� ��ġ�� ���ư��ϴ�!";
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
                noticeText.text = "��� ���� ����� ������ ���̺� ����Ʈ����!\n �ٴڿ� �������� ���̺� ����Ʈ�� �����Ѵ�ϴ�!";
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
                noticeText.text = "���濡 �����̴� �Ķ� �� ������ ���̳���?\n ƨ�� ������ �ʰ� �����ϼ���!";
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
                noticeText.text = "���濡 ����� ������ ���̳���?\n ��! ��! Ƣ��鼭 �� �ָ� �ϴ� ���� �ö� ����!";
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
                noticeText.text = "�տ� ����� ������ ũ�Ⱑ �پ��ٰ� �þ��ٰ� �ϳ׿�!\n ���̺� ����Ʈ�� �Ծ��ڴ� �ѹ� ���� �� ������?\n ������ ���������� ���Ƽ� ���� �ſ�!";
                break;

        }
    }


}
