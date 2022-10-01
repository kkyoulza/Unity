using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    string[] names = new string[] { "�ƴٸ�Ƽ�� �ظ�", "��", "�ӽ� ��" };
    
    // �ܺ� ������
    public GameObject player; // player ��ü

    PlayerCode playerInfo; // player ���� ����(���� ���� ���� Ȯ�ο�)
    PlayerItem playerItem; // player ������ ������ �������� ����

    public GameObject enchantManager; // ��ȭ �Ŵ��� ��ü
    Enchanting enchant; // ��ȭ ���� ���� �ڵ�
    public int weaponIndex; // playerItem���� ���� ��ȭ���� ������ index (������ �����ϱ� ����)

    //UI ��ü��
    public GameObject PopUI; // pop���� ���� UI�� ��� �� ����
    public GameObject DialogPanel; // ��ȭ �г�
    public GameObject ItemUI; // ������ â UI
    public GameObject EnchantPanel; // ��ȭ â

    public Text WeaponName; // ���� �̸�
    public Text DialogTxt; // ��ȭ �ؽ�Ʈ
    public Text EnchantStep; // ��ȭ �ܰ�
    public Text addAtk; // ��ȭ �߰� ���ݷ�
    public Text minusDelay; // ��ȭ ������ ���� ��ġ
    public Text addCritical; // ��ȭ ũ��Ƽ�� Ȯ��
    public Text successPercent; // ���� Ȯ��
    public Text enchantMoney; // ��ȭ ���

    public Image WeaponImg; // ���� �̹���
    public Sprite img1;
    public Sprite img2;
    public Sprite img3;

    // ���� ����
    public bool isNoticeOn; // e�� ������� �ȳ����� ���� �ִ°�?

    // NPC
    public GameObject Smith; // ��������
    Animator animSmith; // �������� �ִϸ��̼�

    // Start is called before the first frame update
    void Start()
    {
        animSmith = Smith.GetComponentInChildren<Animator>();
        playerItem = player.GetComponent<PlayerItem>();
        enchant = enchantManager.GetComponent<Enchanting>();
        isNoticeOn = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnchantWeaponUI()
    {
        DialogPanel.SetActive(true);
        animSmith.SetTrigger("DoTalk");
    }

    public void ShowEnchantUI() // ��ȭ �ϱ⸦ ������ �� (��ȭâ -> ��ȭâ)
    {
        if(playerItem.weapons[0] != null)
        {
            DialogPanel.SetActive(false);
            EnchantPanel.SetActive(true);
        }
        else
        {
            DialogTxt.text = "���⸦ �ϳ��� ������ ���� �ʾ�, ���⸦ ���� ��� �÷�?";
        }

    }

    public void OffEnchantUI()
    {
        EnchantPanel.SetActive(false);
        enchant.isWeaponOn = false;
        weaponIndex = -1; // index �ʱ�ȭ
        ResetEnchantUI();
    }

    public void DisappearEnchantDialogUI()
    {
        DialogPanel.SetActive(false);
        DialogTxt.text = "�ȳ�, ���� ��ȭ �Ϸ��� �Ծ�?";
    }

    public void OnItemUI()
    {
        ItemUI.SetActive(true);
    }

    public void OffItemUI()
    {
        ItemUI.SetActive(false);
    }

    public void NoticeOn()
    {
        PopUI.SetActive(true);

        Invoke("NoticeOff", 1.5f);
    }

    public void NoticeOff()
    {
        PopUI.SetActive(false);
    }

    void fullEnchantNotice(WeaponItemInfo imsi)
    {
        WeaponName.text = names[imsi.weaponCode];
        EnchantStep.text = "�Ѱ�ġ���� ��ȭ �Ͽ����ϴ�. (10��)";
        addAtk.text = (imsi.enchantAtk+imsi.baseAtk).ToString();
        minusDelay.text = (imsi.baseDelay+imsi.enchantDelay).ToString("N2");
        addCritical.text = (imsi.criticalPercent * 100)+" %";
        successPercent.text = "-";
        enchantMoney.text = "-";

        switch (imsi.weaponCode)
        {
            case 0:
                WeaponImg.sprite = img1;
                break;
            case 1:
                WeaponImg.sprite = img2;
                break;
            case 2:
                WeaponImg.sprite = img3;
                break;
        }

    }

    public void ChangeEnchantWeapon(int itemCode)
    {
        WeaponItemInfo imsi = checkWeapon(itemCode);
        enchant.SetTarget(imsi);
        enchant.returnIndex = weaponIndex;

        switch (itemCode)
        {
            case 0:
                if (imsi.enchantCount == imsi.maxEnchant)
                {
                    fullEnchantNotice(imsi);
                    return;
                }    
                SetInfo(imsi);
                enchant.isWeaponOn = true;
                WeaponImg.sprite = img1;
                break;
            case 1:
                if (imsi.enchantCount == imsi.maxEnchant)
                {
                    fullEnchantNotice(imsi);
                    return;
                }
                SetInfo(imsi);
                enchant.isWeaponOn = true;
                WeaponImg.sprite = img2;
                break;
            case 2:
                if (imsi.enchantCount == imsi.maxEnchant)
                {
                    fullEnchantNotice(imsi);
                    return;
                }
                SetInfo(imsi);
                enchant.isWeaponOn = true;
                WeaponImg.sprite = img3;
                break;
        }
    }

    void ResetEnchantUI()
    {
        WeaponImg.sprite = null;

        WeaponName.text = "-";
        EnchantStep.text = "-";
        addAtk.text = "-";
        minusDelay.text = "-";
        addCritical.text = "-";
        successPercent.text = "-";
        enchantMoney.text = "-";

        enchant.ResetTarget(); // ��� �ʱ�ȭ
    }

    WeaponItemInfo checkWeapon(int itemCode)
    {

        for (int i = 0; i < playerItem.weapons.Length; i++)
        {
            if (playerItem.weapons[i] == null)
                break;
            if (playerItem.weapons[i].weaponCode == itemCode)
            {
                weaponIndex = i; // �ε��� ����
                return playerItem.weapons[i]; // ��ư�� �´� ���⸦ �Ծ��ٸ� true
            }
        }

        return null;
    }

    void SetInfo(WeaponItemInfo info)
    {
        WeaponName.text = names[info.weaponCode];
        EnchantStep.text = info.enchantCount + " �� " + (info.enchantCount + 1) + " �� ��ȭ"; // +1 �κ��� ��ȣ�� ���� �־�� �Ѵ�.
        switch (info.weaponCode) // UI ���� ����
        {
            case 0:
                addAtk.text = (info.baseAtk+info.enchantAtk) + " + " + enchant.addMeleeAtk[info.enchantCount]; // ���ݷ� ����ġ ����
                minusDelay.text = (info.baseDelay + info.enchantDelay).ToString("N2") + " - " + enchant.minusMeleeDelay[info.enchantCount]; 
                // ������ ����ġ ����, �����̴� �Ҽ��� 2��° �ڸ������� ǥ��("N2")
                addCritical.text = (info.criticalPercent * 100) + " % + " + (enchant.addMeleeCritical[info.enchantCount] * 100) + " %";
                break;
            case 1:
                addAtk.text = (info.baseAtk + info.enchantAtk) + " + " + enchant.addGunAtk[info.enchantCount];
                minusDelay.text = (info.baseDelay + info.enchantDelay).ToString("N2") + " - " + enchant.minusGunDelay[info.enchantCount];
                addCritical.text = (info.criticalPercent * 100) + " % + " + (enchant.addGunCritical[info.enchantCount] * 100) + " %";
                break;
            case 2:
                addAtk.text = (info.baseAtk + info.enchantAtk) + " + " + enchant.addMGunAtk[info.enchantCount];
                minusDelay.text = (info.baseDelay + info.enchantDelay).ToString("N2") + " - " + enchant.minusMGunDelay[info.enchantCount];
                addCritical.text = (info.criticalPercent * 100) + " % + " + (enchant.addMGunCritical[info.enchantCount] * 100) + " %";
                break;
        }

        successPercent.text = (enchant.percentage[info.enchantCount] * 100) + " %";
        enchantMoney.text = enchant.needGold[info.enchantCount] + " G";

    }

    public void doEnchant()
    {
        enchant.doEnchant();
        if (enchant.isWeaponOn && enchant.EnchantTarget.enchantCount == playerItem.weapons[weaponIndex].maxEnchant)
        {
            fullEnchantNotice(playerItem.weapons[weaponIndex]);
            return;
        }

        if(enchant.isWeaponOn && enchant.EnchantTarget.enchantCount < playerItem.weapons[weaponIndex].maxEnchant)
            SetInfo(playerItem.weapons[weaponIndex]);
    }

}
