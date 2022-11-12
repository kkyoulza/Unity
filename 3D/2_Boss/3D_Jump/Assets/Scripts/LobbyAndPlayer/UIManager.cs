using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    string[] names = new string[] { "","아다만티움 해머", "총", "머신 건" };

    RectTransform rectHP;
    RectTransform rectMP;

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
    public GameObject StatusUI; // 캐릭터 스텟 창 UI
    public GameObject equipUI; // 캐릭터 장비 창 UI
    public GameObject EnchantPanel; // 강화 창

    // UI Text
    public Text popText; // popUI의 텍스트

    // Enchant UI Text
    public Text WeaponName; // 무기 이름
    public Text DialogTxt; // 대화 텍스트
    public Text EnchantStep; // 강화 단계
    public Text addAtk; // 강화 추가 공격력
    public Text minusDelay; // 강화 딜레이 감소 수치
    public Text addCritical; // 강화 크리티컬 확률
    public Text successPercent; // 성공 확률
    public Text enchantMoney; // 강화 비용
    public Text originTxt; // 강화 시 사용할 기원 개수
    public Text originAddPercent; // 기원조각을 통해 오르는 확률 표시 텍스트

    // Item UI
    public Text itemOriginTxt;
    public Text itemHP;
    public Text itemMP;
    public Text goldTxt; // 골드 텍스트

    public Text frontHP; // 단축키에서의 HP포션 개수
    public Text frontMP; // 단축키에서의 MP포션 개수

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

    // 체력, 마나 관련
    public GameObject HP;
    public GameObject MP;
    public Text maxHP;
    public Text cntHP;
    public Text maxMP;
    public Text cntMP;

    public Image WeaponImg; // 무기 이미지
    public Sprite img1;
    public Sprite img2;
    public Sprite img3;

    public Image hpBar;
    public Text healthText; // HP 텍스트

    // 상태 정보
    public bool isNoticeOn; // e를 누르라는 안내문이 나와 있는가?

    // NPC
    public GameObject Smith; // 대장장이
    Animator animSmith; // 대장장이 애니메이션

    // 내부 임시 저장
    public int useOrigin = 0; // 사용 대기중인 기원조각 개수

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
        if(playerInfo.cntEquipWeapon) // 만약 무기를 얻었다면!
            cntBullet.text = playerInfo.cntEquipWeapon.cntCount.ToString()+" ("+playerInfo.cntEquipWeapon.maxCount.ToString()+")";
    }

    public void SetItemInfoUI()
    {
        goldTxt.text = playerItem.playerCntGold.ToString();
        itemOriginTxt.text = playerItem.enchantOrigin.ToString() + " 개";
        itemHP.text = playerItem.cntHPPotion.ToString() + " 개";
        itemMP.text = playerItem.cntMPPotion.ToString() + " 개";
        frontHP.text = playerItem.cntHPPotion.ToString();
        frontMP.text = playerItem.cntMPPotion.ToString();
    }

    public void SetBar()
    {
        // 체력에 따라서 Bar의 크기를 설정하는 것이다.
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
        DialogTxt.text = "안녕, 무기 강화 하려고 왔어?";
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
        enchant.ResetTarget();
        useOrigin = 0; // 기원조각 사용 예정 개수 초기화
        weaponIndex = -1; // index 초기화
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
        EnchantStep.text = "한계치까지 강화 하였습니다. (10강)";
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

        enchant.ResetTarget(); // 대상 초기화
    }

    WeaponItemInfo checkWeapon(int itemCode)
    {

        for (int i = 0; i < playerItem.weapons.Length; i++)
        {
            if (playerItem.weapons[i] == null || playerItem.weapons[i].baseAtk == 0)
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
            case 1:
                addAtk.text = (info.baseAtk+info.enchantAtk) + " + " + enchant.addMeleeAtk[info.enchantCount]; // 공격력 증가치 세팅
                minusDelay.text = (info.baseDelay + info.enchantDelay).ToString("N2") + " - " + enchant.minusMeleeDelay[info.enchantCount]; 
                // 딜레이 감소치 세팅, 딜레이는 소수점 2번째 자리까지만 표시("N2")
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
        if (enchant.isWeaponOn && enchant.EnchantTarget.enchantCount == playerItem.weapons[weaponIndex].maxEnchant) // 풀강일 때는 사용을 못하게 해야 한다.
        {
            StartCoroutine(noticeEtc(1)); // 알림!
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

        if (enchant.isWeaponOn && enchant.EnchantTarget.enchantCount < playerItem.weapons[weaponIndex].maxEnchant) // 풀강이 아니고, 무기가 올려져 있을 때
        {
            if((playerItem.enchantOrigin - useOrigin - 1) >= 0) // 조각이 있으면 (사용 예정인 조각을 빼고, 한개 더 사용 해도 될 만한 조각이 있으면!)
            {
                useOrigin++; // 사용 예정 조각을 올린다.
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
        // 모든 기원조각 사용 (한도 적용)

        if (enchant.EnchantTarget == null || enchant.EnchantTarget.baseAtk == 0)
        {
            // 강화 대상에 아무것도 없을 때
            return;
        }

        useOrigin = playerItem.enchantOrigin >= (1000 - (int)(enchant.percentage[enchant.EnchantTarget.enchantCount] * 1000)) / 5 
            ? (1000 - (int)(enchant.percentage[enchant.EnchantTarget.enchantCount] * 1000)) / 5 
            : playerItem.enchantOrigin ; // 100%까지 되게 하는 모든 개수 사용! (가지고 있는 개수가 모자라다면 남은 개수 모두!)
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
            case 0: // 비용이 부족함을 알려줄 때
                popText.text = "강화 비용이 부족합니다!";
                PopUI.SetActive(true);
                break;
            case 1: // 기원 조각 풀일때
                popText.text = "강화를 한계까지 하여 사용할 수 없습니다!";
                PopUI.SetActive(true);
                break;
            case 2: // 기원 조각이 부족할 때
                popText.text = "조각이 없습니다!";
                PopUI.SetActive(true);
                break;
            case 3:
                popText.text = "100%를 초과하여 올릴 수 없습니다!";
                PopUI.SetActive(true);
                break;
            case 4:
                popText.text = "강화 성공!";
                PopUI.SetActive(true);
                break;
            case 5:
                popText.text = "강화 실패.. 기원 조각 2개 획득!";
                PopUI.SetActive(true);
                break;
            case 999:
                popText.text = "어이! 내 보물에 다가가지 마!";
                PopUI.SetActive(true);
                break;
            case 9999:
                popText.text = "사망! 3초 뒤, 로비로 이동합니다.";
                PopUI.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(1.2f);

        PopUI.SetActive(false);
        popText.text = "E 버튼을 눌러 상호작용 하세요!";

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
