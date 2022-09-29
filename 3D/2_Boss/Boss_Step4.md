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

1. UI
2. 플레이어의 소지 아이템(장비) 관리 컴포넌트 -> 이것이 PlayerItem.cs이다.
3. 강화 로직

이렇게 세 가지가 필요하며, 세 가지가 서로 정보들을 공유하면서 업데이트를 해 주는 방식으로 진행 할 것이다.

<hr>

### 1. UI

강화 UI는 우선 대장장이에게 말을 걸어 강화 의사를 표시하게 되면 강화 창이 뜨게끔 하였다.

그러기 위해서는 우선 대화창과, 강화 창을 만들어 주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/193050572-ee0d1c4c-4c2a-4f1d-a835-d709701165c9.png)

대화창은 특별한 것 없이 앵커를 6시 방향으로 설정 하였으며, Margin을 적당히 설정하여 위치를 맞추어 주었다.

위 대화창에서 강화하기를 누르게 되면 강화 창이 뜨게 되며 강화 창은 아래와 같이 구성하였다.

![image](https://user-images.githubusercontent.com/66288087/193050691-8d433734-f5a5-40bf-b72c-68e6f23ba764.png)

위에 있는 무기 리스트를 누르게 되면 아래에 해당 무기로 사진이 변경되고, 강화 버튼을 누르게 되면 정해진 확률에 따라 강화가 진행되는 것이다.








