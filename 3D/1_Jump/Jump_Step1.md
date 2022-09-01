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






