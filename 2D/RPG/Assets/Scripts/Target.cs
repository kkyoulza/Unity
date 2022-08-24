using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    bool hit = false; // 맞았는지 여부를 나타냄
    GameObject Manager;
    Animator Target1_ani;

    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.Find("Manager");
        if (this.tag == "Score1")
        {
            Target1_ani = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(hit == true)
        {
            if(this.tag == "Score1") // Tag Check
            {
                Target1_ani.SetTrigger("Destroy_Target1");

                Manager.GetComponent<SoundManager>().PlayScore1();
                Manager.GetComponent<StageManager>().MinusTarget1(); // 잡아야 하는 타겟의 수를 줄인다.
                Manager.GetComponent<ScoreManager>().SetOne();

                this.tag = "DeleteState"; // 애니메이션이 나오는 동안 사라지는 상태로 태그를 바꾼다.

                Invoke("Target1_Delete", 0.5f);

            }
            else if(this.tag == "Minus1")
            {
                Manager.GetComponent<SoundManager>().PlayMinus1();
                Manager.GetComponent<ScoreManager>().MinusOne();

                Destroy(gameObject);
               
            }

            // Delete();

        }
    }

    public void beHit()
    {
        hit = true;
    }
    
    void Target1_Delete()
    {
        Destroy(gameObject); // 터치시 삭제
    }
    

}
