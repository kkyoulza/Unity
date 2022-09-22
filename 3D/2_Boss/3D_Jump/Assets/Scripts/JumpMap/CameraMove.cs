using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Vector3 offSet = new Vector3(0,1.4f,-3.5f);
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + offSet; // 플레이어의 위치에서 offSet을 적용한 것이다!
        // 헷갈리지 말 것!
    }

}
