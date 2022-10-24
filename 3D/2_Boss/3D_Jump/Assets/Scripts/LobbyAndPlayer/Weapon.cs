using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    PlayerCode playerCode;

    // 공통
    public int itemCode; // 아이템 종류 코드
    public enum AtkType { Melee, Range }; // Melee - 근접 공격, Range - 원거리 공격
    public AtkType type; // 생성을 해 주어야 한다.
    public int Damage; // 무기 공격력
    public float AtkDelay;
    public float criticalPercent; // 크리티컬 확률
    public int maxEnchant; // 풀강화 수치

    public int AtkDmg; // 데미지 계산 후 들어가는 데미지 (무기 스왑때마다 갱신 [스탯 계산 -> 무기 데미지 갱신!])

    //근접 전용
    public BoxCollider meleeArea; // 근접 공격 범위
    public TrailRenderer trailEffect; // 효과?
    public bool isAttacked = false; // 한 번에 한 번만 공격을 했는가?

    // 원거리 전용
    public Transform bulletPos; // 총알 생성 위치
    public GameObject bullet; // 생성 될 총알
    public Transform bulletCasePos; // 탄피가 생성될 위치 설정
    public GameObject bulletCase; // 탄피
    public int maxCount; // 최대 총알 개수
    public int cntCount; // 현재 총알 개수

    // 강화 정보 적용 관련
    Bullet bulletInfo;

    private void Awake()
    {
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
    }

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
            StopCoroutine("Shot"); // 동작하고 있는 중에도 다 정지시킬 수 있음
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.3f);

        meleeArea.enabled = true; // box Collider 활성화
        trailEffect.enabled = true; // effect 활성화

        if (isAttacked)
        {
            meleeArea.enabled = false; // box Collider 비 활성화
        }

        yield return new WaitForSeconds(0.2f);

        meleeArea.enabled = false; // box Collider 비 활성화
        isAttacked = false;

        yield return new WaitForSeconds(0.3f);

        trailEffect.enabled = false; // effect 비 활성화


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


    /*
    
    IEnumerator Swing()
    {
        // 1
        yield return null; // yield가 필수? -> 결과를 내고 1프레임 대기

        // 2
        yield return new WaitForSeconds(0.1f); // 꼭 1프레임이 아니라 시간을 정해서 대기시킬 수도 있다!

        //3
        yield return null; // 즉, 여러 개의 결과를 낼 수 있음


        yield break; // 이건 그만하는 것! -> 이것보다 아래 있으면 아래는 실행 안함

    }
    
    */

    // Use함수 : 메인 루틴 -> Swing() 서브 루틴이 실행 -> 다시 메인루틴으로 돌아와서 Use() 시행
    // 코루틴 사용 시 : 메인 + 서브 루틴이 동시 실행 (co-op 협동!)


}
