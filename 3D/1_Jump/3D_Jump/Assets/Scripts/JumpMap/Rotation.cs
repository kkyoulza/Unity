using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    float RotateSpeed = 85.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime, Space.World);
        // Update에서는 어떤 환경에서도 동일한 속도로 돌게 하기 위해 Time.deltaTime을 곱해준다.
        // Space.World -> 전역 좌표계 기준
    }
}
