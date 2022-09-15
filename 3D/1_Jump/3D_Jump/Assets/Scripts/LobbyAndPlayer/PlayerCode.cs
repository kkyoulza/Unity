using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCode : MonoBehaviour
{
    float horAxis; // 가로 방향키 입력 시 값을 받을 변수
    float verAxis; // 세로 방향

    bool runDown; // 대쉬 버튼이 눌렸는가?
    bool jumpDown; // 점프 키가 눌렸는가?
    bool isJump; // 점프를 하고 있는가?

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
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
        Move_Ani();
        Jump();
        TrunChar();
    }

    void InputKey()
    {
        horAxis = Input.GetAxisRaw("Horizontal");
        verAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        jumpDown = Input.GetButtonDown("Jump");
    }

    void Move_Ani()
    {
        moveVec = new Vector3(horAxis, 0, verAxis).normalized; // 어느 방향으로 이동하던지 같은 속도로 가게끔 만들어 준다. (단위벡터화)

        transform.position += moveVec * playerSpeed * Time.deltaTime; // deltaTime은 어느 환경에서나 같게 만들기 위함 (왜?)

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
            rigid.AddForce(Vector3.up * 300, ForceMode.Impulse);
            anim.SetTrigger("DoJump");
            isJump = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "base")
        {
            isJump = false;
        }
    }

}
