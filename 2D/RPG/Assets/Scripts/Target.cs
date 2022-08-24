using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    bool hit = false; // �¾Ҵ��� ���θ� ��Ÿ��
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
                Manager.GetComponent<StageManager>().MinusTarget1(); // ��ƾ� �ϴ� Ÿ���� ���� ���δ�.
                Manager.GetComponent<ScoreManager>().SetOne();

                this.tag = "DeleteState"; // �ִϸ��̼��� ������ ���� ������� ���·� �±׸� �ٲ۴�.

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
        Destroy(gameObject); // ��ġ�� ����
    }
    

}
