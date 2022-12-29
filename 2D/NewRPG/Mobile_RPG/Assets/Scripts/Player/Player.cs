using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Ű �Է¿� ���� ������
    int directionValue; // ���� �� ( + �϶��� ������, - �϶��� ���� )

    bool jumpKey;

    // ���¿� ���� ������
    int jumpCount;



    // ������ ����
    Rigidbody2D rigid;
    Vector3 moveVec; // �����̴� ������ ǥ���ϴ� ����


    // �÷��ƾ� ������
    PlayerStats playerStat;
    StatInformation statInfo;

    // �÷��̾� �ܰ�
    SpriteRenderer sprite;

    // �ִϸ��̼�
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
        jumpKey = Input.GetKeyDown(KeyCode.LeftAlt); // ���� Alt Ű�� ���� ������ �� �� ����
    }

    void toStop()
    {
        // �̵��� ���� �� ����
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // ��ư�� ���� �� �ӵ��� �ٿ��� �Ѵ�.(�̲������� �ʰ�)
            directionValue = 0;
        }
    }

    public void tojump()
    {
        if (jumpKey && (jumpCount > 0 || !anim.GetBool("isJump"))) // ���� ī��Ʈ�� ���Ұų� ī��Ʈ�� ��� ������ �� �� �ִ� �ִϸ��̼��� �ȴٸ�(���� ������ �ؾ� �� ���°� �ȴ�.) - ������ ������ �� �� �ְԲ�?
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
            // y�� �ӵ��� ���� = �������� ����
            // �������� ���� ��, ���� üũ�� �����Ѵ�.

            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); // �������� �׸���. (�ð������� ���� ����)

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // ��¥ ������ �׸��� (������ġ, ����, �Ÿ�)
            // Platform�̶�� �̸��� ���̾ ���� �Ϳ� ������� üũ�ض�!

            if(rayHit.collider != null)
            {
                // Platform�� ����� ��

                if (rayHit.distance < 0.8f) // ���� �Ÿ� �̸����� ��������� ��
                {
                    anim.SetBool("isJump", false); // ���� �ִϸ��̼� ����
                    jumpCount = statInfo.playerMaxJumpCount; // ���� ī��Ʈ ����
                }

            }



        }
    }


    void checkSprite()
    {
        // ĳ������ ��/�� ���� ����
        if(Input.GetButton("Horizontal") || directionValue != 0)
        {
            // ��, ��� �̵��� ��
            sprite.flipX = (Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1); // ������ �� �ٲپ� �־�� �ϴ� �������� �̵��� �� �´� ������ �־� �־���.

        }
    }

    public void moveChar()
    {
        float h = Input.GetAxisRaw("Horizontal") + directionValue; // ������ ���� + , ������ ���� -
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > statInfo.playerMaxSpeed) // �ִ� �ӵ��� ���� �ʰ�
        {
            rigid.velocity = new Vector2(statInfo.playerMaxSpeed,rigid.velocity.y);
        }
        else if(rigid.velocity.x < (-1) * statInfo.playerMaxSpeed)
        {
            rigid.velocity = new Vector2((-1) * statInfo.playerMaxSpeed, rigid.velocity.y);
        }

        if(Math.Abs(rigid.velocity.x) < 0.5f) // �ݴ� ���⵵ ����ؾ� �ϱ� ������ ���� ����
        {
            // 0���� �ϰ� �Ǹ� �����ϰ� ����� �ִϸ��̼��� ���߱� ������ 0.5�� ����
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
                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // ��ư�� ���� �� �ӵ��� �ٿ��� �Ѵ�.(�̲������� �ʰ�)
                directionValue = 0;
                break;
            case "R":                
                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // ��ư�� ���� �� �ӵ��� �ٿ��� �Ѵ�.(�̲������� �ʰ�)
                directionValue = 0;
                break;
            case "Jump":
                jumpKey = false;
                break;
        }
    }


}
