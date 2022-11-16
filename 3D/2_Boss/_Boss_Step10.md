# Step10. 상점 UI 및 스탯 UI에서 스탯 강화 시스템 제작

포션, 총알을 살 수 있는 상점과 스탯 창에서 기원 조각을 사용하여 스탯을 강화할 수 있게 하겠다.

<hr>

### 1. 상점 UI

상점 UI는 아이템 창과 유사한 방식으로 구성한다.

![image](https://user-images.githubusercontent.com/66288087/201675141-698e277d-7bf2-4ef1-9040-c08baaebeeca.png)

구매 버튼을 누르게 되면

![image](https://user-images.githubusercontent.com/66288087/201675336-8a96bb14-3d0b-443e-8e96-9f6fb30bdacf.png)

위 사진과 같이 살 개수를 정하게 된다.

UI 매니저에 버튼과 연계되는 새로운 함수들을 만들었다.

<pre>
<code>
public void setBuyItem(int code)
{
    // 살 아이템을 정하고, 개수를 정하는 UI를 불러온다.
    itemCode = code;
    switch (itemCode)
    {
        case 2001:
            itemNameTxt.text = "HP 포션";
            break;
        case 2002:
            itemNameTxt.text = "MP 포션";
            break;
        case 2003:
            itemNameTxt.text = "총알";
            break;
    }
    buyCount = 0;
    buyCountTxt.text = buyCount.ToString();
    shopRemindUI.SetActive(true);
}

public void offShopUI(int status)
{
    // status 0 - 전체 off / 1 - 개수 정하는 것만 off
    if(status == 0)
    {
        shopUIPanel.SetActive(false);
        playerInfo.isTalk = false;
    }
    shopRemindUI.SetActive(false);
}
public void buyItem()
{
    switch (itemCode)
    {
        case 2001:
            if(playerItem.playerCntGold >= buyCount * 20)
            {
                playerItem.playerCntGold -= buyCount * 20;
                playerItem.cntHPPotion += buyCount;
                shopRemindUI.SetActive(false);
                StartCoroutine(noticeEtc(6));
            }
            else
            {
                StartCoroutine(noticeEtc(0));
            }
            break;
        case 2002:
            if (playerItem.playerCntGold >= buyCount * 15)
            {
                playerItem.playerCntGold -= buyCount * 15;
                playerItem.cntMPPotion += buyCount;
                shopRemindUI.SetActive(false);
                StartCoroutine(noticeEtc(6));
            }
            else
            {
                StartCoroutine(noticeEtc(0));
            }
            break;
        case 2003:
            if (playerItem.playerCntGold >= buyCount * 5)
            {
                playerItem.playerCntGold -= buyCount * 5;
                playerInfo.bullet += buyCount;
                shopRemindUI.SetActive(false);
                StartCoroutine(noticeEtc(6));
            }
            else
            {
                StartCoroutine(noticeEtc(0));
            }
            break;
    }
}

public void addBuyCount()
{
    buyCount++;
    buyCountTxt.text = buyCount.ToString();
}
public void minusBuyCount()
{
    if(buyCount > 0)
        buyCount--;
    buyCountTxt.text = buyCount.ToString();
}

public IEnumerator noticeEtc(int type)
{
    switch (type)
    {
        case 0: // 비용이 부족함을 알려줄 때
            popText.text = "비용이 부족합니다!";
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
        case 6:
            popText.text = "구매 완료!";
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
</code>
</pre>

setBuyitem은 int형 매개변수를 설정 해 두어 각 아이템 별로 아이템 코드를 받아서 분기를 마련 해 둔다.

각 분기마다 아이템에 맞는 이름을 세팅 해 준다.

그리고 buyItem에서는 저장 해 두었던 아이템 코드를 이용하여 분기를 마련하고, 소지 골드가 사용하고자 하는 골드 이상일 때, 적절한 아이템을 증가시키게 하였다.

noticeEtc에도 비용 부족, 구매 완료에 대한 부분을 만들어 두었다.

그리고 offShopUI에서도 int 형태의 매개변수를 통하여 개수를 설정하는 UI만을 끌 것인가, 전체 UI를 끌 것인가를 구분하였다. 

전체 UI가 off될 때는 isTalk를 false로 만들어 주어 캐릭터가 다시 움직일 수 있게 만들어 주었다.

<hr>

### 2. 스탯 강화

스탯 강화 시스템도 만들어 보도록 하겠다.

스탯은 스탯 창에서 강화할 수 있으며, 레벨이 없기 때문에 여러모로 만능(?)인 기원 조각을 이용하여 강화하게끔 하였다.

스탯 강화는 초반에는 적은 수의 기원조각을 사용하며, 지수 함수를 이용하여 점점 갈 수록 많은 수의 기원 조각을 필요로 하게 하여 너무 빠르게 극한까지 강화가 진행되는 상황을 방지하였다.

강화를 위해서, 지난 번에 만들어 둔 StatManager.cs를 재활용하여 사용하였다.

당시에는 플레이어의 공격력을 계산하기 위한 용도였지만 이 곳에 강화 진행 함수를 추가 하여 버튼과 연동하도록 하겠다.

우선, UI를 통해서 어느 정도의 강화 비용이 드는 지 안내 할 필요가 있다.

따라서 아래 사진과 같이 안내를 하는 UI를 만들어 주었다.

![image](https://user-images.githubusercontent.com/66288087/202158004-8952ab7c-0980-4539-8e35-1cfe6d531fb9.png)

무엇을 강화 할 것인지와, 몇 개의 기원조각을 소모하는지를 표시하였다. 버튼 위에 마우스를 올리면 나오게 하였다.

(기존에 만들었던 아이템, 스탯 설명과 유사하다. 그렇지만, 다른UI를 세팅해야 하기에 bool 조건을 하나 넣어 주어 분기하였다.)

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject itemNotice; // 아이템 정보들이 적혀있는 객체
    public PlayerItem playerItem; // 플레이어 무기 정보
    public PlayerCode playerCode; // 플레이어 스탯 정보
    public Text Name; // 무기 이름
    public Text Atk; // 무기 공격력
    public Text Delay; // 무기 딜레이

    // 아이템 설명이 들어 갈 내용
    public Text info;
    public Vector3 pos;
    public int value;

    public bool isUpBtn; // 스탯을 올려주는 버튼인가?
    public GameObject infoPanel; // 스탯 강화 시 사용되는 기원조각 비용 안내
    public Text statName;
    public Text useOriginTxt;

    string[] names = new string[] { "", "아다만티움 해머", "총", "머신 건" };

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUpBtn)
        {
            infoPanel.transform.position = transform.position + pos;
        }
        else
        {
            itemNotice.transform.position = transform.position + pos;
        }

        switch (value)
        {
            case -1:
                info.text = "골드\n\n이 세계에서 사용 되는 화폐 단위\n강화, 상점 구매 등에 이용 가능\n점프 맵, 던전 클리어, 광물 파밍을 통해 수급 가능";
                break;
            case 1:

                for(int i = 0; i < playerItem.weapons.Length; i++)
                {
                    if (playerItem.weapons[i] == null || playerItem.weapons[0].baseAtk == 0)
                    {
                        Name.text = "미습득";
                        Atk.text = "-";
                        Delay.text = "-";
                        break;
                    }

                    if (value == playerItem.weapons[i].weaponCode)
                    {
                        Name.text = names[value] + " ( +" + playerItem.weapons[i].enchantCount + " )";
                        Atk.text = (playerItem.weapons[i].baseAtk + playerItem.weapons[i].enchantAtk).ToString();
                        Delay.text = (playerItem.weapons[i].baseDelay + playerItem.weapons[i].enchantDelay).ToString();
                        break;
                    }
                    
                    
                } 
                break;
            case 2:

                for (int i = 0; i < playerItem.weapons.Length; i++)
                {
                    if (playerItem.weapons[i] == null || playerItem.weapons[0].baseAtk == 0)
                    {
                        Name.text = "미습득";
                        Atk.text = "-";
                        Delay.text = "-";
                        break;
                    }

                    if (value == playerItem.weapons[i].weaponCode)
                    {
                        Name.text = names[value] + " ( +" + playerItem.weapons[i].enchantCount + " )";
                        Atk.text = (playerItem.weapons[i].baseAtk + playerItem.weapons[i].enchantAtk).ToString();
                        Delay.text = (playerItem.weapons[i].baseDelay + playerItem.weapons[i].enchantDelay).ToString();
                        break;
                    }

                    
                }
                break;
            case 3:

                for (int i = 0; i < playerItem.weapons.Length; i++)
                {
                    if (playerItem.weapons[i] == null || playerItem.weapons[0].baseAtk == 0)
                    {
                        Name.text = "미습득";
                        Atk.text = "-";
                        Delay.text = "-";
                        break;
                    }

                    if (value == playerItem.weapons[i].weaponCode)
                    {
                        Name.text = names[value] + " ( +" + playerItem.weapons[i].enchantCount + " )";
                        Atk.text = (playerItem.weapons[i].baseAtk + playerItem.weapons[i].enchantAtk).ToString();
                        Delay.text = (playerItem.weapons[i].baseDelay + playerItem.weapons[i].enchantDelay).ToString();
                        break;
                    }
 
                }
                break;
            case 2000:
                info.text = "기원 조각\n\n강화 성공을 기원하며 생기게 된 돌\n1 개당 강화 확률 0.5%p 상승\n(100%까지 상승 가능)";
                break;
            case 2001:
                info.text = "HP 포션\n\nHP를 회복시키는 포션\nHP + 30";
                break;
            case 2002:
                info.text = "MP 포션\n\nMP를 회복시키는 포션\nMP + 10";
                break;
            case 10000:
                info.text = "공격력\n\n데미지에 영향을 미치는 수치\n명중률이 높을수록 일정한 데미지가 나온다.";
                break;
            case 10001:
                if (isUpBtn)
                {
                    statName.text = "힘 (Str)";
                    useOriginTxt.text = Mathf.Pow(2, playerCode.strEnchantCnt).ToString() + " 개 소모";
                }
                else
                {
                    info.text = "힘(Strength)\n\n공격력의 기준이 되는 수치\n올릴 때 마다 힘 + 5";
                }
                break;
            case 10002:
                if (isUpBtn)
                {
                    statName.text = "명중 (Acc)";
                    useOriginTxt.text = Mathf.Pow(3, playerCode.accEnchantCnt).ToString() + " 개 소모";
                }
                else
                {
                    info.text = "명중률(Accuracy)\n\n온전한 공격을 하기 위한 수치\n명중률이 낮을 수록 공격력에 비해 낮은 데미지가 뜰 확률이 높다.\n 올릴 때 마다 명중률 소량 증가";
                }
                break;
            case 10003:
                if (isUpBtn)
                {
                    statName.text = "체력 (HP)";
                    useOriginTxt.text = (10 * (playerCode.HPEnchantCnt + 1)).ToString() + " 개 소모";
                }
                else
                {
                    info.text = "체력(Health Point)\n\n캐릭터의 체력을 결정하는 수치\n체력이 0이 되면 게임 오버가 된다.\n올릴 때 마다 HP + 10";
                }
                break;
            case 10004:
                if (isUpBtn)
                {
                    statName.text = "마나 (MP)";
                    useOriginTxt.text = (20 * (playerCode.MPEnchantCnt + 1)).ToString() + " 개 소모";
                }
                else
                {
                    info.text = "마나(Magic Point)\n\n스킬을 사용하기 위해 지불해야 하는 코스트\n올릴 때 마다 MP + 5";
                }
                break;
            case 20000:
                info.text = "마을에서 기원조각을 이용하여 스탯을 강화할 수 있습니다.\n\n 강화를 할 수록 더 많은 기원조각이 필요합니다.";
                break;

        }
        if (isUpBtn)
        {
            infoPanel.SetActive(true);
        }
        else
        {
            itemNotice.SetActive(true);
        }
        Debug.Log("onMouse");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUpBtn)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            itemNotice.SetActive(false);
        }
        Debug.Log("exitMouse");
    }

}
</code>
</pre>

isUpBtn을 만들어 두어, 강화 버튼에 마우스를 올리는 경우를 구분하였다.

![image](https://user-images.githubusercontent.com/66288087/202158993-9706e216-1eb0-4777-8b7f-3d94578276bd.png)

위 사진과 같이 아이템 코드는 동일하고, isUpBtn만 다르게 하였다.

case를 따로 만들면 되지 않느냐는 의문이 들 수 있지만, 같은 아이템에 대한 설명이고, 다른 UI를 보여주는 것이니 case에 들어가는 번호를 동일하게 하는 것이 좋을 것이라 판단하였다.

![image](https://user-images.githubusercontent.com/66288087/202158648-e9dc43de-2d76-45cf-a7c2-3b7d5a432e7c.png)

또한, 도움말도 넣어 두어 강화에 대한 안내도 해 주었다.

<hr>

**스탯 매니저**

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
    GameObject uiManager;
    PlayerCode playerCode;
    PlayerItem playerItem;
    UIManager ui;

    public Text useOriginTxt;

    // Start is called before the first frame update
    void Awake()
    {
        uiManager = GameObject.FindGameObjectWithTag("uimanager");
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
        playerItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItem>();
        ui = uiManager.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SetStatusUI();
    }

    void SetStatusUI()
    {
        ui.Atk.text = playerCode.playerMinAtk.ToString() + " ~ " + playerCode.playerMaxAtk.ToString();
        ui.Str.text = playerCode.playerStrength.ToString();
        ui.Acc.text = playerCode.playerAccuracy.ToString();
        ui.HPTxt.text = playerCode.playerMaxHealth.ToString();
        ui.MPTxt.text = playerCode.playerMaxMana.ToString();
    }

    public void EnchantStat(int code)
    {
        switch (code)
        {
            case 10001:
                if (playerItem.enchantOrigin >= (int)Mathf.Pow(2, playerCode.strEnchantCnt))
                {
                    playerItem.enchantOrigin -= (int)Mathf.Pow(2, playerCode.strEnchantCnt);
                    playerCode.strEnchantCnt++;

                    playerCode.playerStrength += 5;
                    useOriginTxt.text = Mathf.Pow(2, playerCode.strEnchantCnt).ToString() + " 개 소모";
                }
                else
                {
                    StartCoroutine(ui.noticeEtc(0));
                }
                break;
            case 10002:
                if (playerItem.enchantOrigin >= (int)Mathf.Pow(3, playerCode.accEnchantCnt))
                {
                    playerItem.enchantOrigin -= (int)Mathf.Pow(3, playerCode.accEnchantCnt);
                    playerCode.accEnchantCnt++;

                    playerCode.playerAccuracy += 10;
                    useOriginTxt.text = Mathf.Pow(3, playerCode.accEnchantCnt).ToString() + " 개 소모";
                }
                else
                {
                    StartCoroutine(ui.noticeEtc(0));
                }
                break;
            case 10003:
                if (playerItem.enchantOrigin >= (10 * (playerCode.HPEnchantCnt + 1)))
                {
                    playerItem.enchantOrigin -= (10 * (playerCode.HPEnchantCnt + 1));
                    playerCode.HPEnchantCnt++;

                    playerCode.playerMaxHealth += 10;
                    useOriginTxt.text = (10 * (playerCode.HPEnchantCnt + 1)).ToString() + " 개 소모";
                }
                else
                {
                    StartCoroutine(ui.noticeEtc(0));
                }
                break;
            case 10004:
                if (playerItem.enchantOrigin >= (20 * (playerCode.MPEnchantCnt + 1)))
                {
                    playerItem.enchantOrigin -= (20 * (playerCode.MPEnchantCnt + 1));
                    playerCode.MPEnchantCnt++;

                    playerCode.playerMaxMana += 5;
                    useOriginTxt.text = (10 * (playerCode.MPEnchantCnt + 1)).ToString() + " 개 소모";
                }
                else
                {
                    StartCoroutine(ui.noticeEtc(0));
                }
                break;

        }
    }

}
</code>
</pre>

스탯 매니저에 스탯 강화에 대한 부분도 추가 해 주었다.

기원 조각이 더 많을 경우, 기원 조각을 소모하여 강화를 진행하며, 버튼에 마우스를 올리고 있기 때문에 안내 UI가 있을텐데 그것도 갱신을 해 주었다.

처음에는 각 버튼마다 다른 정보를 보여 주어야 하기 때문에 정보를 어떻게 갱신 해 주어야 하는가 고민하였다. (str을 강화 하게 되면 ui에 str에 대한 정보를 넣어 주어야 하는데, 이것을 어떻게 연결 할 것인가?)

그런데 의미 없는 고민이라는 것을 깨닫게 되었다.

현재 마우스를 올리고 있는 것에 대한 정보만 갱신 해 주면 어차피 다른 스탯 강화를 위해 다른 버튼 위에 마우스를 올리게 되면 정보가 갱신되기 때문이다.

**따라서, 현재 나와 있는 UI text에 현재 강화중인 정보에 대한 기원조각 개수를 갱신 해 주면 되는 것이다.**

<hr>

![image](https://user-images.githubusercontent.com/66288087/202160028-eae8d5a5-ac90-4209-b8a2-a218b3397363.png)

아무튼, 버튼에 위 함수를 적용시켜 주고, 매개변수로 아이템 코드를 넣어 주면 버튼을 누르게 되었을 때, 스탯이 강화되는 것을 볼 수 있을 것이다.

![image](https://user-images.githubusercontent.com/66288087/202160239-6f588d6c-0322-4010-b7b5-92cdd585e3e3.png)

강화를 한 모습 (기원조각 요구량이 32개가 되어 있으며, 힘도 늘어 나 있다.)

<hr>

### 3. 포션 섭취 적용 및 UI 보강(단축키 안내, 던전 입장 전 대화 추가 등)








