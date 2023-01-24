using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    // player 정보들
    public GameObject playerMass;
    public GameObject playerSpriteMesh;
    Player playerCode;
    PlayerStats playerStats;
    PlayerSkills playerSkills;
    Skills[] skills;
    StatInformation statInfo;
    SpriteRenderer sprite;

    // 데미지 텍스트 관련
    dmgPool pooling;
    public GameObject dmgPos;

    // 피격 상태, 물리 관련
    public bool isHit;

    Rigidbody2D rigid;

    // Start is called before the first frame update
    void Awake()
    {
        playerCode = transform.parent.GetComponent<Player>();
        playerStats = transform.parent.GetComponent<PlayerStats>();
        skills = transform.parent.GetComponent<PlayerSkills>().skillInfos;
        statInfo = playerStats.playerStat;
        
        sprite = playerSpriteMesh.GetComponent<SpriteRenderer>();
        rigid = transform.parent.GetComponent<Rigidbody2D>();

        pooling = GetComponent<dmgPool>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 8)
        {
            if (!isHit)
            {
                isHit = true;
                int dmg = (collision.gameObject.GetComponent<Enemy>().monsterAtk - statInfo.playerDefense);                  
                // Debug.Log(GetComponentInChildren<BoxCollider2D>().gameObject.name);
                // Debug.Log(rigid.velocity.x);

                StartCoroutine(playerHit(dmg));

            }

        }
        
        if(collision.gameObject.layer == 7)
        {
            
        }

    }

    public IEnumerator playerHit(int dmg)
    {
        sprite.color = Color.gray;

        rigid.AddForce(new Vector2(playerSpriteMesh.transform.localScale.x * (-1) * 200, 5), ForceMode2D.Force); // 캐릭터가 향하고 있는 반대 방향으로 밀쳐진다. (세게는x)
        statInfo.minusOrAddHP((-1) * dmg);
        GameObject imsiDmg = pooling.GetObj(0);
        imsiDmg.GetComponent<dmgSkins>().setDamage(dmg);
        imsiDmg.transform.position = dmgPos.transform.position;

        yield return new WaitForSeconds(0.2f);

        // 공격 중에 밀쳐지게 되면 완전 튕겨져 나가는 것을 방지하기 위해 일정 딜레이 이후 속도를 0으로 설정 해 준다.
        Vector2 go = rigid.velocity; 
        go.x = 0;
        rigid.velocity = go;

        yield return new WaitForSeconds(0.4f);

        sprite.color = Color.white;

        isHit = false;

    }
    
}
