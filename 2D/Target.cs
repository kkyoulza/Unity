using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    bool hit = false; // 맞았는지 여부를 나타냄
    GameObject Manager;

    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.Find("Manager");
    }

    // Update is called once per frame
    void Update()
    {
        if(hit == true)
        {
            if(this.tag == "Score1") // Tag Check
            {
                Manager.GetComponent<SoundManager>().PlayScore1();
                Manager.GetComponent<StageManager>().MinusTarget1();
                Manager.GetComponent<ScoreManager>().SetOne();
            }
            else if(this.tag == "Minus1")
            {
                Manager.GetComponent<SoundManager>().PlayMinus1();
                Manager.GetComponent<ScoreManager>().MinusOne();
            }

            Destroy(gameObject); // 터치시 삭제
        }
    }

    public void beHit()
    {
        hit = true;
    }

    

}
