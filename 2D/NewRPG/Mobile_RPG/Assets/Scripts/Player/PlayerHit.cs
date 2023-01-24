using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    // player ������
    public GameObject playerMass;
    public GameObject playerSpriteMesh;
    Player playerCode;
    PlayerStats playerStats;
    PlayerSkills playerSkills;
    Skills[] skills;
    StatInformation statInfo;
    SpriteRenderer sprite;

    // ������ �ؽ�Ʈ ����
    dmgPool pooling;
    public GameObject dmgPos;

    // �ǰ� ����, ���� ����
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

        rigid.AddForce(new Vector2(playerSpriteMesh.transform.localScale.x * (-1) * 200, 5), ForceMode2D.Force); // ĳ���Ͱ� ���ϰ� �ִ� �ݴ� �������� ��������. (���Դ�x)
        statInfo.minusOrAddHP((-1) * dmg);
        GameObject imsiDmg = pooling.GetObj(0);
        imsiDmg.GetComponent<dmgSkins>().setDamage(dmg);
        imsiDmg.transform.position = dmgPos.transform.position;

        yield return new WaitForSeconds(0.2f);

        // ���� �߿� �������� �Ǹ� ���� ƨ���� ������ ���� �����ϱ� ���� ���� ������ ���� �ӵ��� 0���� ���� �� �ش�.
        Vector2 go = rigid.velocity; 
        go.x = 0;
        rigid.velocity = go;

        yield return new WaitForSeconds(0.4f);

        sprite.color = Color.white;

        isHit = false;

    }
    
}
