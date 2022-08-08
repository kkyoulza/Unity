using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetMove : MonoBehaviour
{
    Rigidbody2D rigid;
    float changeTime = 0f;
    int RandomMove;
    float TargetA_MaxVel = 10f;

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

    public void horizonChange()
    {
        rigid.velocity = new Vector2(rigid.velocity.x*(-1), rigid.velocity.y);
    }
    public void verticalChange()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * (-1));
    }

    public void ToRight()
    {
        // /*
        
        if(rigid.velocity.x < TargetA_MaxVel)
        {
            rigid.AddForce(new Vector2(50, 0));
        }
        else
        {
            rigid.velocity = new Vector2(TargetA_MaxVel, rigid.velocity.y);
        }
        // */

        // rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);

    }

    public void ToLeft()
    {
        if (rigid.velocity.x > TargetA_MaxVel*(-1))
        {
            rigid.AddForce(new Vector2(-50, 0));
        }
        else
        {
            rigid.velocity = new Vector2(TargetA_MaxVel*(-1), rigid.velocity.y);
        }
    }

    public void ToUp()
    {
        if(rigid.velocity.y < TargetA_MaxVel)
        {
            rigid.AddForce(new Vector2(0, 100));
        }
        else
        {
            rigid.velocity = new Vector2(rigid.velocity.x, TargetA_MaxVel);
        }
        
    }

    public void ToDown()
    {
        if (rigid.velocity.y > TargetA_MaxVel*(-1))
        {
            rigid.AddForce(new Vector2(0, -100));
        }
        else
        {
            rigid.velocity = new Vector2(rigid.velocity.x, TargetA_MaxVel*(-1));
        }
    }


}
