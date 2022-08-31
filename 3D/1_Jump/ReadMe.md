# Practice 1 - Jump Game

## 한 프로젝트 내에서 기능들을 확장 해 가는 확장형 프로젝트이다.

<hr>

## 프로젝트 목표

### Step1. 기본적인 아이템을 먹으면서 장애물을 피해 골인 하는 것이 목표인 점프 맵 제작 (3개 스테이지 목표)

### Step2. 퀘스트를 동반한 RPG (미니보스 1개 설정 목표) - 스텟창, 아이템 창, 간단한 강화 구현

### Step3. 여러 기믹들을 이용한 퍼즐 게임 (주어진 물체들을 밀고, 끌고, 던지면서 다음 스테이지로 가는 게임)

#### 각 Step에는 컨텐츠 제작을 위한 세부 Step이 존재한다. 미리 Step을 만들어 놓고 채워가는 식으로 할 것이다.

<hr>

### 세부 Step0. 계획 수립

##### 1. 이름 : 점프 게임

##### 2. 목표 : 아이템을 먹으면서 골인하게 되어 높은 점수를 받는 것이 목표이다.

##### 3. 결과 산정 요소 : 점수 (먹은 장애물 개수 * 장애물 별 가중치 - (최소 클리어 시간을 넘었을 시 초과한 시간))

##### 4. 구현에 필요한 요소들 : Rigidbody, Collider, Rotate, Excel 관리 Asset (최고점수 등에 사용) 등등

<hr>

### 세부 Step1. 캐릭터 움직임, 아이템, 골인 지점 설정


**플레이어 설정**

우선 캐릭터를 하나 만들어 준다. 캐릭터는 간단하게 구 형태로 만들도록 하겠다.

![image](https://user-images.githubusercontent.com/66288087/187404370-732c34d3-e30a-46cb-a57f-269910661087.png)

구 형태를 만들고 아래와 같은 사진을 Material로 추가 해 주었다. (아잉!)

![image](https://user-images.githubusercontent.com/66288087/187404519-fc263a48-775a-4c12-b11a-340feaee40be.png)


그리고 구의 이동과 점프를 구현 해 준다. 코드는 아래와 같다.

<details>
  <summary>Move.cs</summary>

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Move : MonoBehaviour
    {
        Rigidbody rigid;
        bool isJump; // 점프 여부 판단
        AudioSource item1;
        public int Score;

        float jumpForce = 60.0f;

        // Start is called before the first frame update
        void Awake()
        {
            item1 = GetComponent<AudioSource>();
            rigid = GetComponent<Rigidbody>();
            isJump = false;
            Score = 0;
        }


        void Update()
        {
            if (Input.GetButtonDown("Jump") && !isJump) // not 연산자를 사용한다. (Jump 상태가 아닐 때!)
            {
                rigid.AddForce(new Vector3(0, jumpForce, 0),ForceMode.Impulse);
                isJump = true; // 그런데 다시 false로 언제 만들어 줄까? -> 충돌!
            }
        }

          // Update is called once per frame
          void FixedUpdate()
          {

              float h = Input.GetAxisRaw("Horizontal"); // Raw 는 0,-1,1로 떨어진다.
              float v = Input.GetAxisRaw("Vertical");

              rigid.AddForce(new Vector3(h, 0, v),ForceMode.Impulse); // 3D에서 좌,우는 x축, 앞,뒤는 z축이다.



          }

          private void OnCollisionEnter(Collision collision)
          {
              // 바닥에 부딪혔을 때 점프 여부를 초기화 한다!

              if(collision.gameObject.tag == "base")
              {
                  isJump = false;
              }
          }

          private void OnTriggerEnter(Collider other)
          {

              if (other.tag == "item1")
              {
                  item1.Play();
              }

          }



      }

</details>

Input.GetAxisRaw를 통하여 수평, 수직 방향키를 입력받아 값을 이용한다.

그것을 이용하여 사방으로 힘을 받아 움직이게 하였으며, 점프는 Jump라는 이름으로 등록 된 키(Space Bar)를 이용하여 입력 받아 사용하였다.


**카메라 설정**

카메라는 플레이어를 따라다니게 만들고자 한다. 그런데 기존에 배틀그라운드, 마인크래프트와 같이 3인칭 방향으로 모든 방향을 볼 수 있게 하는 것 까지는 아니고 어느 정도의 거리를 떨어져서 전진할 수 있게끔 만들고자 한다.

(앞을 보는대로 카메가 각도 및 위치가 변하는 것은.. 사람 모양의 캐릭터를 사용할 때 사용하는 것이 좋을 것 같다.)


카메라에 위와 같은 기능을 추가 해 주기 위해서는 카메라의 좌표와 플레이어의 좌표가 필요하다. 그 좌표들의 차이만큼을 offSet으로 만들어 주어 카메라의 위치에 LateUpdate로 적용시켜 주면 된다.

(UI, 카메라 등은 연산이 끝난 뒤에 LateUpdate를 통하여 갱신 해 주어야 한다.)

<details>
  <summary>CameraMove.cs</summary>
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraMove : MonoBehaviour
    {
        Transform playerTransform;
        Vector3 offSet;
        // Start is called before the first frame update
        void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            offSet = transform.position - playerTransform.position;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = playerTransform.position + offSet;
        }
    }


</details>

![image](https://user-images.githubusercontent.com/66288087/187406610-1b3706ba-ddb9-496c-845a-e0aceebc142e.png)

실행하게 되면 위와 같이 카메라가 플레이어 근처에 보이는 것을 볼 수 있다.

![image](https://user-images.githubusercontent.com/66288087/187406752-8c7e3235-db1f-4698-a069-c52383145348.png)

인게임 시점


**아이템 만들기**

이제 아이템을 만들어 보도록 하자.

아이템은 간단하게 큐브를 통하여 만들어 보도록 하자.

![image](https://user-images.githubusercontent.com/66288087/187409223-ce0b6304-c4a1-43fa-af73-1da27af1d6d6.png)


아이템에 플레이어가 닿게 되면 플레이어에게서 소리가 나고(이건 Move.cs 코드에 있다.), 아이템이 비활성화 되게 하기 위해서는 Item Inspector에서 Collider IsTrigger를 체크 해 주면 된다.

![image](https://user-images.githubusercontent.com/66288087/187407127-4b4d55c1-5265-4567-9d7f-10ead683f178.png)

그리고 OnTriggerEnter 이벤트를 이용하여 다른 Collider가 닿았을 때, 비활성화를 걸어주게 되면 된다.

또한, 아이템이 가만히 있으면 좀 허전하니 뱅글뱅글 돌게끔 해 주었다.

<details>
  <summary>ItemRotation.cs</summary>
  
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ItemRotation : MonoBehaviour
    {
        public float RotateSpeed;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime,Space.World); // 단위벡터는 new Vector3(0,1,0) 대신 up을 써도 된다.
            // Update에서 움직인다고 하면 어떠한 환경에서도 같은 속도를 유지하기 위해서 deltaTime을 곱해준다.

            // 여러 가지 함수들이 오버로딩 되어 있기에 매개변수를 조절 해 주면서 필요한 기능들을 사용 해 주는 것이 좋다.
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.name == "Player")
            {
                // Move player = other.gameObject.GameObject<Move>();
                other.gameObject.GetComponent<Move>().Score++;
                gameObject.SetActive(false); // 안보이게 비활성화 해 준다.


            }
        }

    }


</details>

여기서 주목해야 할 점은 Rotate 함수이다.

Rotate함수는 6개의 오버로딩 된 함수들이 존재한다.

여기서 사용한 것 중에 세 번째 매개변수에서는 Space.World라는 것이 보일 것이다. 저것은 좌표의 기준을 전역좌표계를 사용하라는 것이다.

![image](https://user-images.githubusercontent.com/66288087/187409051-ae3e00c1-be19-4415-9562-13b223666c72.png)

위 그림과 같이 지역(로컬) 좌표계는 각 물체의 기준으로 달라지게 된다.

따라서 지역 좌표계로 코드에서와 같이 Vector3(0,1,0) 방향으로 회전하게 되면 기울어진 축을 기준으로 돌게 됨을 볼 수 있다.

그것을 원한다면 그대로 두어도 되지만 그렇지 않다면 전역 좌표계를 사용 해 주어야 한다.

코드에서는 전역 좌표계를 사용하였다.


**맵 구성**

이제 맵 구성을 해 보도록 하겠다.

![image](https://user-images.githubusercontent.com/66288087/187630501-a3336ef0-859c-49ad-bbb1-d4f6ab89d30d.png)

일차적으로 듀토리얼의 역할도 하는 첫 번째 스테이지를 구성 해 보았다.

Physic material에서 Bounce를 할 수 있게 만든 발판도 하나 만들어 주었다.

![image](https://user-images.githubusercontent.com/66288087/187630831-29b54dd3-402b-4a31-9484-3ff435ad0406.png)

또한, 주황색 바닥에 떨어지게 되면 다시 원 위치로 돌아오게 하는 부분도 추가 해 주었다.

그리고 세이브 포인트도 제작하여 세이브 포인트의 위치로 돌아가게끔 하는 기능도 구현할 것이다.



### 세부 Step2. 스테이지 매니징 설정



### 세부 Step3. 대기맵 제작 및 최고점수 표시 게시판 제작






