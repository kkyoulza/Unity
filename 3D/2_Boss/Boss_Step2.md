# Step2. 아이템 습득 및 장착

이제 RPG하면 빠질 수 없는 아이템을 만들어 보도록 하자.

필드에 떨어진 아이템에 다가가 습득을 하는 기능을 먼저 구현 해 보도록 하자

우선 골드메탈님의 에셋에서 아이템 프리팹을 하나 가져 와 준다.

그리고 있어 보이게 아래 사진과 같이 아이템 오브젝트의 각도를 Z축으로 기울여 준다.

![image](https://user-images.githubusercontent.com/66288087/191007657-551a3064-8544-41c4-8c70-39c475e2e124.png)

이렇게 기울여 준 다음, 프리팹 자식에 빈 오브젝트를 하나 생성 해 준다.

**빛 효과 추가**

그 다음 Light 컴포넌트를 추가 해 준다.

![image](https://user-images.githubusercontent.com/66288087/191007767-d9a4fc61-b5b8-416f-a53c-f6e3d9cece6f.png)

위와 같이 범위를 조금 줄여주고, 밝기를 좀 높여주었으며, 아이템 색깔에 맞게 빛 색도 바꾸어 주었다.

**파티클 추가**

빛에 이어 파티클 시스템도 추가 해 주었다.

Material은 Default-Line으로 하였으며, 색은 아이템 색과 유사하게 설정하였다.

![image](https://user-images.githubusercontent.com/66288087/191009600-3204ccb1-08bf-4b63-a6ea-bf73183ac3bb.png)

Limit Velocity over Lifetime에 있는 Drag를 1로 설정하여 파티클이 너무 퍼져 나가는 것을 억제하였다.

(좋은 아이템에만 과한 효과를 주어야 하기 때문)

![image](https://user-images.githubusercontent.com/66288087/191009640-85f2be7c-451d-4986-9d1a-280aebff91ea.png)

또한, Size도 천천히 줄어들게 설정 해 주었다.

**아이템 자체 코드 설정**

아이템에 추가적인 효과를 주고, 아이템 자체에 대한 정보를 저장하기 위해 코드를 하나 만들어 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Armor, Coin, Weapon }; // 열거형 타입
    public Type type;
    public int value;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }
}
</code>
</pre>

enum으로 아이템의 타입을 만들어 주고, value를 통하여 아이템 코드를 생성 해 준다.

enum은 타입을 만들어 두고, 객체처럼 만들어 주어야 사용할 수 있다.

만들기만 하고 왜 Inspector에 뜨지 않는지 고민하지 말자.


**아이템 Collider 설정**

이제 아이템에 Collider를 넣어 준다.

아이템은 중력의 영향을 받아야 하기에 RigidBody를 넣어 준다.

여기서는 아이템에 Collider를 2개를 넣어 주어 1개는 Trigger로 설정하고(아이템 습득 시 적용), 다른 1개는 아이템을 지탱할 수 있게끔 사용한다.

![image](https://user-images.githubusercontent.com/66288087/191208212-ad133c11-d483-4c6e-b461-f3b73de1e923.png)

그림과 같이 두 개를 사용 해 준다.

이제 플레이어 코드에 아이템 습득 관련 코드를 추가 해 보도록 하자

<hr>

**아이템 습득**

플레이어 코드에 아래와 같이 코드를 추가 해 준다.

<pre>
<code>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCode : MonoBehaviour
{
    float horAxis; // 가로 방향키 입력 시 값을 받을 변수
    float verAxis; // 세로 방향

    // 키 입력 변수(bool)
    bool runDown; // 대쉬 버튼이 눌렸는가?
    bool jumpDown; // 점프 키가 눌렸는가?
    bool dodgeDown; // 회피 키가 눌렸는가?
    bool iDown;  // 습득 키가 눌렸는가?
    bool sDown1; // 1번 장비
    bool sDown2; // 2
    bool sDown3; // 3


    // 상태 변수(bool)
    bool isSwap; // 장비를 바꾸고 있는가?
    bool isJump; // 점프를 하고 있는가?
    bool isDodge; // 회피를 하고 있는가?(구르기)
    bool DodgeCool; // 회피 쿨타임중인가?

    Vector3 moveVec;

    public float playerSpeed;

    //무기 관련
    public GameObject[] WeaponList; // 활성화 할 무기 리스트
    public bool[] hasWeapons; // 어떤 무기를 가지고 있는가?

    GameObject nearObject; // 근처에 떨어져 있는 아이템 오브젝트
    Weapon cntEquipWeapon; // 현재 장착하고 있는 무기

    int cntindexWeapon = -1; // 현재 끼고 있는 무기 index, 초기 값은 -1로 해 준다.

    //쿨타임 관련
    public GameObject manager;
    CoolTimeManager coolManager;

    // 애니메이션, 물리 관련
    Animator anim;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>(); // 자식 오브젝트에 있는 컴포넌트를 가져온다.
        coolManager = manager.GetComponent<CoolTimeManager>(); // 쿨타임 매니저를 불러온다.
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
        Move_Ani();
        Jump();
        TrunChar();
        Dodge();
        Swap();
        InterAction();
    }
    
    void InputKey()
    {
        horAxis = Input.GetAxisRaw("Horizontal");
        verAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        jumpDown = Input.GetButtonDown("Jump");
        dodgeDown = Input.GetKey(KeyCode.Z);

        AtkDown = Input.GetKey(KeyCode.X);

        iDown = Input.GetButtonDown("InterAction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");


    }

    void Move_Ani()
    {
        moveVec = new Vector3(horAxis, 0, verAxis).normalized; // 어느 방향으로 이동하던지 같은 속도로 가게끔 만들어 준다. (단위벡터화)

        if (isSwap) // 장비 변경 중에는 딜레이가!
            moveVec = Vector3.zero;

        if (runDown)
        {
            transform.position += moveVec * (playerSpeed+3) * Time.deltaTime; // 달릴 때 속도 늘리기
        }
        else
        {
            transform.position += moveVec * playerSpeed * Time.deltaTime; // deltaTime은 컴퓨터 환경에 이동 거리가 영향을 받지 않게 하기 위함!
        }
        

        anim.SetBool("IsWalk", moveVec != Vector3.zero); // 입력이 있을 때 움직여야 하니 조건문을 넣었다.
        anim.SetBool("IsRun", runDown);
    }

    void TrunChar()
    {
        transform.LookAt(transform.position + moveVec); // 나아가는 방향으로 자동으로 회전되게 한다.
    }

    void Jump()
    {
        if (jumpDown && !isJump && !isSwap) // 점프키가 눌리고 점프 상태가 아닐 때!
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("DoJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (dodgeDown && moveVec != Vector3.zero && !DodgeCool && !isSwap) // Z 키가 눌리고 점프 상태가 아닐 때!
        {
            dodgeVec = moveVec; // 구르는 시점의 이동 벡터
            playerSpeed *= 2;
            anim.SetTrigger("DoDodge");
            isDodge= true;
            DodgeCool = true;
            Invoke("DoDodge", 0.5f);
        }
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || cntindexWeapon == 0)) // 1번을 눌렀을 때, 습득을 안했거나, 이미 같은 것을 장착하고 있을 때
            return; // 실행 x
        if (sDown2 && (!hasWeapons[1] || cntindexWeapon == 1)) // 2번을 눌렀을 때, 습득을 안했거나, 이미 같은 것을 장착하고 있을 때
            return; // 실행 x
        if (sDown3 && (!hasWeapons[2] || cntindexWeapon == 2)) // 3번을 눌렀을 때, 습득을 안했거나, 이미 같은 것을 장착하고 있을 때
            return; // 실행 x

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isSwap && !isDodge) // 1~3번 키가 눌리면서 점프중이 아닐 때! + 무기를 먹었을 때!
        {
            if(cntEquipWeapon != null) // 이미 다른 무기를 끼고 있을 때!
            {
                cntEquipWeapon.gameObject.SetActive(false); // 먼저 해제!
            }
            cntEquipWeapon = WeaponList[weaponIndex].GetComponent<Weapon>();
            cntindexWeapon = weaponIndex;
            cntEquipWeapon.gameObject.SetActive(true);

            isSwap = true;
            anim.SetTrigger("DoSwap");

            Invoke("SwapOut",0.3f);

        }
    }

    void InterAction()
    {
        if(iDown && nearObject != null && !isJump && !isSwap)
        {
            // 무기에 닿고 있고(근처 오브젝트가 null이 아님), 점프 상태가 아닐 때, 상호작용 버튼을 누르게 되면 아이템을 습득하게 된다.
            // 이것을 응용하여 NPC와도 대화를 하게끔?

            if(nearObject.tag == "weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; // value를 index로 설정 할 것!

                hasWeapons[weaponIndex] = true;
                Destroy(nearObject);
            }

        }
    }

    void DoDodge()
    {
        playerSpeed *= 0.5f;
        coolManager.SetCoolTime(5.0f);
        coolManager.coolOn = false;
        isDodge = false;
        Invoke("DodgeCoolDown", 5.0f); // 쿨타임은 5초!
    }

    void SwapOut() // 스왑 딜레이 설정!
    {
        isSwap = false;
    }

    void DodgeCoolDown()
    {
        Debug.Log("회피 쿨타임 종료!");
        coolManager.coolOn = true;
        coolManager.SetAble();
        DodgeCool = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "base")
        {
            anim.SetBool("IsJump", false);
            isJump = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "weapon")
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "weapon")
        {
            nearObject = null;
        }
    }

}
</code>
</pre>

완성된 코드는 위와 같다.

우선, 키 입력을 위해 bool 변수들을 만들어 준다.

대개 E 버튼이 습득 버튼이기에 Project Setiings에 들어가 InterAction이라는 이름으로 e키를 이름지어 준다.

bool 변수 뿐 아니라 GameObject도 nearObject라는 이름으로 만들어 주고, GameObject 배열과, 무기를 장착하고 있는 지 여부를 알려 줄 bool 배열도 만들어 준다.

OnTriggerStay 이벤트를 만들어 주어 트리거에 닿아 있을 때 게임 오브젝트의 태그가 weapon이면 nearObject에 닿은 오브젝트를 넣어 주는 것이다.

update에서 실행 중인 InterAction 함수에서 습득 키를 누르고, nearObject가 null이 아닌 상태이고 점프, 구르기가 아니라면 자동으로 nearObject의 value 값을 가져와 hasWeapons 배열의 Index 값으로 사용하며, 해당 인덱스를 true로 바꾸어 해당 무기를 습득했음을 표기한다.

그리고 장착 버튼을 눌렀을 때는 hasWeapons가 true인지 여부를 따지면서 무기를 바꾸어 준다.

그림으로 요약하면 아래와 같다.

![image](https://user-images.githubusercontent.com/66288087/191219030-34d3519b-6808-47cf-8d35-61e06e3be86f.png)

<hr>

**무기 생성**

GameObject 배열에는 미리 장착하고 있는 무기 오브젝트를 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/191215127-10b79b68-3dd8-486f-adb7-f5482978bec7.png)

즉, 캐릭터의 손에 빈 오브젝트를 만들어, 무기를 넣어 준 다음, 비활성화를 해 두고, 무기를 먹고 장착하게 되면 무기의 외형을 활성화 하는 것이다.

그런데 여기서 주의할 점이, 아까 아이템의 형태로 만든 무기를 손에 쥐어주면 안되고, 모델을 가져와서 쥐어 주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/191215477-2d53f971-ff0d-4c5b-b47d-e5cf6e743ecb.png)

착용하고 있는 모습이다.

<hr>

**착용 무기 설정**

이제, 착용한 무기에 BoxCollider를 추가 해 준다.

![image](https://user-images.githubusercontent.com/66288087/191215629-aa49d28c-6b3d-4bf8-9777-bb67c2ee6400.png)

적당히 실제 부피보다 더 크게 설정 해 준다.

그리고 해당 무기에 관련 정보들을 넣기 위해 Weapon.cs 코드를 하나 만들어 준다.

일단은 정보만을 넣어 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum AtkType { Melee, Range }; // Melee - 근접 공격, Range - 원거리 공격
    public AtkType type; // 생성을 해 주어야 한다.
    public int Damage;
    public float AtkDelay;
    public BoxCollider meleeArea; // 근접 공격 범위
    public TrailRenderer trailEffect; // 효과?

}
</code>
</pre>

**TrailRenderer 설정**

TrailRenderer는 이동했던 경로에 그리는 것이라고 생각하면 된다.

즉, 아래 사진과 같이 망치를 움직이게 되면 생긴다.

![image](https://user-images.githubusercontent.com/66288087/191219855-677bbdd1-625f-43d9-9529-d3692c6a4cba.png)

이것도 아래 사진과 같이 시간이 지날 수록 지워지는 속도를 설정 하고, 시간, 컬러 등을 설정 해 준다.

![image](https://user-images.githubusercontent.com/66288087/191219954-db82ed26-7e4d-46cf-ac29-1e50ecf8fac6.png)

재질은 파티클과 같이 Default-Line을 이용 해 주었다.

효과 설정은 마쳤고, 이 효과는 망치를 휘두를 때 잠깐 나타날 것이다.

<hr>

이제 아이템을 바꾸는 작업을 해 주도록 하자

아이템 변경은 숫자키를 누르면 되는 것으로 하자 bool로 3개를 만들어 주어 입력 받게 하고, 무기를 변경하는 Swap()함수를 만들어 준다.

<pre>
<code>
void Swap()
{
    if (sDown1 && (!hasWeapons[0] || cntindexWeapon == 0)) // 1번을 눌렀을 때, 습득을 안했거나, 이미 같은 것을 장착하고 있을 때
        return; // 실행 x
    if (sDown2 && (!hasWeapons[1] || cntindexWeapon == 1)) // 2번을 눌렀을 때, 습득을 안했거나, 이미 같은 것을 장착하고 있을 때
        return; // 실행 x
    if (sDown3 && (!hasWeapons[2] || cntindexWeapon == 2)) // 3번을 눌렀을 때, 습득을 안했거나, 이미 같은 것을 장착하고 있을 때
        return; // 실행 x

    int weaponIndex = -1;
    if (sDown1) weaponIndex = 0;
    if (sDown2) weaponIndex = 1;
    if (sDown3) weaponIndex = 2;

    if ((sDown1 || sDown2 || sDown3) && !isJump && !isSwap && !isDodge) // 1~3번 키가 눌리면서 점프중이 아닐 때! + 무기를 먹었을 때!
    {
        if(cntEquipWeapon != null) // 이미 다른 무기를 끼고 있을 때!
        {
            cntEquipWeapon.gameObject.SetActive(false); // 먼저 해제!
        }
        cntEquipWeapon = WeaponList[weaponIndex].GetComponent<Weapon>();
        cntindexWeapon = weaponIndex;
        cntEquipWeapon.gameObject.SetActive(true);

        isSwap = true;
        anim.SetTrigger("DoSwap");

        Invoke("SwapOut",0.3f);

    }
}

void SwapOut() // 스왑 딜레이 설정!
{
    isSwap = false;
}
</code>
</pre>

코드는 대략 이러한데, 1번 버튼을 눌렀지만 1번 무기를 획득하지 못했거나 이미 1번 무기를 착용하고 있다면 return하여 swap하지 않게 설정 해 주었다.

return되지 않았다면 index를 눌러진 키에 맞게 설정 한 뒤, 이미 착용하고 있는 무기가 있는지 확인한다.

착용하고 있는 무기가 있다면 먼저 비활성화를 해 주고, 새롭게 WeaponList에서 받아 활성화를 해 준다.

그리고 현재 착용하고 있는 무기에 대한 정보를 저장 해 준다.

마지막으로 애니메이션을 출력 해 준뒤, 딜레이를 주어 isSwap을 꺼 준다.

<hr>

**애니메이션 추가**

이제 Swap을 하는 애니메이션을 설정 해 주자

이미 에셋으로 받아 놓은 Swap 애니메이션을 애니메이터에 추가 해 준다.

![image](https://user-images.githubusercontent.com/66288087/191225094-ba78c82d-6864-4b41-bb82-d27f68cd3066.png)

DoSwap이라는 Trigger를 만들어 준 뒤, Swap을 해 주면 트리거가 발동되게끔 했다.

<hr>

![boss_2_1](https://user-images.githubusercontent.com/66288087/191226095-d91c9b96-20c4-45c2-b8ba-2635f0965e38.gif)

결과는 위 움짤과 같이 나온다.

다음에는 무기를 가지고 공격을 하는 것을 해 보도록 하겠다.
