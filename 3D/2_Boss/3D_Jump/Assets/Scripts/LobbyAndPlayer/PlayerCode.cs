using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCode : MonoBehaviour
{
    // 플레이어 정보
    public int playerHealth; // 플레이어 체력
    public int playerStrength; // 플레이어 힘
    public int playerAccuracy; // 플레이어 명중률

    // 조작 관련
    float horAxis; // 가로 방향키 입력 시 값을 받을 변수
    float verAxis; // 세로 방향

    float AtkDelay; // 공격 딜레이
    float dodgeCoolTime = 3.0f; // 회피 쿨타임

    // 키 입력 변수(bool)
    bool runDown; // 대쉬 버튼이 눌렸는가?
    bool jumpDown; // 점프 키가 눌렸는가?
    bool dodgeDown; // 회피 키가 눌렸는가?
    bool iDown;  // 습득 키가 눌렸는가?
    bool sDown1; // 1번 장비
    bool sDown2; // 2
    bool sDown3; // 3
    bool AtkDown; // 공격 키
    bool rDown; // 재장전 키
    bool itemDown; // 아이템 창 키 


    // 상태 변수(bool)
    bool isSwap; // 장비를 바꾸고 있는가?
    bool isJump; // 점프를 하고 있는가?
    bool isDodge; // 회피를 하고 있는가?(구르기)
    bool DodgeCool; // 회피 쿨타임중인가?
    bool isAtkReady; // 공격 준비가 되었는가?
    bool isBorder; // 경계선에 닿았나?
    bool isReloading; // 재장전중인가?
    bool isDamage; // 피격 당하고 있는 중인가?


    Vector3 moveVec;
    Vector3 dodgeVec; // 회피 방향

    public float playerSpeed;
    public int bullet; // 총알 개수

    //무기 관련
    public GameObject[] WeaponList; // 활성화 할 무기 리스트
    public bool[] hasWeapons; // 어떤 무기를 가지고 있는가?

    public GameObject nearObject; // 근처에 떨어져 있는 아이템 오브젝트
    Weapon cntEquipWeapon; // 현재 장착하고 있는 무기

    int cntindexWeapon = -1; // 현재 끼고 있는 무기 index, 초기 값은 -1로 해 준다.

    PlayerItem playerItem; // 플레이어가 가지고 있는 아이템 정보들이 담겨있음

    //쿨타임 관련
    public GameObject manager;
    CoolTimeManager coolManager;

    // 애니메이션, 물리 관련
    Animator anim;
    Rigidbody rigid;
    MeshRenderer[] meshs; // 플레이어의 모든 매쉬들(몸, 머리, 팔 등)을 가져오기 위해 배열로 생성하였음

    // UI 관련
    public GameObject UIManager;
    UIManager ui;

    private void Awake()
    {
        isDamage = false;

        ui = UIManager.GetComponent<UIManager>(); // ui 매니저 컴포넌트
        anim = GetComponentInChildren<Animator>(); // 자식 오브젝트에 있는 컴포넌트를 가져온다.
        coolManager = manager.GetComponent<CoolTimeManager>(); // 쿨타임 매니저를 불러온다.
        playerItem = GetComponent<PlayerItem>(); // 아이템 컴포넌트
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>(); // 여러 개를 가져오는 것이니 Components s 꼭 붙이기!
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
        Move_Ani();
        Jump();
        Attack();
        ReLoad();
        TrunChar();
        Dodge();
        Swap();
        onUI();
        InterAction();
    }

    void Attack()
    {
        if (cntEquipWeapon == null)
            return;

        AtkDelay += Time.deltaTime;
        isAtkReady = cntEquipWeapon.AtkDelay < AtkDelay; // 공격 속도(무기 딜레이)보다 현재 딜레이 값이 클때만!

        if(AtkDown && isAtkReady && !isDodge && !isSwap)
        {
            cntEquipWeapon.Use(); // 공격할 준비가 되었으니 전달하고 나머지는 위임!
            anim.SetTrigger(cntEquipWeapon.type == Weapon.AtkType.Melee ? "DoSwing" : "DoShot"); // 3항 연산자를 이용하여 한 줄로 두 가지 종류의 애니메이션을 실행한다.
            // 3항 연산자의 사용은 유연하게 가능하다는 것을 명심!
            AtkDelay = 0f;
        }

    }
    void ReLoad()
    {
        if (cntEquipWeapon == null)
            return;

        if (cntEquipWeapon.type == Weapon.AtkType.Melee)
            return;

        if (bullet == 0) // 남은 총알이 0개이면 안된다.
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isAtkReady && !isReloading)
        {
            // 재장전 키가 눌리고, 점프중,회피중,무기교체중이 아닐때이면서 공격 준비가 되었을 때 실행되게 한다.
            isReloading = true;
            anim.SetTrigger("DoReload");

            Invoke("ReLoadOut", 0.7f);

        }

    }

    void ReLoadOut()
    {
        int reCount = bullet < cntEquipWeapon.maxCount ? bullet : cntEquipWeapon.maxCount - cntEquipWeapon.cntCount;
        cntEquipWeapon.cntCount += reCount;

        if (cntEquipWeapon.cntCount > cntEquipWeapon.maxCount)
        {
            reCount = bullet - (cntEquipWeapon.cntCount - cntEquipWeapon.maxCount); // 남은 총알 개수에서 넘치는 부분을 뺀 만큼을 충전해야 한다.
            cntEquipWeapon.cntCount = cntEquipWeapon.maxCount;
        }

        bullet -= reCount; // 플레이어의 총알 개수 갱신

        isReloading = false;
    }

    void InputKey()
    {
        horAxis = Input.GetAxisRaw("Horizontal");
        verAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        jumpDown = Input.GetButtonDown("Jump");
        dodgeDown = Input.GetKeyDown(KeyCode.Z);

        AtkDown = Input.GetKeyDown(KeyCode.X);
        rDown = Input.GetButtonDown("ReLoad");

        iDown = Input.GetButtonDown("InterAction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");

        itemDown = Input.GetKeyDown(KeyCode.I);

    }

    void onUI()
    {
        if (itemDown)
        {
            if (!ui.ItemUI.activeSelf)
                ui.OnItemUI();
            else
                ui.OffItemUI();
        }

    }

    void Move_Ani()
    {
        moveVec = new Vector3(horAxis, 0, verAxis).normalized; // 어느 방향으로 이동하던지 같은 속도로 가게끔 만들어 준다. (단위벡터화)

        if (isSwap || isReloading) // 장비 변경 중에는 움직이지 못하게! or 재장전 시에는 움직이지 못하게!(!isReloading으로 하고 아래에 넣어서 벽을 뚫는 버그가 생겼다. 때를 잘 가릴 것!)
            moveVec = Vector3.zero;

        if(!isBorder || isReloading) // 벽에 가까이 있을 때는 움직이지 못하게! (회전은 됨)
            transform.position += moveVec * playerSpeed * (runDown ? 1.7f : 1.0f) * Time.deltaTime; // deltaTime은 컴퓨터 환경에 이동 거리가 영향을 받지 않게 하기 위함!
            // 달릴 때는 더 빠르게! - 삼항 연산자를 이용하였음!

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

    void SwapOut() // 스왑 딜레이 설정!
    {
        isSwap = false;
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

                ui.PopUI.SetActive(false);
                ui.isNoticeOn = false; // 무기를 먹었을 때 비활성화!
                hasWeapons[weaponIndex] = true;

                playerItem.GetInfo(nearObject);

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Smith")
            {
                ui.PopUI.SetActive(false);
                ui.EnchantWeaponUI();
            }

        }
    }

    void DoDodge()
    {
        playerSpeed *= 0.5f;
        coolManager.SetCoolTime(dodgeCoolTime);
        coolManager.coolOn = false;
        isDodge = false;
        Invoke("DodgeCoolDown", dodgeCoolTime); // 쿨타임은 3초!
    }

    void DodgeCoolDown()
    {
        Debug.Log("회피 쿨타임 종료!");
        coolManager.coolOn = true;
        coolManager.SetAble();
        DodgeCool = false;
    }


    // 물리 문제 해결
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; // 회전속도를 0으로 만들어 준다.
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 3, Color.red);
        isBorder = Physics.Raycast(transform.position + Vector3.up, moveVec, 3, LayerMask.GetMask("Wall"));
    }

    private void FixedUpdate()
    {
        FreezeRotation(); // 회전 속도 0으로 설정!
        StopToWall();
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
        if (other.gameObject.tag == "weapon" && !ui.isNoticeOn)
        {
            nearObject = other.gameObject;
            ui.NoticeOn();
            ui.isNoticeOn = true;
        }
        else if(other.gameObject.tag == "Smith" && !ui.isNoticeOn)
        {
            nearObject = other.gameObject;
            ui.NoticeOn();
            ui.isNoticeOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "weapon")
        {
            nearObject = null;
            ui.NoticeOff();
            ui.isNoticeOn = false;
        }
        else if (other.gameObject.tag == "Smith")
        {
            nearObject = null;
            ui.NoticeOff();
            ui.isNoticeOn = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyBullet")
        {
            if (other.GetComponent<Rigidbody>() != null) // 무적 시간 중에도 추가적으로 투사체를 맞게 되면 사라지게끔!
                Destroy(other.gameObject);

            bool isNoDmgJumpAtk = other.name == "JumpAtkArea"; 
            if(isNoDmgJumpAtk && isDamage) // 무적시간 중에 점프 공격에 맞았을 때!
                StartCoroutine(noDamageNuckBack());

            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                playerHealth -= enemyBullet.damage;

                bool isBossAttack = other.name == "JumpAtkArea"; // 점프 공격에 맞았을 때!

                StartCoroutine(OnDamage(isBossAttack));
            }
        }
    }

    IEnumerator OnDamage(bool isBossAttack)
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        if (isBossAttack) // 찍기 공격을 맞았을 때!
            rigid.AddForce(transform.forward * -40, ForceMode.Impulse); // 넉백!

        yield return new WaitForSeconds(1.0f); // 무적 딜레이 1초!

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAttack) // 넉백 후
            rigid.velocity = Vector3.zero; // 속도 원위치

    }

    IEnumerator noDamageNuckBack()
    {
        rigid.AddForce(transform.forward * -40, ForceMode.Impulse); // 넉백!

        yield return new WaitForSeconds(0.5f); // 넉백 딜레이 0.5초

        rigid.velocity = Vector3.zero; // 속도 원위치

    }



}

