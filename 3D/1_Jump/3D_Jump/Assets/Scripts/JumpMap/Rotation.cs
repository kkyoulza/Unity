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
        // Update������ � ȯ�濡���� ������ �ӵ��� ���� �ϱ� ���� Time.deltaTime�� �����ش�.
        // Space.World -> ���� ��ǥ�� ����
    }
}
