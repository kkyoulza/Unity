using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2; // ȸ�� �Ŀ�
    float scaleValue = 0.1f; // ũ��
    bool isShooting; // ��� �ִ°�?

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

    IEnumerator GainPowerTimer() // �⸦ ������ Ÿ�̸�
    {
        yield return new WaitForSeconds(2.5f);
        isShooting = true;
    }

    IEnumerator GainPower() // �⸦ ������.
    {
        while (!isShooting)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); // ���������� �ӵ��� �÷��� �ϱ� ������ Acceleration�� �ִ´�.

            yield return null; // while�� �ӿ� �����̸� ���� ������ ������ �����ϱ� ������ �� �־�� �Ѵ�.
        }
    }


}
