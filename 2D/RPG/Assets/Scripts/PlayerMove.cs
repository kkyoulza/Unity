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
    Animator anim; // 애니메이터!

    GameObject ScanObj;

    public Manager manager; // 대화시 대화 내용을 출력 해 주는 스크립트를 가져오는 것이다.

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>(); // 초기화
        spriteRenderer = GetComponent<SpriteRenderer>(); // 초기화
        anim = GetComponent<Animator>(); // 초기화!
        JumpCount = maxCnt;
    }

    void Update()
    {
        // 단발적인 키 입력은 FixedUpdate에서 하면 끊길 수 있다. 따라서 Update를 이용한다.

        if (Input.GetButtonDown("Jump") && (!anim.GetBool("IsJumping") || JumpCount > 0) && (manager.isAction == false)) // 점프를 하고 있을 때는 안되게!
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
            JumpCount--;
        }


        // Logic3. Stop
        if (Input.GetButtonUp("Horizontal"))
        {
            // 키보드에서 좌,우 버튼을 뗐을 때!(Up!!)
            // 방향을 뽑아서 쓰기 위해서는 현재 이동 방향을 나타내는 속도를 단위벡터화 해야 한다.(normalized)
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // 버튼을 뗐을 때 속도를 줄여야 한다.(미끄러지지 않게)

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

        // 수정 후
        if (Input.GetButton("Horizontal") && (manager.isAction == false)) // 대화중이 아닐때 좌우반전이 되게끔
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if (Mathf.Abs(rigid.velocity.x) < 0.5)
        {
            // 멈췄을 경우! = 속도벡터가 0..!
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
        // 키를 누른것에 대한 값을 얻는다.
        float h = manager.isAction ? 0 : Input.GetAxisRaw("Horizontal"); // 대화중인 상태일 때는 h에 0을, 아닐 때는 온전한 GetAxisRaw값을 넘긴다.
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Logic2. maxSpeed
        if(rigid.velocity.x > maxSpeed) // right max Speed - 오른쪽으로 최대 속도보다 클 때
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < (-1) * maxSpeed) // Left max Speed - 왼쪽으로 최대 속도 미만일 때 (부등호 잘 쓸 것)
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // Landing Platform

        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // 시작위치, 방향, 거리

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
            Debug.Log("아얏!");
            OnDamaged(collision.transform.position);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "NPC")
        {
            ScanObj = collision.gameObject;
            Debug.Log("NPC와 부딪혔다!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "NPC")
        {
            ScanObj = null;
            Debug.Log("NPC에게서 떠났다!");
        }
    }


    void OnDamaged(Vector2 targetPosition)
    {
        gameObject.layer = 9; // 플레이어의 레이어를 9번(PlayerDamaged)으로 바꿔서 무적을 적용 해 주자.

        spriteRenderer.color = new Color(1, 1, 1, 0.4f); // r,g,b,alpha // 투명도를 설정하여 맞았음을 보여준다.

        //Reaction Force
        int dirc = transform.position.x - targetPosition.x > 0 ? 3 : -3; // 삼항연산자.. ㄷㄷ

        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        //Animation
        anim.SetTrigger("Damaged");


        Invoke("OffDamaged", 1);

    }

    void OffDamaged()
    {
        gameObject.layer = 8; // 원래 레이어로 돌려 놓기

        spriteRenderer.color = new Color(1, 1, 1, 1); // 투명도도 원상태로

    }

}
