# Step2. 캐릭터 공격, 몬스터 피격 구현

### 캐릭터 공격

#### 공격 모습

캐릭터의 공격을 구현 하도록 하겠다.

캐릭터의 공격 모션이 따로 없으니 일단 휘두르는 모션?을 급하게 도트로 찍어 Sprite Sheet로 만들어 준다.

![image](https://user-images.githubusercontent.com/66288087/210128740-1c928e34-2889-46f5-ae79-ab1c258fb9fb.png)

위 사진과 같이 Sheet를 만들어 주고, 캐릭터의 자식 오브젝트로 공격 포인트를 만들어 주어, 공격 포인트의 하위 오브젝트로 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/210128760-dcd17383-3d2e-4acd-87b4-db39b2a49346.png)

캐릭터 계층

![image](https://user-images.githubusercontent.com/66288087/210128780-a36d59bd-0cc8-4b76-ae7a-8439119dd197.png)

AtkPoint 위치

<hr>

#### 공격 버튼 설정

이제, 공격 키를 누르게 되면 공격 애니메이션이 나가게끔 설정 해 주겠다.

아래에 Player.cs에서 키 입력과 공격 발동 함수를 가져왔다.

<pre>
<code>
void getKeys()
{
    jumpKey = Input.GetKeyDown(KeyCode.LeftAlt); // 왼쪽 Alt 키를 통해 점프를 할 수 있음
    attackKey = Input.GetKeyDown(KeyCode.LeftControl); // 왼쪽 Control 키를 통해 공격한다. 
}

public void normalAttack()
{
    if (attackKey && !skillInfos[0].isUse) // 공격중이지 않으면서 공격 키가 눌렸을 때
    {
        skillInfos[0].isUse = true;
        StartCoroutine(playerSkill.ableAtkSkill(0, 0.6f,statInfo.afterDelay));
    }
}
</code>
</pre>

왼쪽 컨트롤 키를 누르게 되면 attackKey가 true가 되며, 공격 함수에서 attack키가 입력되었고, 공격 중이지 않을 때, 공격이 발동되게 해 주었다.

그런데 함수에 보면 skillInfos라는 배열이 보일 것이다.

바로, 공격과 스킬에 대한 정보들을 담고 있는 객체들의 배열이며 일반 공격 역시 스킬로 취급하여 한 번에 관리하고자 하는 목적으로 사용하였다.

<hr>

#### PlayerSkills.cs

RPG에서 플레이어의 스킬 사용은 필수적이다.

따라서 플레이어가 가지고 있는 스킬들을 관리하기 위한 스킬 정보 객체를 만들어 주었으며, 그것을 배열로 만들어 준 것이 skillInfos이다.

PlayerSkills.cs 코드는 아래와 같다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

[System.Serializable]
public class Skills
{
    public bool isGain; // 스킬 획득 여부
    public bool isUse; // 스킬 사용중 여부

    public PlayerSkills.classType skillClass; // 어느 직업 전용 스킬인가?
    public int skillLevel; // 해당 스킬의 강화 레벨

    public int atkCnt; // 스킬의 공격 횟수
    public float skillDmg; // 스킬의 데미지
    
    public GameObject skillObj; // 스킬의 오브젝트
    public string animTrigger; // 애니메이션 실행 트리거 이름
}

public class PlayerSkills : MonoBehaviour
{
    public Skills[] skillInfos;
    Player playerBase;

    public enum classType {common, warrior, magician};

    // Start is called before the first frame update
    void Awake()
    {
        playerBase = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ableAtkSkill(int skillIndex,float skillDuration,float afterDelay) // 스킬 발동 (평타도 스킬로 취급)
    {
        Animator anim;
        skillInfos[skillIndex].skillObj.SetActive(true);
        anim = skillInfos[skillIndex].skillObj.GetComponent<Animator>();
        anim.SetTrigger(skillInfos[skillIndex].animTrigger);

        yield return new WaitForSeconds(skillDuration);

        skillInfos[skillIndex].skillObj.SetActive(false);

        yield return new WaitForSeconds(afterDelay); // 후 딜레이

        skillInfos[skillIndex].isUse = false;
        
    } 

}
</code>
</pre>

우선 스킬들에 대한 정보 저장도 필수이니 Skills 라는 클래스를 만들어 주었으며, Skills 클래스 형태의 객체들을 저장하기 위한 배열인 skillInfos를 만들어 주었다.

스킬 클래스에는
<br>
- 획득 여부
- 사용 중 여부
- 어느 직업의 스킬인가?
- 해당 스킬을 얼마나 강화 하였는가?
- 스킬의 공격 횟수는 몇 회인가?
- 스킬의 각 공격당 데미지는 얼마인가?
- 스킬 오브젝트(스킬 이펙트)를 저장할 수 있는 박스 (스킬을 사용할 때만 활성화 해 주어야 하기 때문)
- 스킬 이펙트의 애니메이션 실행 트리거 이름(스킬을 사용할 때 이펙트가 실행되어야 하기 때문)
<br>
이러한 정보들이 저장되어 있다.

일반 공격 역시 이러한 정보의 형태로 저장 해 두어 스킬과 같이 관리할 수 있게 해 주었다.

그리고 아래 보면 공격 코루틴이 있음을 볼 수 있다.

공격 코루틴의 매개변수에는 **스킬 인덱스, 스킬 이펙트 애니메이션 딜레이, 스킬 시전 후 딜레이**가 있다.

애니메이션 딜레이도 클래스 안에 넣는 것이 나을 것 같기도 한데, 일단은 사용하고 있다.

스킬 후 딜레이는 플레이어가 가진 스텟을 사용하게 된다.

<br>

여기서 잠시 플레이어 스탯 클래스를 가져와 보면 (PlayerStats.cs)

<pre>
<code>
[System.Serializable]
public class StatInformation
{
    public float playerMaxSpeed;
    public float playerJumpPower;
    public int playerMaxJumpCount;
    public int playerHP;
    public int playerMP;

    public int playerStrength;
    public int playerIntelligence;
    public int playerDefense;
    public int playerDodge;

    public float afterDelay;

    public StatInformation()
    {
        playerMaxSpeed = 3f;
        playerJumpPower = 5f;
        playerMaxJumpCount = 2;
        playerHP = 50;
        playerMP = 10;

        playerStrength = 10;
        playerIntelligence = 5;
        playerDefense = 3;
        playerDodge = 1;

        afterDelay = 0.2f;
    }

}
</code>
</pre>

<br>

- 플레이어 최대 속도
- 플레이어 점프 파워
- 플레이어 최대 점프 횟수
- 플레이어 체력, 마나
- 플레이어 힘, 지능, 방어력, 회피력
- 플레이어 공격 후 딜레이

<br>

이렇게 여러 종류의 스탯이 존재한다.

(다 사용할 수 있는 날이 오겠지..?)

<hr>

#### 공격 애니메이션

공격 애니메이션은 위에서 만든 공격 이펙트 Sprite Sheet를 묶어서 Scene으로 드래그 앤 드랍을 해 주게 되면 자동으로 오브젝트와 애니메이터가 생기게 된다.

따라서 해당 오브젝트를 AtkPoint 하위에 넣어 준 다음, 비활성화를 해 준다.

위에서 봤던 코드에서 공격 시에 해당 오브젝트를 활성화 해 준 다음, 애니메이션 트리거를 발동시켜 준다.

![image](https://user-images.githubusercontent.com/66288087/210129285-4eeba6ab-b476-4f6c-89a4-ed986607eaec.png)

공격 애니메이터 내부이다.

빈 애니메이션을 만들어서 기본 상태로 설정 해 주고, Ant State에서 트리거를 조건으로 공격 이펙트가 발동되게 해 주었다.

(Exit로 빠져 나가게 해 주어야 한다.)

Any State는 어떤 상태에서도 해당 애니메이션으로 돌입할 수 있게 해 준다.

따라서 Idle 상태에서 공격모션으로 돌입하는 것 보다 더 부드럽게 공격이 진행된다.

<hr>

#### 공격 버튼 UI

버튼 위치도 살짝 바꿔 주었다.

점프 버튼을 줄이고 공격 버튼을 추가 해 주었다.

![image](https://user-images.githubusercontent.com/66288087/210129347-4412a498-0e21-4c78-88fc-ba761f303adf.png)

<pre>
<code>
public void GetButtonDown(string whatBtn)
{
    switch (whatBtn)
    {
        case "L":
            directionValue = -1;
            break;
        case "R":
            directionValue = 1;
            break;
        case "Jump":
            jumpKey = true;
            tojump();
            break;
        case "Attack":
            attackKey = true;
            normalAttack();
            break;
    }
}

public void GetButtonUp(string whatBtn)
{
    switch (whatBtn)
    {
        case "L":
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y); // 버튼을 뗐을 때 속도를 줄여야 한다.(미끄러지지 않게)
            directionValue = 0;
            break;
        case "R":                
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);
            directionValue = 0;
            break;
        case "Jump":
            jumpKey = false;
            break;
        case "Attack":
            attackKey = false;
            break;
    }
}
</code>
</pre>

버튼에 대한 코드이다.

점프와 같이 attackKey를 true로 해 주고, 공격 함수도 발동시켜 준다.

이제 공격을 해 보면..

<hr>

#### 캐릭터 좌/우 반전 방식 변경

![image](https://user-images.githubusercontent.com/66288087/210129465-205d0064-a82b-4ad1-bafe-c45ce6de333e.png)

위 사진처럼 반대 방향을 보고 있는데 백 어택?을 하고 있다..

왜냐하면 캐릭터의 좌/우 반전 방식을 SpriteRenderer의 FlipX를 on/off하는 것으로 겉 보기만 바꿔 주었기 때문이다.

이렇게 하면 자식 오브젝트들의 위치는 바뀌지 않게 되어 위 사진과 같이 나오게 되는 것이다.

그렇다면 어떻게 해 주어야 할까?

답은 바로 localScale에 있다.

아래 사진과 같이 **Scale의 x 값을 -1**로 해 주게 되면

![image](https://user-images.githubusercontent.com/66288087/210129527-00e5b51b-b9a7-4b74-9749-c8aee90bbbcb.png)

오브젝트의 방향이 바뀌게 된다.

FlipX와 다른 점이 있다면

![image](https://user-images.githubusercontent.com/66288087/210129543-a07a0ee1-1771-45d7-91f7-bd1d6d154eed.png)

**자식 오브젝트들의 위치도 대칭이동** 된다는 것이다.

아래와 같이 Player.cs의 checkSprite() 함수를 바꾸어 준다.

<pre>
<code>
void checkSprite()
{
    // 캐릭터의 좌/우 반전 설정
    if(Input.GetButton("Horizontal") || directionValue != 0)
    {
        // 좌, 우로 이동할 때
        // sprite.flipX = (Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1); // 왼쪽일 때 바꾸어 주어야 하니 왼쪽으로 이동할 때 맞는 조건을 넣어 주었다.

        if((Input.GetAxisRaw("Horizontal") == -1) || (directionValue == -1)) // 위 조건을 똑같이 써 주어, 왼쪽을 볼 때는 scale에 (-1,1,1)벡터를 넣어 준다. (-1,0,0)넣으면 캐릭터가 사라진다.
        {
            transform.localScale = new Vector3(-1, 1, 1); // scale의 x 좌표를 -1로 바꾸어 주면 scale에 들어가는 것이 벡터이니 방향이 바뀌게 된다.
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

    }
}
</code>
</pre>

<hr>

### 왜 그럴까?

아래 이야기 하는 이유는 추측이다.

코드 상으로 Scale 값을 바꾸려면 Vector3값을 넣어서 바꾸어 주어야 한다.

Scale은 크기를 나타내는 것인데, Vector 값은 기본적으로 크기와 **방향**을 가지고 있다.

따라서 절대값을 같게 하고, 부호를 바꾸어 주면 해당 축에서의 **방향을 바꾸어 주는 것으로 추측**된다.

SpriteRenderer에서 FlipX는 이미지 소스만을 뒤집어 주는 것에 반해, Scale은 오브젝트 자체에 영향을 주는 것이기에 자식 오브젝트도 당연히 영향을 받게 된다.

따라서.. **Scale값을 Vector3(-1,1,1)로 바꾸어 주게 되면 자식 오브젝트에 있는 공격 이펙트도 뒤집어지게 된다.**

테스트 해 보면

![image](https://user-images.githubusercontent.com/66288087/210129682-267fabbb-bd88-4a48-9dd6-43516a153c96.png)

**Good!** 잘 된다!

<hr>

#### 좌/우 이동 버튼 입력 방식 변경

여기까지 해서 모바일 어플로 추출하여 테스트를 해 보았다.

그런데, 한 가지 찝찝한 사항이 생겼다.

바로 좌/우 버튼 입력 방식이 별로인 것이다.

좌로 이동하다가 우측으로 이동할 때, 왼쪽 방향키를 떼었다가 다시 오른쪽 방향키를 눌러야 한다는 것이다.

모바일 게임을 할 때, 이것도 매우 예민한 사항이다.

모바일 게임들은 **그냥 왼쪽 방향키 버튼을 누르면서 오른쪽 버튼으로 손가락을 이동**하게 되면 **자연스레 오른쪽으로 이동**하게 된다.

그렇게 바꾸어 주도록 해 보자

<br>

![image](https://user-images.githubusercontent.com/66288087/210129778-19c32fd5-cbfe-4945-b453-fe54443f00ff.png)

현재 적용되고 있는 이벤트는 Pointer Down/Up이다.

하지만, 그것 말고도 Pointer Enter/Exit이 존재한다.

그것을 사용하게 되면 PC에서는 마우스를 버튼 위에 올려만 놓아도 반응하게 되고, 마우스를 떼었을 때 종료되게 된다.

즉, **모바일에서는 손가락으로 누르고 있다가 다른 버튼으로 이동**하게 되면 자연스레 버튼 이벤트가 바뀌게 되는 것이다.

적용 해 보자

![image](https://user-images.githubusercontent.com/66288087/210129823-35ddbce1-f6fd-4782-afca-d144ab37b23e.png)

Enter/Exit로 바꾼 모습

<hr>

#### 모바일 테스트

**버튼 수정 전**

![2d_2_2](https://user-images.githubusercontent.com/66288087/210129996-203b50ed-8936-425b-a554-88961565fde4.gif)

수정 전에는 이동방향 전환을 위해서 버튼에서 손을 떼었어야 했다.


**버튼 수정 후**

![2d_2_1](https://user-images.githubusercontent.com/66288087/210129919-27419eaa-cdce-4c33-8ada-cf2b73129c68.gif)

모바일에서 적용 한 움짤이다.

버튼을 누를 때는 터치 부분이 없어졌다가 생기는데, 좌/우 이동을 할 때는 터치 부분이 없어지지 않으면서 방향 전환이 됨을 볼 수 있다.

<hr>

### 몬스터 피격

몬스터는 플레이어의 공격에 닿게 되면 피격이 된다.

![image](https://user-images.githubusercontent.com/66288087/210236668-5652c6ff-7b85-44fb-8e34-f8ec0f510b7f.png)

즉, 앞서 만들었던 플레이어 자식 오브젝트인 스킬 오브젝트에 닿게 되면 피격 함수를 발동시키면 된다.

몬스터에 들어 갈 Enemy.cs 코드를 만들어 보자

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public bool isFixed; // 고정 몬스터인가?
    public enum Type { Normal, Fire, Ice, Land };
    public Type monsterType; // 몬스터 속성

    public float monsterCntHP;
    public float monsterMaxHP;
    public int monsterAtk; 
    public int monsterDef;

    GameObject skillObj;
    public GameObject dmg; // 데미지 Prefab
    public GameObject dmgPos; // 데미지 생성 위치

    public GameObject HPBar; // HP Bar

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10)
        {
            skillObj = collision.gameObject;
            StartCoroutine(attacked());
        }
    }

    public IEnumerator attacked() // 공격 당했을 때
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Skills skillInfo = skillObj.GetComponent<SkillInfo>().thisSkillInfo;
        sprite.color = Color.red; // 피격 시 빨갛게

        /*
        
        한 번의 공격에서 한 번의 판정이 나야 한다. 따라서 이펙트 딜레이가 끝날 때 까지 Box 비활성화.. 를 하였지만.. 
        몬스터를 잡게 되면 해당 코루틴도 중단 되는 바람에 한 마리의 몬스터를 잡게 되면 Box가 다시 활성화 되지 않는다.

        물론, 잡는 시점에 활성화를 시키면 어떠냐 싶은데.. 딜레이가 긴 스킬을 캔슬하는 용도로 사용될 수 있기 때문에 그 방법은 쓰지 못하였다.

        원인을 다시 생각 해 보면

        때리고 나서 이펙트가 없어지기 전에 방향을 바꿔서 Box를 다시 닿게 하면 원래 타수보다 더 많은 공격을 하게 되는 것이다.

        그렇다면 공격 중에 방향을 바꾸지 못하게 하면 어떨까?.. 하고 생각 해 보니

        메이플에서도 공격을 할 때, 공격 동작에서는 방향을 바꿀 수는 없다. (이동은 된다.)

        따라서, 방향의 기준인 Scale의 x좌표를 이용하여 Player에서 공격 딜레이 중 방향 전환을 제한하였다.

         */
        

        for (int i = 0; i < skillInfo.atkCnt; i++)
        {
            Debug.Log((i + 1) + "타");

            GameObject imsiDmg = Instantiate(dmg);
            imsiDmg.GetComponent<dmgSkins>().damage = ((int)skillInfo.skillDmg - monsterDef);
            monsterCntHP -= ((int)skillInfo.skillDmg - monsterDef);

            float hpRatio = (monsterCntHP / monsterMaxHP); // HP를 int로 설정 했을 때는 나눈 값에 float로 명시적 형 변환을 하면 이미 늦는다. 따라서 HP값 앞에 float로 해 주었어야 했다.
            HPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(hpRatio, 0.1f);
            imsiDmg.transform.position = dmgPos.transform.position;
            
            if (monsterCntHP <= 0)
            {
                Destroy(gameObject);
                Debug.Log("몬스터 퇴치!");
            }

        }

        yield return new WaitForSeconds(0.1f);

        sprite.color = Color.white;

    }


}
</code>
</pre>

전문이다.

그 중에서 OnTriggerEnter2D와 attacked 코루틴 함수를 주목 해 보면

OnTriggerEnter2D에서는 **닿는 물체의 레이어**를 따짐을 볼 수 있다.

당연한 것이다.

몬스터는 기본적으로는 땅에 고정되어 있거나 땅 위를 걸어다니니 땅 Collider에도 닿게 된다.

분별 없이 닿는 모든 것을 공격으로 인식할 수 있으니 플레이어가 가진 스킬에 별도의 레이어를 할당시켜 해당 레이어만을 공격으로 인식하게 해 주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/210234018-36b003e5-1b65-4485-a3c0-1a4c138c319d.png)

Atk로 레이어를 설정하였음

![image](https://user-images.githubusercontent.com/66288087/210234048-88e6f2d1-6c06-4d5c-8bf6-57d9da34702e.png)

스킬 레이어도 설정하였다. (꿈이 커서 방어 스킬 레이어도 추가하였음..)

Trigger에서는 이제 공격이 인식 되면 피격을 받는 것을 처리 할 코루틴 함수로 이동한다.

(처음에는 트리거 함수에서 다 처리했고 Sprite 부분만 코루틴을 사용하였지만 트리거 안에서 다른 코드에 있는 코루틴이 잘 발동되지 않아서 이사시켰다.)

<br>

우선 공격을 당하게 되면 몬스터가 뻘개져야 한다. (공격을 당했기 때문에!)

그 다음, 스킬에 설정 된 타수만큼 데미지가 반복해서 들어가야 한다. (for문 사용!)

그리고 일정 딜레이 이후 빨갛게 된 상태를 해제시켜 주면 된다.

혹시 공격 중에 몬스터 피가 0 이하가 되면 몬스터를 제거 해 준다. (보상드랍 코드는 추가 예정)

공격은 끝... 인줄 알았지만

문제가 생겼다.

<hr>

#### 의도치 않은 추가 공격 꼼수

이미 지금은 해결 했고.. 당시에는 해결에만 몰두하다보니 사진을 못찍었다.

대신 그림으로 어떤 상황인지 정리 하도록 하겠다.

![image](https://user-images.githubusercontent.com/66288087/210236454-3380e050-5035-48d3-88c6-04ff38743c77.png)

바로, 공격을 하면서 생기는 딜레이 동안에 해당 몬스터의 데미지가 계산 된 이후, 다시 공격 범위에 닿게 하였을 때 한 번의 입력으로 의도치 않은 두 번의 공격을 하게 되는 현상이 발생하였다.

이러한 현상을 방지하기 위해 해결책들을 고심하기 시작하였다.

<hr>

#### 첫 번째 생각 : 해당 스킬에 맞았을 때, 스킬 딜레이 동안 해당 스킬에 피격당했음을 체크하고, 코루틴을 통하여 체크한 것을 해제하는 방식

즉, 스킬별로 스킬이 의도한 대로 맞았는지 체크하는 방식이다.

이 방법을 사용하려면 각 몬스터 별로 어떤 스킬에 맞았는지 계산을 해 주어야 한다.

그렇다는 것은 앞서 만들었던 skillInfo에 또 다른 체크 박스를 추가 해 주어야 한다.

생각을 해 보면서, 이렇게 무언가를 붙이면서 하기 보다는 근본적으로 이 문제를 해결 해 보고 싶었다.

따라서 이 방법은 최후의 수단으로 사용하기로 하였다.

무엇보다, 이 방법을 사용하게 되면 스킬의 다양성?을 제한할 것 같았다.

<hr>

#### 두 번째 생각 : 몬스터가 해당 스킬에 맞았을 때, 그 스킬의 BoxCollider를 스킬 이펙트 애니메이션이 끝나는 시점까지 해제하는 것

즉, 처음에 때렸던 대상을 때리고 나서는 BoxCollider를 비활성화 하여 의도치 않은 추가타를 때릴 여지를 만들어 주지 않는 방법이다.

이 방법을 사용하기 위하여 SkillInfo.cs(스킬 오브젝트 자체에 들어간 코드)에 코루틴을 추가하여 관리 해 주었다.

(앞에서 OnTriggerEnter에서 다른 코드에 있는 코루틴을 사용하는데 지장이 있었다고 했는데 이 방법을 사용하면서 생겼던 것이다.)

그런데, 이 것도 문제가 생겼다.

몬스터 공격 -> 몬스터 코드에서 Box 비활성화 코루틴 시작을 명령 -> 스킬 오브젝트에서 BoxCollider 비활성화 코드 실행 -> 스킬 딜레이 이후 다시 활성화

의 순서인데..

몬스터가 죽어서 사라지게 되면 스킬 오브젝트에서 BoxCollider 비활성화 코드(코루틴)가 끊기는 것이다.

**즉, Box Collider가 활성화가 되지 않는다.**

바로 없애지 말고, 스킬 딜레이동안 그냥 모습만 보이지 않게만 해 볼까 생각했지만.. 몬스터 카운팅을 할 때 불편할 것 같아서 그냥 다시 빠꾸했다.

<hr>

#### 세 번째 생각 : 공격 딜레이동안, 반대 방향으로 움직이지 못하게 하였다. (캐릭터가 뒤집히지 못하게!!)

방법을 생각 해 보던 중, 메이플스토리 게임에서 아델이 점프-디바이드를 사용하는 모습이 생각이 났다.

점프-디바이드를 사용할 때, 움직일 수는 있지만 칼로 내려치는 동시에 반대 방향으로 움직이는 것은 한 번도 본 적이 없었다.

여기서 아이디어가 떠올랐다!

스킬 애니메이션이 발동되는 동안**만!** 반대 방향으로 이동하여 캐릭터를 뒤집지 못하게 하는 것이다.

(앞으로 이동 및 점프는 가능!!)

대신, 스킬 애니메이션 딜레이는 대폭 줄였다. (기존 0.6f -> 0.3f)

그리고 애니메이션도 끝 부분을 제거 해 주어, 이펙트를 간소화 시켜 주었다. (평타이기 때문에 한 작업이다. 나중에 추가 할 스킬들은 컨셉에 맞게 길이 조정 예정)

<hr>

#### 공격 시 방향 전환 x 결과

![2d_2_3](https://user-images.githubusercontent.com/66288087/210238599-8199691d-8dbb-4db3-9d75-c2d1fa268939.gif)

위 움짤처럼 공격시에는 뒤로 돌지 않음을 볼 수 있다.

(전에는 공격 애니메이션 후반부가 좌/우 반전되어 나가는 경우도 있었다.)

공격에 대한 적용 모습이고, 몬스터가 피격되는 종합적인 모습은 마지막에 움짤로 첨부할 것이다.

<hr>

### 데미지, HP 바

몬스터에게 몇의 데미지를 입혔는지를 시각적으로 볼 수 있는 데미지 텍스트와 몬스터의 잔여 HP 양을 가늠할 수 있는 HP바는 필수적이다.

<hr>

#### 데미지

데미지는 앞서 3D로 쿼터뷰 게임에서도 만든적이 있었다.

TextMeshPro 오브젝트를 만들어 주어, 아래 코드 dmgSkins.cs 를 넣어 주어 Prefab화 해 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class dmgSkins : MonoBehaviour
{
    TextMeshPro dmgText;
    Color alpha;

    public float movingSpeed;
    public float alphaSpeed;
    public int damage;


    // Start is called before the first frame update
    void Start()
    {
        dmgText = GetComponent<TextMeshPro>();
        dmgText.text = damage.ToString();

        alpha = dmgText.color;
        Invoke("DestroyDmg", 2.0f); // 2초 뒤에 데미지가 사라지게!

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, 1 * movingSpeed * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        dmgText.color = alpha;
    }

    public void DestroyDmg()
    {
        Destroy(gameObject);
    }

}
</code>
</pre>

데미지가 생성되면 Start()에서 (Awake()를 사용하고 그곳에 데미지를 넣어주면 코드로 데미지를 넣어줄 때 적용이 되지 않는다. Awake()가 먼저 실행되기에 그런 것 같다.) 몬스터 피격 시 세팅 된 데미지를 텍스트에 넣어 준다.

그 다음, 일정 시간 뒤에 데미지를 사라지게 하는 Invoke를 설정 해 주고, Update에서 데미지가 위로 떠오르면서 천천히 사라지게 해 준다.

**Mathf.Lerp**는 선형 보간 즉, 양 값 사이에서 특정 비율에 있는 값을 출력 해 준다.

위 코드에서는 현재 투명도 값과 0 사이에서 Time.deltaTime으로 얻는 프레임 간격 값을 이용하여 점점 0에 다가가는 값을 alpha.a에 적용 해 준다.

(즉, 점점 투명해 진다는 것이다. -> alphaSpeed 값을 통하여 투명화 속도를 조절할 수 있다.)

![image](https://user-images.githubusercontent.com/66288087/210239621-829fe7e0-a54c-498c-a389-607a703e7d8c.png)

데미지에 아웃 라인도 적용 해 주었다.

이것은 Prefab화 해 주고, 앞서 적었던 Enemy.cs 코드에서 데미지를 Instantiate를 통해 소환 하는데, 그 부분을 다시 가져와 보았다.

<pre>
<code>
GameObject imsiDmg = Instantiate(dmg);
imsiDmg.GetComponent<dmgSkins>().damage = ((int)skillInfo.skillDmg - monsterDef);
monsterCntHP -= ((int)skillInfo.skillDmg - monsterDef);

float hpRatio = (monsterCntHP / monsterMaxHP); 
// HP를 int로 설정 했을 때는 나눈 값에 float로 명시적 형 변환을 하면 이미 늦는다. 따라서 HP값 앞에 float로 해 주었어야 했다.

HPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(hpRatio, 0.1f);
imsiDmg.transform.position = dmgPos.transform.position;
</code>
</pre>
dmg를 임시로 소환하고, 두 번째 줄에서 데미지 값을 세팅 해 준다.

그 다음 마지막 줄에서 소환된 데미지 텍스트를 위치시킬 위치를 지정 해 준다.

![image](https://user-images.githubusercontent.com/66288087/210239929-83aa8b54-d84c-4490-a0c8-342a102b7245.png)

나는 몬스터의 하위 오브젝트로 빈 오브젝트를 만들어 해당 오브젝트의 위치로 세팅 해 주었다.

![image](https://user-images.githubusercontent.com/66288087/210240035-ff27e70b-0be1-4c18-a28f-962e229e5456.png)

dmgPos의 위치

<hr>

#### HP 바 설정

HP 바는 UI Canvas를 통해 설정할 수 있다.

처음에는 Canvas가 씬 내부에 하나만 존재할 수 있는 줄 알았다.

하지만 그게 아니었다.

일단 새롭게 Canvas 하나를 만들고 몬스터 하위에 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/210240288-d8e8c6cf-32fe-43a2-9227-8cae724cd413.png)

그리고 캔버스 오른쪽 Inspector에서

![image](https://user-images.githubusercontent.com/66288087/210240340-2123e5a4-0fc5-447c-9cf7-8686b614acbf.png)

위와 같이 Canvas에서 Render Mode를 World Space로 설정하고, Canvas Scaler를 체크 해제 해 준다.

그렇게 되면 캔버스를 움직일 수 있게 되고, 캔버스의 사이즈도 자유롭게 변경할 수 있게 되었다.

Scaler를 통해 캔버스 사이즈가 고정되었던 것으로 추측된다.

이제, HP Bar를 하나 만들어 준다.

별건 아니고, 직사각형을 하나 만들고, Anchor를 왼쪽으로 설정 해 준다.

![image](https://user-images.githubusercontent.com/66288087/210240614-623ad662-f67c-46dd-bfe9-c3e75b08076e.png)

그렇게 되면 사이즈를 줄일 때, Anchor이 고정되며 그 방향으로 줄어들기 때문이다.

그리고 Enemy에도 HP Bar를 받을 수 있는 게임 오브젝트 변수를 넣어 주고, 아래 코드처럼 넣어 준다.

<pre>
<code>
GameObject imsiDmg = Instantiate(dmg);
imsiDmg.GetComponent<dmgSkins>().damage = ((int)skillInfo.skillDmg - monsterDef);
monsterCntHP -= ((int)skillInfo.skillDmg - monsterDef);

float hpRatio = (monsterCntHP / monsterMaxHP); 
// HP를 int로 설정 했을 때는 나눈 값에 float로 명시적 형 변환을 하면 이미 늦는다. 따라서 HP값 앞에 float로 해 주었어야 했다.

HPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(hpRatio, 0.1f);
imsiDmg.transform.position = dmgPos.transform.position;
</code>
</pre>

위에서도 봤지만..

HP Bar의 크기 조절을 중심으로 보면

우선 hpRatio는 말 그대로 몬스터의 현재 체력을 최대 체력으로 나눈 것이다.

여기서 주의할 점이 있다.

**몬스터의 현재/최대 체력을 int로 설정 했다면 한 대를 때리면 HP Bar가 사라질 것이다.**

왜냐하면 **int끼리의 나누기**는 **버림**을 수행하기 때문이다.

즉, **0.xx -> 0**이 된다.

그래서 아래처럼

<pre>
<code>
float hpRatio = (float)(monsterCntHP / monsterMaxHP); 
</code>
</pre>

를 해 주면 되겠지..? 

가 아니다.

괄호 속에서 이미 0이 되어 나오기 때문에 형변환을 해 줄거면 괄호 속에

<pre>
<code>
float hpRatio = ((float)monsterCntHP / (float)monsterMaxHP); 
</code>
</pre>

위 코드처럼 해 주어야 한다.

그런데, 그냥 처음부터 현재 체력, 최대 체력을 float로 정의 해 주는 것이 편하다.

<br>

아무튼, 이렇게 해 준 다음

<pre>
<code>
HPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(hpRatio, 0.1f);
</code>
</pre>

HP 바의 **사이즈를 조절** 해 준다.

**사이즈**는 **RectTransform을 이용** 해 주며, **using UnityEngine.UI;**를 **코드 맨 위**에 **추가** 해 주도록 하자.

사이즈는 sizeDelta를 통해 조절 해 주며, Vector값을 넣어 준다.

원래 hpRatio에 100퍼센트 기준 바의 최대 길이를 곱해 주어야 하는데, 1로 설정했기 때문에 그냥 비율만 넣어 주었다.

이렇게 하고 실행 해 보면..

<hr>

### 시연 움짤

![2d_2_4](https://user-images.githubusercontent.com/66288087/210242301-a010f7d8-e9d0-42d4-b0e1-b78e9ab46f17.gif)

좀 중간에 시간을 끌긴 했지만... 잘 적용됨을 볼 수 있었다.

다음에는 플레이어가 맞는 것과, 몬스터 보상, 움직이는 몬스터 등을 만들어 보도록 하겠다.
