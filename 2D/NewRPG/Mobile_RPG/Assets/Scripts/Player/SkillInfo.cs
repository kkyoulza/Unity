using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfo : MonoBehaviour
{
    // 해당 스킬 정보
    public Skills thisSkillInfo;

    // 스킬 Box On/Off 여부
    BoxCollider2D box;

    // 스킬을 사용하는 플레이어에 대한 정보
    public Player player;

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
