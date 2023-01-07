# Step + 1. 캐릭터 피격 데미지 표시 및 캐릭터 오브젝트 교통정리

오브젝트 풀로 몬스터를 소환하는 시스템을 구현하기 전에, 몬스터에 의한 플레이어 몸박 피격을 구현하는 도중 

플레이어 피격시 데미지 텍스트를 소환했을 때, 데미지가 같이 뒤집어져 나오는 현상을 발견하였다.

![image](https://user-images.githubusercontent.com/66288087/211145041-3791fd20-891b-4e25-b584-79c73c20be3d.png)

위 그림과 같이 데미지스킨도 같이 뒤집어져 나오는 현상이 생기게 된 것이다.

이 것에 대한 원인은 데미지 풀에서 **Instantiate**를 할 때, 코드가 적용된 오브젝트의 **자식 오브젝트로 데미지 텍스트를 소환**하였기 때문에, 플레이어 Scale이 뒤집어졌을 때 같이 뒤집어진 것으로 추정된다.

그렇다면 어떻게 해결할 수 있을까?

<hr>

## Solution1. 데미지 풀에서 플레이어 피격시를 구분하여 데미지 텍스트를 자식으로 넣지 않는다.

말 그대로, dmgPool에 새롭게 Player가 피격을 당하고 있는지를 구분하는 bool을 추가하여, true일 때는 어느 곳에도 자식으로 넣지 않아 데미지 텍스트가 뒤집혀 뜨지 않게 해 주는 것이다.

![image](https://user-images.githubusercontent.com/66288087/211145316-3c5d4191-08c3-46dc-b154-a06cd49b555a.png)

하지만 처음 나오는 위치만 캐릭터의 위쪽이고, 데미지가 표기되는 것은 캐릭터를 따라가지 않게 된다.

<hr>

## Solution2. 새롭게 Player 상위 오브젝트를 만들고, 플레이어 Sprite가 적용된 오브젝트와 동등한 계층으로 데미지 텍스트가 뜨게 한다.

따라서, 캐릭터가 뒤집히는 오브젝트와, 그렇지 않은 것들을 별개로 만들어 주는 방법을 택하였다.

### 플레이어 Mass 생성

즉, 플레이어의 기능을 하는 것들을 모아 둔 PlayerMass를 하나 만들어 주어, Tag,Layer를 Player로 설정하여 여태 플레이어 오브젝트가 했던 것들을 위임 해 주었다.

RigidBody2D, Collider2D, Player 코드들을 다 옮겨 주었지만 애니메이션 만큼은 그대로 두었다.

대신, Player.cs 코드 중에서 Animator가 들어가는 코드를 아래와 같이 바꾸어 주었다.

```c#
void Awake()
    {
        playerSkill = GetComponent<PlayerSkills>();
        statInfo = GetComponent<PlayerStats>().playerStat;
        skillInfos = playerSkill.skillInfos;

        rigid = GetComponent<Rigidbody2D>();

        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        pooling = GetComponent<dmgPool>();
        jumpCount = statInfo.playerMaxJumpCount;
    }
```
GetComponentInChildren을 통하여 해당 오브젝트 바로 자식 오브젝트가 가진 컴포넌트도 가져올 수 있다.

Sprite 역시 자식 것을 가져 와 주어야 한다.

**보여지는 것을 분리** 한 것이기 때문에 자식에 남겨 놓아야 하기 때문이다.

<br>

### 플레이어 Sprite 부분 수정

플레이어의 겉 모습이 뒤집어지는 checkSprite() 함수를 수정 해 주도록 하자.

Player의 실제 Sprite가 들어가는 GameObject를 하나 만들어 준 뒤, 해당 GameObject의 Scale을 변경 해 주는 방향으로 바꾸어 주면 된다.

```c#

pubilc GameObject playerMesh;

void checkSprite()
    {
        // 캐릭터의 좌/우 반전 설정
        if(Input.GetButton("Horizontal") || directionValue != 0)
        {
            // 좌, 우로 이동할 때
            // sprite.flipX = (Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1); // 왼쪽일 때 바꾸어 주어야 하니 왼쪽으로 이동할 때 맞는 조건을 넣어 주었다.

            if (!isAttack) // 공격 이펙트가 진행되지 않을 때에만 방향 전환이 가능하게!!
            {
                if ((Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1)) // 위 조건을 똑같이 써 주어, 왼쪽을 볼 때는 scale에 (-1,1,1)벡터를 넣어 준다. (-1,0,0)넣으면 캐릭터가 사라진다.
                {
                    playerMesh.transform.localScale = new Vector3(-1, 1, 1); // scale의 x 좌표를 -1로 바꾸어 주면 scale에 들어가는 것이 벡터이니 방향이 바뀌게 된다.
                }
                else
                {
                    playerMesh.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else // 공격 중일 때는 방향전환 X
            {
                return;
            }
            
        }
    }
```

보여지는 것을 수정했으니 피격을 당해 보도록 하자

![image](https://user-images.githubusercontent.com/66288087/211145827-1a945247-a4ec-45bb-bc12-ddf18936c918.png)

캐릭터가 뒤집어 졌지만(Scale.x = -1), 데미지 텍스트는 똑바로 있음을 볼 수 있다.

<br>

![image](https://user-images.githubusercontent.com/66288087/211145866-aef40098-97c3-49d7-9577-d3c20a345569.png)

캐릭터의 계층 구조를 보도록 해 보자

PlayerHitDmg가 체크 표시를 해 둔 기존의 Player오브젝트 아래에 뜨지 않고, 동일한 레벨의 계층에서 뜨게 됨을 볼 수 있다.

![image](https://user-images.githubusercontent.com/66288087/211145951-f5aea244-7916-481f-8b1d-7dd658004ad2.png)

<br>

![image](https://user-images.githubusercontent.com/66288087/211145972-1fdcd13f-318e-4c5a-96bb-f0fcc1a2b5bd.png)

PlayerSprite(전 Player)만이 좌, 우 반전 시 Scale.x가 -1이 된다.




