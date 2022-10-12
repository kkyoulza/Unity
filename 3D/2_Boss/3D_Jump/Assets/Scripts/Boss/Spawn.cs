using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject spawnTypeA; // AŸ�� ���� - �� ����
    public GameObject spawnTypeB; // BŸ�� ���� - �� ����

    public GameObject onObject = null; // Ʈ���� ���� �÷��� �ִ� ������Ʈ
    public float spawnTime; // ���� Ÿ��
    float deltaT; // ������ �ð�

    Vector3 spawnPlace;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (onObject == null) // �ö� �� �ִ� ��ü�� ������ ���� ���� ������ ���� ī��Ʈ �ٿ�
        {
            deltaT += Time.deltaTime;
        }
            

        if(deltaT >= spawnTime)
        {
            Spawn_Things();
            deltaT = 0f;
        }

        
    }

    public void Spawn_Things()
    {
        int ranNum = Random.Range(0, 4);
        spawnPlace = transform.position;
        spawnPlace.y += 3;
        switch (ranNum)
        {
            case 0:
            case 1:
            case 2:
                Debug.Log(ranNum);
                Instantiate(spawnTypeA, transform.position, transform.rotation);
                break;
            case 3:
                Debug.Log(ranNum);
                Instantiate(spawnTypeB, transform.position, transform.rotation);
                break;
        }
        

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 17)
            onObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 17)
            onObject = null;
    }

}
