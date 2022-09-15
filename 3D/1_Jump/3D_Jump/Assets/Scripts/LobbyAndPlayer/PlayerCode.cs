using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCode : MonoBehaviour
{
    float horAxis; // ���� ����Ű �Է� �� ���� ���� ����
    float verAxis; // ���� ����

    bool runDown; // �뽬 ��ư�� ���ȴ°�?
    bool jumpDown; // ���� Ű�� ���ȴ°�?
    bool isJump; // ������ �ϰ� �ִ°�?

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
        moveVec = new Vector3(horAxis, 0, verAxis).normalized; // ��� �������� �̵��ϴ��� ���� �ӵ��� ���Բ� ����� �ش�. (��������ȭ)

        transform.position += moveVec * playerSpeed * Time.deltaTime; // deltaTime�� ��� ȯ�濡���� ���� ����� ���� (��?)

        anim.SetBool("IsWalk", moveVec != Vector3.zero); // �Է��� ���� �� �������� �ϴ� ���ǹ��� �־���.
        anim.SetBool("IsRun", runDown);
    }

    void TrunChar()
    {
        transform.LookAt(transform.position + moveVec); // ���ư��� �������� �ڵ����� ȸ���ǰ� �Ѵ�.
    }

    void Jump()
    {
        if (jumpDown && !isJump) // ����Ű�� ������ ���� ���°� �ƴ� ��!
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
