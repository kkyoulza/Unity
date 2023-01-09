using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 키 입력에 대한 변수들
    int directionValue; // 방향 값 ( + 일때는 오른쪽, - 일때는 왼쪽 )

    bool jumpKey;
    bool attackKey;

    // 상태에 대한 변수들
    int jumpCount;

    public bool isAttack; // 공격 중일 때
    public bool isHit; // 피격 당하는 중

    // 움직임 관련
    Rigidbody2D rigid;
    Vector3 moveVec; // 움직이는 방향을 표현하는 벡터  

    // 플레아어 정보들
    PlayerStats playerStat;
    PlayerSkills playerSkill;
    StatInformation statInfo;
    Skills[] skillInfos; // 스킬 명단들

    // 플레이어 외관 및 스킬 방향
    SpriteRenderer sprite;

    public GameObject playerMesh; // 플레이어 외관
    public GameObject atkPoint; // 공격 이펙트가 나가는 포인트
    public GameObject dmgPos; // 플레이어 피격 데미지 생성 위치

    // 애니메이션
    Animator anim;

    // pooling
    dmgPool pooling;

    // Start is called before the first frame update
    void Awake()
    {
        playerSkill = GetComponent<PlayerSkills>();
        statInfo = GetComponent<PlayerStats>().playerStat;
        skillInfos = playerSkill.skillInfos;

        rigid = GetComponent<Rigidbody2D>();

        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        pooling = GetComponent<dmgPool>();
        jumpCount = statInfo.playerMaxJumpCount;
    }

    // Update is called once per frame
    void Update()
    {
        // 버튼 입력 같은 것들
        getKeys();
        tojump();
        normalAttack();
        checkLanding();
        checkSprite();
        toStop();
    }

    void FixedUpdate()
    {
        // 물리적 처리
        moveChar();
    }

    void getKeys()
    {
        jumpKey = Input.GetKeyDown(KeyCode.LeftAlt); // 왼쪽 Alt 키를 통해 점프를 할 수 있음
        attackKey = Input.GetKeyDown(KeyCode.LeftControl); // 왼쪽 Control 키를 통해 공격한다. 
    }

    public void normalAttack()
    {
        if (attackKey && !skillInfos[0].isUse) // 공격중이지 않으면서 공격 키가 눌렸을 때
        {
            skillInfos[0].isUse = true;
            StartCoroutine(playerSkill.ableAtkSkill(0, 0.3f,statInfo.afterDelay));
        }
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
            // sprite.flipX = (Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1); // 왼쪽일 때 바꾸어 주어야 하니 왼쪽으로 이동할 때 맞는 조건을 넣어 주었다.

            if (!isAttack) // 공격 이펙트가 진행되지 않을 때에만 방향 전환이 가능하게!!
            {
                if ((Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1)) // 위 조건을 똑같이 써 주어, 왼쪽을 볼 때는 scale에 (-1,1,1)벡터를 넣어 준다. (-1,0,0)넣으면 캐릭터가 사라진다.
                {
                    playerMesh.transform.localScale = new Vector3(-1, 1, 1); // scale의 x 좌표를 -1로 바꾸어 주면 scale에 들어가는 것이 벡터이니 방향이 바뀌게 된다.
                }
                else
                {
                    playerMesh.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else // 공격 중일 때는 방향전환 X
            {
                return;
            }
            
        }
    }

    public void moveChar()
    {
        float h = Input.GetAxisRaw("Horizontal") + directionValue; // 우측일 때는 + , 좌측일 때는 -

        if((isAttack && (transform.localScale.x * h) > 0) || !isAttack) // 공격을 하고, 같은 방향으로 이동할 때 or 공격 중이 아닐 때
        {
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        } 
        else // 공격하는 방향이 아닌 반대 방향으로 돌려고 할 때는 이동하지 못하게!
        {
            return;
        }

        

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
            case "Attack":
                attackKey = true;
                normalAttack();
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
            case "Attack":
                attackKey = false;
                break;
        }
    }

}
