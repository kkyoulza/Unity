using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // ü�� ����
    public int maxHealth;
    public int cntHealth;

    // ������ ����
    public GameObject PosObj; // ������ ���� ��ġ�� �ִ� �� ������Ʈ
    GameObject Damage_Prefab; // ������ ������
    GameObject Damage; // ������

    Vector3 dmgPos; // ������ ��ġ

    // ���� ����
    Rigidbody rigid;
    public BoxCollider meleeArea; // ���� ������ ���� ����

    // ���� ����
    public bool isAttack; // ������ �ϰ� �ִ°�?

    // �Ѻ���
    Material mat;

    // ���� ����
    public bool isChase; // ������ ������ ��Ȳ!
    public Transform target; // ���� ���
    NavMeshAgent navi; // UnityEngine.AI�� �ʼ��� �� ��

    //�ִϸ��̼�
    Animator anim;

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
        float targetRadius = 1.5f;
        float targetRange = 3.0f;

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

        yield return new WaitForSeconds(0.5f); // �ִϸ��̼� ���۵��� ������!

        meleeArea.enabled = true; // �� �ڿ� �ڽ� Ȱ��ȭ�� �Ͽ� ����!

        yield return new WaitForSeconds(1.5f);

        meleeArea.enabled = false;
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
            isChase = false;
            meleeArea.enabled = false;
            navi.enabled = false;
            anim.SetTrigger("DoDie");

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 10, ForceMode.Impulse);

            

            Destroy(gameObject,2); // 2�� �ڿ� Destroy!
        }

    }


}
