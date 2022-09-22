using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int cntHealth;

    public GameObject PosObj; // ������ ���� ��ġ�� �ִ� �� ������Ʈ
    GameObject Damage_Prefab; // ������ ������
    GameObject Damage; // ������

    Vector3 dmgPos; // ������ ��ġ

    Rigidbody rigid;
    BoxCollider boxCollider;

    Material mat;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material; // material�� �������� ���!!
    }
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Damage_Prefab = Resources.Load("Prefabs/LobbyAndRPG/Damage") as GameObject; // ������ �������� GameObject�� ���� �´�.

            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            cntHealth -= weapon.Damage;

            Damage = MonoBehaviour.Instantiate(Damage_Prefab);
            Damage.GetComponent<Damage>().damage = 25; // ������Ʈ �� ������ ������Ʈ�� �ִ� ������ ���� ����
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

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 10, ForceMode.Impulse);

            gameObject.layer = 7; // rayCast������ �޸� ���ڷ� �׳� ���´�.

            Destroy(gameObject,2); // 2�� �ڿ� Destroy!
        }

    }


}
