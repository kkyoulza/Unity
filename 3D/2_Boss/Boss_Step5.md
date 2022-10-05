# Step 5. 몬스터 및 보스 패턴 구현

### 몬스터 제작

이제 몬스터를 제작 해 보도록 해 보자

받은 에셋에 있는 몬스터 Prefab을 들고 와 준다.

![image](https://user-images.githubusercontent.com/66288087/193960498-bf8e66aa-4d74-4fb7-b1b3-ba54aa775a4a.png)

이제 몬스터 로직을 짜기 위해 Enemy.cs를 만들어 준다.

**완성된 Enemy.cs**
<pre>
<code>
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
</code>
</pre>

위 코드는 3종류의 일반 몬스터에 적용된 완성 된 코드이다.

enum을 통하여 몬스터의 타입을 구분 한 다음, 각 타입별로 몬스터의 패턴을 다르게 만들어 주었다.

우선, 몬스터에 물리를 적용하기 위하여 RigidBody와 BoxCollider를 추가시켜 준다.

그리고, 근접 공격을 활성화 하기 위하여 자식 오브젝트로 빈 오브젝트 추가 후, BoxCollider를 Trigger 모드로 추가 해 주며, Layer 와 Tag 이름을 EnemyBullet으로 설정 해 준다.

![image](https://user-images.githubusercontent.com/66288087/193964204-5d134876-c666-4f04-8f13-c60bc1f320d7.png)

그 다음, 아래와 같이 애니메이션을 에셋에서 가져 와 구성해 주며, 변환 조건에 트리거를 설정하여 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/193964548-394f61cd-4c61-473a-b430-a99eb90f96f1.png)

일반 몬스터기 때문에 간단하게 넣어 준다.

<hr>

### 받는 데미지 설정

완성된 코드 중에서 onTriggerEnter에는 몬스터가 공격을 받게 되었을 때, 데미지를 처리하는 부분을 넣고 있다.

<pre>
<code>
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
</code>
</pre>

데미지 부분은 조금 있다가 설명을 하고, 나머지를 보면 근접 공격과, 총알에 맞는 순간 weapon의 데미지수치만큼 체력에서 깎이게 만들었다.

체력이 깎이고 나서는 OnDamage에서 코루틴을 이용 하여 몬스터가 맞았을 때 반응을 설정하였다.

몬스터가 맞게 되면 전체가 붉은 색으로 변하게 된 다음, 0.1초의 딜레이 이후 남은 체력에 따라서 몬스터 소멸 또는 원상복구를 해 준다.

### 몬스터 추적

일반적으로 몬스터는 가만히 있지 않을 것이며, 플레이어를 대상으로 하여 추적할 것이다.

따라서 플레이어를 추적하는 것을 만들어 보자

유니티에서는 추적을 위한 컴포넌트를 제공해 준다.

![image](https://user-images.githubusercontent.com/66288087/193969958-21eea275-5104-4052-bbb7-ea9c4f5cdd0f.png)

새로운 컴포넌트로 nav mesh agent를 추가 해 준다.

여기서 몬스터가 플레이어를 추적하는 속도, 방향전환 속도, 가속도 등을 설정할 수 있다.

이렇게 하고, update에 아래 부분을 넣어 주게 되면

<pre>
<code>
navi.SetDestination(target.position);
</code>
</pre>

추적..을 하지 않는다. Nav는 Navigation을 통해 추적을 할 수 있는 땅을 설정 해 주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/193970731-fdd835fb-5962-41aa-aad1-33b333c04557.png)

지형은 static 체크를 해 주어야만 그 위에 생성이 되게 된다.

![image](https://user-images.githubusercontent.com/66288087/193970462-d04694be-1a94-4f01-9d79-a1bde3878fd5.png)

Window -> AI -> Navigation에 들어 간 다음

![image](https://user-images.githubusercontent.com/66288087/193970803-c232437f-c647-4c4b-bf6c-969b102dd4d2.png)

Bake를 눌러 구워주게 되면

![image](https://user-images.githubusercontent.com/66288087/193970837-4ee69bd4-0d27-4e28-9358-4e6429a39199.png)

위 사진과 같이 범위가 생기게 된다.

Agent Radius를 통하여 네모 칸의 크기를 정할 수 있다.

이제, 실행을 해 보게 되면 플레이어를 추적하게 됨을 볼 수 있다.

### 몬스터 공격

이제 몬스터가 추적을 하는 것 까지 만들었으니 공격을 하는 것도 만들어 보자

공격은 아까 몬스터 전방에 만들어 두었던 Box Collider를 평소에는 비활성화 해 두고, 공격을 하는 타이밍에만 활성화를 하여 공격을 하는 방식으로 만들게 되었다.

<pre>
<code>
// Update is called once per frame
void Update()
{
    if (navi.enabled)
    {// navi가 활성화 되었을 때만 목표를 추적! (기존에는 목표만 잃어버리고 움직이기는 하기 때문에 정지까지 하는 것으로 해 준다!)
        navi.SetDestination(target.position);
        navi.isStopped = !isChase; // 추적을 하고 있지 않을때(false) 정지를 하고(!false = true), 추적을 할 때 멈추는 것을 멈추게(움직이게) 한다.
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
</code>
</pre>

코루틴을 통하여 Attack()을 만들었다. 공격 중에는 추적을 하지 않게끔 bool 변수 isChase를 만들어 추격 모드를 on/off 해 준다. (Update 부분)

처음으로 만드는 A 타입은 공격 애니메이션이 전 딜레이를 주고, 때리는 순간에 Box Collider를 활성화 시켜 준다.

![boss_5_1](https://user-images.githubusercontent.com/66288087/193977086-46323ad5-11b3-41d0-a618-2b6ae117d966.gif)

위 움짤에서는 몬스터가 앞으로 내지르는 순간에 Box Collider가 활성화 되는 것이다.

### 플레이어 피격 판정

몬스터가 공격을 하게 되면 플레이어가 맞았을 때 반응이 있어야 한다.

PlayerCode.cs 코드에 onTriggerEnter() 이벤트를 추가 해 준다.

<pre>
<code>
private void OnTriggerEnter(Collider other)
{
    if(other.tag == "EnemyBullet")
    {
        if (!isDamage)
        {
            Bullet enemyBullet = other.GetComponent<Bullet>();
            playerHealth -= enemyBullet.damage;

            if (other.GetComponent<Rigidbody>() != null) // 무적 시간 중에도 추가적으로 투사체를 맞게 되면 사라지게끔!
              Destroy(other.gameObject);

            StartCoroutine(OnDamage());
        }
    }
}
    
   
IEnumerator OnDamage()
{
    isDamage = true;
    foreach(MeshRenderer mesh in meshs)
    {
        mesh.material.color = Color.red;
    }

    yield return new WaitForSeconds(1.0f); // 무적 딜레이 1초!

    isDamage = false;
    foreach (MeshRenderer mesh in meshs)
    {
        mesh.material.color = Color.white;
    }

}
</code>
</pre>

플레이어가 EnemyBullet tag Collider에 맞게 되면 데미지 처리를 해 준 다음, OnDamage()를 불러와 무적 딜레이와 플레이어 피격 시각 효과를 준다.

![image](https://user-images.githubusercontent.com/66288087/193978274-9d1da72c-ad02-44ca-af66-e2b4779db086.png)

위 사진과 같이 플레이어를 공격하는 순간에 Box Collider가 활성화 되고, 플레이어는 피격 판정이 됨을 볼 수 있다.

<hr>

### 돌격형 타입 몬스터

이제 일반 몬스터를 만들었으니 돌격형 몬스터를 만들어 보겠다.

돌격형은 공격 패턴만 다르게 설정하였다.

<pre>
<code>
case Type.B:
      yield return new WaitForSeconds(0.1f); // 선 딜레이
      rigid.AddForce(transform.forward * 20, ForceMode.Impulse); // 즉각적인 힘으로 돌격!
      meleeArea.enabled = true; // 돌격하는 동안 박스를 활성화!

      yield return new WaitForSeconds(0.5f); // 공격 박스가 활성화 된 시간
      rigid.velocity = Vector3.zero; // 일정 시간 돌격 후 멈춤!
      meleeArea.enabled = false;

      yield return new WaitForSeconds(2.0f); // 후 딜레이
      break;
</code>
</pre>

우선 돌격 전 선 딜레이를 먼저 준 다음, AddForce를 통하여 뒤에서 앞으로 밀어주는 식으로 돌격을 하게 된다.

돌격 하는 동안에 공격 범위를 활성화 하고, 일정시간 돌격 후, 속도를 0으로 만들어 돌격을 멈추게 하였다.












