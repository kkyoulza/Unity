using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dmgPool : MonoBehaviour
{
    // ������ �ؽ�Ʈ Prefab�� ���� �� ���� 
    public GameObject[] dmgPrefabs;

    // ������ Ǯ�� ���� �� ����Ʈ (������ N���� N���� ����Ʈ�� ����� �־�� ��)
    public List<GameObject>[] pools; // ����Ʈ�� �迭!!

    void Awake()
    {
        pools = new List<GameObject>[dmgPrefabs.Length]; // pool�� ũ��� prefab�� ������ŭ!

        for(int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>(); // �������� �ǹ̷� �Լ�ó�� �������� �Ұ�ȣ�� �־� �ش�.
            // pools�� �� �ڸ��� ����Ʈ�� �־� �ش�. (�ʱ�ȭ!)
        }

        Debug.Log(gameObject.name + pools.Length);

    }

    public GameObject GetObj(int index) // ���� ������Ʈ �Ҵ�!
    {
        GameObject select = null; // Ǯ ���ο��� ����ִ� ������Ʈ�� ��� ��ȯ�Ѵ�.

        // ������ Ǯ���� ��� �ִ�(��Ȱ��ȭ ��) ���� ������Ʈ�� ����!
            
            // �߰��ϸ� select ������ �Ҵ�!

        foreach(GameObject skin in pools[index]) // index��ġ�� �ִ� prefab�� ���� pools ����Ʈ�� ���� 
        {
            if (!skin.activeSelf)
            {
                // Ž������ ���� ������Ʈ�� ��Ȱ��ȭ �Ǿ�����!
                select = skin; // �Ҵ�!
                select.SetActive(true); // Ȱ��ȭ!
                break;
            }
        }


        // ��� �ִ� ���� ������Ʈ�� ���ٸ�! (��ΰ� ���� ���̸�)
        if (!select)
        {
            // ���ӿ�����Ʈ�� �ܵ����� ����ϸ� null ���θ� ������ ���̴�! (������ false)

            // ���Ӱ� ����(Instantiate)�Ͽ� select ������ �Ҵ��Ѵ�.
            
            select = Instantiate(dmgPrefabs[index], transform); 
            // �� ���� �����ε� : ��ȯ �� ������Ʈ, ��ȯ �� �θ� ������Ʈ(�������� ������������ �ʰ�!)
            // �θ� ������Ʈ�� transform���� �� ���Ƽ� dmgPool�� ��� �� ������Ʈ�� �ڽ����� ������ �ؽ�Ʈ�� �߰� �Ͽ���.
            
            pools[index].Add(select); // ���Ӱ� Ǯ�� ���!

        }


        return select;
    }


}
