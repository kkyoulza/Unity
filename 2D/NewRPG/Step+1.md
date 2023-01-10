# Step + 1. 캐릭터 피격 데미지 표시 및 캐릭터 오브젝트 교통정리


오브젝트 풀을 이용하여 몬스터를 소환하는 작업을 하기 전, 움직이는 몬스터에 대한 세팅, 그리고 플레이어 피격에 대해 먼저 구현하였다.


## - 캐릭터 피격, 움직이는 몬스터 관련 물리 문제

<hr>

캐릭터가 몬스터에게 피격당했을 때, 캐릭터의 체력이 당연히 감소해야 한다.

따라서 Player.cs에 OnTriggerStay2D() 를 넣어 피격시 체력이 깎이게끔 해 주도록 하자

```c#
private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            if (!isHit)
            {
                isHit = true;
                int dmg = (collision.gameObject.GetComponent<Enemy>().monsterAtk - statInfo.playerDefense);
                Debug.Log(collision.gameObject.name);
                Debug.Log(this.name);
                
                StartCoroutine(playerHit(dmg));

            }

        }

    }
    
    
public IEnumerator playerHit(int dmg)
    {
        sprite.color = Color.gray;

        statInfo.playerCntHP -= dmg;
        GameObject imsiDmg = pooling.GetObj(0);
        imsiDmg.GetComponent<dmgSkins>().setDamage(dmg);
        imsiDmg.transform.position = dmgPos.transform.position;

        yield return new WaitForSeconds(1.5f);

        sprite.color = Color.white;

        isHit = false;

    }
 
```

Trigger와 코루틴을 이용하여 캐릭터의 피격시 행동을 구현 했다.

캐릭터는 몬스터에게 피격당했을 때, 순간무적을 주어 가만히 있다가 손 쓸수 없는 연속 공격으로 캐릭터가 죽는 것을 방지하기 위함이다.

무적 시간은 1.5초로 설정하였으며, 코루틴 함수 내에서 데미지 텍스트를 몬스터 공격시와 같이 소환 해 주었음을 볼 수 있다.

이렇게 해 주고, 디버깅 창을 보게 되면

![image](https://user-images.githubusercontent.com/66288087/211146619-13be55ed-c92e-46da-864f-6495f502ab7c.png)

같은 순간에는 한 번만 피격이 일어났음을 볼 수 있다.

<hr>

## - 움직이는 몬스터 물리 문제 해결

고정형 몬스터와는 다르게 움직이는 몬스터는 RigidBody와 Collider가 있어야 한다.

왜냐하면 지형 위를 움직여야 하기 때문이며, 물리가 적용 되어야 하기 때문이다.

물리가 적용 된다면 Collider끼리 밀려나게 된다.

따라서 움직이는 몬스터와 Player간의 충돌이 일어나게 된다.

보통 플랫포머 게임에서는 몬스터와 캐릭터 간의 충돌이 일어나지 않는다.

따라서 충돌을 방지하기 위하여 몬스터의 Layer와 플레이어 Layer간의 물리 적용이 되지 않게 한다.

![image](https://user-images.githubusercontent.com/66288087/211147506-e479cc67-3565-4259-a407-b9419ff5a320.png)

Project Settings -> Physics **2D** 로 들어 가 준다. (2D로!)

그곳에서 Player - Enemy간의 물리 충돌이 일어나지 않게 해 준다.

![image](https://user-images.githubusercontent.com/66288087/211147590-aace22cc-1d0c-429a-8190-4ffaeb152c4f.png)

이렇게 위 사진처럼 플레이어-몬스터 간 길막?이 되지 않게 된다.

<hr>

+ 그렇다면 공격은 어떻게 하나요?

>> 공격은 스킬 오브젝트에서 Layer를 Atk,AtkSkill로 설정하였기에 상관 없다.


<hr>

## - 데미지 스킨 문제 발생(뒤집어져 뜨는 현상)

그런데, 플레이어가 피격을 당하고 나서, 데미지 스킨이 올라가는 도중! 방향을 바꾸게 되면 어떻게 될까?

플레이어 피격시 데미지 텍스트를 소환했을 때, 데미지가 같이 뒤집어져 나오는 현상을 발견하였다.

![image](https://user-images.githubusercontent.com/66288087/211145041-3791fd20-891b-4e25-b584-79c73c20be3d.png)

위 그림과 같이 데미지스킨도 같이 뒤집어져 나오는 현상이 생기게 된 것이다.

이 것에 대한 원인은 데미지 풀에서 **Instantiate**를 할 때, 코드가 적용된 오브젝트의 **자식 오브젝트로 데미지 텍스트를 소환**하였기 때문에, 플레이어 Scale이 뒤집어졌을 때 같이 뒤집어진 것으로 추정된다.

그렇다면 어떻게 해결할 수 있을까?

<hr>

### Solution1. 데미지 풀에서 플레이어 피격시를 구분하여 데미지 텍스트를 자식으로 넣지 않는다.

말 그대로, dmgPool에 새롭게 Player가 피격을 당하고 있는지를 구분하는 bool을 추가하여, true일 때는 어느 곳에도 자식으로 넣지 않아 데미지 텍스트가 뒤집혀 뜨지 않게 해 주는 것이다.

![image](https://user-images.githubusercontent.com/66288087/211145316-3c5d4191-08c3-46dc-b154-a06cd49b555a.png)

하지만 처음 나오는 위치만 캐릭터의 위쪽이고, 데미지가 표기되는 것은 캐릭터를 따라가지 않게 된다.

<hr>

### Solution2. 새롭게 Player 상위 오브젝트를 만들고, 플레이어 Sprite가 적용된 오브젝트와 동등한 계층으로 데미지 텍스트가 뜨게 한다.

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

<hr>

## - 피격 시 캐릭터 밀림 구현

넉백?이라는 말을 들어 봤을 것이다.

몬스터에 피격 당했을 때, 캐릭터는 뒤로 밀려나게 된다.

뒤로 밀려나는 것을 구현 해 보자.

일단, 몬스터에게 피격을 당하는 부분에서 캐릭터 Scale의 반대 방향으로 AddForce를 줘 보도록 하자


```c#
private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 8)
        {
            if (!isHit)
            {
                isHit = true;
                int dmg = (collision.gameObject.GetComponent<Enemy>().monsterAtk - statInfo.playerDefense);                  

                StartCoroutine(playerHit(dmg));

            }

        }

    }

    public IEnumerator playerHit(int dmg)
    {
        sprite.color = Color.gray;

        rigid.AddForce(new Vector2(playerSpriteMesh.transform.localScale.x * (-1) * 20, 1), ForceMode2D.Impulse); 
        // 캐릭터가 향하고 있는 반대 방향으로 밀쳐진다. (세게는x)
        statInfo.minusOrAddHP((-1) * dmg);
        GameObject imsiDmg = pooling.GetObj(0);
        imsiDmg.GetComponent<dmgSkins>().setDamage(dmg);
        imsiDmg.transform.position = dmgPos.transform.position;

        yield return new WaitForSeconds(1.5f);

        sprite.color = Color.white;

        isHit = false;

    }
```

위와 같이 Player에 적용하게 되면 플레이어가 반대 방향으로 밀려나게 된다.

하지만

![image](https://user-images.githubusercontent.com/66288087/211312755-5eea880d-1ea2-44e3-b05f-88f4762f2ffd.png)

위 사진과 같이 몬스터와 겹친 상태로 공격을 하게 되면 엄청 빠르게 멀리까지 튕겨 나가게 됨을 볼 수 있다.

그 원인중에 하나가 **공격 범위도 플레이어로 인식**하여 플레이어가 직접 닿지 않고 공격이 닿아도 밀려나게 되는 것이다.

왜 그런가 곰곰히 따져 보게 되니

공격 범위는 플레이어의 자식에 있었다.

![image](https://user-images.githubusercontent.com/66288087/211315105-56beaa14-13e2-484a-84ed-bac664e11c43.png)

따라서 위 그림처럼 자식에 있는 공격 범위 Trigger도 Player.cs 코드에 있는 피격 코드에 감지되게 되는 것이다.

<hr>

### -> 플레이어 피격 코드 이사

이에, 새롭게 PlayerHit를 감지할 수 있는 코드를 만들어 주어, 전용 Trigger를 만들어 피격을 감지 해 준다.

![image](https://user-images.githubusercontent.com/66288087/211315734-411ca27f-d199-4808-b2d3-5a674f418c80.png)

PlayerSprite와 같은 계층의 오브젝트로 만들었기에, attack의 직계 부모가 아니다.

따라서 attack까지 피격 범위가 되는 불상사는 피하게 되었다.

<hr>

### 문제 ] 몬스터와 비비면서 피격 시 힘이 과도하게 부여되는 현상

그런데, 공격 범위 피격 현상을 해결하여도 계속 몬스터와 비비면서 공격 시, 플레이어가 과도하게 튕겨져 나가는 현상은 고쳐지지 않는 것이었다.

이에, 왜 그럴까 많은 고민을 해 보았다.

결론은 **AddForce에는 힘이 점점 더해지는 것**이기 때문에 **과도하게 가속도가 붙는다**는 추측을 하게 되었다.

따라서 피격 시, 코루틴을 활용하여 짧은 시간 이후, velocity.x를 0으로 만들어 주어 튕겨져 나가는 한계점을 정해 주었다. (최대 속도와 비슷하다고 보면 된다.)

그것을 코드로 작성하게 되면

```c#
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 8)
        {
            if (!isHit)
            {
                isHit = true;
                int dmg = (collision.gameObject.GetComponent<Enemy>().monsterAtk - statInfo.playerDefense);                  
                
                StartCoroutine(playerHit(dmg));

            }

        }

    }

    public IEnumerator playerHit(int dmg)
    {
        sprite.color = Color.gray;

        rigid.AddForce(new Vector2(playerSpriteMesh.transform.localScale.x * (-1) * 200, 5), ForceMode2D.Force); // 캐릭터가 향하고 있는 반대 방향으로 밀쳐진다. (세게는x)
        statInfo.minusOrAddHP((-1) * dmg);
        GameObject imsiDmg = pooling.GetObj(0);
        imsiDmg.GetComponent<dmgSkins>().setDamage(dmg);
        imsiDmg.transform.position = dmgPos.transform.position;

        yield return new WaitForSeconds(0.2f);

        // 공격 중에 밀쳐지게 되면 완전 튕겨져 나가는 것을 방지하기 위해 일정 딜레이 이후 속도를 0으로 설정 해 준다.
        Vector2 go = rigid.velocity; 
        go.x = 0;
        rigid.velocity = go;

        yield return new WaitForSeconds(0.4f);

        sprite.color = Color.white;

        isHit = false;

    }
```

그리고, 한 가지 더 변경점이 있다.

바로 ForceMode2D의 변경이다.

Impulse -> Force로 변경 해 주어 폭발적인 힘에서 연속적인 힘을 주는 방식으로 바꾸었다.

이에, 힘의 크기도 조금 더해 주었다.(폭발 -> 연속이라 힘이 가해지는 정도가 같은 값이면 약해지기 때문)

<hr>

### 움짤

피격시 튕겨나는 부분에 대한 구현 움짤이다.(20230110 - video로 수정하였음)

https://user-images.githubusercontent.com/66288087/211450450-01ec6e2e-70bb-4bd1-8b2d-c7332bd18fa3.mp4

구현 된 것들을 정리하자면

- 피격 시 뒤로 밀려나는 것
- 공격 범위가 닿아도 밀려나지 않게 수정
- 몬스터와 비비면서 공격 시 과도하게 밀려나지 않게 수정(밀리는 범위 한정)

이다.

이제, 다음으로는 움직이는 몬스터의 패턴을 제작 해 보도록 하자.

<hr>

## - 움직이는 몬스터 무빙 로직 제작


우선 로직을 제작하기 전에, 몬스터들은 어떤 행동을 할지 행동 리스트를 먼저 생각 하여 정리 해 보도록 하자.

- 기본적으로 몬스터는 좌, 우로 움직인다. (횡스크롤 게임에서 당연하다)
- 일정 시간마다 움직임 or 대기 상태를 반복한다.
- 앞에 벽이 있거나 떨어지는 막다를 길이 있을 때, 몬스터가 바로 반대 방향으로 이동하게끔 한다.
- 몬스터가 피격을 당할 때, 공격 방향으로 밀려나가게 되며, 그로 인하여 바닥으로 떨어질 수 있다. (떨어 진 다음에 움직이는 건 동일 로직을 사용한다.)
- (추적 몬스터일 경우) 일정 범위 안에 들어가게 되면 캐릭터를 추적하게 되며, 추적 대상 플레이어가 사라지게 되면 다시 배회하는 로직을 실행한다.


```c#

void FixedUpdate(){
    
    if(움직이는 상태 - 좌){
        
        최대속도 제한과 함께 AddForce를 해 준다.
       
    }else if(움직이는 상태 - 우){
        
        최대 속도 제한과 함께 AddForce를 해 준다.
        
    }
    
    if(Ray를 쏴서 만약에 벽 or 가파른 곳 까지 갔을 때){
        
        반대 방향이동으로 상태 변경
        
    }
    

}

public IEnumerator setState(){
    
    랜덤 값 생성
    
    if(case 1)
        오른쪽 이동
    else if(case 2)
        왼쪽 이동
    else
        Idle 상태
        
    yield return new WaitForSeconds(3.0f);
    
    상태 세팅(bool 값 on/off)
    
    yield return new WaitForSeconds(3.0f);
    
    StartCoroutine(setState()); // 재귀 형식으로 다시 상태 설정

}

```

위 코드는 한글을 적어서 짜 본 의사코드이다.

코루틴에서 몬스터의 행동 상태를 결정하여 FixedUpdate()를 통해 행동하게 만들어 준다.

그리고, 행동 세팅을 한 뒤 일정 시간 이후 다시 코루틴을 활용하여 다시 행동을 세팅하게 만들어 줄 계획이다.
















