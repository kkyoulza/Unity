using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isRock;

    public void SetDamage(int setDmg)
    {
        this.damage = setDmg;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isRock && collision.gameObject.tag == "base")
        {
            Destroy(gameObject, 3);
        }
        else if(collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRock && (other.gameObject.tag == "base" || other.gameObject.tag == "Wall"))
        {
            Destroy(gameObject);
        }

    }


}
