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




