# Step1. 발판 설정 및 캐릭터 이동, 점프

본 내용은 유튜브에 있는 골드메탈님의 플랫포머 강좌를 참고하여 제작하였다.


### 기본 세팅

우선 이 게임은 모바일 세로 모드에서 플레이 된다는 점을 전제로 개발 할 예정이다.

따라서 아래 사진과 같이 미리 여러 기종들의 해상도들을 정리하였다.

![image](https://user-images.githubusercontent.com/66288087/209926129-50f4f54b-b8bd-4a4e-bfba-64bac2116432.png)

우선, 지형과 움직일 수 있는 플레이어에 대한 기본 설정부터 진행 하도록 하겠다.

<hr>

### 발판 설정

육성 진행에 있어서는 2D 횡스크롤 RPG로 제작 할 예정이다.

따라서 캐릭터가 딛을 수 있는 발판을 타일 맵을 이용하여 미리 제작 해 두고자 한다.

<hr>

#### Sprite Collider 범위 설정

- 우선 발판에 사용한 에셋은 [Nature Pixel Asset](https://assetstore.unity.com/packages/2d/environments/nature-pixel-art-base-assets-free-151370)을 사용하였다.
- 또한, 캐릭터는 골드메탈님의 에셋들을 사용하였다. [캐릭터](https://assetstore.unity.com/packages/2d/characters/simple-2d-platformer-assets-pack-188518), [버튼](https://assetstore.unity.com/packages/2d/characters/top-down-2d-rpg-assets-pack-188718)

발판 Sprite에서 Sprite Editor에 들어가 준다.

![image](https://user-images.githubusercontent.com/66288087/209929875-2e313ccc-b422-4fec-a8ee-2724e04733e1.png)

Custom Physics Shape를 눌러 준 다음

![image](https://user-images.githubusercontent.com/66288087/209930196-2bfe3ee7-5853-4c31-a8f6-2714cd3b08d3.png)

Generate를 눌러 준 다음, Collider 경계선을 설정 해 준다.

<hr>

#### 타일 맵 설정 및 땅 그리기

경계선을 설정 해 준 다음, Sprite 조각을 타일 맵 팔렛트에 드래그 하여 세팅 해 준다.

![image](https://user-images.githubusercontent.com/66288087/209930597-1ac50399-b03a-4d97-8091-094cc5c85232.png)

그 다음,

![image](https://user-images.githubusercontent.com/66288087/209929174-7aee6934-827e-47ee-931c-65539610763a.png)

사진과 같이 base 땅을 설정하고

![image](https://user-images.githubusercontent.com/66288087/209929214-9afaf2b1-1217-485a-8503-65de8b099e59.png)

z 좌표를 뒤에 설정 해 주어 배경도 그려 준다.

![image](https://user-images.githubusercontent.com/66288087/209930800-353d4881-dbde-4979-84c2-f76dccb2b39a.png)

3D로 본 모습

base 땅에만 TileMapCollider2D를 설정 해 준다.

이제 위에서 움직일 수 있는 캐릭터를 만들도록 하겠다.

<hr>

### 캐릭터 이동

플레이어 모델은 골드메탈님의 에셋을 사용하였다.

![image](https://user-images.githubusercontent.com/66288087/209931297-9b260fe9-e6f8-4c78-ae05-51cfac212218.png)

<br>

플레이어의 이동은 모바일에서 사용할 수 있는 버튼과, PC에서 키보드를 이용하여 움직이게 설정하였다.


Player.cs 코드이다.

이 코드에는 플레이어의 입력에 의한 리액션(?)을 담았다. -> 그냥 키 입력을 통해 플레이어가 활동하는 것에 대한 내용이 들어 있다고 생각하면 된다.

그 중에서 플레이어가 움직이는 함수를 가져왔다.
<pre>
<code>
public void moveChar()
  {
      float h = Input.GetAxisRaw("Horizontal") + directionValue; // 우측일 때는 + , 좌측일 때는 -
      rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

      if(rigid.velocity.x > statInfo.playerMaxSpeed) // 최대 속도를 넘지 않게
      {
          rigid.velocity = new Vector2(statInfo.playerMaxSpeed,rigid.velocity.y);
      }
      else if(rigid.velocity.x < (-1) * statInfo.playerMaxSpeed)
      {
          rigid.velocity = new Vector2((-1) * statInfo.playerMaxSpeed, rigid.velocity.y);
      }

      if(Math.Abs(rigid.velocity.x) < 0.5f) // 반대 방향도 고려해야 하기 때문에 절댓값 적용
      {
          // 0으로 하게 되면 완전하게 멈춰야 애니메이션이 멈추기 때문에 0.5로 설정
          anim.SetBool("isWalk", false);
      }
      else
      {
          anim.SetBool("isWalk", true);
      }

  }
</code>
</pre>

##### 위 코드에서 살펴 봐야 할 부분들

- **위 함수는 FixedUpdate()에 존재**하여 움직임 명령이 입력되는 즉시 움직인다. 
- **directionValue** -> 버튼을 통해 입력받는 int(정수) 값, **1 - 오른쪽, 0 - 입력 없음, -1 - 왼쪽** 이다.
- **h** -> Input.GetAxisRaw("Horizontal")을 통하여 키보드의 좌, 우 화살표로 입력받는 값을 float값으로 받게 된다. directionValue 값을 더해 주어 모바일/PC에 대한 처리를 한 번에 진행하였다.
- **statInfo.playerMaxSpeed** -> **플레이어의 최대 속도**이다. statInfo는 밑에 적을 PlayerStats.cs 코드에 있는 StatInformation 클래스 객체가 statInfo이다. statInfo에는 플레이어의 체력, 공격력 등의 스탯 정보가 들어 있다.
- 가장 아래에 있는 조건문은 애니메이션 처리를 위한 조건문이다. 아래에도 정리 하겠지만 **속도가 완전히 0이 됐을 때 걷는 애니메이션을 멈추게 하면 플레이어가 손을 떼도 걷는 애니메이션이 너무 늘어지게 나오기 때문에 어느 정도의 값으로 설정** 해 주었다.


<pre>
<code>
public void GetButtonDown(string whatBtn)
    {
        switch (whatBtn)
        {
            case "L":
                directionValue = -1;
                break;
            case "R":
                directionValue = 1;
                break;
        }
    }

public void GetButtonUp(string whatBtn)
    {
        switch (whatBtn)
        {
            case "L":
                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // 버튼을 뗐을 때 속도를 줄여야 한다.(미끄러지지 않게)
                directionValue = 0;
                break;
            case "R":                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // 버튼을 뗐을 때 속도를 줄여야 한다.(미끄러지지 않게)
                directionValue = 0;
                break;
        }
    }
</code>
</pre>

모바일에서의 움직임을 제어하는 함수이다.

버튼에 매개변수로 어느 버튼인지 적어 두게 되면 해당 버튼을 눌렀을 때, 위에서 보았던 directionValue 값을 설정 해 준다.

움직이는 함수는 FixedUpdate()에 세팅되어 있기에 값만 바꾸어 주면 캐릭터가 바로 움직이고 멈출 것이다.

그리고 ButtonUp 함수에서는 버튼을 뗐을 때 속도를 줄여주는 부분을 넣어 주었다. 저 부분은 키보드를 뗐을 때도 발동되게 만든 함수가 있지만.. 키보드를 떼는 경우를 조건으로 넣어 주어서 버튼을 뗄 때는 별개로 세팅 해 주어야 한다.

##### 모바일 UI 세팅

잠깐 모바일 움직임 버튼 세팅을 정리하겠다.

버튼은 누르는 이벤트와 눌렀다 떼는 이벤트 두 개가 있어야 한다.

따라서 위와 그 이벤트에 대응 될 함수를 두 개 만들었고, 그 것을 적용시켜야 한다.

![image](https://user-images.githubusercontent.com/66288087/209970882-0c68f416-bf3b-4619-af0b-325978b103db.png)

위 사진과 같이 Event Trigger를 통하여 PointerDown/PointerUp을 사용하였다.

<hr>

##### 좌/우 반전 세팅

<pre>
<code>
void checkSprite()
{
    // 캐릭터의 좌/우 반전 설정
    if(Input.GetButton("Horizontal") || directionValue != 0)
    {
        // 좌, 우로 이동할 때
        sprite.flipX = (Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1); // 왼쪽일 때 바꾸어 주어야 하니 왼쪽으로 이동할 때 맞는 조건을 넣어 주었다.

    }
}
</code>
</pre>

위 부분에서는 캐릭터가 좌, 우로 움직일 때 캐릭터가 좌/우를 바라볼 수 있게 설정하는 것이다.

GetButton으로 좌/우 방향키가 입력 되는 시점일 때 발동하거나, 모바일에서는 키가 입력이 될 때 발동이 되게 하였다.

**flipX는 true일 때 뒤집어지기에 왼쪽으로 갈 때 true가 되어야 한다.**

따라서 **왼쪽이 입력될 때 맞는 조건**을 넣어 줌으로써, **왼쪽을 볼 때 flipX가 발동**되게 해 주었다.

<hr>

**Player.cs 원문**

<pre>
<code>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 키 입력에 대한 변수들
    int directionValue; // 방향 값 ( + 일때는 오른쪽, - 일때는 왼쪽 )

    // 상태에 대한 bool 변수


    // 움직임 관련
    Rigidbody2D rigid;
    Vector3 moveVec; // 움직이는 방향을 표현하는 벡터


    // 플레아어 정보들
    PlayerStats playerStat;
    StatInformation statInfo;

    // 플레이어 외관
    SpriteRenderer sprite;

    // 애니메이션
    Animator anim;


    // Start is called before the first frame update
    void Awake()
    {
        statInfo = GetComponent<PlayerStats>().playerStat;
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        getKeys();
        keyExecute();
        checkSprite();
        toStop();
    }

    void FixedUpdate()
    {
        moveChar();
    }

    void keyExecute()
    {

    }

    void toStop()
    {
        // 이동을 멈출 때 감속
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // 버튼을 뗐을 때 속도를 줄여야 한다.(미끄러지지 않게)
            directionValue = 0;
        }
    }

    void getKeys()
    {

    }
    
    void checkSprite()
    {
        // 캐릭터의 좌/우 반전 설정
        if(Input.GetButton("Horizontal") || directionValue != 0)
        {
            // 좌, 우로 이동할 때
            sprite.flipX = (Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1); // 왼쪽일 때 바꾸어 주어야 하니 왼쪽으로 이동할 때 맞는 조건을 넣어 주었다.

        }
    }

    public void moveChar()
    {
        float h = Input.GetAxisRaw("Horizontal") + directionValue; // 우측일 때는 + , 좌측일 때는 -
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > statInfo.playerMaxSpeed) // 최대 속도를 넘지 않게
        {
            rigid.velocity = new Vector2(statInfo.playerMaxSpeed,rigid.velocity.y);
        }
        else if(rigid.velocity.x < (-1) * statInfo.playerMaxSpeed)
        {
            rigid.velocity = new Vector2((-1) * statInfo.playerMaxSpeed, rigid.velocity.y);
        }

        if(Math.Abs(rigid.velocity.x) < 0.5f) // 반대 방향도 고려해야 하기 때문에 절댓값 적용
        {
            // 0으로 하게 되면 완전하게 멈춰야 애니메이션이 멈추기 때문에 0.5로 설정
            anim.SetBool("isWalk", false);
        }
        else
        {
            anim.SetBool("isWalk", true);
        }

    }


    public void GetButtonDown(string whatBtn)
    {
        switch (whatBtn)
        {
            case "L":
                directionValue = -1;
                break;
            case "R":
                directionValue = 1;
                break;
        }
    }

    public void GetButtonUp(string whatBtn)
    {
        switch (whatBtn)
        {
            case "L":
                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // 버튼을 뗐을 때 속도를 줄여야 한다.(미끄러지지 않게)
                directionValue = 0;
                break;
            case "R":                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // 버튼을 뗐을 때 속도를 줄여야 한다.(미끄러지지 않게)
                directionValue = 0;
                break;
        }
    }


}
</code>
</pre>

#### 걷는 모습 애니메이션

![image](https://user-images.githubusercontent.com/66288087/209933581-9b33a796-8b3b-4e8d-a7b1-582e58411d80.png)

Sprite에서 원하는 모습들을 골라 위 사진처럼 애니메이션을 만들 수 있다.

Player에 들어 갈 애니메이터도 만들어 주고, Idle, Walk 애니메이션을 만들어 준다.

![image](https://user-images.githubusercontent.com/66288087/209933719-845ae101-534f-4245-b42c-ea2a34f85cd9.png)

그리고 위와 같이 연결시켜 준다. (오른쪽은 점프가 있어서 잘랐다..)

그리고 왼쪽에 Parameter에서 bool로 아래와 같이 만들어 준다.

![image](https://user-images.githubusercontent.com/66288087/209933799-b018b256-109d-494d-a222-eddd71703da7.png)

isWalk 조건(bool)

isWalk는 캐릭터가 걷고 있을 때인지를 알려주는 조건이다.

즉, 애니메이터에서 아래 사진과 같이 설정 해 줄 수 있다.

![image](https://user-images.githubusercontent.com/66288087/209933951-84e48899-b4f2-4662-b440-50ede4b1fc7b.png)

Idle에서 Walk로 애니메이션을 변경하기 위해서는 isWalk가 true가 되어야 한다.

그러면 조건을 어떻게 조절 해 줄까?

아까 위에서 봤던 것 처럼 코드로 해 줄 수 있다.

<pre>
<code>
if(Math.Abs(rigid.velocity.x) < 0.5f) // 반대 방향도 고려해야 하기 때문에 절댓값 적용
{
    // 0으로 하게 되면 완전하게 멈춰야 애니메이션이 멈추기 때문에 0.5로 설정
    anim.SetBool("isWalk", false);
}
else
{
    anim.SetBool("isWalk", true);
}
</code>
</pre>

바로 이 부분이다.

아까 이야기 했듯이 걷는 애니메이션이 발동되는 조건을 **속도의 절대값**이 일정 수치 미만일 때로 설정 해 주었다.

(절대값을 안써주면 왼쪽으로 갈 때는 항상 애니메이션이 발동되지 않는다. 부호는 방향이기 때문!)

속도의 절대 값이 0일 때에만 걷는 애니메이션이 멈추게 해 주면 분명히 멈춰야 하는데 안멈춘다고 생각하는 구간이 생기게 된다.

완전히 속도가 0이 되는 것은 시간이 걸리기 때문에 어느 정도 속도가 낮아졌을 때 걷는 모션이 멈추고 살짝 미끄러지는 듯하게 표현 해 주는 것이다.

![2d_1_1](https://user-images.githubusercontent.com/66288087/209934795-f8a68436-8b9d-4973-b8c4-d9aff36458ef.gif)

테스트를 해 주면 위 움짤과 같이 캐릭터가 움직임을 볼 수 있다.


<hr>

### 캐릭터 점프

캐릭터 점프도 유사하게 설정 해 주면 된다.

Player.cs 코드에서 버튼 Down/Up 함수에 Jump 버튼을 추가 해주고, 점프 키가 눌렸음을 알려 주는 jumpKey라는 bool 변수를 추가 해 준다.

<pre>
<code>
public void GetButtonDown(string whatBtn)
{
    switch (whatBtn)
    {
        case "L":
            directionValue = -1;
            break;
        case "R":
            directionValue = 1;
            break;
        case "Jump":
            jumpKey = true;
            tojump();
            break;
    }
}

public void GetButtonUp(string whatBtn)
{
    switch (whatBtn)
    {
        case "L":
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // 버튼을 뗐을 때 속도를 줄여야 한다.(미끄러지지 않게)
            directionValue = 0;
            break;
        case "R":                
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);
            directionValue = 0;
            break;
        case "Jump":
            jumpKey = false;
            break;
    }
}
</code>
</pre>

점프 키는 아래와 같이 왼쪽 Alt 키로 설정 해 준다.

<pre>
<code>
 void getKeys()
{
    jumpKey = Input.GetKeyDown(KeyCode.LeftAlt); // 왼쪽 Alt 키를 통해 점프를 할 수 있음
}
</code>
</pre>

그리고 점프키를 누르게 되었다는 것을 인식하는 함수를 만들어 주고, Update()에 넣어 준다.

<pre>
<code>
public void tojump()
{
    if (jumpKey && (jumpCount > 0 || !anim.GetBool("isJump")))
    {
        anim.SetBool("isJump", true);
        rigid.AddForce(Vector2.up * statInfo.playerJumpPower,ForceMode2D.Impulse);
        jumpCount--;
    }
}
</code>
</pre>

- 점프를 하게 되면 위에서 만들어 두었던 점프 애니메이션이 발동 되어 점프를 하고 있음을 알려 주고, y축 양의 방향으로 힘을 주게 된다.
- **ForceMode2D.Impulse**를 사용하여 **즉각적인 힘**을 주게 된다.
- 조건문에 있는 jumpCount는 무한정 점프를 하는 것을 막기 위함이다.
- isJump 매개변수의 상태를 조건문에 넣음으로써 **점프중이 아니거나 점프 카운트가 남았을 때** 점프를 할 수 있게 해 주었다.

<hr>

<pre>
<code>
public void GetButtonDown(string whatBtn)
{
    switch (whatBtn)
    {
        case "L":
            directionValue = -1;
            break;
        case "R":
            directionValue = 1;
            break;
        case "Jump":
            jumpKey = true;
            tojump();
            break;
    }
}
</code>
</pre>

위에 적어 놓았던 코드 중에서, 버튼으로 점프 하는 것을 만들어 놓은 것이 있는데, 여기에는 jumpKey를 on하고 끝나는 것이 아닌, 점프 함수가 추가로 이어지게 하여 바로 점프를 할 수 있게 해 주었다.

<hr>

#### 착지 확인

그런데, JumpCount를 통하여 남은 점프 횟수를 관리 한다고 했는데.. JumpCount는 언제 초기화가 될까?

바로 땅에 닿았을 때 초기화가 된다.

땅에 닿았다는 것은 어떻게 체크할 수 있을까?

바로 LayCast를 이용하여 체크 해 준다.

<hr>

#### LayCast

LayCast는 레이저를 쏘아서 닿는 것을 출력 해 준다.

<pre>
<code>
public void checkLanding()
{
    if(rigid.velocity.y < 0)
    {
        // y축 속도가 음수 = 떨어지고 있음
        // 떨어지고 있을 때, 착지 체크를 진행한다.

        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); // 레이저를 그린다. (시각적으로 보기 위함)

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // 진짜 레이저 그리기 (시작위치, 방향, 거리)
        // Platform이라는 이름의 레이어를 가진 것에 닿는지만 체크해라!

        if(rayHit.collider != null)
        {
            // Platform에 닿았을 때

            if (rayHit.distance < 0.8f) // 일정 거리 미만으로 가까워졌을 때
            {
                anim.SetBool("isJump", false); // 점프 애니메이션 해제
                jumpCount = statInfo.playerMaxJumpCount; // 점프 카운트 리셋
            }

        }

    }

}
</code>
</pre>

쏘는 레이저는 보이지 않지만, Debug.DrawRay를 통하여 화면에 보이게 할 수 있다.

RayCast에는 (시작 지점, 레이저 방향, 거리, 특정 레이어 설정) 이 들어가게 된다.

레이저를 쏘는 장면을 화면으로 보면

![image](https://user-images.githubusercontent.com/66288087/210056024-a1d1dc86-a171-4688-8173-5f8a42ca6470.png)

위 사진과 같이 보이게 된다. (연두색이라 잘 안보이지만.. 다리 사이로 무언가가 나가고 있음을 볼 수 있을 것이다.)

아래 조건문에서 땅에 닿았을 때, rayHit.collider에 무언가가 감지 될 것이다.

여기에서도 걸음이 멈출 때와 유사하게 완전히 바닥에 닿았을 때가 아닌, 땅까지의 거리가 0.8f 일 때 점프 카운트가 차고 애니메이션이 풀리게 만들어 주었다.

<hr>

![2d_1_2](https://user-images.githubusercontent.com/66288087/210056820-2d89506a-f7a2-4472-aed8-884802880e3a.gif)

점프를 하는 움짤이다.

점프 버튼은 좌/우 버튼과 똑같이 EventTrigger를 통하여 만들어 주었으며, 매개변수에 Jump를 넣어 주었다.
