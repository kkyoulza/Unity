using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    string[] names = new string[] { "","�ƴٸ�Ƽ�� �ظ�", "��", "�ӽ� ��" };

    RectTransform rectHP;
    RectTransform rectMP;

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
    public GameObject StatusUI; // ĳ���� ���� â UI
    public GameObject equipUI; // ĳ���� ��� â UI
    public GameObject EnchantPanel; // ��ȭ â

    // UI Text
    public Text popText; // popUI�� �ؽ�Ʈ

    // Enchant UI Text
    public Text WeaponName; // ���� �̸�
    public Text DialogTxt; // ��ȭ �ؽ�Ʈ
    public Text EnchantStep; // ��ȭ �ܰ�
    public Text addAtk; // ��ȭ �߰� ���ݷ�
    public Text minusDelay; // ��ȭ ������ ���� ��ġ
    public Text addCritical; // ��ȭ ũ��Ƽ�� Ȯ��
    public Text successPercent; // ���� Ȯ��
    public Text enchantMoney; // ��ȭ ���
    public Text originTxt; // ��ȭ �� ����� ��� ����
    public Text originAddPercent; // ��������� ���� ������ Ȯ�� ǥ�� �ؽ�Ʈ

    // Item UI
    public Text itemOriginTxt;
    public Text itemHP;
    public Text itemMP;
    public Text goldTxt; // ��� �ؽ�Ʈ

    public Text frontHP; // ����Ű������ HP���� ����
    public Text frontMP; // ����Ű������ MP���� ����

    // Status UI
    public Text Atk;
    public Text Str;
    public Text Acc;
    public Text HPTxt;
    public Text MPTxt;

    //Bullet UI
    public GameObject bulletUI;
    public Text maxBullet;
    public Text cntBullet;

    // ü��, ���� ����
    public GameObject HP;
    public GameObject MP;
    public Text maxHP;
    public Text cntHP;
    public Text maxMP;
    public Text cntMP;

    public Image WeaponImg; // ���� �̹���
    public Sprite img1;
    public Sprite img2;
    public Sprite img3;

    public Image hpBar;
    public Text healthText; // HP �ؽ�Ʈ

    // ���� ����
    public bool isNoticeOn; // e�� ������� �ȳ����� ���� �ִ°�?

    // NPC
    public GameObject Smith; // ��������
    Animator animSmith; // �������� �ִϸ��̼�

    // ���� �ӽ� ����
    public int useOrigin = 0; // ��� ������� ������� ����

    // Start is called before the first frame update
    void Start()
    {
        playerInfo = player.GetComponent<PlayerCode>();
        playerItem = player.GetComponent<PlayerItem>();
        if(enchantManager != null)
            enchant = enchantManager.GetComponent<Enchanting>();
        isNoticeOn = false;
        rectHP = HP.GetComponent<RectTransform>();
        rectMP = MP.GetComponent<RectTransform>();


}

    // Update is called once per frame
    void Update()
    {
        SetItemInfoUI();
        setBulletUI();
        SetBar();
    }

    public void setBulletUI()
    {
        maxBullet.text = playerInfo.bullet.ToString();
        if(playerInfo.cntEquipWeapon) // ���� ���⸦ ����ٸ�!
            cntBullet.text = playerInfo.cntEquipWeapon.cntCount.ToString()+" ("+playerInfo.cntEquipWeapon.maxCount.ToString()+")";
    }

    public void SetItemInfoUI()
    {
        goldTxt.text = playerItem.playerCntGold.ToString();
        itemOriginTxt.text = playerItem.enchantOrigin.ToString() + " ��";
        itemHP.text = playerItem.cntHPPotion.ToString() + " ��";
        itemMP.text = playerItem.cntMPPotion.ToString() + " ��";
        frontHP.text = playerItem.cntHPPotion.ToString();
        frontMP.text = playerItem.cntMPPotion.ToString();
    }

    public void SetBar()
    {
        // ü�¿� ���� Bar�� ũ�⸦ �����ϴ� ���̴�.
        rectHP.sizeDelta = new Vector2(194 * playerInfo.playerHealth/playerInfo.playerMaxHealth,24);
        rectMP.sizeDelta = new Vector2(194 * playerInfo.playerMana / playerInfo.playerMaxMana, 24);
        cntHP.text = playerInfo.playerHealth.ToString();
        maxHP.text = playerInfo.playerMaxHealth.ToString();
        cntMP.text = playerInfo.playerMana.ToString();
        maxMP.text = playerInfo.playerMaxMana.ToString();
    }

    public void EnchantWeaponUI()
    {
        animSmith = Smith.GetComponentInChildren<Animator>();
        DialogTxt.text = "�ȳ�, ���� ��ȭ �Ϸ��� �Ծ�?";
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
        enchant.ResetTarget();
        useOrigin = 0; // ������� ��� ���� ���� �ʱ�ȭ
        weaponIndex = -1; // index �ʱ�ȭ
        playerInfo.isTalk = false;
        ResetEnchantUI();
    }

    public void DisappearEnchantDialogUI()
    {
        playerInfo.isTalk = false;
        DialogPanel.SetActive(false);
    }

    public void OnItemUI()
    {
        ItemUI.SetActive(true);
    }

    public void OffItemUI()
    {
        ItemUI.SetActive(false);
    }

    public void OnStatusUI()
    {
        StatusUI.SetActive(true);
    }
    public void OffStatusUI()
    {
        StatusUI.SetActive(false);
    }

    public void OnEquipUI()
    {
        equipUI.SetActive(true);
    }

    public void OffEquipUI()
    {
        equipUI.SetActive(false);
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
        originAddPercent.text = "-";
        originTxt.text = "-";

        switch (imsi.weaponCode)
        {
            case 1:
                WeaponImg.sprite = img1;
                break;
            case 2:
                WeaponImg.sprite = img2;
                break;
            case 3:
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
            case 1:
                if (imsi.enchantCount == imsi.maxEnchant)
                {
                    fullEnchantNotice(imsi);
                    return;
                }    
                SetInfo(imsi);
                enchant.isWeaponOn = true;
                WeaponImg.sprite = img1;
                break;
            case 2:
                if (imsi.enchantCount == imsi.maxEnchant)
                {
                    fullEnchantNotice(imsi);
                    return;
                }
                SetInfo(imsi);
                enchant.isWeaponOn = true;
                WeaponImg.sprite = img2;
                break;
            case 3:
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
        originAddPercent.text = "( + " + (0.5f * useOrigin) + " %)";
        originTxt.text = "0";

        enchant.ResetTarget(); // ��� �ʱ�ȭ
    }

    WeaponItemInfo checkWeapon(int itemCode)
    {

        for (int i = 0; i < playerItem.weapons.Length; i++)
        {
            if (playerItem.weapons[i] == null || playerItem.weapons[i].baseAtk == 0)
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
            case 1:
                addAtk.text = (info.baseAtk+info.enchantAtk) + " + " + enchant.addMeleeAtk[info.enchantCount]; // ���ݷ� ����ġ ����
                minusDelay.text = (info.baseDelay + info.enchantDelay).ToString("N2") + " - " + enchant.minusMeleeDelay[info.enchantCount]; 
                // ������ ����ġ ����, �����̴� �Ҽ��� 2��° �ڸ������� ǥ��("N2")
                addCritical.text = (info.criticalPercent * 100) + " % + " + (enchant.addMeleeCritical[info.enchantCount] * 100) + " %";
                break;
            case 2:
                addAtk.text = (info.baseAtk + info.enchantAtk) + " + " + enchant.addGunAtk[info.enchantCount];
                minusDelay.text = (info.baseDelay + info.enchantDelay).ToString("N2") + " - " + enchant.minusGunDelay[info.enchantCount];
                addCritical.text = (info.criticalPercent * 100) + " % + " + (enchant.addGunCritical[info.enchantCount] * 100) + " %";
                break;
            case 3:
                addAtk.text = (info.baseAtk + info.enchantAtk) + " + " + enchant.addMGunAtk[info.enchantCount];
                minusDelay.text = (info.baseDelay + info.enchantDelay).ToString("N2") + " - " + enchant.minusMGunDelay[info.enchantCount];
                addCritical.text = (info.criticalPercent * 100) + " % + " + (enchant.addMGunCritical[info.enchantCount] * 100) + " %";
                break;
        }

        successPercent.text = (enchant.percentage[info.enchantCount] * 100) + " %";
        enchantMoney.text = enchant.needGold[info.enchantCount] + " G";
        useOrigin = 0;
        originAddPercent.text = "( + " + (0.5f * useOrigin) + " %)";
        originTxt.text = "0";

    }

    public void doEnchant()
    {
        enchant.doEnchant(useOrigin);
        goldTxt.text = playerItem.playerCntGold.ToString();
        if (enchant.isWeaponOn && enchant.EnchantTarget.enchantCount == playerItem.weapons[weaponIndex].maxEnchant)
        {
            fullEnchantNotice(playerItem.weapons[weaponIndex]);
            return;
        }

        if(enchant.isWeaponOn && enchant.EnchantTarget.enchantCount < playerItem.weapons[weaponIndex].maxEnchant)
            SetInfo(playerItem.weapons[weaponIndex]);
    }

    public void upOrigin()
    {
        if (enchant.isWeaponOn && enchant.EnchantTarget.enchantCount == playerItem.weapons[weaponIndex].maxEnchant) // Ǯ���� ���� ����� ���ϰ� �ؾ� �Ѵ�.
        {
            StartCoroutine(noticeEtc(1)); // �˸�!
            return;
        }

        if(enchant.EnchantTarget == null || enchant.EnchantTarget.baseAtk == 0)
        {
            return;
        }

        if(enchant.percentage[enchant.EnchantTarget.enchantCount] * 1000 + useOrigin * 5 >= 1000)
        {
            StartCoroutine(noticeEtc(3));
            return;
        }

        if (enchant.isWeaponOn && enchant.EnchantTarget.enchantCount < playerItem.weapons[weaponIndex].maxEnchant) // Ǯ���� �ƴϰ�, ���Ⱑ �÷��� ���� ��
        {
            if((playerItem.enchantOrigin - useOrigin - 1) >= 0) // ������ ������ (��� ������ ������ ����, �Ѱ� �� ��� �ص� �� ���� ������ ������!)
            {
                useOrigin++; // ��� ���� ������ �ø���.
                //int imsi = int.Parse(originTxt.text);
                //imsi++;
                originTxt.text = useOrigin.ToString();
                originAddPercent.text = "( + " + (0.5f * useOrigin) + " %)";
            }
            else
            {
                StartCoroutine(noticeEtc(2));
            }
            
        }

    }

    public void useAllOrigin()
    {
        // ��� ������� ��� (�ѵ� ����)

        if (enchant.EnchantTarget == null || enchant.EnchantTarget.baseAtk == 0)
        {
            // ��ȭ ��� �ƹ��͵� ���� ��
            return;
        }

        useOrigin = playerItem.enchantOrigin >= (1000 - (int)(enchant.percentage[enchant.EnchantTarget.enchantCount] * 1000)) / 5 
            ? (1000 - (int)(enchant.percentage[enchant.EnchantTarget.enchantCount] * 1000)) / 5 
            : playerItem.enchantOrigin ; // 100%���� �ǰ� �ϴ� ��� ���� ���! (������ �ִ� ������ ���ڶ�ٸ� ���� ���� ���!)
        originTxt.text = useOrigin.ToString();
        originAddPercent.text = "( + " + (0.5f * useOrigin) + " %)";
    }

    public void minusOrigin()
    {
        int imsi = int.Parse(originTxt.text);
        if (imsi == 0)
        {
            return;
        }
        else if(imsi > 0)
        {
            imsi--;
            useOrigin--;
            originAddPercent.text = "( + " + (0.5f * useOrigin) + " %)";
            originTxt.text = imsi.ToString();
        }
        
    }

    public IEnumerator noticeEtc(int type)
    {
        switch (type)
        {
            case 0: // ����� �������� �˷��� ��
                popText.text = "��ȭ ����� �����մϴ�!";
                PopUI.SetActive(true);
                break;
            case 1: // ��� ���� Ǯ�϶�
                popText.text = "��ȭ�� �Ѱ���� �Ͽ� ����� �� �����ϴ�!";
                PopUI.SetActive(true);
                break;
            case 2: // ��� ������ ������ ��
                popText.text = "������ �����ϴ�!";
                PopUI.SetActive(true);
                break;
            case 3:
                popText.text = "100%�� �ʰ��Ͽ� �ø� �� �����ϴ�!";
                PopUI.SetActive(true);
                break;
            case 4:
                popText.text = "��ȭ ����!";
                PopUI.SetActive(true);
                break;
            case 5:
                popText.text = "��ȭ ����.. ��� ���� 2�� ȹ��!";
                PopUI.SetActive(true);
                break;
            case 999:
                popText.text = "����! �� ������ �ٰ����� ��!";
                PopUI.SetActive(true);
                break;
            case 9999:
                popText.text = "���! 3�� ��, �κ�� �̵��մϴ�.";
                PopUI.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(1.2f);

        PopUI.SetActive(false);
        popText.text = "E ��ư�� ���� ��ȣ�ۿ� �ϼ���!";

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
