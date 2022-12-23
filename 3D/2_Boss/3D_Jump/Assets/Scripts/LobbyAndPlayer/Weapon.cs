using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    PlayerCode playerCode;
    PlayerSkills playerSkills;

    // ����
    public int itemCode; // ������ ���� �ڵ�
    public enum AtkType { Melee, Range }; // Melee - ���� ����, Range - ���Ÿ� ����
    public AtkType type; // ������ �� �־�� �Ѵ�.
    public int Damage; // ���� ���ݷ�
    public float AtkDelay;
    public float criticalPercent; // ũ��Ƽ�� Ȯ��
    public int maxEnchant; // Ǯ��ȭ ��ġ

    public int AtkDmg; // ������ ��� �� ���� ������ (���� ���Ҷ����� ���� [���� ��� -> ���� ������ ����!])

    //���� ����
    public BoxCollider meleeArea; // ���� ���� ����
    public TrailRenderer trailEffect; // ȿ��?
    public bool isAttacked = false; // �� ���� �� ���� ������ �ߴ°�?

    // ���Ÿ� ����
    public Transform bulletPos; // �Ѿ� ���� ��ġ
    public GameObject bullet; // ���� �� �Ѿ�
    public Transform bulletCasePos; // ź�ǰ� ������ ��ġ ����
    public GameObject bulletCase; // ź��
    public int maxCount; // �ִ� �Ѿ� ����
    public int cntCount; // ���� �Ѿ� ����

    // ��ȭ ���� ���� ����
    Bullet bulletInfo;

    private void Awake()
    {
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
        playerSkills = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkills>();
    }

    public void Use()
    {
        AtkDmg = Random.Range(playerCode.playerMinAtk, playerCode.playerMaxAtk);
        if (type == AtkType.Melee)
        {
            StopCoroutine("Swing"); // �����ϰ� �ִ� �߿��� �� ������ų �� ����
            StartCoroutine("Swing");
        }
        else if(type == AtkType.Range && cntCount > 0)
        {
            bullet.GetComponent<Bullet>().SetDamage(AtkDmg); // ������ ����
            cntCount--;
            if (playerSkills.isRangeSkillOn[0])
            {
                StopCoroutine("SpreadShot"); // �����ϰ� �ִ� �߿��� �� ������ų �� ����
                StartCoroutine("SpreadShot");
            }
            else
            {
                StopCoroutine("Shot"); // �����ϰ� �ִ� �߿��� �� ������ų �� ����
                StartCoroutine("Shot");
            }
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.3f);

        meleeArea.enabled = true; // box Collider Ȱ��ȭ
        trailEffect.enabled = true; // effect Ȱ��ȭ

        if (isAttacked)
        {
            meleeArea.enabled = false; // box Collider �� Ȱ��ȭ
        }

        yield return new WaitForSeconds(0.2f);

        meleeArea.enabled = false; // box Collider �� Ȱ��ȭ
        isAttacked = false;

        yield return new WaitForSeconds(0.3f);

        trailEffect.enabled = false; // effect �� Ȱ��ȭ


    }

    IEnumerator Shot()
    {

        // �Ѿ� �߻�
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();

        bulletRigid.velocity = bulletPos.forward * 50; // �Ѿ��� �ӵ��� ����

        yield return null; // 1������ ����

        // ź�� ����

        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();

        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -1) + Vector3.up * Random.Range(2, 3); // ������ ������ ����!

        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // ȸ���ϴ� ���� �ִ� ��!
    }

    IEnumerator SpreadShot()
    {
        // �Ѿ� �߻�
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

        bulletRigidOne.velocity = bulletPos.forward * 50; // �Ѿ��� �ӵ��� ����
        bulletRigidTwo.velocity = additionalBulletRot2 * 50; // �Ѿ��� �ӵ��� ����
        bulletRigidThree.velocity = additionalBulletRot3 * 50; // �Ѿ��� �ӵ��� ����

        yield return null; // 1������ ����

        // ź�� ����

        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        GameObject instantCase2 = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid2 = instantCase.GetComponent<Rigidbody>();
        GameObject instantCase3 = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid3 = instantCase.GetComponent<Rigidbody>();

        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -1) + Vector3.up * Random.Range(2, 3); // ������ ������ ����!

        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // ȸ���ϴ� ���� �ִ� ��!
        caseRigid2.AddForce(caseVec, ForceMode.Impulse);
        caseRigid2.AddTorque(Vector3.up * 10, ForceMode.Impulse); // ȸ���ϴ� ���� �ִ� ��!
        caseRigid3.AddForce(caseVec, ForceMode.Impulse);
        caseRigid3.AddTorque(Vector3.up * 10, ForceMode.Impulse); // ȸ���ϴ� ���� �ִ� ��!
    }


    /*
    
    IEnumerator Swing()
    {
        // 1
        yield return null; // yield�� �ʼ�? -> ����� ���� 1������ ���

        // 2
        yield return new WaitForSeconds(0.1f); // �� 1�������� �ƴ϶� �ð��� ���ؼ� ����ų ���� �ִ�!

        //3
        yield return null; // ��, ���� ���� ����� �� �� ����


        yield break; // �̰� �׸��ϴ� ��! -> �̰ͺ��� �Ʒ� ������ �Ʒ��� ���� ����

    }
    
    */

    // Use�Լ� : ���� ��ƾ -> Swing() ���� ��ƾ�� ���� -> �ٽ� ���η�ƾ���� ���ƿͼ� Use() ����
    // �ڷ�ƾ ��� �� : ���� + ���� ��ƾ�� ���� ���� (co-op ����!)


}
