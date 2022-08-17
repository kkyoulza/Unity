using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove; // 행동지표를 결정할 변수 생성
    Animator anim; // 애니메이션 관리
    SpriteRenderer spriteRenderer; // 좌,우 반전을 하기 위함! (보여지는 것을 관리한다.)
    public float nextThinkTime;

    // Start is called before the first frame update
    void Awake()
    {
        nextThinkTime = Random.Range(2f, 5f);

        rigid = GetComponent<Rigidbody2D>();

        Invoke("Think",nextThinkTime); // 해당하는 함수 이름을 5초의 딜레이 뒤에 호출하게 되는 것이다.

        anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate() // 물리 기반이기에 FixedUpdate로!!
    {
        // 기본 움직임
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y); // y축의 속도는 가지고 있는 그대로 넣을 것, 0을 넣으면 이상이 생길 수 있다.

        // 지형 체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        //몬스터는 절벽이 있냐 없냐를 진행 방향에서 한 칸 앞서서 봐야 하기에 x축에 아까 nextMove를 더해준다.
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        
        
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); // 시작위치, 방향, 거리

        if (rayHit.collider == null)
        {
            Turn();
        }


    }

    void Think()
    {
        // Set Next Active
        nextMove = Random.Range(-1, 2); // 최저값은 "이상" 이지만 최대값은 "미만"이기 때문에 최대 값에는 생각한 것 보다 1 큰 수를 넣어야 한다.

        // Think(); // 재귀함수 - 자기 자신을 자신이 호출하게끔! (첫 시작은 Awake에서 끊고, 그 다음에 계속 호출되게끔 한다.)

        // 딜레이를 두지 않고 계속 반복하는 것은 과부하의 위험이 있다.

        // 생각하는 시간도 랜덤으로 부여할 수 있다.

        
        // 애니메이션 세팅
        anim.SetInteger("WalkSpeed", nextMove); // 아 이래서 0 또는 0이 아닐때로 했구나.. 움직이거나 아니나 이니까!

        
        if(nextMove != 0)
            spriteRenderer.flipX = nextMove == 1; // flipX가 true일 때 오른쪽을 향해 가므로 nextMove가 오른쪽인게 1이니 1과 같을 때 true가 나오게 된다.
                                                  // 그런데 이상태로 놔두면 0일때 무조건 왼쪽을 보게 된다. 따라서 If문을 사용하여 0이 아닐때만 발동되게 하였다.
        

        // 재귀
        nextThinkTime = Random.Range(2f, 5f); // float형 박스에 담으려 하기 때문에 뒤에 f를 붙이면 된다.

        Invoke("Think", nextThinkTime); // 따라서 Invoke를 써서 해당하는 함수 이름을 5초의 딜레이 뒤에 호출하게 되는 것이다.

        // 재귀는 통상적으로 맨 아래에 넣는다.

    }

    void Turn()
    {
        // Debug.Log("경고! 앞이 낭떠러지임!");
        nextMove *= -1; // 앞이 낭떠러지면 방향을 반대로 바꿔서 가게끔 한다!!
                        // 그런데 방향을 바꿨는데 Invoke가 바로 발동되면 말짱 도루묵이다.. 그러면 어떻게 하나?
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke(); // 현재 진행중인 Invoke를 멈춘다! (5초를 세던 기존 것을 멈춘다!)

        Invoke("Think", 5); // 다시 Invoke를 진행시켜준다!! (새롭게 5초를 세고!)
    }

}
