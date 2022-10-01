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
