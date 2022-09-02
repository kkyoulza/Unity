## Step2. 1 스테이지 세팅 및 여러 가지 기믹 발판들 생성

**발판 세팅**

![image](https://user-images.githubusercontent.com/66288087/188149324-10737745-f891-4b15-8602-bc9f543f404f.png)

스테이지 1의 발판 구성은 위 사진과 같이 하였다.

다양한 종류의 발판들을 세팅하는 것을 목표로 하였으며, 카메라 방향이 바뀌지 않는 특성을 고려하여 앞으로 전진만 하게끔 세팅하였다.

<hr>

**발판 종류**


1. 일반 발판

![image](https://user-images.githubusercontent.com/66288087/188149688-b2114b25-7815-4823-976d-742eb1196e17.png)

일반적으로 딛을 수 있는 발판이다.


2. 움직이는 발판

![image](https://user-images.githubusercontent.com/66288087/188149830-fbee0d97-2857-4f67-b9f6-65a4874de8bd.png)

움직이는 발판이다.

움직이는 패턴은 아래와 같이 애니메이션을 통하여 설정 해 주었다.

![image](https://user-images.githubusercontent.com/66288087/188149969-532e121c-129f-441e-98d7-f6361dc54c92.png)

그런데 이 방식은 위치를 이동시키려면 다시 세팅 해 주어야 한다는 단점이 있다.

따라서 2 스테이지를 세팅할 때는 코드를 통하여 움직이게끔 해 줄 것이다.


3. 통통 튀기는 발판

![image](https://user-images.githubusercontent.com/66288087/188150194-38e96df8-b886-4a31-8408-facd17be962e.png)

말 그대로 위로 올라가면 통통 튀기는 발판이다.

이 것은 Physic Material에서 Bounciness를 1로 설정 한 다음, Bounce Combine을 Maximum으로 설정하여 통통 튀기는 효과를 극대화 하였다.


4. 늘었다가 줄었다가 하는 발판

![image](https://user-images.githubusercontent.com/66288087/188150678-d1a1a9eb-520a-4cc2-baad-abc1ae2f35df.png)

처음에는 사라졌다가 생겼다를 반복하는 발판으로 기획하였지만 애니메이션에서 Collision 효과를 on/off할 수 있음을 보고 기획을 변경하여 이용하였다.

![image](https://user-images.githubusercontent.com/66288087/188151072-642d8b6e-707f-41ad-a1ed-befcc6a0e5b3.png)

중간에 Box Collision을 off 해 주어 작아진 시점에서는 딛을 수 없게 만들어 주었다.

![image](https://user-images.githubusercontent.com/66288087/188151235-d3520c55-cc07-444b-927a-4a5e0dc13c96.png)

또한 타이밍을 잡기 싫어하는 사람에게는 돌아서 갈 수 있는 길도 마련하였다.


5. 세이브 포인트

![image](https://user-images.githubusercontent.com/66288087/188151558-10ef25c4-4287-4efd-ae65-17de71f0bf8b.png)

아래 빨간 바닥에 떨어지게 되면 처음 지점으로 돌아가게 된다. 그런데 세이브 포인트를 만들어 두어 세이브 포인트에 먼저 도달한 다음, 떨어지게 되면 세이브 포인트로 돌아갈 수 있게끔 하였다.

세이브 포인트에 대한 코드는 다음과 같다.


Player.cs 코드 - SavePointOneEnabled 의 bool 변수를 제거하고 돌아 갈 포지션만을 갱신 해 주는 것으로 바꾸어 주었다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject manager;
    Managing managing;
    Rigidbody rigid;

    bool isJumpState = false;
    // bool SavePointOneEnabled = false;

    Vector3 ReturnPos = new Vector3(1,1,-18); // 세이브 포인트를 먹지 않았을 때
    float jumpForce = 60.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        manager = GameObject.FindGameObjectWithTag("Manager");
        managing = manager.GetComponent<Managing>();

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
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

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
            rigid.velocity = Vector3.zero;
            managing.MoveToTarget(ReturnPos);

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SavePoint")
        {
            ReturnPos = other.gameObject.transform.position; // 돌아 갈 지점 갱신
            other.gameObject.SetActive(false); // 세이브 포인트를 먹었으니 비활성화
        }

    }

}
</code>
</pre>


Managing.cs 코드

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Managing : MonoBehaviour
{
    GameObject player;
    Vector3 startPos;

    public Text noticeText;
    public GameObject panel; // 판넬
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
    
    }

    public void MoveToTarget(Vector3 target)
    {
        // 플레이어를 타겟으로 이동시키는 함수
        player.transform.position = target;
    }

}
</code>
</pre>

Managing은 변한것이 없다.


<hr>

발판 색깔별로 특성을 부여하여 듀토리얼 스테이지(1 스테이지) 이후로는 발판 색만 보고도 유추할 수 있게 만들어 주어야 한다.

그래서 발판에 대한 안내가 주어질 필요가 있다.

<hr>

**UI 설정**

이제 UI에 대한 설정을 해 줄 것이다.

빈 오브젝트에 Box Collider만을 Trigger 버전으로 넣어 두어 특정 지점에 닿게 되면 UI로 설명이 나오게 할 것이다.

![image](https://user-images.githubusercontent.com/66288087/188152621-cf97bbb7-9f5f-457a-b9fc-6e8a644f29ba.png)

![image](https://user-images.githubusercontent.com/66288087/188152961-23959a16-e8a8-4acb-968e-76432f508065.png)

위 사진과 같이 Trigger를 설정 해 두고, Tag를 Notice, 이름은 순서대로 숫자로 설정 해 둔다.

그런데, 무한정 반복하여 뜨게 하면 되지 않기에 순서대로 세팅을 해 준다.

Player.cs 코드
<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject manager;
    Managing managing;
    Rigidbody rigid;

    bool isJumpState = false;
    // bool SavePointOneEnabled = false;

    Vector3 ReturnPos = new Vector3(1,1,-18); // 세이브 포인트를 먹지 않았을 때
    float jumpForce = 60.0f;

    int showNotice = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        manager = GameObject.FindGameObjectWithTag("Manager");
        managing = manager.GetComponent<Managing>();

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
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

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
            rigid.velocity = Vector3.zero;
            managing.MoveToTarget(ReturnPos);

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SavePoint")
        {
            ReturnPos = other.gameObject.transform.position;
            other.gameObject.SetActive(false); // 세이브 포인트를 먹었으니 비활성화
        }

        if(other.gameObject.tag == "Notice")
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

        }

    }

}
</code>
</pre>

Notice 태그를 가지고 있고, 순서(showNotice)가 앞서가지 않으면서 이름이 적당한 순서라면 Managing에 있는 UI를 보여주는 함수를 출력한다.

Managing.cs 코드

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Managing : MonoBehaviour
{
    GameObject player;
    Vector3 startPos;

    public Text noticeText;
    public GameObject panel; // 판넬
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
        // 플레이어를 타겟으로 이동시키는 함수
        player.transform.position = target;
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
                noticeText.text = "바닥에 떨어지면 처음 위치로 돌아갑니다!";
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
                noticeText.text = "방금 먹은 노란색 꼬깔은 세이브 포인트에요!\n 바닥에 떨어지면 세이브 포인트로 복귀한답니다!";
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
                noticeText.text = "전방에 움직이는 파란 색 발판이 보이나요?\n 튕겨 나가지 않게 조심하세요!";
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
                noticeText.text = "전방에 보라색 발판이 보이나요?\n 통! 통! 튀기면서 저 멀리 하늘 위로 올라가 봐요!";
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
                noticeText.text = "앞에 노란색 발판은 크기가 줄었다가 늘었다가 하네요!\n 세이브 포인트도 먹었겠다 한번 도전 해 볼래요?\n 싫으면 오른쪽으로 돌아서 가면 돼요!";
                break;

        }
    }
}
</code>
</pre>

switch 함수를 통하여 panel를 Active로 해 주고, text를 변경하여 UI를 출력 해 준다.

UI는 3초동안 출력되며, 만약 사라지기 전에 다른 알림 Trigger를 발동시키면 지속시간이 다시 0초부터 시작한다.

<hr>

![image](https://user-images.githubusercontent.com/66288087/188153612-27ef64ab-9f9e-4f9b-81a3-6567dbe05e76.png)

이렇게 UI가 출력되게 된다.


[블로그](https://mini-noriter.tistory.com/49)에 들어가면 더 자세한 설명을 볼 수 있다.
