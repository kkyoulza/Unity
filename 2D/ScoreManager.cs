using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int TargetNum = 0; // 맞춰진 타겟의 종류!
    int cntScore;
    int Rand_Spawn;
    public Text Score;
    float RandomFloatX,RandomFloatY;
    public GameObject TargetPrefab1;
    public GameObject TargetPrefab2;
    public GameObject TargetPrefab3;
    public GameObject Bomb1;
    private GameObject Target;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        switch (TargetNum)
        {
            case -1:
                cntScore = int.Parse(Score.text);
                if (cntScore > 0)
                {
                    cntScore -= 1;
                    Score.text = cntScore.ToString();
                }
                TargetNum = 0;
                break;
            case 1:
                cntScore = int.Parse(Score.text);
                cntScore += 1;
                Score.text = cntScore.ToString();
                TargetNum = 0;
                break;
            case 2:
                cntScore = int.Parse(Score.text);
                cntScore += 2;
                Score.text = cntScore.ToString();
                TargetNum = 0;
                break;
        }

    }

    public void SetOne()
    {
        TargetNum = 1;
    }

    public void SetTwo()
    {
        TargetNum = 2;
    }

    public void MinusOne()
    {
        TargetNum = -1;
    }

    public void SpawnTarget()
    {
        Rand_Spawn = UnityEngine.Random.Range(0,3);
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        RandomFloatX = UnityEngine.Random.Range(-8.2f, 8.4f);
        RandomFloatY = UnityEngine.Random.Range(-4.4f, 4.4f);

        if(Rand_Spawn >= 0 && Rand_Spawn < 2)
        {
            Target = Instantiate(TargetPrefab1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;
        }
        else if(Rand_Spawn == 2)
        {
            Target = Instantiate(Bomb1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;
        }


        if (RandomFloatX > 0)
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, 0));
        else
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 0));
    }

}
