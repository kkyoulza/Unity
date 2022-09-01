## Jump Map Step 1 - Player, Camera, Stage Setting

**Player Setting**

우선 플레이어를 만들어 준다. 간단하게 구 모양으로 만들어 주고, 추후에 바꾸어 줄 수 있으면 주도록 한다.

![image](https://user-images.githubusercontent.com/66288087/187885079-d637d54e-d940-4e11-9c1e-d48642cc7899.png)

구는 아래 사진과 같은 패턴을 씌워 주었다.

![image](https://user-images.githubusercontent.com/66288087/187885170-d3ed30e6-3e10-4a7d-a481-f8e2c91f1d5d.png)

본 연습에서는 [Polygon Starter Pack](https://assetstore.unity.com/packages/p/polygon-starter-pack-low-poly-3d-art-by-synty-156819)을 사용하였다.

![image](https://user-images.githubusercontent.com/66288087/187885396-81858ea5-6968-4193-9461-f48b4167ce41.png)


우선 플레이어가 발 딛을 수 있는 발판을 세팅 해 준다.

![image](https://user-images.githubusercontent.com/66288087/187885781-200f12e3-38b6-45f6-b058-b07002659ab3.png)

빨간색은 Plane을 사용하여 자동으로 Collider가 적용 되어 있지만 초록색 발판은 Collider가 적용되어 있지 않아 새롭게 Box Collider를 씌워 주었다.


이제 공의 움직임을 줘 보도록 해 보자

<pre>
  <code>
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class Player : MonoBehaviour
  {
      Rigidbody rigid;

      bool isJumpState = false;
      float jumpForce = 60.0f;

      // Start is called before the first frame update
      void Start()
      {
          rigid = GetComponent<Rigidbody>();

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
      }

  }  
  </code>

</pre>

GetAxisRaw를 통하여 상,하,좌,우의 움직임을 주었고 Jump 키(Space Bar)를 통하여 점프를 할 수 있게 하였다.

여기서 무한정 점프하는 것을 막기 위하여 발판에 tag를 추가 하여, 딛을 수 있는 발판이라면(base) 점프 가능 횟수를 다시 리필할 수 있게 하였다.
(OnCollisionEnter 사용)

위와 같이 코드를 써 주게 되면 공이 움직일 것이다.

그런데 몇 번 공을 굴려 보면 알겠지만 공이 너무 잘 미끄러지게 된다. 이것은 바닥에 마찰력이 없어서 그런 것이다.

Asset에서 마우스 오른쪽 버튼을 누른 뒤 아래 사진과 같은 것을 선택 해 준다. (Physic Material)

![image](https://user-images.githubusercontent.com/66288087/187887386-e0aac573-111b-4dba-9cbc-e8ab405ce795.png)


![image](https://user-images.githubusercontent.com/66288087/187887707-0cb3eb85-1fb8-4796-a58e-e28fd48b6437.png)

이 곳에서는 동적, 정적 마찰력, 탄성력 등을 설정할 수 있는데 위 사진과 같이 통 크게(?) 해 주어야 미끄러지지 않게 된다.


![jump_1](https://user-images.githubusercontent.com/66288087/187888883-b910ec57-75fe-4164-a666-24777ea3be78.gif)

위 움짤을 보면 얼음처럼 미끄러지지 않음을 볼 수 있다. (초반 세팅은 너무 미끄러웠다..)


<hr>

**Camera**

이제 카메라 세팅을 해 보도록 하자. 움짤에서는 이미 공의 뒤를 졸졸 따라다니는 것을 볼 수 있을 것이다.

코드에서는 플레이어와, 플레이어-카메라 간의 거리를 저장할 offSet Vector3변수를 만들어 놓은 다음, Player Position에서 offSet만큼 떨어진 곳에 위치시켜 주면 된다.

그런데 카메라,UI 등은 LateUpdate를 이용 해 주어야 한다.

CameraMove.cs 코드이다.
<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Vector3 offSet;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offSet = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + offSet; // 플레이어의 위치에서 offSet을 적용한 것이다!
        // 헷갈리지 말 것!
    }

}
</code>
</pre>

여기서 player의 position 말고 camera의 position + offSet을 적는 경우가 있는데, 그럴 때는 플레이어와 무한정 멀어지게 되니 주의해야 한다.


<hr>

**Stage Setting**

스테이지 발판을 세팅하기 전에 우선 아래에 떨어지지 않고 목적지에 도달하는 것이 목표이다.

따라서 맨 아래로 떨어지게 되면 시작 지점 or 세이브 포인트로 돌아가게 해 주어야 한다.

그런데 여기서 플레이어를 이동시킬 때, UI를 통해서 이동한다고 말을 해 주는 것이 좋을 것이다. 따라서 Prefab으로 만들어야 하는 Player에서 UI를 관리하는 것 보다는 Manager가 관리를 하는 것이 더 좋을 것이다.

즉, 플레이어가 아래로 떨어지게 되면 (under tag에 닿게 되면) Manager의 이동 함수를 호출하는 것이다.

그렇다면 그에 맞게 Player.cs 코드를 수정 할 필요가 있다.

수정된 Player.cs 코드
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
    bool SavePointOneEnabled = false;

    Vector3 ReturnPos = new Vector3(1,1,-18); // 세이브 포인트를 먹지 않았을 때 돌아 갈 지점
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
            if (!SavePointOneEnabled)
            {
                rigid.velocity = Vector3.zero;
                managing.MoveToTarget(ReturnPos);
            }

        }
    }

}
</code>
</pre>

세이브 포인트를 먹은 여부를 bool 변수로 나타내어 주고, 그 조건에 따라서 특정 지점으로 이동시켜 주는 함수를 호출한다.

Managing.cs 코드
<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managing : MonoBehaviour
{

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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

Managing 코드는 일단 이동 함수밖에 없지만 점수 갱신 등의 여러 기능들을 추가 할 예정이다.


