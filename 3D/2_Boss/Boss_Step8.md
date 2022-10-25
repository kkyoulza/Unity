# Step8. UI


이제 아이템 창, 장비 창, 단축키를 보여주는 등의 UI들을 설정하는 작업을 진행 하도록 하겠다.

또한, 앞서 만들어 놓은 강화 창에서 기원 조각 기능을 적용시키고, 그에 대한 디테일을 설정 해 주도록 하겠다.

#### 캐릭터 상태 창

캐릭터 상태 창은 캐릭터의 초상화, 그리고 HP, MP 바가 체력, 마나에 비례하여 변하는 기능을 추가 하도록 하겠다.

그리고 캐릭터 초상화를 누르게 되면 캐릭터의 세부 스탯이 나오게끔 설정 할 예정이다.

![image](https://user-images.githubusercontent.com/66288087/197345349-165bf9d8-ff99-45bb-a4f8-e498e475b949.png)

일단 상태창의 전반적인 모습은 위 사진과 같다.

**체력바를 체력에 맞게 크기 설정하기**

체력바의 크기는 기본적으로 최대 모습인 경우를 설정 해 둔 다음, **현재 체력/최대 체력의 비율을 곱**해서 크기 설정을 해 준다.

UIManager.cs에 아래 함수를 추가하여 체력/마력에 맞게 바의 길이를 조절 해 준다.
<pre>
<code>
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
</code>
</pre>

주의할 점은 아래 사진과 같이 좌표의 기준점을 왼쪽으로 잡아 놓아야 체력이 깎이는 것처럼 보이게 된다.

![image](https://user-images.githubusercontent.com/66288087/197345905-863dfa77-7f7a-47d6-a4ee-8dadcfd367c6.png)


#### 아이템 창, 캐릭터 스탯 창, 장비 창

RPG에서 아이템 창, 스탯 창, 장비 창 등의 아이템 창 인터페이스는 필수적이다.

따라서 그에 대한 것들을 구현 해 보고자 한다.

**아이템 창**

아이템 창은 처음에는 다양한 아이템들을 얻어 버튼으로 사용하게 하려 했으나, 아이템이 한정적이기에 단순하게 아이템 창을 구현하였다.

대신, 아이템 창 on/off와 드래그를 통하여 창을 이동시키는 것에 대한 것을 중점으로 구현 해 보고자 한다.

![image](https://user-images.githubusercontent.com/66288087/197664869-ea9d725a-b6c6-4628-9d4d-fec920e48f2a.png)

아이템 창의 모습이다. 강화에 사용되는 기원조각과 HP/MP 포션이 있고 골드 양이 들어 갈 자리도 있다.

아이템 창의 계층구조는 아래 사진과 같다.

![image](https://user-images.githubusercontent.com/66288087/197677685-504a092c-c010-4fe5-bb77-0b8a1a1e42cd.png)

Panel 안에는 각 아이템들의 사진, 정보, 아이템 개수를 나타내는 Line을 만들어 주고

![image](https://user-images.githubusercontent.com/66288087/197677773-cc1f6f21-36e3-4b20-a1cb-b11fe6676066.png)

각 라인에는 아이템 이미지, 이름, 개수를 넣어 준다.

아이템에 대한 정보는 Update()를 통해서 실시간으로 갱신 해 주게 되며 UIManager.cs에 아래 함수를 추가 해 준다.

<pre>
<code>
 public void SetItemInfoUI()
{
    goldTxt.text = playerItem.playerCntGold.ToString();
    itemOriginTxt.text = playerItem.enchantOrigin.ToString() + " 개";
    itemHP.text = playerItem.cntHPPotion.ToString() + " 개";
    itemMP.text = playerItem.cntMPPotion.ToString() + " 개";
}
</code>
</pre>

골드, 기원조각, HP포션, MP 포션의 개수를 갱신 해 준다.

그리고 아이템에는 각 아이템에 대한 설명이 들어 가 주어야 한다.

아이템 위에 마우스 포인터를 가져가게 되면 설명이 나오게끔 해 줄 것이다.

각 아이템 패널마다 아이템 코드를 넣어 주고, 설명이 들어갈 판에 아이템 코드별로 알맞는 설명을 넣어주면 된다.

마우스 포인터가 올라갔을 때는 이벤트 처리를 사용하여 구현하였다.

Item
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
    public Text info;
    public Vector3 pos;
    public int value;
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        itemNotice.transform.position = transform.position + pos;
        switch (value)
        {
            case -1:
                info.text = "골드\n\n이 세계에서 사용 되는 화폐 단위\n강화, 상점 구매 등에 이용 가능\n점프 맵, 던전 클리어, 광물 파밍을 통해 수급 가능";
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
                info.text = "힘(Strength)\n\n공격력의 기준이 되는 수치\n올릴 때 마다 공격력 + 1";
                break;
            case 10002:
                info.text = "명중률(Accuracy)\n\n온전한 공격을 하기 위한 수치\n명중률이 낮을 수록 공격력에 비해 낮은 데미지가 뜰 확률이 높다.\n 올릴 때 마다 명중률 + 1%p";
                break;
            case 10003:
                info.text = "체력(Health Point)\n\n캐릭터의 체력을 결정하는 수치\n체력이 0이 되면 게임 오버가 된다.\n올릴 때 마다 HP + 10";
                break;
            case 10004:
                info.text = "마력(Magic Point)\n\n스킬을 사용하기 위해 지불해야 하는 코스트\n올릴 때 마다 MP + 5";
                break;
        }
        
        itemNotice.SetActive(true);
        Debug.Log("onMouse");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemNotice.SetActive(false);
        Debug.Log("exitMouse");
    }

}

</code>
</pre>


![image](https://user-images.githubusercontent.com/66288087/197345428-ed24d972-f022-4a8d-baba-5ccf59d08ac1.png)

완성된 아이템 창, 스탯 창의 모습




#### 무기 교체 단축 키, 남은 총알 표시, 스킬 쿨타임 표시






