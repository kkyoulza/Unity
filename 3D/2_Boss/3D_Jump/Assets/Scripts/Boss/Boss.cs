using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy // Enemy ���
{
    public GameObject missile; // ���� �̻���
    public Transform missilePortA; // �̻��� ��Ʈ1
    public Transform missilePortB; // �̻��� ��Ʈ2
    // ���� ���� Enemy���� bullet�� �̹� �����ϱ⿡ �ű⿡ ���� ������ �ȴ�.

    Vector3 lookVector; // �÷��̾ ���� ������ �����ϴ� ����
    Vector3 tauntVector; // ������� ��ġ ����
    
    public bool isLook; // ���� ���� ����!


    void Awake() // Awake�� ��� �� �ִ� �ڵ忡���� ������� �ʴ´�.(�ڽ����׼��� ����..) -> �� Enemy�� �ִ� Awake�� ���� X
    {
        // �ذ���� �θ� �ڵ忡 �ִ� Awake�� Start�� �ٲپ� �ְų� �θ� Awake�� �ִ� ������� �ڽĿ� �����ϴ� ����� �ִ�.
        // �θ� �Ժη� �ٲٸ� �θ� ����ϴ� ��ü�鵵 ������ �ޱ⿡ �ڽ����� �����Ͽ���.
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<MeshRenderer>().material; // material�� �������� ���!!
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
            StopAllCoroutines(); // �ϰ� �ִ� ��� �ڷ�ƾ���� break �� �ش�.
            return; // �Ʒ��� �������� ���ϰ�!
        }

        if (isLook)
        {
            // ������ �÷��̾ �ٶ󺸰� �ִ� ���̶��?

            // �÷��̾��� �Է¿� ���� �ٶ󺸴� ������ �ٲ��� �Ѵ�.
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            lookVector = new Vector3(h, 0, v) * 5f;

            transform.LookAt(target.position + lookVector); // ����� ���� �ٶ󺸰� �ٲپ��!
            // target�� Enemy���� ��ӵ� �����̴�.

        }
        else // �ٶ󺸴� ���� �ƴ� ��
        {
            navi.SetDestination(tauntVector); // ��� ���͸� ���󰡰�!
        }
    }

    IEnumerator Think() // ����!
    {
        yield return new WaitForSeconds(0.1f); // �����ϴ� �ð�(�� ������) -> Ŭ ���� ���̵��� ������ (���̵� ������)

        int ranAction = Random.Range(0, 5); // 0~4���� �������� ����!

        switch (ranAction)
        {
            // break ���� �ٿ��� ������ Ȯ���� �ø� �� �ִ�!
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
        StartCoroutine(Think()); // �ִϸ��̼� �ߵ��� ���� ��, �ٽ� ������ �ؾ� �Ѵ�!
    }

    IEnumerator RockShot()
    {
        isLook = false; // ȸ���� ���� �ʰ� ��� �⸦ ������.
        anim.SetTrigger("DoBigShot"); // �ִϸ��̼� ���
        Instantiate(monsterMissile, transform.position, transform.rotation);
        yield return new WaitForSeconds(3.0f);
        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVector = target.position + lookVector; // �������� ��ġ = Ÿ�� ��ġ + �ٶ󺸴� ��ġ ���� �κ�

        isLook = false; // ������� �������� ���� ����!
        navi.isStopped = false;
        boxCollider.enabled = false; // ���� ���߿� �÷��̾ ���� �ʰ� �ϱ� ���� boxCollider�� ��Ȱ��ȭ �Ѵ�.
        anim.SetTrigger("DoTaunt"); // �ִϸ��̼� ���!

        yield return new WaitForSeconds(1.5f); 
        meleeArea.enabled = true; // �������� ��!

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1.0f);
        isLook = true;
        navi.isStopped = true;
        boxCollider.enabled = true; // �ٽ� �ڽ� Ȱ��ȭ

        StartCoroutine(Think());
    }


}
