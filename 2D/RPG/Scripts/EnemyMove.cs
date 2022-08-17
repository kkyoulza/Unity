using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove; // �ൿ��ǥ�� ������ ���� ����
    Animator anim; // �ִϸ��̼� ����
    SpriteRenderer spriteRenderer; // ��,�� ������ �ϱ� ����! (�������� ���� �����Ѵ�.)
    public float nextThinkTime;

    // Start is called before the first frame update
    void Awake()
    {
        nextThinkTime = Random.Range(2f, 5f);

        rigid = GetComponent<Rigidbody2D>();

        Invoke("Think",nextThinkTime); // �ش��ϴ� �Լ� �̸��� 5���� ������ �ڿ� ȣ���ϰ� �Ǵ� ���̴�.

        anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate() // ���� ����̱⿡ FixedUpdate��!!
    {
        // �⺻ ������
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y); // y���� �ӵ��� ������ �ִ� �״�� ���� ��, 0�� ������ �̻��� ���� �� �ִ�.

        // ���� üũ
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        //���ʹ� ������ �ֳ� ���ĸ� ���� ���⿡�� �� ĭ �ռ��� ���� �ϱ⿡ x�࿡ �Ʊ� nextMove�� �����ش�.
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        
        
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); // ������ġ, ����, �Ÿ�

        if (rayHit.collider == null)
        {
            Turn();
        }


    }

    void Think()
    {
        // Set Next Active
        nextMove = Random.Range(-1, 2); // �������� "�̻�" ������ �ִ밪�� "�̸�"�̱� ������ �ִ� ������ ������ �� ���� 1 ū ���� �־�� �Ѵ�.

        // Think(); // ����Լ� - �ڱ� �ڽ��� �ڽ��� ȣ���ϰԲ�! (ù ������ Awake���� ����, �� ������ ��� ȣ��ǰԲ� �Ѵ�.)

        // �����̸� ���� �ʰ� ��� �ݺ��ϴ� ���� �������� ������ �ִ�.

        // �����ϴ� �ð��� �������� �ο��� �� �ִ�.

        
        // �ִϸ��̼� ����
        anim.SetInteger("WalkSpeed", nextMove); // �� �̷��� 0 �Ǵ� 0�� �ƴҶ��� �߱���.. �����̰ų� �ƴϳ� �̴ϱ�!

        
        if(nextMove != 0)
            spriteRenderer.flipX = nextMove == 1; // flipX�� true�� �� �������� ���� ���Ƿ� nextMove�� �������ΰ� 1�̴� 1�� ���� �� true�� ������ �ȴ�.
                                                  // �׷��� �̻��·� ���θ� 0�϶� ������ ������ ���� �ȴ�. ���� If���� ����Ͽ� 0�� �ƴҶ��� �ߵ��ǰ� �Ͽ���.
        

        // ���
        nextThinkTime = Random.Range(2f, 5f); // float�� �ڽ��� ������ �ϱ� ������ �ڿ� f�� ���̸� �ȴ�.

        Invoke("Think", nextThinkTime); // ���� Invoke�� �Ἥ �ش��ϴ� �Լ� �̸��� 5���� ������ �ڿ� ȣ���ϰ� �Ǵ� ���̴�.

        // ��ʹ� ��������� �� �Ʒ��� �ִ´�.

    }

    void Turn()
    {
        // Debug.Log("���! ���� ����������!");
        nextMove *= -1; // ���� ���������� ������ �ݴ�� �ٲ㼭 ���Բ� �Ѵ�!!
                        // �׷��� ������ �ٲ�µ� Invoke�� �ٷ� �ߵ��Ǹ� ��¯ ���繬�̴�.. �׷��� ��� �ϳ�?
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke(); // ���� �������� Invoke�� �����! (5�ʸ� ���� ���� ���� �����!)

        Invoke("Think", 5); // �ٽ� Invoke�� ��������ش�!! (���Ӱ� 5�ʸ� ����!)
    }

}
