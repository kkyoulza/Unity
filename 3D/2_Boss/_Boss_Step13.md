# Step 13. 스킬 제작


알피지에서 빠질 수 없는 것이 스킬이다.

**<목표>**
무기별로 사용할 수 있는 스킬을 2개씩 만들고, 스킬 쿨타임을 볼 수 있는 기능까지 만들어 보고자 한다.

<hr>

### PlayerSkills.cs 제작

플레이어의 스킬을 관리하는 스크립트를 제작 해 보도록 하자.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public bool[] meleeSkills = new bool[2];
    // 근접공격 무기 착용시 발동 가능한 스킬(스킬이 발동중인지 여부를 판단)
    public bool[] rangeSkills = new bool[2];
    // 총 무기 착용시 발동 가능한 스킬(발동 여부 확인) - 스프레드 슈팅(0번)

    public float[] meleeCoolTime = new float[2];
    public float[] rangeCoolTime = new float[2];


    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator onMeleeSkill(int index)
    {
        if (!meleeSkills[index]) // 스킬 사용 중이 아니면
            meleeSkills[index] = true; // 스킬 사용중임을 알려 주고
        else // 스킬이 사용중이라면
            yield break; // 코루틴을 끝낸다.

        yield return new WaitForSeconds(meleeCoolTime[index]);

        meleeSkills[index] = false;

    }

    public IEnumerator onRangeSkill(int index)
    {
        if (!rangeSkills[index]) // 스킬 사용 중이 아니면
            rangeSkills[index] = true; // 스킬 사용중임을 알려 주고
        else // 스킬이 사용중이라면
            yield break; // 코루틴을 끝낸다.

        Debug.Log("스킬 사용!");

        yield return new WaitForSeconds(rangeCoolTime[index]);

        Debug.Log("쿨타임 on");
        rangeSkills[index] = false;

    }


}

</code>
</pre>

일단, 근거리 무기와 원거리 무기의 스킬 사용 여부를 나타내는 bool 배열 2개, 그리고 각 스킬의 쿨타임을 나타내는 int 배열 2개를 만들었다.

버프 형식은 가동 시간도 있어야 하니 일단 스킬 구현 후, bool 변수를 추가하여 조정 해 주도록 하자.

PlayerCode.cs에서는 새롭게 버튼 bool을 만들어 주어 키를 세팅 해 준다.

일단 임시로 h 키를 할당 해 주었으며, 해당 스킬이 발동 되었을 때, 코루틴을 발동 시키는 작업을 해 보자

<hr>

### 원거리 1번 스킬 

<pre>
<code>
void UseSkills()
{
    if (spreadShotKey)
    {
        StartCoroutine(playerSkills.onRangeSkill(0));
    }
}
</code>
</pre>

위 함수도 Update()에 넣어 준다.

해당 쿨타임이 지나게 되면 다시 스킬을 사용할 수 있게 되는 것이다.

PlayerCode.cs에서 공격 시에 발동하는 함수를 보게 되면 cntEquipWeapon에 있는 Weapon 컴포넌트의 Use() 함수를 통하여 공격이 발동됨을 볼 수 있다. (아래 코드)

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

따라서 Weapon.cs 코드에서 분기를 마련 해 주도록 하자

<pre>
<code>
public void Use()
{
    AtkDmg = Random.Range(playerCode.playerMinAtk, playerCode.playerMaxAtk);
    if (type == AtkType.Melee)
    {
        StopCoroutine("Swing"); // 동작하고 있는 중에도 다 정지시킬 수 있음
        StartCoroutine("Swing");
    }
    else if(type == AtkType.Range && cntCount > 0)
    {
        bullet.GetComponent<Bullet>().SetDamage(AtkDmg); // 데미지 설정
        cntCount--;
        if (playerSkills.rangeSkills[0])
        {
            StopCoroutine("SpreadShot"); // 동작하고 있는 중에도 다 정지시킬 수 있음
            StartCoroutine("SpreadShot");
        }
        else
        {
            StopCoroutine("Shot"); // 동작하고 있는 중에도 다 정지시킬 수 있음
            StartCoroutine("Shot");
        }
    }
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

IEnumerator SpreadShot()
{
    // 총알 발사
    GameObject instantBulletOne = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
    Rigidbody bulletRigidOne = instantBulletOne.GetComponent<Rigidbody>();
    GameObject instantBulletTwo = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
    Rigidbody bulletRigidTwo = instantBulletTwo.GetComponent<Rigidbody>();
    GameObject instantBulletThreee = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
    Rigidbody bulletRigidThree = instantBulletThreee.GetComponent<Rigidbody>();

    Vector3 additionalBulletRot2 = bulletPos.forward;
    Vector3 additionalBulletRot3 = bulletPos.forward;

    additionalBulletRot2.x += 0.1f;
    additionalBulletRot3.x -= 0.1f;

    bulletRigidOne.velocity = bulletPos.forward * 50; // 총알의 속도를 설정
    bulletRigidTwo.velocity = additionalBulletRot2 * 50; // 총알의 속도를 설정
    bulletRigidThree.velocity = additionalBulletRot3 * 50; // 총알의 속도를 설정

    yield return null; // 1프레임 쉬고

    // 탄피 배출

    GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
    Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
    GameObject instantCase2 = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
    Rigidbody caseRigid2 = instantCase.GetComponent<Rigidbody>();
    GameObject instantCase3 = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
    Rigidbody caseRigid3 = instantCase.GetComponent<Rigidbody>();

    Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -1) + Vector3.up * Random.Range(2, 3); // 랜덤한 힘으로 배출!

    caseRigid.AddForce(caseVec, ForceMode.Impulse);
    caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
    caseRigid2.AddForce(caseVec, ForceMode.Impulse);
    caseRigid2.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
    caseRigid3.AddForce(caseVec, ForceMode.Impulse);
    caseRigid3.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
}
</code>
</pre>

Weapon.cs 코드에서 Use() 함수와 Shot(), 그리고 새롭게 만든 SpreadShot() 함수를 가져왔다.

SpreadShot()은, 버프 지속시간 동안 한 번에 세 개의 총알이 나가게 하는 것이다. (소비량은 같게!) - 모 게임에 나오는 어느 스킬과 유사하다.

Use() 함수에서 스킬 사용 여부로 분기를 따진 다음에 어떤 코루틴 함수를 불러올 지 결정한다. (스킬 사용 시간 동안에는 스프레드가 발동되게 하였다.)

SpreadShot() 코루틴에서는 총알 3개를 동시에 소환하여 각도를 다르게 해 준뒤, 공격 방향을 새롭게 Vector3 변수로 설정하여 힘을 주었다.

발동 비교는 아래 사진에서 볼 수 있다.

![image](https://user-images.githubusercontent.com/66288087/209320297-ed725a21-08e1-4ec4-a78d-44374784fced.png)

일반 공격 시

![image](https://user-images.githubusercontent.com/66288087/209320390-c1253dd7-be4f-44cb-9ba0-86ae6d888c56.png)

스프레드 슈터 사용 시(버프 형태)

![image](https://user-images.githubusercontent.com/66288087/209320622-41069f72-6ea8-4623-a263-247f1156cf13.png)

쿨타임을 30초로 설정 해 두어, 30초 뒤에, 쿨타임이 끝나게 하였다.(사진에는 쿨타임 on...이라고 적었네 ;;)

<hr>

#### 쿨타임과 버프 지속시간 별개로 설정하기

쿨타임이 존재하는 버프는 가동률이 1이 되게 하면.. 쿨타임의 의미가 없다고 생각한다. (물론 가동률이 100퍼센트이지만 너무 남발시키지 않게 하기 위함도 있긴 할 것이다..)

따라서 지속 시간이 담긴 자료구조도 따로 만들어 주도록 하자.

PlayerSkills.cs에서

<pre>
<code>
public bool[] meleeSkills = new bool[2];
// 근접공격 무기 착용시 발동 가능한 스킬 - 스킬 쿨타임이 돌아왔는지 여부 판단
public bool[] rangeSkills = new bool[2];
// 총 무기 착용시 발동 가능한 스킬(발동 여부 확인) - 스프레드 슈팅(0번)

public bool[] isMeleeSkillOn = new bool[2];
// 근접스킬이 지속 중인지 확인!
public bool[] isRangeSkillOn = new bool[2];
// 원거리스킬이 지속 중인지 확인!

public float[] meleeCoolTime = new float[2]; // 근접스킬 쿨타임
public float[] rangeCoolTime = new float[2]; // 원거리스킬 쿨타임

public float[] meleeSkillDurationTime = new float[2]; // 근거리 스킬 지속 시간(액티브 스킬이면 0으로 설정)
public float[] rangeSkillDurationTime = new float[2]; // 원거리 스킬 지속 시간(액티브 스킬이면 0으로 설정)

public IEnumerator onMeleeSkillCoolTime(int index)
{
    if (!meleeSkills[index]) // 스킬 쿨타임이 아니라면
        meleeSkills[index] = true; // 스킬 쿨타임임을 알려주고
    else // 스킬이 쿨타임이라면
        yield break; // 코루틴을 끝낸다.

    yield return new WaitForSeconds(meleeCoolTime[index]); // 쿨타임 계산용

    meleeSkills[index] = false;

}

public IEnumerator onMeleeSkill(int index)
{
    if (!isMeleeSkillOn[index]) // 스킬 사용 중이 아니면
        isMeleeSkillOn[index] = true; // 스킬 사용중임을 알려 주고
    else // 스킬이 사용중이라면
        yield break; // 코루틴을 끝낸다.

    yield return new WaitForSeconds(meleeSkillDurationTime[index]); // 지속 시간 계산용

    isMeleeSkillOn[index] = false;

}

public IEnumerator onRangeSkillCoolTime(int index)
{
    if (!rangeSkills[index]) // 스킬 쿨타임이 아니라면
        rangeSkills[index] = true; // 스킬 쿨타임임을 알려주고
    else // 스킬이 쿨타임이라면
        yield break; // 코루틴을 끝낸다.

    Debug.Log("스킬 사용!");

    yield return new WaitForSeconds(rangeCoolTime[index]);

    Debug.Log("쿨타임 end");
    rangeSkills[index] = false;

}

public IEnumerator onRangeSkill(int index)
{
    if (!isRangeSkillOn[index]) // 스킬 사용 중이 아니면
        isRangeSkillOn[index] = true; // 스킬 사용중임을 알려 주고
    else // 스킬이 사용중이라면
        yield break; // 코루틴을 끝낸다.

    yield return new WaitForSeconds(rangeSkillDurationTime[index]); // 지속 시간 계산용

    Debug.Log("스킬 끝!");

    isRangeSkillOn[index] = false;

}
</code>
</pre>

이렇게 코루틴 함수들도 새롭게 만들어 주었다.

쿨타임 계산용과, 지속시간 계산용으로!

<pre>
<code>
if (playerSkills.isRangeSkillOn[0])
{
    StopCoroutine("SpreadShot"); // 동작하고 있는 중에도 다 정지시킬 수 있음
    StartCoroutine("SpreadShot");
}
else
{
    StopCoroutine("Shot"); // 동작하고 있는 중에도 다 정지시킬 수 있음
    StartCoroutine("Shot");
}
</code>
</pre>

Use() 함수에서 스킬 사용 여부도 새로 지속시간을 넣어 둔 isRangeSkillOn[0]으로 바꿔 주었다.

그리고 PlayerCode.cs에서, 스킬 버튼을 눌렀을 때, 쿨타임과, 지속 시간이 동시에 돌게 만들어 주었다.

<pre>
<code>
void UseSkills()
{
    if (spreadShotKey)
    {
        StartCoroutine(playerSkills.onRangeSkillCoolTime(0));
        StartCoroutine(playerSkills.onRangeSkill(0));
    }
}
</code>
</pre>

이제 실행 해 보면






