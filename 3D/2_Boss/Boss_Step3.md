# Step3. 캐릭터 공격(CoRoutine)

앞서 캐릭터가 무기를 먹는 것을 구현하였다.

이제는 캐릭터가 얻은 무기를 가지고 공격하는 것을 구현 해 보도록 하겠다.

우선 공격은 근접 공격과 원거리 공격이 있다.

<hr>

#### 근접 공격

근접 공격은 둔기를 휘두르는 것으로 설정하였다.

몬스터와 가까이 붙어 공격해야 하기에 Collider의 범위를 실제 크기보다 크게 설정하였으며, 데미지를 높게 설정하였다.

![image](https://user-images.githubusercontent.com/66288087/191972405-d923aafe-4dfd-451d-ba8b-1259a70cf439.png)

그리고 무기에 넣어 주었던 Weapon.cs에서 이제 공격을 실행할 수 있게 코드를 추가 해 보자.

앞서 Collider를 추가 해 주었었는데, 이 것은 공격 시에만 활성화 되고, 공격이 아닐 때는 활성화가 되면 안된다.

따라서 Collider를 활성화 한 다음, 딜레이 이후에 Invoke를 통하여 공격을 다시 할 수 있게 해 주는 함수를 출력하게 해 주면 된다.

그런데.. 이렇게 하는 것 보다 더 좋은 방법이 있다.

<hr>

### 코루틴(CoRoutine)을 이용한 공격 구현

코루틴은 작업을 분산시킬 수 있는 역할을 한다.

즉, A가 일어나고 0.1초 뒤에 B가 일어나고 0.1초 뒤에 C가 일어나는 것을 구현한다고 해 보자.

일반적인 Invoke로 하게 되면 먼저 Invoke("A",0.1f); 를 통하여 A를 0.1초 뒤에 실행시키고

Invoke("B",0.1f); 를 통하여 B를 0.1초 뒤에 실행,

Invoke("C",0.1f); 를 통하여 C를 0.1초 뒤에 실행시킨다.

그렇지만 코루틴을 이용하면 1개의 함수만을 이용하여 A,B,C의 기능들을 다 실행시킬 수 있다.

코루틴을 이용한 Weapon.cs코드는 아래와 같다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum AtkType { Melee, Range }; // Melee - 근접 공격, Range - 원거리 공격
    public AtkType type; // 생성을 해 주어야 한다.
    public int Damage;
    public float AtkDelay;
    public BoxCollider meleeArea; // 근접 공격 범위
    public TrailRenderer trailEffect; // 효과?

    public void Use()
    {
        if (type == AtkType.Melee)
        {
            StopCoroutine("Swing"); // 동작하고 있는 중에도 다 정지시킬 수 있음
            StartCoroutine("Swing");
        }
        
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);

        meleeArea.enabled = true; // box Collider 활성화
        trailEffect.enabled = true; // effect 활성화

        yield return new WaitForSeconds(0.3f);

        meleeArea.enabled = false; // box Collider 활성화

        yield return new WaitForSeconds(0.3f);

        trailEffect.enabled = false; // effect 활성화


    }

    // Use함수 : 메인 루틴 -> Swing() 서브 루틴이 실행 -> 다시 메인루틴으로 돌아와서 Use() 시행
    // 코루틴 사용 시 : 메인 + 서브 루틴이 동시 실행 (co-op 협동!)


}

</code>
</pre>

코루틴은 기본적으로 StartCoroutine을 이용하여 실행한다.
멈추는 것은 StopCoroutine을 이용한다.

코루틴 함수는 IEnumerator로 시작하게 되고, return만 쓰는 것이 아닌 yield return이 필수이다.

기본적으로 yield return null;은 1프레임을 쉬고 다음 작업으로 가는 것이며

yield break;는 남은 작업에 관계 없이 끝내라는 의미이다. 아래에 남은 작업들은 당연히 실행되지 않을 것이다.

그리고 위에서는 yield return new WaitForSeconds(time); 을 사용하였는데 이것은 time만큼의 딜레이를 준 다음 다음 작업으로 넘어 가라는 것이다.

이렇게 되면 위에서 제시한 예시를 어떻게 풀어가야 하는 지 감이 올 것이다.

아무튼 위 코드를 통하여 Use()가 실행되면 만약 무기 타입이 근접 무기라면 BoxCollider와 TrailEffect를 활성화 시키고, 0.2초 뒤에 다시 비활성화 시키는 작업을 할 수 있게 된다.

플레이어에서는 공격 키를 눌렀을 때, 위와 같은 작업(Use() -> Swing())으로 이어질 수 있게 연결 해 주어야 한다.

<pre>
<code>
void Attack()
{
    if (cntEquipWeapon == null)
        return;

    AtkDelay += Time.deltaTime;
    isAtkReady = cntEquipWeapon.AtkDelay < AtkDelay; // 공격 속도(무기 딜레이)보다 현재 딜레이 값이 클때만!

    if(AtkDown && isAtkReady && !isDodge && !isSwap)
    {
        cntEquipWeapon.Use(); // 공격할 준비가 되었으니 전달하고 나머지는 위임!
        anim.SetTrigger("DoSwing"); // 3항 연산자를 이용하여 한 줄로 두 가지 종류의 애니메이션을 실행한다.
        // 3항 연산자의 사용은 유연하게 가능하다는 것을 명심!
        AtkDelay = 0f;
    }

}
</code>
</pre>

input은 AtkDown으로 설정 해 둔다.

그리고 Time.deltaTime으로 시간을 누적 해 주고, 미리 설정 해 둔 무기 딜레이를 넘어서게 되면 공격 준비가 완료되게끔 설정 해 두었다.(AtkReady)

공격 준비가 된 상태에서, 공격 키를 누르게 되면 공격을 시전하고(Use() 함수 실행과 더불어 애니메이션 실행) 시간 딜레이 값을 0으로 초기화 해 준다.

![image](https://user-images.githubusercontent.com/66288087/192076777-718375a2-29c1-40fe-9717-4c33208fb97f.png)

플레이어 애니메이터에 Swing을 추가 한 모습이다.

Trigger로 DoSwing을 하나 만들어 주어 AnyState -> Swing -> Exit으로 이어질 수 있게 만들었다.

이렇게 해 주면

![boss_3_1](https://user-images.githubusercontent.com/66288087/192076721-1fa25a6d-38b7-48b6-b590-becd20b7803e.gif)

위 움짤처럼 공격을 하게 됨을 볼 수 있다.

<hr>

#### 원거리 공격(총)

근거리 공격은 공격 범위가 존재하였다.

원거리 공격은 총을 쏘면서 원거리에서 하는 공격이기에 스킬이 아닌 이상 공격 범위가 무기에 붙어있지는 않다.

대신 총알에 Collider를 넣어 주어 총알에 닿으면 공격 판정이 나게끔 할 것이다.(총알에는 대상이 밀리면 안되기에 isTrigger를 체크 해 준다.)

총알을 제작 해 보자

<hr>

**총알 제작**

![image](https://user-images.githubusercontent.com/66288087/192077149-aa35ef99-1bbf-46da-afad-ff4d56045a10.png)

빈 오브젝트를 만들고 RigidBody와 Sphere Collider를 넣어 주어 중력의 영향을 받으면서 공격 판정이 나게끔 설정 하였다.

그리고 골드메탈님은 여기에 Trail Effect를 추가 해 주었다.

![image](https://user-images.githubusercontent.com/66288087/192077358-ef0199d3-5404-4397-8cc4-3f590bc30d59.png)

![image](https://user-images.githubusercontent.com/66288087/192077488-c4d4bcd5-6859-4cd7-9af6-031d3e41fa3c.png)

총 색과 관련있게 색을 설정 해 주었으며, 크기도 적절하게 세팅 해 주면 된다.

그리고 아까 총 Weapon.cs에서 총에는 데미지를 0으로 설정 해 놨을 것이다.

총 자체에서 데미지 계산이 일어나는 것이 아니라 총알에서 일어나는 것이기에 Bullet.cs 코드를 하나 만들어 여기서 데미지를 설정 해 보도록 하자.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "base")
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
        if (other.gameObject.tag == "base" || other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }

    }

}
</code>
</pre>

데미지를 설정하고, 벽에 닿거나 땅에 닿게 되면 사라지게끔 해 주었다.

(Collision은 탄피에 적용시키려고 만들어 놓은 것이다.)

그리고 총알을 Prefab화 시켜 준다.

머신건 총알도 크기만 작게 해서 비슷하게 설정 해 준다.

![image](https://user-images.githubusercontent.com/66288087/192077545-dea06f02-c7b6-4e06-8927-c2ac236d1184.png)

이것 역시 Prafab화 시켜준다.

탄피도 만들어서 RigidBody, Collider를 적용시켜 주고, Bullet.cs를 넣은 다음 Prefab화 시켜 준다.

<hr>

**총알 발사**

그리고 Weapon.cs에서 원거리 공격 전용 요소들을 만들어 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum AtkType { Melee, Range }; // Melee - 근접 공격, Range - 원거리 공격
    public AtkType type; // 생성을 해 주어야 한다.
    public int Damage;
    public float AtkDelay;
    public BoxCollider meleeArea; // 근접 공격 범위
    public TrailRenderer trailEffect; // 효과?

    // 원거리 전용
    public Transform bulletPos; // 총알 생성 위치
    public GameObject bullet; // 생성 될 총알
    public Transform bulletCasePos; // 탄피가 생성될 위치 설정
    public GameObject bulletCase; // 탄피
    public int maxCount; // 최대 총알 개수
    public int cntCount; // 현재 총알 개수


    public void Use()
    {
        if (type == AtkType.Melee)
        {
            StopCoroutine("Swing"); // 동작하고 있는 중에도 다 정지시킬 수 있음
            StartCoroutine("Swing");
        }
        else if(type == AtkType.Range && cntCount > 0)
        {
            cntCount--;
            StopCoroutine("Shot"); // 동작하고 있는 중에도 다 정지시킬 수 있음
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);

        meleeArea.enabled = true; // box Collider 활성화
        trailEffect.enabled = true; // effect 활성화

        yield return new WaitForSeconds(0.3f);

        meleeArea.enabled = false; // box Collider 활성화

        yield return new WaitForSeconds(0.3f);

        trailEffect.enabled = false; // effect 활성화


    }

    IEnumerator Shot()
    {

        // 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();

        bulletRigid.velocity = bulletPos.forward * 50; // 총알의 속도를 설정

        yield return null; // 1프레임 쉬고

        // 탄피 배출

        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();

        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -1) + Vector3.up * Random.Range(2, 3); // 랜덤한 힘으로 배출!

        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
    }

    // Use함수 : 메인 루틴 -> Swing() 서브 루틴이 실행 -> 다시 메인루틴으로 돌아와서 Use() 시행
    // 코루틴 사용 시 : 메인 + 서브 루틴이 동시 실행 (co-op 협동!)

}
</code>
</pre>

우선 원거리 공격은 총알이 나갈 위치와 탄피가 배출 될 위치, 총알 prefab, 탄피 prefab, 현재 총알 개수, 최대 총알 개수가 들어 갈 변수를 설정 해 준다.

![image](https://user-images.githubusercontent.com/66288087/192077901-44271682-43c0-4422-891a-8af49d1a3bef.png)

총알이 나갈 위치는 Empty GameObject를 만들어 위 사진과 같이 위치시킨다.

![image](https://user-images.githubusercontent.com/66288087/192077935-48d9e00d-8fa3-49d3-837e-2f3781957650.png)

그리고 탄피 배출위치는 무기 자식 오브젝트에 추가 해 주어 위 사진과 같이 위치시켜 준다.

![image](https://user-images.githubusercontent.com/66288087/192078178-e66c6c37-4594-4da2-abd5-c4788f4e719d.png)

그리고 위와 같이 다 넣어 준다.

<pre>
<code>
// 총알 발사
GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();

bulletRigid.velocity = bulletPos.forward * 50; // 총알의 속도를 설정

yield return null; // 1프레임 쉬고
</code>
</pre>

그 다음에 코루틴을 이용한 Shot()에서 Instantiate()를 통하여 prefab을 생성 해 준다.

(아까 설정 했던 위치를 사용한다. bulletPos)

그런데 생성만 해 줘서는 안된다. 총알에 힘을 주어야 한다.

따라서 Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>(); 을 통하여 힘을 줄 수 있게 해 준다.

AddForce를 해 주어도 되지만 총알에는 속도를 설정 해 준다.

속도는 bulletPos의 forward 벡터를 기반으로 설정하였다.

그 다음에는 코루틴의 특징을 따라서 yield return null; 을 통해 1프레임 쉬고 탄피 배출 로직으로 이동한다.

<pre>
<code>
GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();

Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -1) + Vector3.up * Random.Range(2, 3); // 랜덤한 힘으로 배출!

caseRigid.AddForce(caseVec, ForceMode.Impulse);
caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
</code>
</pre>

탄피는 비슷하지만 AddForce를 해 준다. 그리고 방향 벡터를 랜덤으로 설정 해 두어 탄피가 튀는 맛이 있게 하였다.

<pre>
<code>
public void Use()
{
    if (type == AtkType.Melee)
    {
        StopCoroutine("Swing"); // 동작하고 있는 중에도 다 정지시킬 수 있음
        StartCoroutine("Swing");
    }
    else if(type == AtkType.Range && cntCount > 0)
    {
        cntCount--;
        StopCoroutine("Shot"); // 동작하고 있는 중에도 다 정지시킬 수 있음
        StartCoroutine("Shot");
    }
}
</code>
</pre>

그리고 위 코드에서 보면 현재 총알 개수가 1개 이상이어야 Shot()이 발동되게 하였다.

![image](https://user-images.githubusercontent.com/66288087/192079114-4145e560-faf4-4e29-95ad-081914dbfc06.png)

그리고 애니메이션도 설정 해 두고, DoShot 트리거를 통하여 발동되게 하였다.

그런데 PlayerCode.cs에서 Attack() 함수는 애니메이션 트리거가 DoSwing만 있었다. 여기서 착용한 무기에 따라서 애니메이션이 바뀌어야 하는데 if문 대신 삼항 연산자를 활용할 수있다.

<pre>
<code>
void Attack()
{
    if (cntEquipWeapon == null)
        return;

    AtkDelay += Time.deltaTime;
    isAtkReady = cntEquipWeapon.AtkDelay < AtkDelay; // 공격 속도(무기 딜레이)보다 현재 딜레이 값이 클때만!

    if(AtkDown && isAtkReady && !isDodge && !isSwap)
    {
        cntEquipWeapon.Use(); // 공격할 준비가 되었으니 전달하고 나머지는 위임!
        anim.SetTrigger(cntEquipWeapon.type == Weapon.AtkType.Melee ? "DoSwing" : "DoShot"); // 3항 연산자를 이용하여 한 줄로 두 가지 종류의 애니메이션을 실행한다.
        // 3항 연산자의 사용은 유연하게 가능하다는 것을 명심!
        AtkDelay = 0f;
    }

}
</code>
</pre>

이렇듯 삼항 연산자는 유연하게 활용할 수 있는 여지가 많으니 활용 해 주어야 한다.

![boss_3_2](https://user-images.githubusercontent.com/66288087/192079221-a3f38025-12e6-48a5-b38f-12c37c82df2b.gif)

이제 총을 먹고 공격을 해 보면 위와 같이 총알을 쏘게 됨을 볼 수 있다.

<hr>

**총알 재장전**

총에는 최대 총알 개수가 있고, 이것을 재 장전을 통해서 채워 주어야 한다.

재장전 기능을 만들어 보았다.

<pre>
<code>
void ReLoad()
{
    if (cntEquipWeapon == null)
        return;

    if (cntEquipWeapon.type == Weapon.AtkType.Melee)
        return;

    if (bullet == 0) // 남은 총알이 0개이면 안된다.
        return;

    if(rDown && !isJump && !isDodge && !isSwap && isAtkReady && !isReloading)
    {
        // 재장전 키가 눌리고, 점프중,회피중,무기교체중이 아닐때이면서 공격 준비가 되었을 때 실행되게 한다.
        isReloading = true;
        anim.SetTrigger("DoReload");

        Invoke("ReLoadOut", 0.7f);

    }

}

 void ReLoadOut()
{
    int reCount = bullet < cntEquipWeapon.maxCount ? bullet : cntEquipWeapon.maxCount - cntEquipWeapon.cntCount;
    cntEquipWeapon.cntCount += reCount;

    if (cntEquipWeapon.cntCount > cntEquipWeapon.maxCount)
    {
        reCount = bullet - (cntEquipWeapon.cntCount - cntEquipWeapon.maxCount); // 남은 총알 개수에서 넘치는 부분을 뺀 만큼을 충전해야 한다.
        cntEquipWeapon.cntCount = cntEquipWeapon.maxCount;
    }

    bullet -= reCount; // 플레이어의 총알 개수 갱신

    isReloading = false;
}

</code>
</pre>

r 버튼을 누르면 재장전 함수가 실행되게 하였으며, 애니메이션이 발동 된 다음(0.7초 딜레이 이후) ReLoadOut() 함수에서 실제로 총알이 리필되게 하였다.

여기서 플레이어가 가진 총알의 개수와, 몇 개의 총알을 더 채워야 하는지 등을 고려하여 총알을 채워 주어야 한다.

우선 reCount는 채울 총알의 개수이다. 

여기서 삼항 연산자를 이용하여 플레이어의 남은 총알 개수가 탄창 용량보다 작으면(탄창에 다 못들어갈 때) 남은 총알을 다 때려 넣고, 탄창 용량보다 많으면 최대치와 현재 가진 총알 개수의 차이만큼 넣어주는 것으로 설정하였다.

그런데, 애매한 순간이 있다.

예를 들어 탄창 용량이 10개이고 남은 총알이 6개인데, 탄창에 8발을 남긴 상태로 재장전을 하면, 분명히 남은 총알 개수가 탄창 용량보다는 작기 때문에 6발을 다 때려 넣게 되면 14발이 되어 탄창 용량을 초과하게 된다.

따라서 초과를 하게 되면, 남은 총알에서 초과 한 부분을 빼 준 값을 reCount에 다시 넣어주게 된다.

ex - 6(남은 총알 개수) - (14(남은것을 다 넣었을 때 총알 개수) - 10(탄창 용량))(초과분) = 2 (남은 총알에서 채울 수 있는 개수)

그리고 현재 총알 개수를 탄창 용량으로 맞추고 reCount에 다시 넣어 준 채울 수 있는 개수를 남은 총알 개수에서 빼 주면 된다.

이렇게 해 주면 아래 영상과 같이 장전이 되게 된다.

https://user-images.githubusercontent.com/66288087/192079751-d89d0494-88a6-4cac-9684-f88053adda38.mp4

이제 다음으로는 강화 시스템을 만들어 보도록 하겠다.


