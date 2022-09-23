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


<<<움짤 자리>>>


<hr>

#### 원거리 공격(총)

골드메탈님의 에셋에는 총이 두 종류가 있지만 원리는 똑같기에 공격 딜레이만 다르게 설정하였다.





