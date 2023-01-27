using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Rigidbody2D rigid;
    CircleCollider2D colliderPhysics;

    public enum ItemType { coin, useItem, weapon, clothe };
    public ItemType type;

    public int addGold;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        colliderPhysics = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log(collision.gameObject.name);
            switch (type)
            {
                case ItemType.coin:
                    collision.gameObject.GetComponent<PlayerItem>().addGold(addGold);
                    Debug.Log("add Bronze Coin!");
                    Destroy(gameObject);
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Platform")
        {
            rigid.isKinematic = true;
            if(colliderPhysics != null)
            {
                colliderPhysics.enabled = false;
            }
        }

    }


}
