using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Enemy
{
    public GameObject silverC;
    public GameObject goldC;
    public GameObject emeraldC;
    public GameObject rubyC;
    public GameObject origin;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead && enemyType == Type.RockA)
        {
            StartCoroutine(DropCoinsA());
        }
        else if(isDead && enemyType == Type.RockB)
        {
            StartCoroutine(DropCoinsB());
        }
    }

    void DropCoins(GameObject dropType, int cntRanNum, int minCount, int upperDistance)
    {
        Debug.Log(cntRanNum + " : " + dropType.name);

        if (cntRanNum >= 1 && cntRanNum < 41)
        {
            // 40%
            for (int i = 0; i < minCount; i++) // 
            {
                GameObject insCoin = Instantiate(dropType, transform.position, transform.rotation);

                Rigidbody coinRigid = insCoin.GetComponent<Rigidbody>();

                Vector3 caseVec = transform.forward * Random.Range(-3, 10) + Vector3.up * Random.Range(15, 20); // 랜덤한 힘으로 배출!

                coinRigid.AddForce(caseVec, ForceMode.Impulse);
                coinRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
            }

        }
        else if (cntRanNum >= 41 && cntRanNum < 71)
        {
            // 30%
            for (int i = 0; i < minCount + upperDistance; i++) // 
            {
                GameObject insCoin = Instantiate(dropType, transform.position, transform.rotation);

                Rigidbody coinRigid = insCoin.GetComponent<Rigidbody>();

                Vector3 caseVec = transform.forward * Random.Range(-3, 10) + Vector3.up * Random.Range(15, 20); // 랜덤한 힘으로 배출!

                coinRigid.AddForce(caseVec, ForceMode.Impulse);
                coinRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
            }
        }
        else if (cntRanNum >= 71 && cntRanNum < 91)
        {
            // 20%
            for (int i = 0; i < minCount + upperDistance*2; i++) //
            {
                GameObject insCoin = Instantiate(dropType, transform.position, transform.rotation);

                Rigidbody coinRigid = insCoin.GetComponent<Rigidbody>();

                Vector3 caseVec = transform.forward * Random.Range(-3, 10) + Vector3.up * Random.Range(15, 20); // 랜덤한 힘으로 배출!

                coinRigid.AddForce(caseVec, ForceMode.Impulse);
                coinRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
            }
        }
        else
        {
            // 10%
            for (int i = 0; i < minCount + upperDistance * 3; i++) // 
            {
                GameObject insCoin = Instantiate(dropType, transform.position, transform.rotation);

                Rigidbody coinRigid = insCoin.GetComponent<Rigidbody>();

                Vector3 caseVec = transform.forward * Random.Range(-3, 10) + Vector3.up * Random.Range(15, 20); // 랜덤한 힘으로 배출!

                coinRigid.AddForce(caseVec, ForceMode.Impulse);
                coinRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
            }
        }
    }

    IEnumerator DropCoinsA()
    {
        // yield return new WaitForSeconds(0.3f);

        int ranSilverCount = Random.Range(1, 101);
        int ranGoldCount = Random.Range(1, 101);
        int ranEmerCount = Random.Range(1, 101);
        int ranRubyCount = Random.Range(1, 101);

        DropCoins(silverC, ranEmerCount, 5, 2); // 실버 코인을 최소 5개(40%), 확률이 내려갈 때 마다 4개씩 늘려서 드랍할 것
        // 40%, 30%, 20%, 10%
        DropCoins(goldC, ranGoldCount, 3, 2); // 골드 코인
        DropCoins(emeraldC, ranEmerCount, 1, 1); // 에메랄드 코인

        isDead = false;

        Destroy(gameObject);

        yield return null;

    }

    IEnumerator DropCoinsB()
    {
        // yield return new WaitForSeconds(0.3f);

        int ranSilverCount = Random.Range(1, 101);
        int ranGoldCount = Random.Range(1, 101);
        int ranEmerCount = Random.Range(1, 101);
        int ranRubyCount = Random.Range(1, 101);
        int ranOriginCount = Random.Range(1, 101);

        DropCoins(silverC, ranEmerCount, 4, 2); // 실버 코인을 최소 5개(40%), 확률이 내려갈 때 마다 4개씩 늘려서 드랍할 것
        // 40%, 30%, 20%, 10%
        DropCoins(goldC, ranGoldCount, 3, 2); // 골드 코인
        DropCoins(emeraldC, ranEmerCount, 2, 1); // 에메랄드 코인
        DropCoins(rubyC, ranEmerCount, 0, 1); // 루비 코인
        DropCoins(origin, ranOriginCount, 0, 1); // 기원 조각

        isDead = false;

        Destroy(gameObject);

        yield return null;

    }


}
