using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public float maxSpeed;
    public float jumpPower;
    private int JumpCount;
    public int maxCnt;
    SpriteRenderer spriteRenderer;
    Animator anim; // �ִϸ�����!

    GameObject ScanObj;

    public Manager manager; // ��ȭ�� ��ȭ ������ ��� �� �ִ� ��ũ��Ʈ�� �������� ���̴�.

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>(); // �ʱ�ȭ
        spriteRenderer = GetComponent<SpriteRenderer>(); // �ʱ�ȭ
        anim = GetComponent<Animator>(); // �ʱ�ȭ!
        JumpCount = maxCnt;
    }

    void Update()
    {
        // �ܹ����� Ű �Է��� FixedUpdate���� �ϸ� ���� �� �ִ�. ���� Update�� �̿��Ѵ�.

        if (Input.GetButtonDown("Jump") && (!anim.GetBool("IsJumping") || JumpCount > 0) && (manager.isAction == false)) // ������ �ϰ� ���� ���� �ȵǰ�!
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
            JumpCount--;
        }


        // Logic3. Stop
        if (Input.GetButtonUp("Horizontal"))
        {
            // Ű���忡�� ��,�� ��ư�� ���� ��!(Up!!)
            // ������ �̾Ƽ� ���� ���ؼ��� ���� �̵� ������ ��Ÿ���� �ӵ��� ��������ȭ �ؾ� �Ѵ�.(normalized)
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // ��ư�� ���� �� �ӵ��� �ٿ��� �Ѵ�.(�̲������� �ʰ�)

        }

        // Direction Sprite
        /*
        if (Input.GetButtonDown("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }*/
        /*
        if (Input.GetKey(KeyCode.RightArrow)) spriteRenderer.flipX = false;
        if (Input.GetKey(KeyCode.LeftArrow)) spriteRenderer.flipX = true;
        */

        // ���� ��
        if (Input.GetButton("Horizontal") && (manager.isAction == false)) // ��ȭ���� �ƴҶ� �¿������ �ǰԲ�
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if (Mathf.Abs(rigid.velocity.x) < 0.5)
        {
            // ������ ���! = �ӵ����Ͱ� 0..!
            anim.SetBool("IsWalking", false);
        }
        else
        {
            anim.SetBool("IsWalking", true);
        }

        if(Input.GetKeyDown(KeyCode.B) && ScanObj != null)
        {
            manager.Action(ScanObj);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Logic1. Move
        // Ű�� �����Ϳ� ���� ���� ��´�.
        float h = manager.isAction ? 0 : Input.GetAxisRaw("Horizontal"); // ��ȭ���� ������ ���� h�� 0��, �ƴ� ���� ������ GetAxisRaw���� �ѱ��.
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Logic2. maxSpeed
        if(rigid.velocity.x > maxSpeed) // right max Speed - ���������� �ִ� �ӵ����� Ŭ ��
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < (-1) * maxSpeed) // Left max Speed - �������� �ִ� �ӵ� �̸��� �� (�ε�ȣ �� �� ��)
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // Landing Platform

        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // ������ġ, ����, �Ÿ�

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("IsJumping", false);
                    JumpCount = maxCnt;
                }

            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("�ƾ�!");
            OnDamaged(collision.transform.position);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "NPC")
        {
            ScanObj = collision.gameObject;
            Debug.Log("NPC�� �ε�����!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "NPC")
        {
            ScanObj = null;
            Debug.Log("NPC���Լ� ������!");
        }
    }


    void OnDamaged(Vector2 targetPosition)
    {
        gameObject.layer = 9; // �÷��̾��� ���̾ 9��(PlayerDamaged)���� �ٲ㼭 ������ ���� �� ����.

        spriteRenderer.color = new Color(1, 1, 1, 0.4f); // r,g,b,alpha // ������ �����Ͽ� �¾����� �����ش�.

        //Reaction Force
        int dirc = transform.position.x - targetPosition.x > 0 ? 3 : -3; // ���׿�����.. ����

        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        //Animation
        anim.SetTrigger("Damaged");


        Invoke("OffDamaged", 1);

    }

    void OffDamaged()
    {
        gameObject.layer = 8; // ���� ���̾�� ���� ����

        spriteRenderer.color = new Color(1, 1, 1, 1); // ������ �����·�

    }

}
