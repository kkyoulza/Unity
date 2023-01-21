using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCode : MonoBehaviour
{
    // �÷��̾� ����
    public int playerMaxHealth; // �÷��̾� �ִ� ü��
    public int playerHealth; // �÷��̾� ü��
    public int playerMana; // �÷��̾� ���� ����
    public int playerMaxMana; // �÷��̾� �ִ� ����
    public int playerStrength; // �÷��̾� ��
    public int playerAccuracy; // �÷��̾� ���߷�

    public int playerMaxAtk; // �ִ� ���ݷ�
    public int playerMinAtk; // �ּ� ���ݷ�
    public SaveInformation saveJump; // ������ ����

    // ���߷� ��� -> �⺻ ���߷� 50% + �߰� ���߷� �� log�� ������ 10���� ���� �� ���� (�⺻ ���߷� 5, 0.5 + 0.06 = 0.56 �������� ����)
    // ���ݷ� ��� -> �ּ� ���ݷ� : (ĳ���� �� + ���� ���ݷ�) * ���߷�(�ִ� 95%)
    // �ִ� ���ݷ� : (�⺻ ���ݷ� + ���� ���ݷ�)
    // ������ : �ּ�~�ִ� ���ݷ� ���� ���� - ������ ����

    public int strEnchantCnt; // �� ��ȭ Ƚ�� (2�� ī��Ʈ ���� ������ŭ ������� �Ҹ�) �� �� ��ȭ�� + 5
    public int accEnchantCnt; // ���߷� ��ȭ Ƚ�� (3�� ī��Ʈ ���� ������ŭ ������� �Ҹ�) �� �� ��ȭ�� + 10 (���� ���߷��� 5%,2%,2%... �̷��� ����) ������ ����õ
    public int HPEnchantCnt; // HP��ȭ Ƚ�� (10 * ��ȭ Ƚ��) �� �� ��ȭ�� +10
    public int MPEnchantCnt; // MP��ȭ Ƚ�� (20 * ��ȭ Ƚ��) �� �� ��ȭ�� +5

    // ��� ȹ�� ����
    public GameObject gainPos; // ��� ȹ���� ��Ÿ�� �ִ� Text ��ġ
    public GameObject gainGoldPrefab; // ���ȹ�� �˸� �ؽ�Ʈ ������
    public GameObject gainOriginPrefab; // ������� ȹ�� �� �˸� ������
    GameObject gainText;

    // ���� ����
    float horAxis; // ���� ����Ű �Է� �� ���� ���� ����
    float verAxis; // ���� ����

    float AtkDelay; // ���� ������
    float dodgeCoolTime = 3.0f; // ȸ�� ��Ÿ��

    // Ű �Է� ����(bool)
    bool runDown; // �뽬 ��ư�� ���ȴ°�?
    bool jumpDown; // ���� Ű�� ���ȴ°�?
    bool dodgeDown; // ȸ�� Ű�� ���ȴ°�?
    bool iDown;  // ���� Ű�� ���ȴ°�?
    bool sDown1; // 1�� ���
    bool sDown2; // 2
    bool sDown3; // 3
    bool AtkDown; // ���� Ű
    bool rDown; // ������ Ű
    bool itemDown; // ������ â Ű 
    bool statusDown; // ĳ���� ���� â Ű
    bool equipDown; // ĳ���� ��� â Ű
    bool HPKey; // HP���� Ű
    bool MPKey; // MP���� Ű
    bool escKey; // esc Ű

    // ��ų Ű ����
    bool spreadShotKey; // range1 skill key


    // ���� ����(bool)
    bool isSwap; // ��� �ٲٰ� �ִ°�?
    bool isJump; // ������ �ϰ� �ִ°�?
    bool isDodge; // ȸ�Ǹ� �ϰ� �ִ°�?(������)
    bool DodgeCool; // ȸ�� ��Ÿ�����ΰ�?
    bool isAtkReady; // ���� �غ� �Ǿ��°�?
    bool isBorder; // ��輱�� ��ҳ�?
    bool isReloading; // ���������ΰ�?
    bool isDamage; // �ǰ� ���ϰ� �ִ� ���ΰ�?
    public bool isTalk; // ��ȭ ���ΰ�? (��ȸ���϶��� �������� ���ϰ�!)

    Vector3 moveVec;
    Vector3 dodgeVec; // ȸ�� ����

    public float playerSpeed;
    public int bullet; // �Ѿ� ����

    //���� ����
    public GameObject[] WeaponList; // Ȱ��ȭ �� ���� ����Ʈ
    public bool[] hasWeapons; // � ���⸦ ������ �ִ°�?

    public GameObject nearObject; // ��ó�� ������ �ִ� ������ ������Ʈ
    public Weapon cntEquipWeapon; // ���� �����ϰ� �ִ� ����

    int cntindexWeapon = -1; // ���� ���� �ִ� ���� index, �ʱ� ���� -1�� �� �ش�.

    // �÷��̾��� Ÿ ������Ʈ
    PlayerItem playerItem; // �÷��̾ ������ �ִ� ������ �������� �������
    PlayerSkills playerSkills; // �÷��̾ ������ �ִ� ��ų��

    //��Ÿ�� ����
    public GameObject manager;
    CoolTimeManager coolManager;

    // �ִϸ��̼�, ���� ����
    Animator anim;
    Rigidbody rigid;
    MeshRenderer[] meshs; // �÷��̾��� ��� �Ž���(��, �Ӹ�, �� ��)�� �������� ���� �迭�� �����Ͽ���

    // UI ����
    public GameObject UIManager;
    UIManager ui;

    // ���� ���� �ڵ�
    SaveInfos saveinfo;

    private void Awake()
    {
        isDamage = false;
        ui = UIManager.GetComponent<UIManager>(); // ui �Ŵ��� ������Ʈ
        anim = GetComponentInChildren<Animator>(); // �ڽ� ������Ʈ�� �ִ� ������Ʈ�� �����´�.
        coolManager = manager.GetComponent<CoolTimeManager>(); // ��Ÿ�� �Ŵ����� �ҷ��´�.

        playerItem = GetComponent<PlayerItem>(); // ������ ������Ʈ
        playerSkills = GetComponent<PlayerSkills>(); // ��ų ������

        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>(); // ���� ���� �������� ���̴� Components s �� ���̱�!
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
        if (!isTalk) // ��ȭ������ ���� ���� �����ϰ�!
        {
            Move_Ani(); // ĳ���� ������
            Jump(); // ����
            Attack(); // ����
            UseSkills();
            ReLoad(); // ������
            TrunChar(); // ĳ���� ȸ��
            Dodge(); // ĳ���� ������
            Swap(); // ĳ���� ���� ����
            InterAction(); // ��ȣ�ۿ�
            UseItem(); // �Һ� ������ ���(����Ű �̿�)
        }
        onUI(); // ĳ���� UIâ ����
        calStatus(); // ĳ���� ���� ���
        checkHP(); // ���� HPüũ
        saveinfo.savePlayerStats(playerMaxHealth, playerHealth, playerMana,playerMaxMana, playerStrength, playerAccuracy, playerItem.playerCntGold,
            playerItem.enchantOrigin,playerItem.cntHPPotion,playerItem.cntMPPotion);
        saveinfo.saveStatCnt(strEnchantCnt, accEnchantCnt, HPEnchantCnt, MPEnchantCnt);
        // �÷��̾� ��ü ����, ��� �� ����
    }
    
    public void calStatus()
    {
        // ���� ���
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
        isAtkReady = cntEquipWeapon.AtkDelay < AtkDelay; // ���� �ӵ�(���� ������)���� ���� ������ ���� Ŭ����!

        if(AtkDown && isAtkReady && !isDodge && !isSwap)
        {
            cntEquipWeapon.Use(); // ������ �غ� �Ǿ����� �����ϰ� �������� ����!
            anim.SetTrigger(cntEquipWeapon.type == Weapon.AtkType.Melee ? "DoSwing" : "DoShot"); // 3�� �����ڸ� �̿��Ͽ� �� �ٷ� �� ���� ������ �ִϸ��̼��� �����Ѵ�.
            // 3�� �������� ����� �����ϰ� �����ϴٴ� ���� ����!
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

        if (bullet == 0) // ���� �Ѿ��� 0���̸� �ȵȴ�.
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isAtkReady && !isReloading)
        {
            // ������ Ű�� ������, ������,ȸ����,���ⱳü���� �ƴҶ��̸鼭 ���� �غ� �Ǿ��� �� ����ǰ� �Ѵ�.
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
            reCount = bullet - (cntEquipWeapon.cntCount - cntEquipWeapon.maxCount); // ���� �Ѿ� �������� ��ġ�� �κ��� �� ��ŭ�� �����ؾ� �Ѵ�.
            cntEquipWeapon.cntCount = cntEquipWeapon.maxCount;
        }

        bullet -= reCount; // �÷��̾��� �Ѿ� ���� ����

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
        moveVec = new Vector3(horAxis, 0, verAxis).normalized; // ��� �������� �̵��ϴ��� ���� �ӵ��� ���Բ� ����� �ش�. (��������ȭ)

        if (isSwap || isReloading) // ��� ���� �߿��� �������� ���ϰ�! or ������ �ÿ��� �������� ���ϰ�!(!isReloading���� �ϰ� �Ʒ��� �־ ���� �մ� ���װ� �����. ���� �� ���� ��!)
            moveVec = Vector3.zero;

        if(!isBorder || isReloading) // ���� ������ ���� ���� �������� ���ϰ�! (ȸ���� ��)
            transform.position += moveVec * playerSpeed * (runDown ? 1.7f : 1.0f) * Time.deltaTime; // deltaTime�� ��ǻ�� ȯ�濡 �̵� �Ÿ��� ������ ���� �ʰ� �ϱ� ����!
            // �޸� ���� �� ������! - ���� �����ڸ� �̿��Ͽ���!

        anim.SetBool("IsWalk", moveVec != Vector3.zero); // �Է��� ���� �� �������� �ϴ� ���ǹ��� �־���.
        anim.SetBool("IsRun", runDown);

    }

    void TrunChar()
    {
        transform.LookAt(transform.position + moveVec); // ���ư��� �������� �ڵ����� ȸ���ǰ� �Ѵ�.
    }

    void Jump()
    {
        if (jumpDown && !isJump && !isSwap) // ����Ű�� ������ ���� ���°� �ƴ� ��!
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("DoJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (dodgeDown && moveVec != Vector3.zero && !DodgeCool && !isSwap) // Z Ű�� ������ ���� ���°� �ƴ� ��!
        {
            if(playerMana < 2)
            {
                StartCoroutine(ui.noticeEtc(8));
                return;
            }
            playerMana -= 2;
            dodgeVec = moveVec; // ������ ������ �̵� ����
            playerSpeed *= 2;
            anim.SetTrigger("DoDodge");
            isDodge= true;
            DodgeCool = true;
            Invoke("DoDodge", 0.5f);
        }
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[1] || cntindexWeapon == 1)) // 1���� ������ ��, ������ ���߰ų�, �̹� ���� ���� �����ϰ� ���� ��
            return; // ���� x
        if (sDown2 && (!hasWeapons[2] || cntindexWeapon == 2)) // 2���� ������ ��, ������ ���߰ų�, �̹� ���� ���� �����ϰ� ���� ��
            return; // ���� x
        if (sDown3 && (!hasWeapons[3] || cntindexWeapon == 3)) // 3���� ������ ��, ������ ���߰ų�, �̹� ���� ���� �����ϰ� ���� ��
            return; // ���� x

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 1;
        if (sDown2) weaponIndex = 2;
        if (sDown3) weaponIndex = 3;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isSwap && !isDodge) // 1~3�� Ű�� �����鼭 �������� �ƴ� ��! + ���⸦ �Ծ��� ��!
        {
            if(cntEquipWeapon != null) // �̹� �ٸ� ���⸦ ���� ���� ��!
            {
                cntEquipWeapon.gameObject.SetActive(false); // ���� ����!
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

    void SwapOut() // ���� ������ ����!
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
        StartCoroutine(ui.noticeEtc(9999)); // �׾��� �� PopUI �˸�

        yield return new WaitForSeconds(3.0f); // 3�� ���

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
            // ���⿡ ��� �ְ�(��ó ������Ʈ�� null�� �ƴ�), ���� ���°� �ƴ� ��, ��ȣ�ۿ� ��ư�� ������ �Ǹ� �������� �����ϰ� �ȴ�.
            // �̰��� �����Ͽ� NPC�͵� ��ȭ�� �ϰԲ�?

            if(nearObject.layer == 14)
            {
                Item nearObjItem = nearObject.GetComponent<Item>();
                switch (nearObjItem.type)
                {
                    case Item.Type.Weapon:
                        int weaponIndex = nearObjItem.value; // value�� index�� ���� �� ��!

                        ui.PopUI.SetActive(false);
                        ui.isNoticeOn = false; // ���⸦ �Ծ��� �� ��Ȱ��ȭ!
                        hasWeapons[weaponIndex] = true;

                        playerItem.GetInfo(nearObject);

                        saveinfo.SaveItemInfo(playerItem.weapons[playerItem.returnIndex(weaponIndex)]);
                        // ������ ���� ���̺�, �� ��ü�� �����Ϳ� ���� ����?�� �Ǿ ��ȭ�� �� ��, ��ȭ â������ ������ �ص� ���̺� �ڷῡ���� �ݿ��� �ǰ� �ȴ�. 

                        Destroy(nearObject);
                        break;
                    case Item.Type.Coin:
                        Coin nearCoin = nearObject.GetComponent<Coin>();

                        gainText = MonoBehaviour.Instantiate(gainGoldPrefab);
                        gainText.GetComponent<Damage>().damage = nearCoin.addGold;
                        gainText.transform.position = gainPos.transform.position;

                        ui.PopUI.SetActive(false);
                        ui.isNoticeOn = false; // ���⸦ �Ծ��� �� ��Ȱ��ȭ!
                        playerItem.playerCntGold += nearCoin.addGold; // ������ �� ��ŭ ���Ѵ�.
                        Destroy(nearObject);
                        break;
                    case Item.Type.Origin:
                        
                        gainText = MonoBehaviour.Instantiate(gainOriginPrefab);
                        gainText.transform.position = gainPos.transform.position;

                        ui.PopUI.SetActive(false);
                        ui.isNoticeOn = false; // ���⸦ �Ծ��� �� ��Ȱ��ȭ!
                        playerItem.enchantOrigin += 1; // ������� 1�� ����!
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
        Invoke("DodgeCoolDown", dodgeCoolTime); // ��Ÿ���� 3��!
    }

    void DodgeCoolDown()
    {
        Debug.Log("ȸ�� ��Ÿ�� ����!");
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

    // ���� ���� �ذ�
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; // ȸ���ӵ��� 0���� ����� �ش�.
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 3, Color.red);
        isBorder = Physics.Raycast(transform.position + Vector3.up, moveVec, 3, LayerMask.GetMask("Wall"));
    }

    private void FixedUpdate()
    {
        FreezeRotation(); // ȸ�� �ӵ� 0���� ����!
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
            if (other.GetComponent<Rigidbody>() != null) // ���� �ð� �߿��� �߰������� ����ü�� �°� �Ǹ� ������Բ�!
                Destroy(other.gameObject);

            bool isNoDmgJumpAtk = other.name == "JumpAtkArea"; 
            if(isNoDmgJumpAtk && isDamage) // �����ð� �߿� ���� ���ݿ� �¾��� ��!
                StartCoroutine(noDamageNuckBack());

            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                playerHealth -= enemyBullet.damage;

                bool isBossAttack = other.name == "JumpAtkArea"; // ���� ���ݿ� �¾��� ��!

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

        if (isBossAttack) // ��� ������ �¾��� ��!
            rigid.AddForce(transform.forward * -40, ForceMode.Impulse); // �˹�!

        yield return new WaitForSeconds(1.0f); // ���� ������ 1��!

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAttack) // �˹� ��
            rigid.velocity = Vector3.zero; // �ӵ� ����ġ

    }

    IEnumerator noDamageNuckBack()
    {
        rigid.AddForce(transform.forward * -40, ForceMode.Impulse); // �˹�!

        yield return new WaitForSeconds(0.5f); // �˹� ������ 0.5��

        rigid.velocity = Vector3.zero; // �ӵ� ����ġ

    }



}
