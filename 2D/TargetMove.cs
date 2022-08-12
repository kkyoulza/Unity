using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetMove : MonoBehaviour
{
    Rigidbody2D rigid;
    float changeTime = 0f;
    int RandomMove;
    float TargetA_MaxVel = 8f;

    float Bomb1_MaxVel = 6.5f;

    float MaxVel; // Tag에 따라서 MaxVel 값을 설정 해 주기 위함

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();        
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        changeTime += Time.deltaTime;

        if (transform.position.x < -8.3f)
        {
            ToRight();
            /*
            if(rigid.velocity.x < 0)
            {
                horizonChange();
            }
            */
        }

        if (transform.position.x > 8.3f)
        {
            ToLeft();
            /*
            if (rigid.velocity.x > 0)
            {
                horizonChange();
            }*/

        }

        if (transform.position.y > 4.4f)
        {
            ToDown();
            /*
            if (rigid.velocity.y > 0)
            {
                verticalChange();
            }*/

        }

        if (transform.position.y < -4.4f)
        {
            ToUp();
            /*
            if(rigid.velocity.y < 0)
            {
                verticalChange();
            }*/

        }



        if (changeTime > 2.0f)
        {
            Debug.Log(rigid.velocity);
            RandomMove = UnityEngine.Random.Range(0, 3);

            switch (RandomMove)
            {
                case 0:
                    ToRight();
                    //horizonChange();
                    changeTime = 0f;
                    break;
                case 1:
                    ToLeft();
                    //verticalChange();
                    changeTime = 0f;
                    break;
                case 2:
                    ToDown();
                    //verticalChange();
                    changeTime = 0f;
                    break;
                case 3:
                    ToUp();
                    //verticalChange();
                    changeTime = 0f;
                    break;
            }

        }
        
    }


    public void ToRight()
    {
        // /*

        if (this.tag == "Score1")
            MaxVel = TargetA_MaxVel;
        else if (this.tag == "Minus1")
            MaxVel = Bomb1_MaxVel;
        
        if(rigid.velocity.x < MaxVel)
        {
            rigid.AddForce(new Vector2(50, 0));
        }
        else
        {
            rigid.velocity = new Vector2(MaxVel, rigid.velocity.y);
        }
        // */

        // rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);

    }

    public void ToLeft()
    {

        if (this.tag == "Score1")
            MaxVel = TargetA_MaxVel;
        else if (this.tag == "Minus1")
            MaxVel = Bomb1_MaxVel;

        if (rigid.velocity.x > MaxVel * (-1))
        {
            rigid.AddForce(new Vector2(-50, 0));
        }
        else
        {
            rigid.velocity = new Vector2(MaxVel * (-1), rigid.velocity.y);
        }
    }

    public void ToUp()
    {

        if (this.tag == "Score1")
            MaxVel = TargetA_MaxVel;
        else if (this.tag == "Minus1")
            MaxVel = Bomb1_MaxVel;

        if (rigid.velocity.y < MaxVel)
        {
            rigid.AddForce(new Vector2(0, 100));
        }
        else
        {
            rigid.velocity = new Vector2(rigid.velocity.x, MaxVel);
        }
        
    }

    public void ToDown()
    {

        if (this.tag == "Score1")
            MaxVel = TargetA_MaxVel;
        else if (this.tag == "Minus1")
            MaxVel = Bomb1_MaxVel;

        if (rigid.velocity.y > MaxVel * (-1))
        {
            rigid.AddForce(new Vector2(0, -100));
        }
        else
        {
            rigid.velocity = new Vector2(rigid.velocity.x, MaxVel * (-1));
        }
    }


}
