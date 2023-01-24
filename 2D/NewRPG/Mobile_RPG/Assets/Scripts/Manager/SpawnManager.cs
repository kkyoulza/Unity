using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<SpawnPlace> places;

    public List<GameObject>[] pools; // 몬스터들이 들어 갈 pool

    public GameObject[] spawnMonsters;
    public GameObject spawnParent;

    // Start is called before the first frame update
    void Awake()
    {
        pools = new List<GameObject>[spawnMonsters.Length]; // pool의 크기는 prefab의 종류만큼!

        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>(); // 생성자의 의미로 함수처럼 마지막에 소괄호를 넣어 준다.
            // pools의 각 자리에 리스트를 넣어 준다. (초기화!)
        }

        StartCoroutine(SpawnCheck());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnCheck()
    {

        foreach(SpawnPlace place in places)
        {
            if(place.spawnedMonster == null || !place.spawnedMonster.activeSelf)
            {
                GameObject spawnedMonster = GetMonster(0);
                spawnedMonster.GetComponent<Enemy>().isHit = false;
                spawnedMonster.transform.position = place.gameObject.transform.position;
                place.spawnedMonster = spawnedMonster;

                spawnedMonster.GetComponent<Enemy>().StartMove();

                Debug.Log("Spawned");
            }
        }

        yield return new WaitForSeconds(6.0f);


        StartCoroutine(SpawnCheck());

    }

    GameObject GetMonster(int index)
    {
        GameObject select = null; // 풀 내부에서 놀고있는 오브젝트를 골라서 반환한다.

        // 선택한 풀에서 놀고 있는(비활성화 된) 게임 오브젝트에 접근!

        // 발견하면 select 변수에 할당!

        foreach (GameObject mob in pools[index]) // index위치에 있는 prefab을 가진 pools 리스트에 접근 
        {
            if (!mob.activeSelf)
            {
                // 탐색중인 게임 오브젝트가 비활성화 되었으면!
                select = mob; // 할당!
                select.SetActive(true); // 활성화!
                break;
            }
        }


        // 놀고 있는 게임 오브젝트가 없다면! (모두가 동작 중이면)
        if (!select)
        {
            // 게임오브젝트를 단독으로 사용하면 null 여부를 따지는 것이다! (없으면 false)

            // 새롭게 생성(Instantiate)하여 select 변수에 할당한다.

            select = Instantiate(spawnMonsters[index], spawnParent.transform);
            // 두 번쩨 오버로딩 : 소환 할 오브젝트, 소환 할 부모 오브젝트(계층란이 지저분해지지 않게!)
            // 부모 오브젝트를 transform으로 해 놓아서 dmgPool이 들어 간 오브젝트의 자식으로 데미지 텍스트가 뜨게 하였다.

            pools[index].Add(select); // 새롭게 풀에 등록!

        }


        return select;
    }

}
