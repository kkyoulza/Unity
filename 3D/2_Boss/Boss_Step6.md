# Step 6. 보스


### 에셋 및 Collider 세팅

앞서 만들었던 몬스터를 확장하여 보스를 제작 해 보자

우선 받은 에셋에서 보스 Prefab을 끌어다 Scene에 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/194699709-365522f7-beb9-43bf-bdc2-b1d339d91c0f.png)

그런데 받은 에셋에서 Boss의 Scale을 변경하게 되면 애니메이션 크기와 Scene에 있는 보스의 크기가 맞지 않는 현상이 있다.

![image](https://user-images.githubusercontent.com/66288087/194700603-5f60c8a0-e7d9-4143-8732-c880ef9ae242.png)
![image](https://user-images.githubusercontent.com/66288087/194700567-b9f3719e-213c-4754-8b00-85e390d7ea99.png)

이 때는 위 사진처럼 Boss하위 오브젝트인 Mesh Object의 Scale을 1,1,1로 맞추어 준 다음 겉에 있는 Boss의 Scale을 바꾸어 주면 애니메이션 크기와 차이 없이 크기를 바꿀 수 있게 된다.

이제 보스 패턴을 위한 준비를 해 보도록 하자.

골드메탈님이 기획한 패턴은 3가지가 있다.

**미사일 발사, 돌 굴리기, 순간이동 & 찍기

미사일 발사를 위해서는 미사일을 Prefab화 하여 생성하면 되고, 돌 역시 Prefab화 하여 생성하면 된다.

![image](https://user-images.githubusercontent.com/66288087/194701372-c031d8a9-6a23-4643-be60-687090bb1d3c.png)

우선 보스 자체에 BoxCollider를 추가 해 준다.

그 다음, 순간이동과 찍기 판정을 하기 위해 Boss 자식 오브젝트에 빈 오브젝트를 만들고 BoxCollider를 Trigger로 추가하여 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/194702173-c95e870b-2916-46bb-898f-e31c3f6b3bfe.png)

그리고 미사일 발사를 위하여 미사일이 나올 위치를 빈 오브젝트로 위치시켜 준다.

#### 미사일 설정

미사일은 추적 미사일이기에 nav mesh 를 사용하게 된다. 즉, 물체가 바닥에 닿고 있어야 작동하게 되는 것이다.

따라서 미사일은 몬스터의 미사일을 만들듯이 자식 오브젝트만을 위로 띄워주어, 미사일 오브젝트는 바닥에 닿고 있지만 실제 미사일은 떠 있게끔 설정 해 주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/194702272-5781173e-409e-4294-9a9d-afb8c4fecdf0.png)

즉, 위 사진과 같이 미사일의 위치는 바닥에, 미사일 실제 객체는 떠 있게끔 해 주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/194703341-6a7e3878-bc99-440c-810b-34799a8605ea.png)

그리고 미사일이 날아가는 효과를 주기 위하여 particle 효과를 추가 해 준다.

몬스터 미사일처럼 뱅글뱅글 도는 효과를 추가 해 주기 위해 만들어 두었던 Missile.cs 스크립트를 자식 Mesh Object에 넣어 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right * 3);
    }
}
</code>
</pre>

그리고 미사일 본체에는 Bullet을 넣어 주어 데미지를 설정 해 주면 되는데, 몬스터 미사일과 다르게 추적 효과를 추가 해 주어야 한다.

따라서 Bullet의 효과를 가지면서 Boss Missile만의 효과인 추적 기능을 넣어 줄 수 있게 Bullet을 상속한 코드인 BossMissile.cs를 만들어 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet // 상속!!
{
    public Transform target;
    NavMeshAgent navi;

    void Awake()
    {
        navi = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        navi.SetDestination(target.position);
    }
}

</code>
</pre>

![image](https://user-images.githubusercontent.com/66288087/194703666-e8b71514-5837-4836-b595-33cde897f021.png)

Inspector 창을 보게 되면 위 사진과 같이 Bullet의 속성들을 사용할 수 있음을 볼 수 있다.

#### 돌 설정

![image](https://user-images.githubusercontent.com/66288087/194703864-eb177f7f-92ab-41e4-ac28-721af656e87d.png)

보스가 돌을 소환하게 되면 돌이 점점 커지고 나서 앞으로 굴러가게 되는 패턴이다.

돌에도 Bullet과 같은 속성을 가지면서 돌만의 자체적인 기능을 추가해 주어야 하기 때문에 Bullet을 상속 해 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2; // 회전 파워
    float scaleValue = 0.1f; // 크기
    bool isShooting; // 쏘고 있는가?

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        isShooting = false;
        StartCoroutine("GainPowerTimer");
        StartCoroutine("GainPower");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GainPowerTimer() // 기를 모으는 타이머
    {
        yield return new WaitForSeconds(2.5f);
        isShooting = true;
    }

    IEnumerator GainPower() // 기를 모은다.
    {
        while (!isShooting)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); // 지속적으로 속도를 올려야 하기 때문에 Acceleration을 넣는다.

            yield return null; // while문 속에 딜레이를 주지 않으면 게임이 정지하기 때문에 꼭 주어야 한다.
        }
    }

}
</code>
</pre>

보스가 기를 모으는 동안 돌이 점점 커지게 되고, 돌이 다 커지게 되면 앞으로 돌을 굴려주는 코드이다.

코루틴을 사용하여 2.5초 간의 대기시간을 주고, 대기시간 동안 크기를 증가시키고, 돌림힘 가속도를 올려 주어 구르게 해 준다.

돌은 플레이어에 맞거나, 벽에 닿았을 때 사라져야 한다. 따라서 Bullet.cs 원본을 조금 더 수정 해 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isRock;
    public bool isMelee; // 벽이랑 바닥에 총알이 닿았을 때, Destroy를 실행하는데, 만약 근접 Collider랑 벽이랑 닿아서 없어지면 안된다. -> 근접 Collider 여부를 판단하기 위함!
    public void SetDamage(int setDmg)
    {
        this.damage = setDmg;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isRock && collision.gameObject.tag == "base")
        {
            Destroy(gameObject, 3);
        }
        else if(collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMelee && !isRock && (other.gameObject.tag == "base" || other.gameObject.tag == "Wall"))
        {
            // 보스 돌이 아니고, 근접공격 Collider가 아니면서, 벽 or 바닥에 닿으면
            Destroy(gameObject); // 제거
        }

    }

}
</code>
</pre>

우선, 돌인지 아닌지 여부를 따지는 bool 변수를 설정하고, 땅에 닿을 때는 돌이 아닐 때에만 사라지게 만들어 준다. (총알, 총알 케이스)

#### 보스 패턴 세부 설정

보스의 컨셉은 가만히 있으면서 플레이어를 향해 고개가 바뀌면서 패턴을 진행하게 된다.

보스가 플레이어를 바라보는 것은 Update에서 isLook이 true일 때만 업데이트 해 주고,

Think() 코루틴을 통하여 보스가 앞서 설정했던 패턴들을 확률적으로 실행하는 것을 반복시켜 준다.

코루틴을 이용하여 딜레이를 주며, 패턴이 끝나게 되면 다시 패턴을 결정하는 함수를 실행시켜 주는 방식이다.

코드는 아래와 같다.

<pre>
<code>
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
        mat = GetComponentsInChildren<MeshRenderer>(); // material을 가져오는 방법!!
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
</code>
</pre>

Think() 를 통하여 패턴을 결정하고, 패턴 역시 코루틴으로 만들어 준다.

<pre>
<code>
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
</code>
</pre>

세 가지 패턴들에 대한 함수이다. 코루틴을 통하여 만들어 주었다.

각 패턴들에 따라서 애니메이션을 실행시킨 다음, 딜레이를 주면서 미사일, 돌, 내려찍기 등을 수행한다.

그 중에서 내려찍기(Taunt())는 추적(nav mesh)을 진행 해 주어야 한다.

추적 할 대상의 위치(target.position)에서 보스가 바라보는 만큼의 Vector를 더해 준 부분으로 내려찍기를 진행하게 될 것이다. (플레이어의 이동방향보다 살짝 앞)

각도를 대상으로 고정하고, nav mesh에서 멈추게 하는 상태변수를 false로 만들어 준다.(움직이게끔!)

그리고 내려찍기 애니메이션을 실행 한 뒤, 애니메이션에서 내려 찍는 행동을 할 때 즈음 내려찍기 범위로 설정했던 Collider를 활성화 해 준다.

내려찍기가 일어난 다음, 내려찍기 범위를 비활성화 해 주고, Boss 자체의 Collider를 활성화 해 준다.


<hr>

그런데 여기서, 보스 찍기 공격을 당하고 난 뒤, 플레이어가 보스의 위치랑 겹치게 되었을 때, Boss의 Collider가 활성화 된다면 낑기게 되어 플레이어가 튕겨 나가는 현상이 발생하게 된다.

따라서 넉백을 맞게 되면 뒤로 넉백이 되어 겹치지 않게 만들어 주어야 한다.

<pre>
<code>
private void OnTriggerEnter(Collider other)
{
    if(other.tag == "EnemyBullet")
    {
        if (other.GetComponent<Rigidbody>() != null) // 무적 시간 중에도 추가적으로 투사체를 맞게 되면 사라지게끔!
            Destroy(other.gameObject);

        bool isNoDmgJumpAtk = other.name == "JumpAtkArea"; 
        if(isNoDmgJumpAtk && isDamage) // 무적시간 중에 점프 공격에 맞았을 때!
            StartCoroutine(noDamageNuckBack());

        if (!isDamage)
        {
            Bullet enemyBullet = other.GetComponent<Bullet>();
            playerHealth -= enemyBullet.damage;

            bool isBossAttack = other.name == "JumpAtkArea"; // 점프 공격에 맞았을 때!

            StartCoroutine(OnDamage(isBossAttack));
        }
    }
}

IEnumerator OnDamage(bool isBossAttack)
{
    isDamage = true;
    foreach(MeshRenderer mesh in meshs)
    {
        mesh.material.color = Color.red;
    }

    if (isBossAttack) // 찍기 공격을 맞았을 때!
        rigid.AddForce(transform.forward * -40, ForceMode.Impulse); // 넉백!

    yield return new WaitForSeconds(1.0f); // 무적 딜레이 1초!

    isDamage = false;
    foreach (MeshRenderer mesh in meshs)
    {
        mesh.material.color = Color.white;
    }

    if (isBossAttack) // 넉백 후
        rigid.velocity = Vector3.zero; // 속도 원위치

}

IEnumerator noDamageNuckBack()
{
    rigid.AddForce(transform.forward * -40, ForceMode.Impulse); // 넉백!

    yield return new WaitForSeconds(0.5f); // 넉백 딜레이 0.5초

    rigid.velocity = Vector3.zero; // 속도 원위치

}
</code>
</pre>

onTriggerEnter에서 EnemyBullet 이라는 이름의 tag를 가지고 있는 Collider랑 닿았을 때 공격 판정이 나게 했다.

여기서 보스의 찍기 패턴일 때는(찍는 범위의 Object 이름으로 구분) 새롭게 OnDamage에 넉백 현상을 추가하였다.

그런데, 처음에 몬스터 공격에 맞을 때, 플레이어에게 1초 무적 시간을 주기로 하였는데 이 상태에서 넉백을 맞게 되면 의도와는 다르게 밀려나지 않게 된다.

따라서 무적시간에는 데미지는 받지 않고, 밀려나기만 하게 해 주었다. (noDamageNuckBack())

패턴들은 아래 움짤들처럼 수행되게 된다.

**돌 굴리기 패턴**

![boss_6_1](https://user-images.githubusercontent.com/66288087/194793895-9cad3322-af65-4f05-8d49-201c8f39e55b.gif)

**유도 미사일 패턴**

![boss_6_2](https://user-images.githubusercontent.com/66288087/194793908-b268cc2c-2167-4648-8ed1-9a85f00f7803.gif)

**내려찍기 패턴**

![boss_6_3](https://user-images.githubusercontent.com/66288087/194793940-bfdba06f-8ce8-44c2-8c69-c2229f7f3e2e.gif)


이제 다음에는 점프 게임 개인 순위표, 점프게임과 재화 연결, 강화에서 재화 및 기원 조각 등을 구현 해 보도록 하겠다.
