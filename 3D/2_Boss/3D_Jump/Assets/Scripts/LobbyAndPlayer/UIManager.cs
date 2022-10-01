using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    string[] names = new string[] { "아다만티움 해머", "총", "머신 건" };
    
    // 외부 정보들
    public GameObject player; // player 객체

    PlayerCode playerInfo; // player 세부 정보(무기 습득 여부 확인용)
    PlayerItem playerItem; // player 아이템 정보를 가져오기 위함

    public GameObject enchantManager; // 강화 매니저 객체
    Enchanting enchant; // 강화 관련 로직 코드
    public int weaponIndex; // playerItem에서 현재 강화중인 무기의 index (역으로 적용하기 위함)

    //UI 객체들
    public GameObject PopUI; // pop으로 나올 UI가 들어 갈 변수
    public GameObject DialogPanel; // 대화 패널
    public GameObject ItemUI; // 아이템 창 UI
    public GameObject EnchantPanel; // 강화 창

    public Text WeaponName; // 무기 이름
    public Text DialogTxt; // 대화 텍스트
    public Text EnchantStep; // 강화 단계
    public Text addAtk; // 강화 추가 공격력
    public Text minusDelay; // 강화 딜레이 감소 수치
    public Text addCritical; // 강화 크리티컬 확률
    public Text successPercent; // 성공 확률
    public Text enchantMoney; // 강화 비용

    public Image WeaponImg; // 무기 이미지
    public Sprite img1;
    public Sprite img2;
    public Sprite img3;

    // 상태 정보
    public bool isNoticeOn; // e를 누르라는 안내문이 나와 있는가?

    // NPC
    public GameObject Smith; // 대장장이
    Animator animSmith; // 대장장이 애니메이션

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

    public void ShowEnchantUI() // 강화 하기를 눌렀을 때 (대화창 -> 강화창)
    {
        if(playerItem.weapons[0] != null)
        {
            DialogPanel.SetActive(false);
            EnchantPanel.SetActive(true);
        }
        else
        {
            DialogTxt.text = "무기를 하나도 가지고 있지 않아, 무기를 먼저 얻고 올래?";
        }

    }

    public void OffEnchantUI()
    {
        EnchantPanel.SetActive(false);
        enchant.isWeaponOn = false;
        weaponIndex = -1; // index 초기화
        ResetEnchantUI();
    }

    public void DisappearEnchantDialogUI()
    {
        DialogPanel.SetActive(false);
        DialogTxt.text = "안녕, 무기 강화 하려고 왔어?";
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
        EnchantStep.text = "한계치까지 강화 하였습니다. (10강)";
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

        enchant.ResetTarget(); // 대상 초기화
    }

    WeaponItemInfo checkWeapon(int itemCode)
    {

        for (int i = 0; i < playerItem.weapons.Length; i++)
        {
            if (playerItem.weapons[i] == null)
                break;
            if (playerItem.weapons[i].weaponCode == itemCode)
            {
                weaponIndex = i; // 인덱스 저장
                return playerItem.weapons[i]; // 버튼에 맞는 무기를 먹었다면 true
            }
        }

        return null;
    }

    void SetInfo(WeaponItemInfo info)
    {
        WeaponName.text = names[info.weaponCode];
        EnchantStep.text = info.enchantCount + " → " + (info.enchantCount + 1) + " 강 강화"; // +1 부분은 괄호로 묶어 주어야 한다.
        switch (info.weaponCode) // UI 정보 세팅
        {
            case 0:
                addAtk.text = (info.baseAtk+info.enchantAtk) + " + " + enchant.addMeleeAtk[info.enchantCount]; // 공격력 증가치 세팅
                minusDelay.text = (info.baseDelay + info.enchantDelay).ToString("N2") + " - " + enchant.minusMeleeDelay[info.enchantCount]; 
                // 딜레이 감소치 세팅, 딜레이는 소수점 2번째 자리까지만 표시("N2")
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
