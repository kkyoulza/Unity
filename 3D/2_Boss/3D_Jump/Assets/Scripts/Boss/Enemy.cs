using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // 몬스터 타입 설정
    public enum Type { A,B,C,Boss }; // 변수의 종류를 만든다.
    public Type enemyType; // 적의 타입을 넣을 변수

    // 체력 정보
    public int maxHealth;
    public int cntHealth;

    // 데미지 관련
    public GameObject PosObj; // 데미지 생성 위치에 있는 빈 오브젝트
    GameObject Damage_Prefab; // 데미지 프리팹
    GameObject Damage; // 데미지

    Vector3 dmgPos; // 데미지 위치

    // 물리 관련
    protected Rigidbody rigid;
    public BoxCollider meleeArea; // 공격 범위를 담을 변수
    public BoxCollider boxCollider; // 겉면 collider?

    // 원거리 몬스터 전용
    public GameObject monsterMissile; // 몬스터 미사일 프리팹을 담을 변수

    // 상태 관련
    public bool isAttack; // 공격을 하고 있는가?
    public bool isDead; // 죽은 상태인가?

    // 겉보기
    protected Material mat;

    // 추적 관련
    public bool isChase; // 추적이 가능한 상황!
    public Transform target; // 추적 대상
    protected NavMeshAgent navi; // UnityEngine.AI를 필수로 쓸 것

    //애니메이션
    protected Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<MeshRenderer>().material; // material을 가져오는 방법!!
        navi = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        Invoke("ChaseStart", 2.0f);

    }
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (navi.enabled)
        {// navi가 활성화 되었을 때만 목표를 추적! (기존에는 목표만 잃어버리고 움직이기는 하기 때문에 정지까지 하는 것으로 해 준다!)
            navi.SetDestination(target.position);
            navi.isStopped = !isChase; // 추적을 하고 있지 않을때(false) 정지를 하고(!false = true), 추적을 할 때 멈추는 것을 멈추게(움직이게) 한다.
        }
            
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity(); // 회전 속도 0으로 설정!
    }

    void Targeting()
    {
        // 공격을 하기 위한 타겟 설정
        float targetRadius = 0f;
        float targetRange = 0f;

        if (!isDead && enemyType != Type.Boss) // 죽은 상태가 아니고, 보스가 아닐 때만 타겟팅을 실행
        {
            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3.0f;
                    break;
                case Type.B:
                    targetRadius = 1f; // 타겟을 찾을 두께 (티스토리 참고)
                    targetRange = 12.0f; // 플레이어 타겟팅 범위
                    break;
                case Type.C: // 원거리는 타겟팅이 넓고 정확해야 한다.
                    targetRadius = 0.5f;
                    targetRange = 25.0f; // 플레이어 타겟팅 범위
                    break;
            }
        }
        

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        // 자신의 위치, 구체 반지름, 나아가는 방향(어느 방향으로 쏠 것인가?), 거리, 대상 레이어
        
        if(rayHits.Length > 0 && !isAttack)
        {
            // 플레이어가 몬스터의 레이더 망에 감지됨과 동시에 공격 중이 아니라면!
            StartCoroutine("Attack"); // 공격!

        }
            

    }

    IEnumerator Attack()
    {
        // 일반적인 몬스터는 잠시 정지 후, 공격하고 다시 쫓아가는 패턴으로!

        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.5f); // 애니메이션 동작동안 딜레이!

                meleeArea.enabled = true; // 그 뒤에 박스 활성화를 하여 공격!

                yield return new WaitForSeconds(0.3f); // 공격 박스가 활성화 된 시간

                meleeArea.enabled = false;

                yield return new WaitForSeconds(0.8f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f); // 선 딜레이
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse); // 즉각적인 힘으로 돌격!
                meleeArea.enabled = true; // 돌격하는 동안 박스를 활성화!

                yield return new WaitForSeconds(0.5f); // 공격 박스가 활성화 된 시간
                rigid.velocity = Vector3.zero; // 일정 시간 돌격 후 멈춤!
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2.0f); // 후 딜레이
                break;
            case Type.C: // 미사일을 만들어야 한다.
                yield return new WaitForSeconds(0.4f); // 선 딜레이

                GameObject instantBullet = Instantiate(monsterMissile, transform.position,transform.rotation); // 몬스터와 같은 위치에 미사일 생성
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20; // 총알에 속도를 부여

                yield return new WaitForSeconds(2.0f); // 후 딜레이
                break;
        }


        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }


    void ChaseStart()
    {
        isChase = true; // 추적을 가능하게 하고
        anim.SetBool("isWalk", true); // 애니메이션 상태를 변경!
    }

    void FreezeVelocity() // 플레이어와 충돌 시 날라가서 추적을 하지 못하는 상황 방지
    {
        if (isChase) // 추적중일 때만 제약!
        {
            rigid.velocity = Vector3.zero; // 속도 0
            rigid.angularVelocity = Vector3.zero; // 회전 속도 0
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Damage_Prefab = Resources.Load("Prefabs/LobbyAndRPG/Damage") as GameObject; // 데미지 프리팹을 GameObject에 가져 온다.

            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            cntHealth -= weapon.Damage;

            Damage = MonoBehaviour.Instantiate(Damage_Prefab);
            Damage.GetComponent<Damage>().damage = weapon.Damage; // 오브젝트 속 데미지 컴포넌트에 있는 데미지 변수 세팅
            Damage.transform.position = PosObj.transform.position;

            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));

        }
        else if (other.tag == "Bullet")
        {
            Damage_Prefab = Resources.Load("Prefabs/LobbyAndRPG/Damage") as GameObject; // 데미지 프리팹을 GameObject에 가져 온다.

            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            cntHealth -= bullet.damage;

            Damage = MonoBehaviour.Instantiate(Damage_Prefab);
            Damage.GetComponent<Damage>().damage = bullet.damage; // 오브젝트 속 데미지 컴포넌트에 있는 데미지 변수 세팅
            Damage.transform.position = PosObj.transform.position;

            Vector3 reactVec = transform.position - other.transform.position;

            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }

    }
    

    IEnumerator OnDamage(Vector3 reactVec) // 피격시 반응 설정
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(cntHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {        
            mat.color = Color.gray;

            gameObject.layer = 7; // rayCast에서와 달리 숫자로 그냥 적는다.
            isDead = true;
            isChase = false;
            if(enemyType == Type.A || enemyType == Type.B)
                meleeArea.enabled = false;
            navi.enabled = false;
            anim.SetTrigger("DoDie");

            reactVec = reactVec.normalized; // 몬스터가 죽을 때 팔짝 뛴 다음에 죽는 모습을 연출하기 위함
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 10, ForceMode.Impulse);

            

            Destroy(gameObject,2); // 2초 뒤에 Destroy!
        }

    }


}
