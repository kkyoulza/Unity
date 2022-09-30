# Step4. 강화 시스템 제작


RPG에서 빠질 수 없는 요소인 강화를 만들어 보도록 하겠다.

강화 제작에 앞서 플레이어가 가지고 있는 아이템들에 대한 교통정리?가 필요할 것 같아 새롭게 PlayerItem.cs를 만들었다.

PlayerItem.cs
<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

[System.Serializable]
public class WeaponItemInfo
{
    public int weaponCode; // 아이템 코드
    public int enchantCount; // 아이템 강화 수치
    public int baseAtk; // 베이스 공격력
    public int enchantAtk; // 강화 공격력
    public float baseDelay; // 기본 무기 딜레이
    public float enchantDelay; // 강화 딜레이 감소 수치
    public Weapon.AtkType type; // 무기 공격 타입

    public WeaponItemInfo(int code,int count,int Atk,float delay,Weapon.AtkType type)
    {
        this.weaponCode = code;
        this.enchantCount = count;
        this.baseAtk = Atk;
        this.baseDelay = delay;
        this.enchantAtk = 0;
        this.enchantDelay = 0.0f;
        this.type = type;
    }

}


public class PlayerItem : MonoBehaviour
{
    public WeaponItemInfo[] weapons; // 아이템 정보들이 들어 가 있는 배열을 만든다.
    int weaponIndex;
    int maxIndex = 10;


    private void Awake()
    {
        weaponIndex = 0;
        weapons = new WeaponItemInfo[maxIndex];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetInfo(GameObject obj)
    {
        Item item = obj.GetComponent<Item>();

        switch (item.type)
        {
            case (Item.Type.Weapon):

                PlayerCode playerCode = GetComponent<PlayerCode>();
                Weapon weapon = playerCode.WeaponList[item.value].GetComponent<Weapon>();

                WeaponItemInfo weaponinfo = new WeaponItemInfo(item.value, 0, weapon.Damage, weapon.AtkDelay, weapon.type);
                if (weapon.bullet) // 만약 원거리라면 무기 자체에 세팅한 총알 데미지를 갱신 해 준다.
                {
                    Bullet bullet = weapon.bullet.GetComponent<Bullet>();
                    bullet.SetDamage(weapon.Damage);
                }
                weapons[weaponIndex] = weaponinfo;
                weaponIndex++;
                break;
        }

    }

}
</code>
</pre>

점프 맵에서 누적 스코어를 저장할 때 사용했던 방법으로 정보를 모아 둘 Class를 만들고 Serialize화 하여 저장하는 등의 활용을 위해 Serialize를 해 준다.

PlayerCode.cs에서 아이템을 습득하는 부분에서 정보를 주는 GetInfo(nearObject)를 실행 시켜 weaponList에 있는 Weapon에서 무기에 대한 정보를 가져와서 클래스에 저장한다.


<hr>

## 강화 시스템 구성

강화 시스템을 구성하기 위해 필요한 것들이 있다.

**UI**<br>
**플레이어의 소지 아이템(장비) 관리 컴포넌트 -> 이것이 PlayerItem.cs이다.**<br>
**강화 로직**

이렇게 세 가지가 필요하며, 세 가지가 서로 정보들을 공유하면서 업데이트를 해 주는 방식으로 진행 할 것이다.

전체적인 강화 진행에 대한 것을 요약하여 그림으로 그리면 아래 사진과 같다.

![image](https://user-images.githubusercontent.com/66288087/193273287-0795401c-154f-42fb-86ed-a9bd5fe7f73d.png)


<hr>

### UI

강화 UI는 우선 대장장이에게 말을 걸어 강화 의사를 표시하게 되면 강화 창이 뜨게끔 하였다.

그러기 위해서는 우선 대화창과, 강화 창을 만들어 주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/193050572-ee0d1c4c-4c2a-4f1d-a835-d709701165c9.png)

대화창은 특별한 것 없이 앵커를 6시 방향으로 설정 하였으며, Margin을 적당히 설정하여 위치를 맞추어 주었다.

위 대화창에서 강화하기를 누르게 되면 강화 창이 뜨게 되며 강화 창은 아래와 같이 구성하였다.

![image](https://user-images.githubusercontent.com/66288087/193050691-8d433734-f5a5-40bf-b72c-68e6f23ba764.png)

위에 있는 무기 리스트를 누르게 되면 아래에 해당 무기로 사진이 변경되고, 강화 버튼을 누르게 되면 정해진 확률에 따라 강화가 진행되는 것이다.

우선 버튼과 기능을 연계하는 코드를 작성 해 보도록 하자.

**UIManager.cs**

<pre>
<code>
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
        EnchantStep.text = "한계치까지 강화 하였습니다.(10강)";
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
</code>
</pre>

코드가 조금 길기 때문에 함수별로 기능을 요약하여 설명하도록 하겠다.

우선 ChangeEnchantWeapon(int itemCode) 는 강화 테이블에 대상 무기를 올리는 함수이다.

그림에서는 위에 있는 3개의 무기 선택 버튼과 연결하여 매개변수로 무기의 종류 번호를 넣어주게 되면 PlayerItem.cs 안에 있는 weapons 배열에 저장된 무기 정보들의 itemCode와 비교하여 해당되는 정보를 올려주게 된다. (enchant.SetTarget(imsi);)

itemCode를 비교하여 맞는 WeaponItemInfo 객체를 찾아내는 역할을 하는 함수인 checkWeapon()을 사용하였다.

아이템을 세팅하였으면 세팅 한 아이템 정보를 UI에 내보내 주는 SetInfo() 함수를 실행한다.

매개변수로는 앞서 찾은 WeaponItemInfo 객체를 넣어 주어 해당하는 정보들을 노출시킨다.

그러면 아래와 같이 나오게 된다.

![image](https://user-images.githubusercontent.com/66288087/193277708-bf09e03a-cf56-4855-b664-7be60541a4ff.png)

현재 무기 상태와 강화 성공 시 증가하는 스탯을 같이 세팅 해 주었다.

강화 버튼을 누르게 되면 doEnchant() 함수가 실행되게 되어 enchant의 강화 함수로 연결시켜 준다.

그리고 만약 강화 최대치까지 도달하게 되면 아래 사진과 같이 더 이상 강화할 수 없게 만들어 준다.

![image](https://user-images.githubusercontent.com/66288087/193278909-e27c8fc6-6e03-4854-9c11-e696bef00d59.png)


이제 강화 로직이 담긴 Enchanting.cs 함수를 살펴 보자

<hr>

### 강화 

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enchanting : MonoBehaviour
{
    int sumGold; // 누적 골드

    int randomNum;
    public bool isWeaponOn; // 무기가 올려져 있는 상태인가?
    public GameObject player; // 플레이어 객체
    
    public WeaponItemInfo EnchantTarget; // 강화 대상의 정보
    PlayerItem playerItem; // 플레이어 아이템 정보
    public int returnIndex; // 강화 내용을 적용 할 index

    // 강화 시스템 정보들
    public int[] addMeleeAtk = new int[] { 2, 2, 3, 3, 5, 6, 7, 8, 10, 30 }; // 근접 무기 공격력 상향폭
    public int[] addGunAtk = new int[] { 1, 1, 2, 3, 4, 4, 5, 5, 6, 15 }; // 총 공격력 상향 폭
    public int[] addMGunAtk = new int[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 10 }; // 머신건 공격력 상향 폭

    public float[] minusMeleeDelay = new float[] { 0, 0, 0, 0, 0, 0.01f, 0.01f, 0.01f, 0.02f, 0.05f };
    public float[] minusGunDelay = new float[] { 0, 0, 0, 0, 0, 0.005f, 0.005f, 0.005f, 0.005f, 0.02f };
    public float[] minusMGunDelay = new float[] { 0, 0, 0, 0, 0, 0.005f, 0.005f, 0.01f, 0.01f, 0.03f };

    public float[] percentage = new float[] { 0.8f, 0.75f, 0.7f, 0.65f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0.1f }; // 성공 확률

    public float[] addMeleeCritical = new float[] { 0, 0, 0, 0, 0, 0, 0.05f, 0.05f, 0.15f, 0.25f }; // 근접 크리티컬 확률 증가폭 (max 50%)
    public float[] addGunCritical = new float[] { 0, 0, 0, 0, 0, 0, 0.05f, 0.05f, 0.1f, 0.15f }; // 일반 총 크리티컬 확률 증가폭 (max 35%)
    public float[] addMGunCritical = new float[] { 0, 0, 0, 0, 0, 0, 0.02f, 0.03f, 0.05f, 0.15f }; // 머신 건 크리티컬 확률 증가폭 (max 25%)

    public int[] needGold = new int[] { 1000, 1100, 1200, 1300, 1400, 2000, 2400, 2800, 3200, 4500 }; // 필요 골드

    private void Awake()
    {
        EnchantTarget = null;
        isWeaponOn = false;
        playerItem = player.GetComponent<PlayerItem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTarget(WeaponItemInfo weaponInfo)
    {
        EnchantTarget = weaponInfo;
    }

    public void ResetTarget()
    {
        EnchantTarget = null;
    }

    public void doEnchant()
    {
        if (!isWeaponOn)
            return;

        if (EnchantTarget.enchantCount == EnchantTarget.maxEnchant)
            return;

        randomNum = 0;

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        randomNum = UnityEngine.Random.Range(1, 1001); // 1~1000 의 값을 랜덤으로 생성한다.

        sumGold += needGold[EnchantTarget.enchantCount];

        Debug.Log(randomNum + ", 누적 사용 골드 : " + sumGold);

        if (randomNum <= (int)(percentage[EnchantTarget.enchantCount] * 1000))
        {
            Debug.Log(EnchantTarget.enchantCount + "강 강화 성공!");

            EnchantSuccess();

        }
        else
        {
            Debug.Log("강화 실패..");
        }


    }

    void EnchantSuccess()
    {
        // 임시 정보 적용
        switch (EnchantTarget.weaponCode)
        {
            case 0:
                EnchantTarget.enchantAtk += addMeleeAtk[EnchantTarget.enchantCount];
                EnchantTarget.enchantDelay -= minusMeleeDelay[EnchantTarget.enchantCount];
                EnchantTarget.criticalPercent += addMeleeCritical[EnchantTarget.enchantCount];
                // 돈 까이는거 추가할 것
                break;
            case 1:
                EnchantTarget.enchantAtk += addGunAtk[EnchantTarget.enchantCount];
                EnchantTarget.enchantDelay -= minusGunDelay[EnchantTarget.enchantCount];
                EnchantTarget.criticalPercent += addGunCritical[EnchantTarget.enchantCount];
                // 돈 까이는거 추가할 것
                break;
            case 2:
                EnchantTarget.enchantAtk += addMGunAtk[EnchantTarget.enchantCount];
                EnchantTarget.enchantDelay -= minusMGunDelay[EnchantTarget.enchantCount];
                EnchantTarget.criticalPercent += addMGunCritical[EnchantTarget.enchantCount];
                // 돈 까이는거 추가할 것
                break;
        }

        EnchantTarget.enchantCount++;

        // 원본 적용을 해야 되는 줄 알았는데 원본도 같이 변한다. -> C에서 참조에 의한 호출이랑 비슷하게 변한다.
        // 즉, enchantTarget으로 playerItem의 weapons 원소를 받았는데 그것이 단순 값 복사가 아니었던 것이다.
        // 

        //playerItem.weapons[returnIndex].enchantAtk = EnchantTarget.enchantAtk;
        //playerItem.weapons[returnIndex].enchantDelay = EnchantTarget.enchantDelay;
        //playerItem.weapons[returnIndex].criticalPercent = EnchantTarget.criticalPercent;
        //playerItem.weapons[returnIndex].enchantCount++;

        playerItem.SetEnchantInfo(returnIndex);

    }

}
</code>
</pre>

우선, 강화 단계별로 증가하는 공격력, 공격 딜레이 감소치 등을 배열로 저장 해 두어, 현재 강화 단계를 Index로 손쉽게 접근할 수 있게 하였다.

그리고 SetTarget()을 통하여 매개변수로 들어 온 WeaponItemInfo 정보를 강화 테이블(EnchantTarget)에 올려 주며, doEnchant()를 통하여 확률에 따른 강화를 진행하게 된다.

doEnchant()는 만약 무기가 올려져 있지 않거나, 풀강 상태라면 바로 return을 하여 빠져 나가게 하였으며, 성공 확률도 배열로 하여 각 아이템별 확률을 쉽게 적용할 수 있게 하였다.

그리고 강화는 1~1000의 숫자를 랜덤으로 뽑아서 확률에 1000을 곱한 값으로 커트하여 확률을 조정하였다.

강화에 성공하면 EnchantSuccess()를 실행하여 해당 단계에 맞는 증가폭을 적용시킨다.

처음에는 원본도 변화를 적용시키려 하였으나 강화 테이블 위에 있는 놈만을 변화 시켜도 원본도 같이 변화하기에 주석처리 하였다. (주소에 의한 참조같은..건가 싶다)


따라서 테이블에 올려 진 것만 갱신해도 PlayerItem에 있는 정보들이 갱신된다.

이제 PlayerItem에서 실제 데미지 계산이 되는 Weapon에 정보를 갱신해 주면 된다.

<pre>
<code>
public void SetEnchantInfo(int returnIndex)
    {
        // 강화 후 정보를 무기에 적용하는 과정

        weapon = playerInfo.WeaponList[weapons[returnIndex].weaponCode].GetComponent<Weapon>();

        weapon.Damage = weapons[weapons[returnIndex].weaponCode].baseAtk + weapons[weapons[returnIndex].weaponCode].enchantAtk; // 강화 수치만큼 늘어나게끔!
        weapon.AtkDelay = weapons[weapons[returnIndex].weaponCode].baseDelay + weapons[weapons[returnIndex].weaponCode].enchantDelay; // 딜레이는 강화 수치만큼 줄어들게! (이미 enchantDelay가 마이너스이기 때문에 +를 적는다.)
        weapon.criticalPercent = weapons[weapons[returnIndex].weaponCode].criticalPercent; // 강화 수치만큼 늘어나게끔!

        if(weapons[returnIndex].type == Weapon.AtkType.Range)
        {
            Bullet bullet = weapon.bullet.GetComponent<Bullet>();
            bullet.SetDamage(weapon.Damage);
        }

    }
</code>
</pre>

PlayerItem.cs 코드의 일부이다. Weapon에 정보를 전달 해 주는 역할을 한다.

여기서 주의해야 할 점이, 아이템 정보에는 기본 공격력과, 강화로 인해 증가하는 공격력을 따로 설정 해 두었는데 Weapon에서는 다 합산되어 저장되기에 +=를 함부로 사용하다가는 이중으로 데미지가 증가할 수 있다.

그리고 무기 타입이 원거리일 경우, 총알에도 데미지를 같이 갱신해 주어야 한다.

<hr>

![image](https://user-images.githubusercontent.com/66288087/193284279-230964f7-2b2d-4ea3-b254-0c9321746e16.png)

풀강을 하고 나서 PlayerItem.cs의 정보가 갱신 된 모습이다.

![image](https://user-images.githubusercontent.com/66288087/193284505-0e0507db-4fbe-4e81-ac24-8c29d9e806e1.png)

Weapon.cs에도 정보가 갱신되었다.

이제 다음에는 강화 한 무기를 사용 해 볼 수 있는 몬스터를 만들어 보도록 하겠다.
