using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStart : MonoBehaviour
{
    public int zoneNum; // 해당 존
    public int mobCount; // 처음에 생성 될 몹 개수

    public bool isZoneClear; // 해당 존 클리어 여부
    public bool isStarted; // 해당 존에 들어 갔는가?
    public bool isSpawned; // 몹이 스폰 되었는가?

    public int xRange; // x 범위 설정
    public int zRange; // z 범위 설정

    public GameObject nextDoor; // 다음으로 열릴 문
    public GameObject[] spawnMonsters; // 현재 범위 안에 있는 몬스터들 리스트(다 잡았는가 확인하기 위함!)

    public GameObject spawnTypeA;
    public GameObject spawnTypeB;
    public GameObject spawnTypeC;
    public GameObject spawnBoss; // boss 소환


    // Start is called before the first frame update
    void Awake()
    {
        if(zoneNum == 0)
        {
            // 0일때는 맨 처음에 바로 소환!
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
        // Collider에 닿고 있는 몬스터들을 찾는 것
        spawnMonsters = GameObject.FindGameObjectsWithTag("Enemy");
    }

    void checkMonster()
    {
        // 몬스터를 다 처치했는지 확인하는 것

        if(spawnMonsters.Length == 0 && !isZoneClear && (isStarted || zoneNum == 0))
        {
            Debug.Log("몬스터가 다 사라졌다!");
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
