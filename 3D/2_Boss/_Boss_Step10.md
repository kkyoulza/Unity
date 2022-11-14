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


