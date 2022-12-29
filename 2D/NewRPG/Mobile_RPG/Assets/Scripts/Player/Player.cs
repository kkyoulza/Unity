using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 키 입력에 대한 변수들
    int directionValue; // 방향 값 ( + 일때는 오른쪽, - 일때는 왼쪽 )

    bool jumpKey;

    // 상태에 대한 변수들
    int jumpCount;



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
        jumpCount = statInfo.playerMaxJumpCount;
    }

    // Update is called once per frame
    void Update()
    {
        getKeys();
        tojump();
        checkLanding();
        checkSprite();
        toStop();
    }

    void FixedUpdate()
    {
        moveChar();
    }

    void getKeys()
    {
        jumpKey = Input.GetKeyDown(KeyCode.LeftAlt); // 왼쪽 Alt 키를 통해 점프를 할 수 있음
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

    public void tojump()
    {
        if (jumpKey && (jumpCount > 0 || !anim.GetBool("isJump"))) // 점프 카운트가 남았거나 카운트가 없어도 점프를 할 수 있는 애니메이션이 된다면(물론 착지를 해야 이 상태가 된다.) - 찰나의 순간에 할 수 있게끔?
        {
            anim.SetBool("isJump", true);
            rigid.AddForce(Vector2.up * statInfo.playerJumpPower,ForceMode2D.Impulse);
            jumpCount--;
        }
    }

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
                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // 버튼을 뗐을 때 속도를 줄여야 한다.(미끄러지지 않게)
                directionValue = 0;
                break;
            case "Jump":
                jumpKey = false;
                break;
        }
    }


}
