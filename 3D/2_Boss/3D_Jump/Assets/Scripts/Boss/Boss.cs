using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy // Enemy 상속
{
    public GameObject missile; // 보스 미사일
    public Transform missilePortA; // 미사일 포트1
    public Transform missilePortB; // 미사일 포트2
    // 보스 돌은 Enemy에서 bullet이 이미 존재하기에 거기에 돌을 넣으면 된다.

    Vector3 lookVector; // 플레이어가 가는 방향을 예측하는 벡터
    Vector3 tauntVector; // 내려찍는 위치 벡터
    
    public bool isLook; // 보는 상태 유지!


    void Awake() // Awake는 상속 해 주는 코드에서는 실행되지 않는다.(자식한테서만 실행..) -> 즉 Enemy에 있는 Awake는 실행 X
    {
        // 해결법은 부모 코드에 있는 Awake를 Start로 바꾸어 주거나 부모 Awake에 있는 내용들을 자식에 복사하는 방법이 있다.
        // 부모를 함부로 바꾸면 부모를 사용하는 객체들도 영향을 받기에 자식으로 복사하였다.
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<MeshRenderer>().material; // material을 가져오는 방법!!
        navi = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();

        navi.isStopped = true;
        StartCoroutine("Think");

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines(); // 하고 있는 모든 코루틴들을 break 해 준다.
            return; // 아래로 진행하지 못하게!
        }

        if (isLook)
        {
            // 보스가 플레이어를 바라보고 있는 중이라면?

            // 플레이어의 입력에 따라 바라보는 방향이 바뀌어야 한다.
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            lookVector = new Vector3(h, 0, v) * 5f;

            transform.LookAt(target.position + lookVector); // 대상을 향해 바라보게 바꾸어라!
            // target은 Enemy에서 상속된 변수이다.

        }
        else // 바라보는 것이 아닐 때
        {
            navi.SetDestination(tauntVector); // 찍기 벡터를 따라가게!
        }
    }

    IEnumerator Think() // 생각!
    {
        yield return new WaitForSeconds(0.1f); // 생각하는 시간(선 딜레이) -> 클 수록 난이도가 쉬워짐 (난이도 조절용)

        int ranAction = Random.Range(0, 5); // 0~4까지 랜덤으로 생성!

        switch (ranAction)
        {
            // break 없이 붙여서 놓으면 확률을 올릴 수 있다!
            case 0:
                StartCoroutine("MissileShot");
                break;
            case 1:
            case 2:
            case 3:
                StartCoroutine("RockShot");
                break;
            case 4:
                StartCoroutine("Taunt");
                break;
        }


    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("DoShot");

        yield return new WaitForSeconds(0.2f);

        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);

        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);

        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);

        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2.0f);
        StartCoroutine(Think()); // 애니메이션 발동이 끝난 뒤, 다시 생각을 해야 한다!
    }

    IEnumerator RockShot()
    {
        isLook = false; // 회전을 하지 않고 잠시 기를 모은다.
        anim.SetTrigger("DoBigShot"); // 애니메이션 재생
        Instantiate(monsterMissile, transform.position, transform.rotation);
        yield return new WaitForSeconds(3.0f);
        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVector = target.position + lookVector; // 내려찍을 위치 = 타겟 위치 + 바라보는 위치 더한 부분

        isLook = false; // 내려찍는 방향으로 각도 고정!
        navi.isStopped = false;
        boxCollider.enabled = false; // 점프 도중에 플레이어를 밀지 않게 하기 위해 boxCollider를 비활성화 한다.
        anim.SetTrigger("DoTaunt"); // 애니메이션 재생!

        yield return new WaitForSeconds(1.5f); 
        meleeArea.enabled = true; // 내려찍을 때!

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1.0f);
        isLook = true;
        navi.isStopped = true;
        boxCollider.enabled = true; // 다시 박스 활성화

        StartCoroutine(Think());
    }


}
