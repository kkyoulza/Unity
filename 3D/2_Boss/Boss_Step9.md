# Step 9. 스테이지 설정

이제는 던전 형식으로 NPC에게 말을 걸어 들어가는 던전에 대한 설정을 하도록 하겠다.

<hr>

### 던전 구상

- 던전은 N개의 방으로 이루어 지도록 구성한다.
- 방마다 적이 스폰되어 해당 적들을 퇴치하게 되면 다음 방 문이 열리는 방식이다.

![image](https://user-images.githubusercontent.com/66288087/200790494-c82c6d26-8b3a-48b5-b708-17e3e24b2d3c.png)

일자형 던전의 모습이다.

#### 스폰 방식

- 스폰 방식은 돌을 스폰하는 것과 유사하게 빈 오브젝트를 만들어 플레이어가 닿게 되면 몬스터가 스폰되게끔 해 주도록 하겠다.
- Trigger 반응시 몬스터를 스폰하는 것은 빈 오브젝트에서 코드로 만들어 관리를 해 줄 예정이다. (OnTriggerEnter)
- 던전 매니저를 만들어 해당 부분에서 잡힌 몬스터의 수를 계수하여 or 일정 지역에서 생존하고 있는 몬스터가 0마리가 되면 문이 열리게끔 할 예정이다.
- 사냥터와 같이 꾸준하게 n마리의 몬스터를 유지하는 컴포넌트도 개발할 것

<hr>

+ 단순히 코드로 1회성 기능을 개발하는 것이 아닌, 재사용할 수 있는 컴포넌트를 개발한다고 생각할 것

<hr>

#### 스폰 컴포넌트

스폰 컴포넌트에서는 스폰 위치를 지정 해 주며, 해당 스테이지에서 모든 몬스터를 퇴치했을 경우 다음 문이 열리게끔 해 준다.

몬스터는 순차적으로 생성된다는 점을 이용하여 GameObject.FindObjectsWithTag()를 이용하여 Update()문에서 객체 리스트들을 불러 와 준다.

그 리스트의 길이가 0이 되면 다음 문이 열리게 되는 것이다.

스폰 컴포넌트의 대략적인 코드는 아래와 같다.

<pre>
<code>
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
</code>
</pre>







