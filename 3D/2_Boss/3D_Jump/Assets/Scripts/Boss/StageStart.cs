using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStart : MonoBehaviour
{
    public int zoneNum; // �ش� ��
    public int mobCount; // ó���� ���� �� �� ����

    public bool isZoneClear; // �ش� �� Ŭ���� ����
    public bool isStarted; // �ش� ���� ��� ���°�?
    public bool isSpawned; // ���� ���� �Ǿ��°�?

    public int xRange; // x ���� ����
    public int zRange; // z ���� ����

    public GameObject nextDoor; // �������� ���� ��
    public GameObject[] spawnMonsters; // ���� ���� �ȿ� �ִ� ���͵� ����Ʈ(�� ��Ҵ°� Ȯ���ϱ� ����!)

    public GameObject spawnTypeA;
    public GameObject spawnTypeB;
    public GameObject spawnTypeC;
    public GameObject spawnBoss; // boss ��ȯ


    // Start is called before the first frame update
    void Awake()
    {
        if(zoneNum == 0)
        {
            // 0�϶��� �� ó���� �ٷ� ��ȯ!
            for(int i = 0; i < mobCount; i++)
            {
                int ran = Random.Range(-xRange, xRange);
                int ran2 = Random.Range(-zRange, zRange);
                Vector3 pos = transform.position;
                pos.x += ran;
                pos.z += ran2;
                Instantiate(spawnTypeA, pos, transform.rotation);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpawnMonsters();
        findMonster();
        checkMonster();
    }

    void SpawnMonsters()
    {
        if(isStarted && !isSpawned)
        {
            switch (zoneNum)
            {
                case 1:
                    for(int i = 0; i < mobCount/3; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeA, pos, transform.rotation);
                    }

                    for (int i = mobCount / 3; i < mobCount * 2 / 3; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeB, pos, transform.rotation);
                    }

                    for (int i = mobCount * 2 / 3; i < mobCount; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeC, pos, transform.rotation);
                    }

                    isSpawned = true;
                    break;
                case 2:
                    for (int i = 0; i < mobCount / 3; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeA, pos, transform.rotation);
                    }

                    for (int i = mobCount / 3; i < mobCount * 2 / 3; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeB, pos, transform.rotation);
                    }

                    for (int i = mobCount * 2 / 3; i < mobCount-1; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeC, pos, transform.rotation);
                    }

                    isSpawned = true;
                    break;
                case 3:
                    Instantiate(spawnBoss, transform.position, transform.rotation);
                    isSpawned = true;
                    break;

            }
        }
    }
    void findMonster()
    {
        
        for(int i = 0; i < spawnMonsters.Length; i++)
        {
            spawnMonsters[i] = null;
        }
        // Collider�� ��� �ִ� ���͵��� ã�� ��
        spawnMonsters = GameObject.FindGameObjectsWithTag("Enemy");
    }

    void checkMonster()
    {
        // ���͸� �� óġ�ߴ��� Ȯ���ϴ� ��

        if(spawnMonsters.Length == 0 && !isZoneClear && (isStarted || zoneNum == 0))
        {
            Debug.Log("���Ͱ� �� �������!");
            Animator ani = nextDoor.GetComponent<Animator>();
            ani.SetTrigger("Open");
            AudioSource aud = nextDoor.GetComponent<AudioSource>();
            aud.Play();
            isZoneClear = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && zoneNum != 0 && !isStarted)
        {
            isStarted = true;
        }
    }

}
