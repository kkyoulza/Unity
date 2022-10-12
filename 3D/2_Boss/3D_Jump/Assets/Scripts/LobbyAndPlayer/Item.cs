using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Armor, Coin, Weapon }; // 열거형 타입
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        // 같은 게 있을 경우 첫 번째 것만 가져온다. 따라서 물리 담당을 하는 것이 맨 위에 있어야 한다.
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
