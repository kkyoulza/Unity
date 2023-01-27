using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // 이동 관련
    public bool isFixed; // 고정 몬스터인가?
    public bool isChange; // 방향을 바꿨는가?

    public int velocityDir; // 이동 방향
    public int ranNum; // 행동을 결정 할 숫자

    // 물리 관련
    Rigidbody2D rigid;

    public Vector2 vel;
    public bool isUsePool; // pool을 사용하여 데미지 텍스트를 출력할 것인가?(테스트용)

    // 상태 관련
    public bool isHit;

    // 정보 관련
    public enum Type { Normal, Fire, Ice, Land };
    public Type monsterType; // 몬스터 속성
    public int monsterCode; // 몬스터 코드

    public List<GameObject> attackObj; // 몬스터를 타격하는 오브젝트 모음

    // 몬스터 스탯
    public float monsterCntHP;
    public float monsterMaxHP;
    public int monsterAtk; 
    public int monsterDef;
    public int addExp; // 해당 몬스터를 잡을 때 더해지는 경험치 수치

    GameObject skillObj;
    public GameObject dmg; // 데미지 Prefab
    public GameObject dmgPos; // 데미지 생성 위치
    dmgPool pooling; // 데미지 스킨 풀링

    public GameObject HPBar; // HP Bar

    // 외형,애니메이션 관련
    SpriteRenderer sprite;
    Animator anim;

    // 보상 관련
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
        Debug.DrawRay(rigid.position + Vector2.right * (sprite.flipX ? 1 : -1), Vector3.down, new Color(1, 0, 0)); // 레이저를 그린다. (시각적으로 보기 위함)

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position + Vector2.right * (sprite.flipX ? 1 : -1), Vector3.down, 2, LayerMask.GetMask("Platform")); // 진짜 레이저 그리기 (시작위치, 방향, 거리)

        if (rayHit.collider == null && !isChange)
        {
            // 절벽 근처에 있을 때

            velocityDir *= -1; // 방향 전환
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

        anim.SetInteger("isRun", velocityDir); // 이동 상태는 나타내는 숫자를 이용하여 애니메이터의 매개변수를 int로 설정하였음

        yield return new WaitForSeconds(3.5f);

        StartCoroutine(setState());

    }

    public IEnumerator attacked() // 공격 당했을 때
    {
        Skills skillInfo = skillObj.GetComponent<SkillInfo>().thisSkillInfo;
        sprite.color = Color.red; // 피격 시 빨갛게

        if (!isFixed)
        {
            rigid.AddForce(Vector2.up, ForceMode2D.Impulse);
        }

        for (int i = 0; i < skillInfo.atkCnt; i++)
        {
            Debug.Log((i + 1) + "타");

            GameObject imsiDmg;

            if (isUsePool)
                imsiDmg = pooling.GetObj(0);
            else
                imsiDmg = Instantiate(dmg);

            imsiDmg.GetComponent<dmgSkins>().setDamage(((int)skillInfo.skillDmg - monsterDef));
            monsterCntHP -= ((int)skillInfo.skillDmg - monsterDef);
            imsiDmg.transform.position = dmgPos.transform.position;

            float hpRatio = (monsterCntHP / monsterMaxHP); // HP를 int로 설정 했을 때는 나눈 값에 float로 명시적 형 변환을 하면 이미 늦는다. 따라서 HP값 앞에 float로 해 주었어야 했다.
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
                Debug.Log("몬스터 퇴치!");

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
        // 몬스터를 때린 놈들에 대한 리스트 생성 -> 경험치 분배할 때 사용 예정
        if(attackObj.Count == 0)
        {
            attackObj.Add(addObj);
            return;
        }

        Player inputPlayerInfo = input.GetComponent<SkillInfo>().player;

        for(int i = 0; i < attackObj.Count; i++)
        {
            if (attackObj[i].GetComponent<Player>().userId == inputPlayerInfo.userId) // 이미 등록되어 있다면
                return; // 리턴
        }

        attackObj.Add(addObj); // 등록되어 있는 것이 없다면 새로 등록

    }

    void sendEXP()
    {
        // 몬스터가 죽었을 때, 경험치를 보낸다.
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