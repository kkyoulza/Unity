# Step8. UI


이제 아이템 창, 장비 창, 단축키를 보여주는 등의 UI들을 설정하는 작업을 진행 하도록 하겠다.

또한, 앞서 만들어 놓은 강화 창에서 기원 조각 기능을 적용시키고, 그에 대한 디테일을 설정 해 주도록 하겠다.

### 캐릭터 상태 창

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


### 아이템 창, 캐릭터 스탯 창, 장비 창

RPG에서 아이템 창, 스탯 창, 장비 창 등의 아이템 창 인터페이스는 필수적이다.

따라서 그에 대한 것들을 구현 해 보고자 한다.

<hr>

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

<hr>

**아이템 설명**

그리고 아이템에는 각 아이템에 대한 설명이 들어 가 주어야 한다.

아이템 위에 마우스 포인터를 가져가게 되면 설명이 나오게끔 해 줄 것이다.

각 아이템 패널마다 아이템 코드를 넣어 주고, 설명이 들어갈 판에 아이템 코드별로 알맞는 설명을 넣어주면 된다.

마우스 포인터가 올라갔을 때는 IPointerEnterHandler, IPointerExitHandler를 사용하여 구현하였다.

마우스가 해당 객체에 올라갈 때, 그리고 나갈 때 정보 UI를 on/off 해 줌으로써 아이템에 대한 정보를 알 수 있게 하였다.

ItemSlot.cs 코드
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

![image](https://user-images.githubusercontent.com/66288087/197682296-1c17cf89-4f1a-4c6e-badd-4fa916c252e9.png)

마우스 포인터는 캡쳐가 되지 않았지만 아이템 패널 위에 위에 있을 때 아이템 설명이 노출되었음을 볼 수 있다.

<hr>

**아이템 창 드래그**

이제 아이템 창을 드래그 하는 것을 구현 해 보도록 하자

아이템 창 드래그는 [이곳](https://husk321.tistory.com/m/214)에 나온 코드를 이용하였다.

코드를 보게 되면

아래와 같이 나온다.

Drag.cs
<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{

    RectTransform rect;
    public GameObject obj;
    [SerializeField] public Canvas canvas;
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Awake()
    {
        rect = obj.GetComponent<RectTransform>();
        canvasGroup = obj.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // PointerEventData 의 delta는 얼마나 이동했는지를 나타 내 주는 것이다.
        // 따라서 해당 객체의 rectTransform을 delta만큼 이동시켜 준다.

        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnDrop(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }


}
</code>
</pre>

Drag.cs 코드는 아이템 창 상단 라인 객체에 넣어주었다.

![image](https://user-images.githubusercontent.com/66288087/197682953-973d02c0-23a3-4510-b0cd-2f8444f4a80f.png)

설정한 public 변수들에는 위 사진과 같이 넣어준다.

obj에는 아이템 창 전체를, Canvas에는 Canvas를 넣어 준다.

캔버스 그룹은 UI 요소들을 하나로 묶어 관리하기 위해 사용한다. 

즉, Item 창의 전체를 관리하기 위함이다.

OnBeginDrag() 에서는 드래그를 시작할 때 일어나는 일들을 설정할 수 있다.

아이템창 전체의 투명도를 좀 더 투명하게 설정 해 준다. (드래그를 하는 중이라는 것을 알려주기 위하여)

OnEndDrag() 에서는 다시 원상태로 돌려 놓는다.

그렇다면 드래그 중일때는?

OnDrag() 함수를 이용하여 다룬다.

드래그 중일때는 아이템창의 위치를 eventData를 통해 들어오는 위치 변화량만큼 이동시켜 준다.

canvas.sacleFactor는 캔버스의 크기와 맞추기 위함이라 생각하면 된다.

<hr>

**스탯 창**

스탯 창도 아이템 창과 비슷하게 해 주면 된다.

단지 다른 점은 플레이어의 스탯을 계산하여 Update() 해 주는 작업이 추가 되어야 한다는 점이다.

플레이어의 스탯 리스트를 나열하면 다음과 같다.

- 공격력
- 힘
- 명중률
- HP
- MP

그런데 여기서 명중률은 몬스터(or 돌)에게 얼마나 자신이 가진 최대의 데미지를 줄 수 있는가에 영향을 주게 설정 할 것이다.

다시 말해, 자신의 공격력이 100이라고 하면 명중률에 따라서 80에서 100의 데미지가 나올 수도 있고, 60에서 100의 데미지가 나올 수도 있는 것이다.

(메이플에서는 숙련도가 이러한 역할을 한다.)

아무튼, 명중률에 의해서 플레이어의 최소 공격력, 최대 공격력을 산출할 수 있다.

우선, 플레이어 자체의 힘과 자신이 장착하고 있는 무기의 공격력을 더한다.

이 값이 최대 공격력이다.

최소 공격력은 명중률에 영향을 받게 되는데, 명중률은 float가 아닌 int 값이다.

기본적으로 공격은 맞거나/빗나가거나 둘 중에 하나이다. 따라서 기본적인 명중률은 50%(0.5)로 시작한다.

그런데 플레이어에게 주어지는 기본 명중률 스탯은 5이기 때문에 이 것을 소수로 바꾸어 주어서 0.5에 더해주면 된다.

그렇다면 단순하게 10을 나누어서 0.5를 해서 더해주면 될까?

아니다.

그렇게 되면 1.0이 되어 버려서 최소, 최대 공격력을 나누게 되는 의미가 없다.

따라서 명중률은 그 수치를 올리게 되어도 초반에 효율이 좋고, 너무 많이 올리게 되면 비효율적이게 설계 할 것이다.

따라서

**최소 공격력 = (플레이어 힘 + 장착 무기 공격력) * (0.5 + 0.1 * log(플레이어 명중률));** 로 설정하였다.

log(5) 가 0.698 정도가 나오게 된다. 0.5 + 0.698 > 1.0 이기 때문에 log5 값에 10을 나누어 주었다.

따라서 진짜 명중률 초기 값은 0.569 = 56.9% 정도가 된다.

아무튼 이렇게 스탯을 계산 한 값을 코드에도 적용시켜 준다.

PlayerCode.cs 내에 있는 스탯계산 함수
<pre>
<code>
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
</code>
</pre>

위 코드를 Update()에 넣어 두어 스탯을 계산하게끔 해 주었다.

그리고 스탯은 스탯 창 자체에서 강화를 할 수 있게 할 계획이기 때문에 manager에 넣을 새로운 코드인 StatManager.cs를 하나 만들어 주었다.

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
    UIManager ui;

    // Start is called before the first frame update
    void Awake()
    {
        uiManager = GameObject.FindGameObjectWithTag("uimanager");
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
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
    }


}
</code>
</pre>

이렇게 만들어 주면 아래 사진과 같이 완성이 되게 된다.

(강화 버튼은 아직 구현하지 않았다.)

![image](https://user-images.githubusercontent.com/66288087/197345428-ed24d972-f022-4a8d-baba-5ccf59d08ac1.png)

완성된 아이템 창, 스탯 창 UI의 모습(위 사진에서는 스탯이 적용되지 않은 상태)

![image](https://user-images.githubusercontent.com/66288087/197685378-609523e3-182d-4c46-a096-47e7b516fe55.png)

스탯 적용, 무기 장착 시 스탯 창 모습

<hr>

**장비 창**

장비 창도 비슷하게 만들어 준다.

![image](https://user-images.githubusercontent.com/66288087/197802015-3fb01ba6-75bf-4f41-9fb9-838b6e3d2bad.png)

각 무기를 표시하는 Panel에는 무기 그림을 넣어 주고 아이템 설명을 해 주는 Panel과 같이 정보를 넣어 주는 것으로 하였다.

![image](https://user-images.githubusercontent.com/66288087/197804724-8eaf5f47-6fc4-447a-80a6-24faa6cb41e9.png)

미습득 때 정보가 표시된 모습

![image](https://user-images.githubusercontent.com/66288087/197805039-414ef4e0-46f0-4b82-9bb3-c5a71dc65548.png)

정보를 표시한 모습은 위 사진과 같다.

정보를 표시하게 되는 ItemSlot.cs 도 수정 해 준다.

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
    public Text Name; // 무기 이름
    public Text Atk; // 무기 공격력
    public Text Delay; // 무기 딜레이
    public Text info;
    public Vector3 pos;
    public int value;
    
    string[] names = new string[] { "", "아다만티움 해머", "총", "머신 건" };

    public void OnPointerEnter(PointerEventData eventData)
    {
        itemNotice.transform.position = transform.position + pos;
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
                        Name.text = names[value] + " ( +" + playerItem.weapons[i].enchantCount + ")";
                        Atk.text = (playerItem.weapons[i].baseAtk + playerItem.weapons[i].enchantAtk).ToString();
                        Delay.text = (playerItem.weapons[i].baseDelay + playerItem.weapons[i].enchantDelay).ToString();
                        break;
                    }
                    
                    
                } 
                break;
            case 2:
                if (playerItem.weapons == null)
                {
                    break;
                }

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
                        Name.text = names[value] + " ( +" + playerItem.weapons[i].enchantCount + ")";
                        Atk.text = (playerItem.weapons[i].baseAtk + playerItem.weapons[i].enchantAtk).ToString();
                        Delay.text = (playerItem.weapons[i].baseDelay + playerItem.weapons[i].enchantDelay).ToString();
                        break;
                    }

                    
                }
                break;
            case 3:
                if (playerItem.weapons == null)
                {
                    break;
                }

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
                        Name.text = names[value] + " ( +" + playerItem.weapons[i].enchantCount + ")";
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

무기 아이템 코드(1~3)인 경우에는 if (playerItem.weapons[i] == null || playerItem.weapons[0].baseAtk == 0) 조건을 걸어 주어서

만약에 무기를 하나도 습득하지 않았다면 weapons의 원소가 null이 아니거나, baseAtk가 0일 때 바로 return하게 해 주었다.

그리고 습득한 무기의 weaponCode와 해당 panel의 아이템 value가 같아지게 되면 해당 정보를 넣은 Info가 위치에 뜨게 만들어 준다.

![image](https://user-images.githubusercontent.com/66288087/197808304-d36b367f-67ba-4459-bbfc-d2ef2c2d712a.png)

위치는 각 panel별로 맞춤식으로 설정할 수 있다.

아이템 창, 스탯 창, 장비 창에는 모두 드래그 효과를 적용하였다.

<hr>

### 무기 교체 단축 키, 남은 총알 표시, 스킬 쿨타임 표시

이제 무기 교체 단축 키를 나타 내 주는 것과, 남은 총알, 쿨타임 표시를 해 주도록 하겠다.

**무기 교체 단축키 안내판 표시**

길게 써 놓았는데, 그냥 지금 어떤 무기를 착용하고 있는지랑, 해당 무기를 장착하기 위해 어느 버튼을 눌러야 하는 지를 표시 해 주는 것이다.

![image](https://user-images.githubusercontent.com/66288087/197988795-e1c475aa-72b4-4421-86cc-97e46eab627a.png)

일단 완성 된 모습은 위 사진과 같다.

기본적으로 장착하고 있지 않은 무기는 투명하게, 착용하고 있는 무기는 투명하지 않게 설정하였다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelActive : MonoBehaviour
{
    public int itemCode;
    public Image img;
    PlayerCode playerCode;
    // Start is called before the first frame update
    void Awake()
    {
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
    }

    // Update is called once per frame
    void Update()
    {
        isActive();
    }

    public void isActive()
    {
        if(playerCode.cntEquipWeapon == null)
        {
            return;
        }

        if(playerCode.cntEquipWeapon.itemCode == this.itemCode)
        {
            Color color = new Color(1, 1, 1, 1);
            img.color = color;
        }
        else
        {
            Color color = new Color(1, 1, 1, 0.5f);
            img.color = color;
        }

    }

}
</code>
</pre>


PanelActive.cs 라는 코드로 만들어 해당 Panel에 넣어 주었다.

player의 cntEquipWeapon(현재 장착중인 무기) GameObject에 들어 있는 itemCode 정보를 가져와서 사용하였다.

<hr>

**남은 총알 표시 UI**

남은 총알을 표시하는 UI는 총알을 사용하는 무기를 꼈을 때만 나오게 하였다.

![image](https://user-images.githubusercontent.com/66288087/197993358-e36105ee-62bd-4166-998d-daec90d71e92.png)

총 무기를 꼈을 때 UI가 나온 모습

UI를 on/off 하는 부분은 PlayerCode.cs에서 무기를 Swap하는 순간에 넣어 주었다.

<pre>
<code>
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
</code>
</pre>

itemCode가 총 or 머신 건 일때 UIManager.cs에 저장 해 놓은 bulletUI를 활성화/비활성화 함을 볼 수 있다.

UI 구성은 max는 플레이어가 가지고 있는 전체 총알 개수, 아래 있는 것은 현재 장전되어 있는 총알 개수이다. 괄호 친 부분은 탄창의 용량이다.

<hr>

**스킬 쿨타임 표시**

아직 스킬이 추가 된 것이 거의 없어서 넣을까 고민하였지만 넣는 것이 맞는 것 같아 넣어 주었다.

대쉬를 하고 나면 CoolTimeManager.cs에서 쿨타임을 계산 해 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolTimeManager : MonoBehaviour
{
    // 대쉬 스킬 관련
    public Text CoolText;
    public Image dashImage;

    public bool coolOn;
    float coolTime;
    int cool;



    // Start is called before the first frame update
    void Start()
    {
        coolOn = true;
        SetAble();
    }

    // Update is called once per frame
    void Update()
    {
        if(coolOn == false)
        {
            coolUpdate();
        }
        
    }

    public void SetCoolTime(float coolTime)
    {
        Color color = new Color(1, 1, 1, 0.5f);
        dashImage.color = color;
        this.coolTime = coolTime;
        CoolText.text = coolTime.ToString();
    }

    void coolUpdate()
    {
        this.coolTime = this.coolTime - Time.deltaTime;
        cool = (int)coolTime;
        CoolText.text = cool.ToString();

    }

    public void SetAble()
    {
        Color color = new Color(1, 1, 1, 1);
        dashImage.color = color;
        CoolText.text = "";
    }

}
</code>
</pre>

위 코드를 통해 쿨타임을 계산하여 Text에 실시간으로 적용 해 준다.

그 텍스트를 대쉬 아이콘에 넣어주면 된다.

![image](https://user-images.githubusercontent.com/66288087/197995382-f41dbdb9-1b71-415d-b74e-14bb63709a8d.png)

그러면 이렇게 숫자가 생기게 되면서 카운트가 된다.(캡처를 늦게 해서 0초의 순간이 찍혔다..)

<hr>

이제 다음으로는 몬스터를 소환하는 스테이지를 만들어 보도록 하겠다.

스테이지 별로 어느 지점에서 몬스터를 스폰하고, 몬스터를 다 잡게 되면 다음으로 이동하고, 보스를 잡으면 클리어 하는 등의 기능을 넣을 예정이다.

