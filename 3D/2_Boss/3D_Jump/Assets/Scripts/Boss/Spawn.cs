using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject spawnTypeA; // A타입 광물 - 은 광맥
    public GameObject spawnTypeB; // B타입 광물 - 금 광맥

    public GameObject onObject = null; // 트리거 위에 올려져 있는 오브젝트
    public float spawnTime; // 스폰 타임
    float deltaT; // 지나간 시간

    Vector3 spawnPlace;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (onObject == null) // 올라 가 있는 물체가 없으니 다음 광맥 스폰을 위한 카운트 다운
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
