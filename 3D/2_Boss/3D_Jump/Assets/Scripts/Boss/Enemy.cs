using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // ���� Ÿ�� ����
    public enum Type { A,B,C,Boss }; // ������ ������ �����.
    public Type enemyType; // ���� Ÿ���� ���� ����

    // ü�� ����
    public int maxHealth;
    public int cntHealth;

    // ������ ����
    public GameObject PosObj; // ������ ���� ��ġ�� �ִ� �� ������Ʈ
    GameObject Damage_Prefab; // ������ ������
    GameObject Damage; // ������

    Vector3 dmgPos; // ������ ��ġ

    // ���� ����
    protected Rigidbody rigid;
    public BoxCollider meleeArea; // ���� ������ ���� ����
    public BoxCollider boxCollider; // �Ѹ� collider?

    // ���Ÿ� ���� ����
    public GameObject monsterMissile; // ���� �̻��� �������� ���� ����

    // ���� ����
    public bool isAttack; // ������ �ϰ� �ִ°�?
    public bool isDead; // ���� �����ΰ�?

    // �Ѻ���
    protected Material mat;

    // ���� ����
    public bool isChase; // ������ ������ ��Ȳ!
    public Transform target; // ���� ���
    protected NavMeshAgent navi; // UnityEngine.AI�� �ʼ��� �� ��

    //�ִϸ��̼�
    protected Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<MeshRenderer>().material; // material�� �������� ���!!
        navi = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        Invoke("ChaseStart", 2.0f);

    }
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (navi.enabled)
        {// navi�� Ȱ��ȭ �Ǿ��� ���� ��ǥ�� ����! (�������� ��ǥ�� �Ҿ������ �����̱�� �ϱ� ������ �������� �ϴ� ������ �� �ش�!)
            navi.SetDestination(target.position);
            navi.isStopped = !isChase; // ������ �ϰ� ���� ������(false) ������ �ϰ�(!false = true), ������ �� �� ���ߴ� ���� ���߰�(�����̰�) �Ѵ�.
        }
            
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity(); // ȸ�� �ӵ� 0���� ����!
    }

    void Targeting()
    {
        // ������ �ϱ� ���� Ÿ�� ����
        float targetRadius = 0f;
        float targetRange = 0f;

        if (!isDead && enemyType != Type.Boss) // ���� ���°� �ƴϰ�, ������ �ƴ� ���� Ÿ������ ����
        {
            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3.0f;
                    break;
                case Type.B:
                    targetRadius = 1f; // Ÿ���� ã�� �β� (Ƽ���丮 ����)
                    targetRange = 12.0f; // �÷��̾� Ÿ���� ����
                    break;
                case Type.C: // ���Ÿ��� Ÿ������ �а� ��Ȯ�ؾ� �Ѵ�.
                    targetRadius = 0.5f;
                    targetRange = 25.0f; // �÷��̾� Ÿ���� ����
                    break;
            }
        }
        

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        // �ڽ��� ��ġ, ��ü ������, ���ư��� ����(��� �������� �� ���ΰ�?), �Ÿ�, ��� ���̾�
        
        if(rayHits.Length > 0 && !isAttack)
        {
            // �÷��̾ ������ ���̴� ���� �����ʰ� ���ÿ� ���� ���� �ƴ϶��!
            StartCoroutine("Attack"); // ����!

        }
            

    }

    IEnumerator Attack()
    {
        // �Ϲ����� ���ʹ� ��� ���� ��, �����ϰ� �ٽ� �Ѿư��� ��������!

        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.5f); // �ִϸ��̼� ���۵��� ������!

                meleeArea.enabled = true; // �� �ڿ� �ڽ� Ȱ��ȭ�� �Ͽ� ����!

                yield return new WaitForSeconds(0.3f); // ���� �ڽ��� Ȱ��ȭ �� �ð�

                meleeArea.enabled = false;

                yield return new WaitForSeconds(0.8f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f); // �� ������
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse); // �ﰢ���� ������ ����!
                meleeArea.enabled = true; // �����ϴ� ���� �ڽ��� Ȱ��ȭ!

                yield return new WaitForSeconds(0.5f); // ���� �ڽ��� Ȱ��ȭ �� �ð�
                rigid.velocity = Vector3.zero; // ���� �ð� ���� �� ����!
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2.0f); // �� ������
                break;
            case Type.C: // �̻����� ������ �Ѵ�.
                yield return new WaitForSeconds(0.4f); // �� ������

                GameObject instantBullet = Instantiate(monsterMissile, transform.position,transform.rotation); // ���Ϳ� ���� ��ġ�� �̻��� ����
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20; // �Ѿ˿� �ӵ��� �ο�

                yield return new WaitForSeconds(2.0f); // �� ������
                break;
        }


        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }


    void ChaseStart()
    {
        isChase = true; // ������ �����ϰ� �ϰ�
        anim.SetBool("isWalk", true); // �ִϸ��̼� ���¸� ����!
    }

    void FreezeVelocity() // �÷��̾�� �浹 �� ���󰡼� ������ ���� ���ϴ� ��Ȳ ����
    {
        if (isChase) // �������� ���� ����!
        {
            rigid.velocity = Vector3.zero; // �ӵ� 0
            rigid.angularVelocity = Vector3.zero; // ȸ�� �ӵ� 0
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Damage_Prefab = Resources.Load("Prefabs/LobbyAndRPG/Damage") as GameObject; // ������ �������� GameObject�� ���� �´�.

            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            cntHealth -= weapon.Damage;

            Damage = MonoBehaviour.Instantiate(Damage_Prefab);
            Damage.GetComponent<Damage>().damage = weapon.Damage; // ������Ʈ �� ������ ������Ʈ�� �ִ� ������ ���� ����
            Damage.transform.position = PosObj.transform.position;

            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));

        }
        else if (other.tag == "Bullet")
        {
            Damage_Prefab = Resources.Load("Prefabs/LobbyAndRPG/Damage") as GameObject; // ������ �������� GameObject�� ���� �´�.

            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            cntHealth -= bullet.damage;

            Damage = MonoBehaviour.Instantiate(Damage_Prefab);
            Damage.GetComponent<Damage>().damage = bullet.damage; // ������Ʈ �� ������ ������Ʈ�� �ִ� ������ ���� ����
            Damage.transform.position = PosObj.transform.position;

            Vector3 reactVec = transform.position - other.transform.position;

            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }

    }
    

    IEnumerator OnDamage(Vector3 reactVec) // �ǰݽ� ���� ����
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(cntHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {        
            mat.color = Color.gray;

            gameObject.layer = 7; // rayCast������ �޸� ���ڷ� �׳� ���´�.
            isDead = true;
            isChase = false;
            if(enemyType == Type.A || enemyType == Type.B)
                meleeArea.enabled = false;
            navi.enabled = false;
            anim.SetTrigger("DoDie");

            reactVec = reactVec.normalized; // ���Ͱ� ���� �� ��¦ �� ������ �״� ����� �����ϱ� ����
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 10, ForceMode.Impulse);

            

            Destroy(gameObject,2); // 2�� �ڿ� Destroy!
        }

    }


}
