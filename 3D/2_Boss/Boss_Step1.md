# Step1. 캐릭터 이동, 점프, 회피(구르기?)

가장 기초적이고 중요한 부분은 역시 플레이어가 아닐까 생각한다.

플레이어는 [골드메탈님의 Asset](https://assetstore.unity.com/packages/3d/characters/quarter-view-3d-action-assets-pack-188720)을 사용하여 진행하였으며,

[골드메탈님의 쿼터뷰 강좌](https://www.youtube.com/playlist?list=PLO-mt5Iu5TeYkrBzWKuTCl6IUm_bA6BKy)를 참고하였다.

처음 기초적인 부분을 강좌로 배운 뒤, 응용하여 다양한 기믹들을 제작 할 예정이다.

<hr>

우선 에셋으로 받은 플레이어이다.

![image](https://user-images.githubusercontent.com/66288087/190604782-7d1c2be8-73e8-4e22-a447-c1978a618238.png)

플레이어에 rigidBody와 Capsule Collider를 배치 해 주었다.

그리고 플레이어의 이동을 위해 코드를 하나 만들어 준다.

PlayerCode.cs

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCode : MonoBehaviour
{
    float horAxis; // 가로 방향키 입력 시 값을 받을 변수
    float verAxis; // 세로 방향

    bool runDown; // 대쉬 버튼이 눌렸는가?
    
    Vector3 moveVec;

    public float playerSpeed;

    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    
        horAxis = Input.GetAxisRaw("Horizontal");
        verAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        
        moveVec = new Vector3(horAxis, 0, verAxis).normalized; // 어느 방향으로 이동하던지 같은 속도로 가게끔 만들어 준다. (단위벡터화)

        if (runDown)
        {
            transform.position += moveVec * (playerSpeed+3) * Time.deltaTime; // 달릴 때 속도 늘리기
        }
        else
        {
            transform.position += moveVec * playerSpeed * Time.deltaTime; // deltaTime은 어느 환경에서나 같게 만들기 위함
        }
    }

}
</code>
</pre>

우선 단순하게 이동만을 고려한 코드이다.

이렇게 되면 플레이어는 서 있는 상태로 위치만 변하게 될 것이다.

그리고 player의 이동에서 Time.deltaTime을 곱해 주어, 컴퓨터의 성능에 따라 이동 거리가 달라지지 않게끔 해 주었다. [왜 deltaTime을 곱해주는가?](https://wergia.tistory.com/313)

왜 그런지는 앞 링크에서 나오는 블로그에서 볼 수 있다.

<hr>

**이동 애니메이션 넣기**

그런데 플레이어가 그냥 무미건조하게 움직이니 생동감이 너무 없다.

따라서 플레이어의 이동에 생동감을 불어넣기 위해 애니메이션을 넣어 준다.

새롭게 애니메이터를 만들어 준 다음, 아래 사진과 같이 애니메이터 속에 걷기, 달리기 애니메이션을 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/190607174-9720c4af-104b-4dde-9891-4d10d4cb9fb2.png)

Parameter들은 IsWalk, IsRun (bool)을 사용 해 주었으며, 애니메이션 상태를 이동할 때, 각 bool 변수를 조건으로 넣어주어 작동하게 해 준다.

(ex - Idle -> Walking 은 IsWalk가 true일 때, Walking -> Idle은 IsWalk 가 false 일 때)

그런데, 여기서 주의할 점이 있다.

![image](https://user-images.githubusercontent.com/66288087/190608097-b285881a-5984-4f5d-baf8-3f5797b6f152.png)

Idle -> Run 에서는 걷는 것과 동시에 달릴 수도 있으니, IsWalk 와 IsRun을 둘 다 조건으로 넣어주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/190608166-bece1047-5096-4a98-bacd-7d649fd39b3c.png)

그리고 Run -> Idle에서는 IsWalk가 false가 되는 것을 조건으로 넣어 주어야 한다.

왜냐하면 달리기 모드만이 풀리는 것은 그대로 걸어갈 수도 있는 것이기 때문이다.

아예 Idle 상태로 가는 것은 걷는 것을 멈추었을 때만 해당된다.

이제 Animator 설정이 끝났다면 애니메이터는 플레이어의 자식 오브젝트에 넣어 주도록 한다.

![image](https://user-images.githubusercontent.com/66288087/190608506-d2d779aa-e366-458c-8953-51bcfd522097.png)

Mesh Object!

그리고 코드를 수정 해 주도록 하자.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCode : MonoBehaviour
{
    float horAxis; // 가로 방향키 입력 시 값을 받을 변수
    float verAxis; // 세로 방향

    bool runDown; // 대쉬 버튼이 눌렸는가?

    Vector3 moveVec;

    public float playerSpeed;

    Animator anim;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>(); // 자식 오브젝트에 있는 컴포넌트를 가져온다.
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
        Move_Ani();
    }

    void InputKey()
    {
        horAxis = Input.GetAxisRaw("Horizontal");
        verAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");

    }

    void Move_Ani()
    {
        moveVec = new Vector3(horAxis, 0, verAxis).normalized; // 어느 방향으로 이동하던지 같은 속도로 가게끔 만들어 준다. (단위벡터화)

        if (runDown)
        {
            transform.position += moveVec * (playerSpeed+3) * Time.deltaTime; // 달릴 때 속도 늘리기
        }
        else
        {
            transform.position += moveVec * playerSpeed * Time.deltaTime; // deltaTime은 어느 환경에서나 같게 만들기 위함
        }
        

        anim.SetBool("IsWalk", moveVec != Vector3.zero); // 입력이 있을 때 움직여야 하니 조건문을 넣었다.
        anim.SetBool("IsRun", runDown);
    }

}
</code>
</pre>

같은 기능들을 하는 것은 함수로 따로 묶어 놓았다.

그리고 IsWalk를 활성화 시키는 것은 moveVec가 0벡터가 아니어야 된다. (즉, 움직이고 있어야 한다!)

조건문을 통하여 true/false를 조절할 수 있다.

<hr>

**점프, 회피(구르기) 추가**

이제, 점프를 구현 해 보도록 하자.

점프는 걸을 때, 달릴 때, 가만히 있을 때 구분 없이 어느 상태에서나 애니메이션이 적용 될 수 있어야 한다.

![image](https://user-images.githubusercontent.com/66288087/190611907-bce24c41-0d84-4122-a728-11e3d835cebb.png)

따라서 사진과 같이 애니메이터를 구성 해 준다.

Jump와 Dodge를 같이 넣어 주었으며, (회피도 어느 상태에서나 되어야 하기 때문)

특별히 점프는 착지 모션도 있기에 같이 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/190612229-0dd8c549-e51a-407a-b79c-406ee43cd3a6.png)

그리고 Jump에 대한 애니메이션 출력 조건도 Parameter로 추가 해 주는데, 여기서는 Trigger를 넣어 준다.

IsJump 로 이름 지은 bool Parameter도 넣어 주었는데, 이것은 Land 애니메이션을 시작하게 만들어 주는 조건으로 사용된다.

일단 점프를 위한 코드를 작성 해 보자

<pre>
<code>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCode : MonoBehaviour
{
    float horAxis; // 가로 방향키 입력 시 값을 받을 변수
    float verAxis; // 세로 방향

    bool runDown; // 대쉬 버튼이 눌렸는가?
    bool jumpDown; // 점프 키가 눌렸는가?
    bool dodgeDown; // 회피 키가 눌렸는가?

    bool isJump; // 점프를 하고 있는가?
    bool isDodge; // 회피를 하고 있는가?(구르기)

    Vector3 moveVec;
    Vector3 dodgeVec; // 회피 방향

    public float playerSpeed;

    Animator anim;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>(); // 자식 오브젝트에 있는 컴포넌트를 가져온다.
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
        Move_Ani();
        Jump();
        TrunChar();
        Dodge();
    }

    void InputKey()
    {
        horAxis = Input.GetAxisRaw("Horizontal");
        verAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        jumpDown = Input.GetButtonDown("Jump");
        dodgeDown = Input.GetKey(KeyCode.Z);

    }

    void Move_Ani()
    {
        moveVec = new Vector3(horAxis, 0, verAxis).normalized; // 어느 방향으로 이동하던지 같은 속도로 가게끔 만들어 준다. (단위벡터화)

        if (runDown)
        {
            transform.position += moveVec * (playerSpeed+3) * Time.deltaTime; // 달릴 때 속도 늘리기
        }
        else
        {
            transform.position += moveVec * playerSpeed * Time.deltaTime; // deltaTime은 컴퓨터 환경에 이동 거리가 영향을 받지 않게 하기 위함!
        }
        

        anim.SetBool("IsWalk", moveVec != Vector3.zero); // 입력이 있을 때 움직여야 하니 조건문을 넣었다.
        anim.SetBool("IsRun", runDown);
    }

    void TrunChar()
    {
        transform.LookAt(transform.position + moveVec); // 나아가는 방향으로 자동으로 회전되게 한다.
    }

    void Jump()
    {
        if (jumpDown && !isJump) // 점프키가 눌리고 점프 상태가 아닐 때!
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("DoJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (dodgeDown && moveVec != Vector3.zero && !isDodge) // Z 키가 눌리고 점프 상태가 아닐 때!
        {
            dodgeVec = moveVec; // 구르는 시점의 이동 벡터
            playerSpeed *= 2;
            anim.SetTrigger("DoDodge");
            isDodge= true;

            Invoke("DoDodge", 0.5f);
        }
    }

    void DoDodge()
    {
        playerSpeed *= 0.5f;
        Invoke("DodgeCoolDown", 5.0f); // 쿨타임은 5초!
    }

    void DodgeCoolDown()
    {
        Debug.Log("회피 쿨타임 종료!");
        isDodge = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "base")
        {
            anim.SetBool("IsJump", false);
            isJump = false;
        }
    }

}
</code>
</pre>

설명하면 우선 점프는 무한 점프를 막기 위하여 점프를 뛰게 되면 bool 변수인 isJump를 true로 만들어 준다.

그리고, 땅에 착지하게 되면(OnCollisionEnter), isJump를 false로 만들어 주어 다시 점프할 수 있게 해 준다. (땅의 tag가 base여야 한다.)

점프 애니메이션 역시 땅에 닿았을 때 IsJump Parameter를 false로 만들어 주어 애니메이션을 재생 해 준다.


회피는 점프와 비슷하게 Z키를 눌렀을 때, 트리거가 발동되며, 이동속도를 2배로 만들어 구르는 느낌을 준다.

그 다음, 일정 시간의 딜레이를 준 다음, 이동 속도를 원래대로 돌려 놓고 나는 여기에 쿨타임을 추가 해 주었다.

5초의 시간 뒤에 isDodge를 false로 만들어 주어 다시 구를 수 있게 하였다. ( 이것은 나중에 UI로 표현하면 좋을 것 같다. )

![image](https://user-images.githubusercontent.com/66288087/190614771-871c03d0-0599-4379-948e-2a8a63718238.png)

Dodge가 딜레이에 비해 애니메이션 속도가 느리다면 Animator에서 speed를 높게 설정 해 주면 된다.

이제 이렇게 해 주면 캐릭터의 이동은 얼추 완성이다.













