using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfo : MonoBehaviour
{
    // �ش� ��ų ����
    public Skills thisSkillInfo;

    // ��ų Box On/Off ����
    BoxCollider2D box;
    public bool isOff; // �̹� Collider�� ������ �ڷ�ƾ�� ����ǰ� �ִ°�?

    // Start is called before the first frame update
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();   
    }

    // Update is called once per frame
    void Update()
    {

    }

}
