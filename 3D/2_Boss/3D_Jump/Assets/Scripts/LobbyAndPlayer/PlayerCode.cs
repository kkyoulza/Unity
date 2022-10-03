using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCode : MonoBehaviour
{
    // �÷��̾� ����
    public int playerHealth; // �÷��̾� ü��
    public int playerStrength; // �÷��̾� ��
    public int playerAccuracy; // �÷��̾� ���߷�

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


    // ���� ����(bool)
    bool isSwap; // ��� �ٲٰ� �ִ°�?
    bool isJump; // ������ �ϰ� �ִ°�?
    bool isDodge; // ȸ�Ǹ� �ϰ� �ִ°�?(������)
    bool DodgeCool; // ȸ�� ��Ÿ�����ΰ�?
    bool isAtkReady; // ���� �غ� �Ǿ��°�?
    bool isBorder; // ��輱�� ��ҳ�?
    bool isReloading; // ���������ΰ�?
    bool isDamage; // �ǰ� ���ϰ� �ִ� ���ΰ�?


    Vector3 moveVec;
    Vector3 dodgeVec; // ȸ�� ����

    public float playerSpeed;
    public int bullet; // �Ѿ� ����

    //���� ����
    public GameObject[] WeaponList; // Ȱ��ȭ �� ���� ����Ʈ
    public bool[] hasWeapons; // � ���⸦ ������ �ִ°�?

    public GameObject nearObject; // ��ó�� ������ �ִ� ������ ������Ʈ
    Weapon cntEquipWeapon; // ���� �����ϰ� �ִ� ����

    int cntindexWeapon = -1; // ���� ���� �ִ� ���� index, �ʱ� ���� -1�� �� �ش�.

    PlayerItem playerItem; // �÷��̾ ������ �ִ� ������ �������� �������

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

    private void Awake()
    {
        isDamage = false;

        ui = UIManager.GetComponent<UIManager>(); // ui �Ŵ��� ������Ʈ
        anim = GetComponentInChildren<Animator>(); // �ڽ� ������Ʈ�� �ִ� ������Ʈ�� �����´�.
        coolManager = manager.GetComponent<CoolTimeManager>(); // ��Ÿ�� �Ŵ����� �ҷ��´�.
        playerItem = GetComponent<PlayerItem>(); // ������ ������Ʈ
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>(); // ���� ���� �������� ���̴� Components s �� ���̱�!
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
        isAtkReady = cntEquipWeapon.AtkDelay < AtkDelay; // ���� �ӵ�(���� ������)���� ���� ������ ���� Ŭ����!

        if(AtkDown && isAtkReady && !isDodge && !isSwap)
        {
            cntEquipWeapon.Use(); // ������ �غ� �Ǿ����� �����ϰ� �������� ����!
            anim.SetTrigger(cntEquipWeapon.type == Weapon.AtkType.Melee ? "DoSwing" : "DoShot"); // 3�� �����ڸ� �̿��Ͽ� �� �ٷ� �� ���� ������ �ִϸ��̼��� �����Ѵ�.
            // 3�� �������� ����� �����ϰ� �����ϴٴ� ���� ���!
            AtkDelay = 0f;
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
        if (sDown1 && (!hasWeapons[0] || cntindexWeapon == 0)) // 1���� ������ ��, ������ ���߰ų�, �̹� ���� ���� �����ϰ� ���� ��
            return; // ���� x
        if (sDown2 && (!hasWeapons[1] || cntindexWeapon == 1)) // 2���� ������ ��, ������ ���߰ų�, �̹� ���� ���� �����ϰ� ���� ��
            return; // ���� x
        if (sDown3 && (!hasWeapons[2] || cntindexWeapon == 2)) // 3���� ������ ��, ������ ���߰ų�, �̹� ���� ���� �����ϰ� ���� ��
            return; // ���� x

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isSwap && !isDodge) // 1~3�� Ű�� �����鼭 �������� �ƴ� ��! + ���⸦ �Ծ��� ��!
        {
            if(cntEquipWeapon != null) // �̹� �ٸ� ���⸦ ���� ���� ��!
            {
                cntEquipWeapon.gameObject.SetActive(false); // ���� ����!
            }
            cntEquipWeapon = WeaponList[weaponIndex].GetComponent<Weapon>();
            cntindexWeapon = weaponIndex;
            cntEquipWeapon.gameObject.SetActive(true);

            isSwap = true;
            anim.SetTrigger("DoSwap");

            Invoke("SwapOut",0.3f);

        }
    }

    void SwapOut() // ���� ������ ����!
    {
        isSwap = false;
    }

    void InterAction()
    {
        if(iDown && nearObject != null && !isJump && !isSwap)
        {
            // ���⿡ ��� �ְ�(��ó ������Ʈ�� null�� �ƴ�), ���� ���°� �ƴ� ��, ��ȣ�ۿ� ��ư�� ������ �Ǹ� �������� �����ϰ� �ȴ�.
            // �̰��� �����Ͽ� NPC�͵� ��ȭ�� �ϰԲ�?

            if(nearObject.tag == "weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; // value�� index�� ���� �� ��!

                ui.PopUI.SetActive(false);
                ui.isNoticeOn = false; // ���⸦ �Ծ��� �� ��Ȱ��ȭ!
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
        Invoke("DodgeCoolDown", dodgeCoolTime); // ��Ÿ���� 3��!
    }

    void DodgeCoolDown()
    {
        Debug.Log("ȸ�� ��Ÿ�� ����!");
        coolManager.coolOn = true;
        coolManager.SetAble();
        DodgeCool = false;
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

