using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Armor, Coin, Weapon }; // ������ Ÿ��
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        // ���� �� ���� ��� ù ��° �͸� �����´�. ���� ���� ����� �ϴ� ���� �� ���� �־�� �Ѵ�.
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "base")
        {
            rigid.isKinematic = true;
            if(sphereCollider != null)
                sphereCollider.enabled = false;
        }
    }

}
