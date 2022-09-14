using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    GameObject manager;
    GameObject saveObject;
    public GameObject startPos; // ��������

    SaveInformation saveInfo;
    ChooseRoute choose;
    Managing managing;
    Rigidbody rigid;

    bool isJumpState = false;
    bool dontMove = false;
    
    Vector3 ReturnPos; // ���̺� ����Ʈ�� ���� �ʾ��� ��
    float jumpForce = 60.0f;

    int showNotice = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        manager = GameObject.FindGameObjectWithTag("Manager");
        saveObject = GameObject.FindGameObjectWithTag("information");

        ReturnPos = startPos.transform.position;
        managing = manager.GetComponent<Managing>();
        saveInfo = saveObject.GetComponent<SaveInformation>();

    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !isJumpState)
        {
            isJumpState = true;
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float h = dontMove ? 0f : Input.GetAxisRaw("Horizontal");
        float v = dontMove ? 0f : Input.GetAxisRaw("Vertical");

        rigid.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "base")
        {
            isJumpState = false;
        }
        else if(collision.gameObject.tag == "under")
        {
            managing.ShowFallNotice();
            rigid.velocity = Vector3.zero;
            managing.MoveToTarget(ReturnPos);

        }

    }

    void gotoNextMap()
    {
        saveInfo.clearCntScore(); // ���� �������� ���� �ʱ�ȭ!(�� ���������� ���� ����) - ������ ������ ���ķ� �ʱ�ȭ�� ���� �־�� �Ѵ�.
        dontMove = false;

        switch (saveInfo.GetStage())
        {
            case 1:
                saveInfo.stageUp(); // �̰����� �Ű� �־�� �� ���� �� ���������� �ǳ� �ٴ� ���� ������ �� ����
                SceneManager.LoadScene("Jump_2");
                break;
            case 2:
                saveInfo.stageUp();
                SceneManager.LoadScene("Jump_3");
                break;
            case 3:
                saveInfo.stageUp();
                SceneManager.LoadScene("Lobby");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SavePoint")
        {
            ReturnPos = other.gameObject.transform.position;
            other.gameObject.SetActive(false); // ���̺� ����Ʈ�� �Ծ����� ��Ȱ��ȭ
        }

        if(other.gameObject.tag == "gold")
        {
            managing.addScore(1);
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "silver")
        {
            managing.addScore(0);
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "chooseRoute")
        {
            ReturnPos = other.gameObject.transform.position;
            choose = other.gameObject.GetComponent<ChooseRoute>();
            choose.onWall();
            other.gameObject.SetActive(false); // ���̺� ����Ʈ�� �Ծ����� ��Ȱ��ȭ
        }


        if (other.gameObject.tag == "goal")
        {
            rigid.velocity = Vector3.zero;
            dontMove = true;
            saveInfo.SumScore(); // ���� �������� ������ �ջ�

            managing.StageClearUI();

            Invoke("gotoNextMap",2.0f);

        }

        if(other.gameObject.tag == "question")
        {
            switch (saveInfo.GetStage())
            {
                case 2:
                    managing.ShowInfoStage2Plus();                
                    break;

            }

        }


        if (other.gameObject.tag == "Notice")
        {
            if(other.gameObject.name == "1" && showNotice < 1)
            {
                managing.ShowNotices(1);
                showNotice++;
            }
            else if(other.gameObject.name == "2" && showNotice < 2)
            {
                managing.ShowNotices(2);
                showNotice++;
            }
            else if (other.gameObject.name == "3" && showNotice < 3)
            {
                managing.ShowNotices(3);
                showNotice++;
            }
            else if (other.gameObject.name == "4" && showNotice < 4)
            {
                managing.ShowNotices(4);
                showNotice++;
            }
            else if (other.gameObject.name == "5" && showNotice < 5)
            {
                managing.ShowNotices(5);
                showNotice++;
            }
            else if (other.gameObject.name == "6" && showNotice < 6)
            {
                managing.ShowNotices(6);
                showNotice++;
            }
            else if (other.gameObject.name == "7" && showNotice < 7)
            {
                managing.ShowNotices(7);
                showNotice++;
            }

        }

    }

}
