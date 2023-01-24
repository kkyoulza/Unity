using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<SpawnPlace> places;

    public List<GameObject>[] pools; // ���͵��� ��� �� pool

    public GameObject[] spawnMonsters;
    public GameObject spawnParent;

    // Start is called before the first frame update
    void Awake()
    {
        pools = new List<GameObject>[spawnMonsters.Length]; // pool�� ũ��� prefab�� ������ŭ!

        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>(); // �������� �ǹ̷� �Լ�ó�� �������� �Ұ�ȣ�� �־� �ش�.
            // pools�� �� �ڸ��� ����Ʈ�� �־� �ش�. (�ʱ�ȭ!)
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
        GameObject select = null; // Ǯ ���ο��� ����ִ� ������Ʈ�� ��� ��ȯ�Ѵ�.

        // ������ Ǯ���� ��� �ִ�(��Ȱ��ȭ ��) ���� ������Ʈ�� ����!

        // �߰��ϸ� select ������ �Ҵ�!

        foreach (GameObject mob in pools[index]) // index��ġ�� �ִ� prefab�� ���� pools ����Ʈ�� ���� 
        {
            if (!mob.activeSelf)
            {
                // Ž������ ���� ������Ʈ�� ��Ȱ��ȭ �Ǿ�����!
                select = mob; // �Ҵ�!
                select.SetActive(true); // Ȱ��ȭ!
                break;
            }
        }


        // ��� �ִ� ���� ������Ʈ�� ���ٸ�! (��ΰ� ���� ���̸�)
        if (!select)
        {
            // ���ӿ�����Ʈ�� �ܵ����� ����ϸ� null ���θ� ������ ���̴�! (������ false)

            // ���Ӱ� ����(Instantiate)�Ͽ� select ������ �Ҵ��Ѵ�.

            select = Instantiate(spawnMonsters[index], spawnParent.transform);
            // �� ���� �����ε� : ��ȯ �� ������Ʈ, ��ȯ �� �θ� ������Ʈ(�������� ������������ �ʰ�!)
            // �θ� ������Ʈ�� transform���� �� ���Ƽ� dmgPool�� ��� �� ������Ʈ�� �ڽ����� ������ �ؽ�Ʈ�� �߰� �Ͽ���.

            pools[index].Add(select); // ���Ӱ� Ǯ�� ���!

        }


        return select;
    }

}
