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
            Manager.GetComponent<ScoreManager>().SetOne();
            Destroy(gameObject); // 터치시 삭제
        }
    }

    public void beHit()
    {
        hit = true;
    }

    

}
