using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public bool isFixed; // 고정 몬스터인가?
    public enum Type { Normal, Fire, Ice, Land };
    public Type monsterType; // 몬스터 속성

    public float monsterCntHP;
    public float monsterMaxHP;
    public int monsterAtk; 
    public int monsterDef;

    GameObject skillObj;
    public GameObject dmg; // 데미지 Prefab
    public GameObject dmgPos; // 데미지 생성 위치
    dmgPool pooling; // 데미지 스킨 풀링

    public GameObject HPBar; // HP Bar

    // Start is called before the first frame update
    void Awake()
    {
        pooling = GetComponent<dmgPool>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10)
        {
            skillObj = collision.gameObject;
            StartCoroutine(attacked());
        }
    }

    public IEnumerator attacked() // 공격 당했을 때
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Skills skillInfo = skillObj.GetComponent<SkillInfo>().thisSkillInfo;
        sprite.color = Color.red; // 피격 시 빨갛게
        

        for (int i = 0; i < skillInfo.atkCnt; i++)
        {
            Debug.Log((i + 1) + "타");

            GameObject imsiDmg = pooling.GetObj(0);
            imsiDmg.GetComponent<dmgSkins>().setDamage(((int)skillInfo.skillDmg - monsterDef));
            monsterCntHP -= ((int)skillInfo.skillDmg - monsterDef);
            imsiDmg.transform.position = dmgPos.transform.position;

            float hpRatio = (monsterCntHP / monsterMaxHP); // HP를 int로 설정 했을 때는 나눈 값에 float로 명시적 형 변환을 하면 이미 늦는다. 따라서 HP값 앞에 float로 해 주었어야 했다.
            HPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(hpRatio, 0.1f);
                 
            if (monsterCntHP <= 0)
            {
                Destroy(gameObject);
                Debug.Log("몬스터 퇴치!");
            }

        }

        yield return new WaitForSeconds(0.1f);

        sprite.color = Color.white;

    }




}
