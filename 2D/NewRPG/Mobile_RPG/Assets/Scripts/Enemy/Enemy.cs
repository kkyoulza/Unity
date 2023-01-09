using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public bool isFixed; // ���� �����ΰ�?
    public enum Type { Normal, Fire, Ice, Land };
    public Type monsterType; // ���� �Ӽ�

    public float monsterCntHP;
    public float monsterMaxHP;
    public int monsterAtk; 
    public int monsterDef;

    GameObject skillObj;
    public GameObject dmg; // ������ Prefab
    public GameObject dmgPos; // ������ ���� ��ġ
    dmgPool pooling; // ������ ��Ų Ǯ��

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

    public IEnumerator attacked() // ���� ������ ��
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Skills skillInfo = skillObj.GetComponent<SkillInfo>().thisSkillInfo;
        sprite.color = Color.red; // �ǰ� �� ������
        

        for (int i = 0; i < skillInfo.atkCnt; i++)
        {
            Debug.Log((i + 1) + "Ÿ");

            GameObject imsiDmg = pooling.GetObj(0);
            imsiDmg.GetComponent<dmgSkins>().setDamage(((int)skillInfo.skillDmg - monsterDef));
            monsterCntHP -= ((int)skillInfo.skillDmg - monsterDef);
            imsiDmg.transform.position = dmgPos.transform.position;

            float hpRatio = (monsterCntHP / monsterMaxHP); // HP�� int�� ���� ���� ���� ���� ���� float�� ����� �� ��ȯ�� �ϸ� �̹� �ʴ´�. ���� HP�� �տ� float�� �� �־���� �ߴ�.
            HPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(hpRatio, 0.1f);
                 
            if (monsterCntHP <= 0)
            {
                Destroy(gameObject);
                Debug.Log("���� ��ġ!");
            }

        }

        yield return new WaitForSeconds(0.1f);

        sprite.color = Color.white;

    }




}
