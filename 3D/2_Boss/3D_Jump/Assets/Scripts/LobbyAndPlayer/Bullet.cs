using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isRock;
    public bool isMelee; // 벽이랑 바닥에 총알이 닿았을 때, Destroy를 실행하는데, 만약 근접 Collider랑 벽이랑 닿아서 없어지면 안된다. -> 근접 Collider 여부를 판단하기 위함!
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
        if (!isMelee && !isRock && (other.gameObject.tag == "base" || other.gameObject.tag == "Wall"))
        {
            // 보스 돌이 아니고, 근접공격 Collider가 아니면서, 벽 or 바닥에 닿으면
            Destroy(gameObject); // 제거
        }

    }


}
