using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Ű �Է¿� ���� ������
    int directionValue; // ���� �� ( + �϶��� ������, - �϶��� ���� )

    bool jumpKey;
    bool attackKey;

    // ���¿� ���� ������
    int jumpCount;

    public bool isAttack; // ���� ���� ��
    public bool isHit; // �ǰ� ���ϴ� ��

    // ������ ����
    Rigidbody2D rigid;
    Vector3 moveVec; // �����̴� ������ ǥ���ϴ� ����  

    // �÷��ƾ� ������
    PlayerStats playerStat;
    PlayerSkills playerSkill;
    StatInformation statInfo;
    Skills[] skillInfos; // ��ų ��ܵ�

    // �÷��̾� �ܰ� �� ��ų ����
    SpriteRenderer sprite;

    public GameObject playerMesh; // �÷��̾� �ܰ�
    public GameObject atkPoint; // ���� ����Ʈ�� ������ ����Ʈ
    public GameObject dmgPos; // �÷��̾� �ǰ� ������ ���� ��ġ

    // �ִϸ��̼�
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
        // ��ư �Է� ���� �͵�
        getKeys();
        tojump();
        normalAttack();
        checkLanding();
        checkSprite();
        toStop();
    }

    void FixedUpdate()
    {
        // ������ ó��
        moveChar();
    }

    void getKeys()
    {
        jumpKey = Input.GetKeyDown(KeyCode.LeftAlt); // ���� Alt Ű�� ���� ������ �� �� ����
        attackKey = Input.GetKeyDown(KeyCode.LeftControl); // ���� Control Ű�� ���� �����Ѵ�. 
    }

    public void normalAttack()
    {
        if (attackKey && !skillInfos[0].isUse) // ���������� �����鼭 ���� Ű�� ������ ��
        {
            skillInfos[0].isUse = true;
            StartCoroutine(playerSkill.ableAtkSkill(0, 0.3f,statInfo.afterDelay));
        }
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
            // sprite.flipX = (Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1); // ������ �� �ٲپ� �־�� �ϴ� �������� �̵��� �� �´� ������ �־� �־���.

            if (!isAttack) // ���� ����Ʈ�� ������� ���� ������ ���� ��ȯ�� �����ϰ�!!
            {
                if ((Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1)) // �� ������ �Ȱ��� �� �־�, ������ �� ���� scale�� (-1,1,1)���͸� �־� �ش�. (-1,0,0)������ ĳ���Ͱ� �������.
                {
                    playerMesh.transform.localScale = new Vector3(-1, 1, 1); // scale�� x ��ǥ�� -1�� �ٲپ� �ָ� scale�� ���� ���� �����̴� ������ �ٲ�� �ȴ�.
                }
                else
                {
                    playerMesh.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else // ���� ���� ���� ������ȯ X
            {
                return;
            }
            
        }
    }

    public void moveChar()
    {
        float h = Input.GetAxisRaw("Horizontal") + directionValue; // ������ ���� + , ������ ���� -

        if((isAttack && (transform.localScale.x * h) > 0) || !isAttack) // ������ �ϰ�, ���� �������� �̵��� �� or ���� ���� �ƴ� ��
        {
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        } 
        else // �����ϴ� ������ �ƴ� �ݴ� �������� ������ �� ���� �̵����� ���ϰ�!
        {
            return;
        }

        

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
                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // ��ư�� ���� �� �ӵ��� �ٿ��� �Ѵ�.(�̲������� �ʰ�)
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
