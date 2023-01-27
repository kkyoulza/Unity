using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // �̵� ����
    public bool isFixed; // ���� �����ΰ�?
    public bool isChange; // ������ �ٲ�°�?

    public int velocityDir; // �̵� ����
    public int ranNum; // �ൿ�� ���� �� ����

    // ���� ����
    Rigidbody2D rigid;

    public Vector2 vel;
    public bool isUsePool; // pool�� ����Ͽ� ������ �ؽ�Ʈ�� ����� ���ΰ�?(�׽�Ʈ��)

    // ���� ����
    public bool isHit;

    // ���� ����
    public enum Type { Normal, Fire, Ice, Land };
    public Type monsterType; // ���� �Ӽ�
    public int monsterCode; // ���� �ڵ�

    public List<GameObject> attackObj; // ���͸� Ÿ���ϴ� ������Ʈ ����

    // ���� ����
    public float monsterCntHP;
    public float monsterMaxHP;
    public int monsterAtk; 
    public int monsterDef;
    public int addExp; // �ش� ���͸� ���� �� �������� ����ġ ��ġ

    GameObject skillObj;
    public GameObject dmg; // ������ Prefab
    public GameObject dmgPos; // ������ ���� ��ġ
    dmgPool pooling; // ������ ��Ų Ǯ��

    public GameObject HPBar; // HP Bar

    // ����,�ִϸ��̼� ����
    SpriteRenderer sprite;
    Animator anim;

    // ���� ����
    public GameObject bronzeCoin;


    void Awake()
    {
        pooling = GetComponent<dmgPool>();
        rigid = GetComponent<Rigidbody2D>();
        velocityDir = 0; // idle
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (!isFixed)
            StartCoroutine(setState());
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(velocityDir, rigid.velocity.y); 

        vel = rigid.velocity;

        checkTerraian();
    }

    void Update()
    {
        checkSprite();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10 && !isHit)
        {
            checkHitList(collision.gameObject);
            isHit = true;
            skillObj = collision.gameObject;
            StartCoroutine(attacked());
        }
    }

    public void checkSprite()
    {
        sprite.flipX = rigid.velocity.x > 0 ? true : false;
    }

    public void checkTerraian()
    {
        Debug.DrawRay(rigid.position + Vector2.right * (sprite.flipX ? 1 : -1), Vector3.down, new Color(1, 0, 0)); // �������� �׸���. (�ð������� ���� ����)

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position + Vector2.right * (sprite.flipX ? 1 : -1), Vector3.down, 2, LayerMask.GetMask("Platform")); // ��¥ ������ �׸��� (������ġ, ����, �Ÿ�)

        if (rayHit.collider == null && !isChange)
        {
            // ���� ��ó�� ���� ��

            velocityDir *= -1; // ���� ��ȯ
            isChange = true;
            // Debug.Log("changeOn");
        }
        else if(rayHit.collider != null)
        {
            isChange = false;
        }

    }

    public void StartMove()
    {
        StartCoroutine(setState());
    }

    public IEnumerator setState()
    {
        velocityDir = Random.Range(-1, 2); // -1 ~ 1

        anim.SetInteger("isRun", velocityDir); // �̵� ���´� ��Ÿ���� ���ڸ� �̿��Ͽ� �ִϸ������� �Ű������� int�� �����Ͽ���

        yield return new WaitForSeconds(3.5f);

        StartCoroutine(setState());

    }

    public IEnumerator attacked() // ���� ������ ��
    {
        Skills skillInfo = skillObj.GetComponent<SkillInfo>().thisSkillInfo;
        sprite.color = Color.red; // �ǰ� �� ������

        if (!isFixed)
        {
            rigid.AddForce(Vector2.up, ForceMode2D.Impulse);
        }

        for (int i = 0; i < skillInfo.atkCnt; i++)
        {
            Debug.Log((i + 1) + "Ÿ");

            GameObject imsiDmg;

            if (isUsePool)
                imsiDmg = pooling.GetObj(0);
            else
                imsiDmg = Instantiate(dmg);

            imsiDmg.GetComponent<dmgSkins>().setDamage(((int)skillInfo.skillDmg - monsterDef));
            monsterCntHP -= ((int)skillInfo.skillDmg - monsterDef);
            imsiDmg.transform.position = dmgPos.transform.position;

            float hpRatio = (monsterCntHP / monsterMaxHP); // HP�� int�� ���� ���� ���� ���� ���� float�� ����� �� ��ȯ�� �ϸ� �̹� �ʴ´�. ���� HP�� �տ� float�� �� �־���� �ߴ�.
            HPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(hpRatio, 0.1f);
                 
            if (monsterCntHP <= 0)
            {
                sprite.color = Color.white;

                sendEXP();
                attackObj.Clear();
                DropItems();

                gameObject.SetActive(false);
                
                monsterCntHP = monsterMaxHP;
                HPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(1.0f, 0.1f);
                Debug.Log("���� ��ġ!");

                yield break;
            }

        }

        yield return null;

        isHit = false;

        yield return new WaitForSeconds(0.1f);

        sprite.color = Color.white;

    }

    void checkHitList(GameObject input)
    {
        GameObject addObj = input.GetComponent<SkillInfo>().player.gameObject;
        // ���͸� ���� ��鿡 ���� ����Ʈ ���� -> ����ġ �й��� �� ��� ����
        if(attackObj.Count == 0)
        {
            attackObj.Add(addObj);
            return;
        }

        Player inputPlayerInfo = input.GetComponent<SkillInfo>().player;

        for(int i = 0; i < attackObj.Count; i++)
        {
            if (attackObj[i].GetComponent<Player>().userId == inputPlayerInfo.userId) // �̹� ��ϵǾ� �ִٸ�
                return; // ����
        }

        attackObj.Add(addObj); // ��ϵǾ� �ִ� ���� ���ٸ� ���� ���

    }

    void sendEXP()
    {
        // ���Ͱ� �׾��� ��, ����ġ�� ������.
        for(int i = 0; i < attackObj.Count; i++)
        {
            attackObj[i].GetComponent<PlayerStats>().playerStat.playerCntExperience += (int)((float)addExp / (float)attackObj.Count);
            attackObj[i].GetComponent<LogManager>().playerLog.addMonsterCount(monsterCode);
            // Debug.Log("Exp Add + "+(int)((float)addExp / (float)attackObj.Count));
        }

    }

    void DropItems()
    {
        int ran = Random.Range(0, 2); // 0 ~ 1

        switch (ran)
        {
            case 0:
                Instantiate(bronzeCoin,transform.position,transform.rotation);
                break;
            case 1:
                Instantiate(bronzeCoin, transform.position, transform.rotation);
                Instantiate(bronzeCoin, transform.position, transform.rotation);
                break;

        }

    }

}