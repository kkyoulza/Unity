using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCode : MonoBehaviour
{
    // 플레이어 정보
    public int playerMaxHealth; // 플레이어 최대 체력
    public int playerHealth; // 플레이어 체력
    public int playerMana; // 플레이어 현재 마나
    public int playerMaxMana; // 플레이어 최대 마나
    public int playerStrength; // 플레이어 힘
    public int playerAccuracy; // 플레이어 명중률

    public int playerMaxAtk; // 최대 공격력
    public int playerMinAtk; // 최소 공격력
    public SaveInformation saveJump; // 점프맵 정보

    // 명중률 계산 -> 기본 명중률 50% + 추가 명중률 에 log를 씌워서 10으로 나눈 후 더함 (기본 명중률 5, 0.5 + 0.06 = 0.56 정도에서 시작)
    // 공격력 계산 -> 최소 공격력 : (캐릭터 힘 + 무기 공격력) * 명중률(최대 95%)
    // 최대 공격력 : (기본 공격력 + 무기 공격력)
    // 데미지 : 최소~최대 공격력 랜덤 생성 - 몬스터의 방어력

    public int strEnchantCnt; // 힘 강화 횟수 (2의 카운트 개수 제곱만큼 기원조각 소모) 한 번 강화에 + 5
    public int accEnchantCnt; // 명중률 강화 횟수 (3의 카운트 개수 제곱만큼 기원조각 소모) 한 번 강화에 + 10 (실제 명중률은 5%,2%,2%... 이렇게 증가) 과투자 비추천
    public int HPEnchantCnt; // HP강화 횟수 (10 * 강화 횟수) 한 번 강화에 +10
    public int MPEnchantCnt; // MP강화 횟수 (20 * 강화 횟수) 한 번 강화에 +5

    // 골드 획득 관련
    public GameObject gainPos; // 골드 획득을 나타내 주는 Text 위치
    public GameObject gainGoldPrefab; // 골드획득 알림 텍스트 프리팹
    public GameObject gainOriginPrefab; // 기원조각 획득 시 알림 프리팹
    GameObject gainText;

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
    bool statusDown; // 캐릭터 스텟 창 키
    bool equipDown; // 캐릭터 장비 창 키
    bool HPKey; // HP포션 키
    bool MPKey; // MP포션 키
    bool escKey; // esc 키

    // 스킬 키 모음
    bool spreadShotKey; // range1 skill key


    // 상태 변수(bool)
    bool isSwap; // 장비를 바꾸고 있는가?
    bool isJump; // 점프를 하고 있는가?
    bool isDodge; // 회피를 하고 있는가?(구르기)
    bool DodgeCool; // 회피 쿨타임중인가?
    bool isAtkReady; // 공격 준비가 되었는가?
    bool isBorder; // 경계선에 닿았나?
    bool isReloading; // 재장전중인가?
    bool isDamage; // 피격 당하고 있는 중인가?
    public bool isTalk; // 대화 중인가? (대회중일때는 움직이지 못하게!)

    Vector3 moveVec;
    Vector3 dodgeVec; // 회피 방향

    public float playerSpeed;
    public int bullet; // 총알 개수

    //무기 관련
    public GameObject[] WeaponList; // 활성화 할 무기 리스트
    public bool[] hasWeapons; // 어떤 무기를 가지고 있는가?

    public GameObject nearObject; // 근처에 떨어져 있는 아이템 오브젝트
    public Weapon cntEquipWeapon; // 현재 장착하고 있는 무기

    int cntindexWeapon = -1; // 현재 끼고 있는 무기 index, 초기 값은 -1로 해 준다.

    // 플레이어의 타 컴포넌트
    PlayerItem playerItem; // 플레이어가 가지고 있는 아이템 정보들이 담겨있음
    PlayerSkills playerSkills; // 플레이어가 가지고 있는 스킬들

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

    // 정보 저장 코드
    SaveInfos saveinfo;

    private void Awake()
    {
        isDamage = false;
        ui = UIManager.GetComponent<UIManager>(); // ui 매니저 컴포넌트
        anim = GetComponentInChildren<Animator>(); // 자식 오브젝트에 있는 컴포넌트를 가져온다.
        coolManager = manager.GetComponent<CoolTimeManager>(); // 쿨타임 매니저를 불러온다.

        playerItem = GetComponent<PlayerItem>(); // 아이템 컴포넌트
        playerSkills = GetComponent<PlayerSkills>(); // 스킬 정보들

        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>(); // 여러 개를 가져오는 것이니 Components s 꼭 붙이기!
        saveinfo = GameObject.FindGameObjectWithTag("saveInfo").GetComponent<SaveInfos>();
        saveJump = GameObject.FindGameObjectWithTag("information").GetComponent<SaveInformation>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
        if (!isTalk) // 대화중이지 않을 때만 가능하게!
        {
            Move_Ani(); // 캐릭터 움직임
            Jump(); // 점프
            Attack(); // 공격
            UseSkills();
            ReLoad(); // 재장전
            TrunChar(); // 캐릭터 회전
            Dodge(); // 캐릭터 구르기
            Swap(); // 캐릭터 무기 변경
            InterAction(); // 상호작용
            UseItem(); // 소비 아이템 사용(단축키 이용)
        }
        onUI(); // 캐릭터 UI창 열기
        calStatus(); // 캐릭터 스탯 계산
        checkHP(); // 남은 HP체크
        saveinfo.savePlayerStats(playerMaxHealth, playerHealth, playerMana,playerMaxMana, playerStrength, playerAccuracy, playerItem.playerCntGold,
            playerItem.enchantOrigin,playerItem.cntHPPotion,playerItem.cntMPPotion);
        saveinfo.saveStatCnt(strEnchantCnt, accEnchantCnt, HPEnchantCnt, MPEnchantCnt);
        // 플레이어 자체 스탯, 골드 양 저장
    }
    
    public void calStatus()
    {
        // 스탯 계산
        if (cntEquipWeapon == null)
        {
            playerMaxAtk = 0;
            playerMinAtk = 0;
        }
        else
        {
            playerMaxAtk = cntEquipWeapon.Damage + playerStrength;
            playerMinAtk = (int)((cntEquipWeapon.Damage + playerStrength) * (0.5f + 0.1f * Mathf.Log(playerAccuracy, 10)));
        }
        

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

    void UseSkills()
    {
        if (spreadShotKey)
        {
            StartCoroutine(playerSkills.onRangeSkillCoolTime(0));
            StartCoroutine(playerSkills.onRangeSkill(0));
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
        statusDown = Input.GetKeyDown(KeyCode.U);
        equipDown = Input.GetKeyDown(KeyCode.Y);

        HPKey = Input.GetKeyDown(KeyCode.F);
        MPKey = Input.GetKeyDown(KeyCode.G);

        spreadShotKey = Input.GetKeyDown(KeyCode.H);


        escKey = Input.GetButtonDown("Cancel");

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

        if (statusDown)
        {
            if (!ui.StatusUI.activeSelf)
                ui.OnStatusUI();
            else
                ui.OffStatusUI();

        }

        if (equipDown)
        {
            if (!ui.equipUI.activeSelf)
                ui.OnEquipUI();
            else
                ui.OffEquipUI();
        }

        if (escKey)
        {
            ui.onOffExitUI();
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
            if(playerMana < 2)
            {
                StartCoroutine(ui.noticeEtc(8));
                return;
            }
            playerMana -= 2;
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
        if (sDown1 && (!hasWeapons[1] || cntindexWeapon == 1)) // 1번을 눌렀을 때, 습득을 안했거나, 이미 같은 것을 장착하고 있을 때
            return; // 실행 x
        if (sDown2 && (!hasWeapons[2] || cntindexWeapon == 2)) // 2번을 눌렀을 때, 습득을 안했거나, 이미 같은 것을 장착하고 있을 때
            return; // 실행 x
        if (sDown3 && (!hasWeapons[3] || cntindexWeapon == 3)) // 3번을 눌렀을 때, 습득을 안했거나, 이미 같은 것을 장착하고 있을 때
            return; // 실행 x

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 1;
        if (sDown2) weaponIndex = 2;
        if (sDown3) weaponIndex = 3;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isSwap && !isDodge) // 1~3번 키가 눌리면서 점프중이 아닐 때! + 무기를 먹었을 때!
        {
            if(cntEquipWeapon != null) // 이미 다른 무기를 끼고 있을 때!
            {
                cntEquipWeapon.gameObject.SetActive(false); // 먼저 해제!
            }
            cntEquipWeapon = WeaponList[weaponIndex].GetComponent<Weapon>();
            cntindexWeapon = weaponIndex;
            cntEquipWeapon.gameObject.SetActive(true);
            
            if(cntEquipWeapon.itemCode == 2 || cntEquipWeapon.itemCode == 3)
            {
                ui.bulletUI.SetActive(true);
            }
            else
            {
                ui.bulletUI.SetActive(false);
            }

            isSwap = true;
            anim.SetTrigger("DoSwap");

            Invoke("SwapOut",0.3f);

        }
    }

    void SwapOut() // 스왑 딜레이 설정!
    {
        isSwap = false;
    }

    void checkHP()
    {
        if(playerHealth <= 0 && !isTalk)
        {
            isDamage = true;
            isTalk = true;
            anim.SetBool("isDie", true);
            StartCoroutine(GoLobby());
        }
    }

    IEnumerator GoLobby()
    {
        StartCoroutine(ui.noticeEtc(9999)); // 죽었을 때 PopUI 알림

        yield return new WaitForSeconds(3.0f); // 3초 대기

        isTalk = false;
        isDamage = false;
        anim.SetBool("isDie", false);
        playerHealth = playerMaxHealth;
        playerMana = playerMaxMana;

        yield return null;

        SceneManager.LoadScene("Boss1");

        yield return null;

    }

    void InterAction()
    {
        if(iDown && nearObject != null && !isJump && !isSwap)
        {
            // 무기에 닿고 있고(근처 오브젝트가 null이 아님), 점프 상태가 아닐 때, 상호작용 버튼을 누르게 되면 아이템을 습득하게 된다.
            // 이것을 응용하여 NPC와도 대화를 하게끔?

            if(nearObject.layer == 14)
            {
                Item nearObjItem = nearObject.GetComponent<Item>();
                switch (nearObjItem.type)
                {
                    case Item.Type.Weapon:
                        int weaponIndex = nearObjItem.value; // value를 index로 설정 할 것!

                        ui.PopUI.SetActive(false);
                        ui.isNoticeOn = false; // 무기를 먹었을 때 비활성화!
                        hasWeapons[weaponIndex] = true;

                        playerItem.GetInfo(nearObject);

                        saveinfo.SaveItemInfo(playerItem.weapons[playerItem.returnIndex(weaponIndex)]);
                        // 아이템 정보 세이브, 이 자체가 포인터에 의한 참조?가 되어서 강화를 할 때, 강화 창에서만 갱신을 해도 세이브 자료에서도 반영이 되게 된다. 

                        Destroy(nearObject);
                        break;
                    case Item.Type.Coin:
                        Coin nearCoin = nearObject.GetComponent<Coin>();

                        gainText = MonoBehaviour.Instantiate(gainGoldPrefab);
                        gainText.GetComponent<Damage>().damage = nearCoin.addGold;
                        gainText.transform.position = gainPos.transform.position;

                        ui.PopUI.SetActive(false);
                        ui.isNoticeOn = false; // 무기를 먹었을 때 비활성화!
                        playerItem.playerCntGold += nearCoin.addGold; // 오르는 값 만큼 더한다.
                        Destroy(nearObject);
                        break;
                    case Item.Type.Origin:
                        
                        gainText = MonoBehaviour.Instantiate(gainOriginPrefab);
                        gainText.transform.position = gainPos.transform.position;

                        ui.PopUI.SetActive(false);
                        ui.isNoticeOn = false; // 무기를 먹었을 때 비활성화!
                        playerItem.enchantOrigin += 1; // 기원조각 1개 더함!
                        Destroy(nearObject);
                        break;

                }
                
            }
            else if (nearObject.layer == 13) // NPC layer
            {
                isTalk = true;
                switch (nearObject.tag)
                {
                    case "Smith":
                        ui.PopUI.SetActive(false);
                        ui.EnchantWeaponUI(0);
                        break;
                    case "Shop":
                        ui.PopUI.SetActive(false);
                        ui.shopUIPanel.SetActive(true);
                        break;
                    case "showRank":
                        ui.onOffRankingUI();
                        break;
                }
                
            }
            else if(nearObject.layer == 16) // Portal layer
            {
                
                switch (nearObject.tag)
                {
                    case "GoStage1":
                        isTalk = true;
                        ui.PopUI.SetActive(false);
                        ui.EnchantWeaponUI(1);
                        break;
                    case "GoLobby":
                        SceneManager.LoadScene("Boss1");
                        break;
                    case "GoJump":
                        isTalk = true;
                        ui.PopUI.SetActive(false);
                        ui.EnchantWeaponUI(2);
                        break;
                    case "reward":
                        DungeonUI dungeon = GameObject.FindGameObjectWithTag("dungeonUI").GetComponent<DungeonUI>();
                        Animator aniBox = GameObject.FindGameObjectWithTag("topBox").GetComponent<Animator>();
                        aniBox.SetTrigger("open");
                        dungeon.setReward();
                        break;

                }
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

    void UseItem()
    {
        if (HPKey)
        {
            if(playerItem.cntHPPotion > 0)
            {
                playerItem.cntHPPotion--;
                playerHealth = (playerHealth + 30 > playerMaxHealth) ? playerMaxHealth : playerHealth + 30;
            }
            else
            {
                StartCoroutine(ui.noticeEtc(7));
            }
        }

        if (MPKey)
        {
            if (playerItem.cntMPPotion > 0)
            {
                playerItem.cntMPPotion--;
                playerMana = (playerMana + 10 > playerMaxMana) ? playerMaxMana : playerMana + 10;
            }
            else
            {
                StartCoroutine(ui.noticeEtc(7));
            }
        }
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
        if (other.gameObject.layer == 14 && !ui.isNoticeOn)
        {
            nearObject = other.gameObject;
            ui.NoticeOn();
            ui.isNoticeOn = true;
        }
        else if((other.gameObject.layer == 13 || other.gameObject.layer == 16) && !ui.isNoticeOn)
        {
            nearObject = other.gameObject;
            ui.NoticeOn();
            ui.isNoticeOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 14)
        {
            nearObject = null;
            ui.NoticeOff();
            ui.isNoticeOn = false;
        }
        else if (other.gameObject.layer == 13 || other.gameObject.layer == 16)
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
        }else if(other.tag == "easter1")
        {
            StartCoroutine(ui.noticeEtc(999));
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

