# Step+ 3. 몬스터 퇴치 시 보상 드랍

몬스터를 퇴치했으면 보상을 드랍해야 한다.

몬스터는 앞서 구현하였지만, 보상을 구현하지 않아 외전 느낌으로 Step+에서 구현 하고자 한다.

우선, 아이템 보상도 오브젝트이며, 만약에 아이템을 구현한다면, 아이템이 높은 빈도로 출현 할 것이라는 사실을 인지해야 한다.

그렇다면 어떻게 해야 할까?

<br>
.
<br>
.
<br>
.

바로 **오브젝트 풀링**을 이용하는 것이다!

<hr>

## 몬스터 퇴치 시 보상 드랍 로직 제작

우선, 오브젝트 풀링을 사용하지 않고 몬스터를 퇴치하면 보상이 나오는 시스템을 만들어 보도록 하자.

**Enemy.cs**

```c#
void DropItems()
{
    int ran = Random.Range(0, 2); // 0 ~ 1

    switch (ran)
    {
        case 0:
            Instantiate(bronzeCoin,transform.position,transform.rotation);
            break;
        case 1:
            Instantiate(bronzeCoin, transform.position, transform.rotation);
            Instantiate(bronzeCoin, transform.position, transform.rotation);
            break;

    }

}
```

Enemy.cs 코드에 보상을 드랍하는 함수를 하나 추가 해 주었다.

간단하게 랜덤 함수를 사용하여 랜덤 수를 추가 해 주었다. 0이면 코인 1개, 1이면 코인 2개를 드랍하게 하였다.

실행 해 보자

![image](https://user-images.githubusercontent.com/66288087/214777974-cf3708df-1104-48ea-b61c-0bb02a733842.png)

코인을 1개 드랍했다.

<hr>

### 아이템 코드 설정

이제 플레이어가 아이템을 얻기 위해 아이템에 코드를 추가 해 보자

**Item.cs**

```c#
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
```

아이템 코드는 범용적으로 사용할 수 있도록 enum으로 아이템 타입을 설정 해 주었다.

그리고 OnTriggerEnter2D를 만들어 만약 Player가 아이템에 닿으면 아이템 타입에 따라서 다음 행동을 하게끔 해 주었다.

코인은 플레이어 소지 코인을 설정 된 만큼 늘려주게 된다.

그리고, 코인이 떨어지는 효과를 주면서, 플레이어와 충돌하지 않게끔 하기 위해 CollisionEnter2D를 이용하여 땅에 닿게 되면 RigidBody 세팅을 dynamic에서 Kinematic으로 바꾸어 주고, Collider를 비활성화 해 주면 플레이어와 충돌하지 않으면서 땅에 떨어지는 효과도 줄 수 있게 된다.

<hr>

**-> Physics2D 세팅에서 Player와 Item이 상호작용을 못하게 하면 되지 않나요?**

--> 그러면 코인 획득도 못하기 때문에 안됩니다!

<hr>

![image](https://user-images.githubusercontent.com/66288087/214781670-1ec5b4bc-2013-4445-a844-4fca2192172d.png)

코인에 대한 Collider 설정을 해 준다.

밖에 있는 큰 원이 Trigger이며, 안에 있는 원이 Collider이다.

<hr>

### 테스트

테스트를 해 보면

![image](https://user-images.githubusercontent.com/66288087/214781862-31dbffce-b6f8-448c-8d22-4baa70de1954.png)

위 사진처럼 Log가 기록 되었으며,

![image](https://user-images.githubusercontent.com/66288087/214781962-00205475-8344-4f8d-98fb-666632e6b6a4.png)

두 개를 먹었기 때문에 20코인이 추가 되었음을 볼 수 있다.

<hr>

## 오브젝트 풀링 적용?







