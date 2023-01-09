using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfo : MonoBehaviour
{
    // 해당 스킬 정보
    public Skills thisSkillInfo;

    // 스킬 Box On/Off 여부
    BoxCollider2D box;
    public bool isOff; // 이미 Collider가 꺼지는 코루틴이 실행되고 있는가?

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
