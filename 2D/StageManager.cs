using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StageManager : MonoBehaviour
{
    float RandomFloatX, RandomFloatY; // 생성 위치
    
    public GameObject TargetPrefab1; // 타겟 1
    public GameObject TargetPrefab2; // 타겟 2
    public GameObject TargetPrefab3; // 타겟 3
    public GameObject Bomb1; // 폭탄 1
    private GameObject Target; // 동적으로 생성 된 타겟


    private List<String> Target1Name = new List<String>();
    private List<String> Bomb1Name = new List<String>();

    int remainTarget1 = -1; // 남은 타겟의 수를 실시간으로 갱신해 주기 위한 것, 이 것이 0이 되면 다음 스테이지로 간다.
    // -1 > 체크를 하지 않는 상태


    // Start is called before the first frame update
    void Start()
    {
        Stage1();
    }

    // Update is called once per frame
    void Update()
    {
        Stage1Check();
    }

    public void SpawnTarget1(int num)
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        RandomFloatX = UnityEngine.Random.Range(-8.2f, 8.4f);
        RandomFloatY = UnityEngine.Random.Range(-4.4f, 4.4f);

        Target = Instantiate(TargetPrefab1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;

        Target.name = "Target1_" + num; // 이름 + 숫자를 적용하여 이름 변경(스테이지 초기화 시 마다 숫자 초기화)
        Target1Name.Add(Target.name); // 리스트에 저장(게임 오버시에나 폭탄이 남았을 때 제거하기 위함)

        if (RandomFloatX > 0)
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, 0));
        else
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 0));
    }

    public void SpawnBomb1(int num)
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        RandomFloatX = UnityEngine.Random.Range(-8.2f, 8.4f);
        RandomFloatY = UnityEngine.Random.Range(-4.4f, 4.4f);

        Target = Instantiate(Bomb1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;

        Target.name = "Bomb1_" + num; // 이름 + 숫자를 적용하여 이름 변경(스테이지 초기화 시 마다 숫자 초기화)
        Bomb1Name.Add(Target.name); // 리스트에 저장(게임 오버시에나 폭탄이 남았을 때 제거하기 위함)

        if (RandomFloatX > 0)
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, 0));
        else
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 0));
    }

    public void Stage1()
    {
        remainTarget1 = 10;
        for(int i = 0; i < 10; i++)
        {
            SpawnTarget1(i); // 타겟 10개 소환
            Debug.Log("타겟" + i + "개 소환");
            if (i < 5) // 폭탄 5개 소환
                SpawnBomb1(i);
        }

    }

    public void Stage1Check()
    {
        if(remainTarget1 == 0)
        {
            Debug.Log("스테이지 1 클리어! 폭탄을 제거합니다.");
            for(int i = 0; i < Bomb1Name.Count; i++)
            {
                try
                {
                    Destroy(GameObject.Find(Bomb1Name[i]));
                }
                catch
                {
                    Debug.Log("이미 맞춘 폭탄이 있어서 다음 것을 제거합니다!");
                    continue;
                }

            }
            Bomb1Name.Clear();
            Target1Name.Clear();

            remainTarget1 = -1; // checkOff상태로 변경!
        }
    }

    public void MinusTarget1()
    {
        remainTarget1--;
    }

}
