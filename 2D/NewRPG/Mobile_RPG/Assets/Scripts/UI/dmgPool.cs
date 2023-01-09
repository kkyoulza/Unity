using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dmgPool : MonoBehaviour
{
    // 데미지 텍스트 Prefab을 저장 할 변수 
    public GameObject[] dmgPrefabs;

    // 실제로 풀을 저장 할 리스트 (종류가 N개면 N개의 리스트를 만들어 주어야 함)
    public List<GameObject>[] pools; // 리스트의 배열!!

    void Awake()
    {
        pools = new List<GameObject>[dmgPrefabs.Length]; // pool의 크기는 prefab의 종류만큼!

        for(int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>(); // 생성자의 의미로 함수처럼 마지막에 소괄호를 넣어 준다.
            // pools의 각 자리에 리스트를 넣어 준다. (초기화!)
        }

        Debug.Log(gameObject.name + pools.Length);

    }

    public GameObject GetObj(int index) // 게임 오브젝트 할당!
    {
        GameObject select = null; // 풀 내부에서 놀고있는 오브젝트를 골라서 반환한다.

        // 선택한 풀에서 놀고 있는(비활성화 된) 게임 오브젝트에 접근!
            
            // 발견하면 select 변수에 할당!

        foreach(GameObject skin in pools[index]) // index위치에 있는 prefab을 가진 pools 리스트에 접근 
        {
            if (!skin.activeSelf)
            {
                // 탐색중인 게임 오브젝트가 비활성화 되었으면!
                select = skin; // 할당!
                select.SetActive(true); // 활성화!
                break;
            }
        }


        // 놀고 있는 게임 오브젝트가 없다면! (모두가 동작 중이면)
        if (!select)
        {
            // 게임오브젝트를 단독으로 사용하면 null 여부를 따지는 것이다! (없으면 false)

            // 새롭게 생성(Instantiate)하여 select 변수에 할당한다.
            
            select = Instantiate(dmgPrefabs[index], transform); 
            // 두 번쩨 오버로딩 : 소환 할 오브젝트, 소환 할 부모 오브젝트(계층란이 지저분해지지 않게!)
            // 부모 오브젝트를 transform으로 해 놓아서 dmgPool이 들어 간 오브젝트의 자식으로 데미지 텍스트가 뜨게 하였다.
            
            pools[index].Add(select); // 새롭게 풀에 등록!

        }


        return select;
    }


}
