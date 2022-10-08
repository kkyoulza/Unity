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





