using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum AtkType { Melee, Range }; // Melee - ���� ����, Range - ���Ÿ� ����
    public AtkType type; // ������ �� �־�� �Ѵ�.
    public int Damage;
    public float AtkDelay;
    public BoxCollider meleeArea; // ���� ���� ����
    public TrailRenderer trailEffect; // ȿ��?

    // ���Ÿ� ����
    public Transform bulletPos; // �Ѿ� ���� ��ġ
    public GameObject bullet; // ���� �� �Ѿ�
    public Transform bulletCasePos; // ź�ǰ� ������ ��ġ ����
    public GameObject bulletCase; // ź��
    public int maxCount; // �ִ� �Ѿ� ����
    public int cntCount; // ���� �Ѿ� ����


    public void Use()
    {
        if (type == AtkType.Melee)
        {
            StopCoroutine("Swing"); // �����ϰ� �ִ� �߿��� �� ������ų �� ����
            StartCoroutine("Swing");
        }
        else if(type == AtkType.Range && cntCount > 0)
        {
            cntCount--;
            StopCoroutine("Shot"); // �����ϰ� �ִ� �߿��� �� ������ų �� ����
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);

        meleeArea.enabled = true; // box Collider Ȱ��ȭ
        trailEffect.enabled = true; // effect Ȱ��ȭ

        yield return new WaitForSeconds(0.3f);

        meleeArea.enabled = false; // box Collider Ȱ��ȭ

        yield return new WaitForSeconds(0.3f);

        trailEffect.enabled = false; // effect Ȱ��ȭ


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
