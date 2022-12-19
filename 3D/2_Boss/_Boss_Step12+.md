#Step12+.버그 수정

### 로드 후 새로운 무기 습득 시 특정 무기가 미습득이 되는 현상

예상치 못한... 버그가 생겼다.

데이터를 로드 한 뒤, 기존에 얻지 못했던 머신건을 먹게 되니 기존에 있던 해머가 미습득이 되어 버리는 현상이 나타났다.

따라서 디버깅을 해 보니

![image](https://user-images.githubusercontent.com/66288087/208404454-3aaaaa4a-8084-4d9f-a568-81a547d064e5.png)

위 사진과 같이 머신 건을 먹기 전에는 해머 정보가 멀쩡하게 살아 있었지만

![image](https://user-images.githubusercontent.com/66288087/208404549-5acdbe94-f914-438f-8212-3d3ba65ee79c.png)

갑자기 해머 정보 위에 덮어 씌워지게 되는 현상이 생겼다..

이 것을 해결하기 위해 PlayerCode.cs에 무기 습득 부분 코드로 가 보겠다.

<pre>
<code>
void InterAction()
    {
        if(iDown && nearObject != null && !isJump && !isSwap)
        {
            // 무기에 닿고 있고(근처 오브젝트가 null이 아님), 점프 상태가 아닐 때, 상호작용 버튼을 누르게 되면 아이템을 습득하게 된다.
            // 이것을 응용하여 NPC와도 대화를 하게끔?

            if(nearObject.layer == 14)
            {
                Item nearObjItem = nearObject.GetComponent<Item>();
                switch (nearObjItem.type)
                {
                    case Item.Type.Weapon:
                        int weaponIndex = nearObjItem.value; // value를 index로 설정 할 것!

                        ui.PopUI.SetActive(false);
                        ui.isNoticeOn = false; // 무기를 먹었을 때 비활성화!
                        hasWeapons[weaponIndex] = true;

                        playerItem.GetInfo(nearObject);

                        saveinfo.SaveItemInfo(playerItem.weapons[playerItem.returnIndex(weaponIndex)]);
                        // 아이템 정보 세이브, 이 자체가 포인터에 의한 참조?가 되어서 강화를 할 때, 강화 창에서만 갱신을 해도 세이브 자료에서도 반영이 되게 된다. 

                        Destroy(nearObject);
                        break;
</code>
</pre>

중간에 끊은건 불편하긴 하지만... 그래도 코드를 살펴보면

아이템 앞에서 상호작용 키(E키)를 누르게 되면 상호 작용이 발동하며, 만약 아이템이 무기면 PlayerItem의 GetInfo() 함수로 이동하게 된다.

PlayerItem 함수로 가 보게 되면

<pre>
<code>
public void GetInfo(GameObject obj)
{
    Item item = obj.GetComponent<Item>();

    switch (item.type)
    {
        case (Item.Type.Weapon):

            PlayerCode playerCode = GetComponent<PlayerCode>();
            Weapon weapon = playerCode.WeaponList[item.value].GetComponent<Weapon>();

            WeaponItemInfo weaponinfo = new WeaponItemInfo(item.value, 0 ,weapon.maxEnchant, weapon.Damage, weapon.AtkDelay, weapon.type,weapon.criticalPercent);
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
</code>
</pre>

이 부분이 weapons 배열에 무기를 세팅하는 함수이다.

겹쳐서 무기 정보가 저장 된 것으로 보아 인덱스 부분을 집중적으로 살펴 보아야 할 것이다.

따라서 
<pre>
<code>
weapons[weaponIndex] = weaponinfo;
weaponIndex++;
</code>
</pre>
위 두 줄을 보게 되면 weaponIndex가 0부터 순차적으로 세팅됨을 알 수 있다.

방법은 두 가지이다.

1. Index 정보도 같이 저장한다
2. 매번 빈 Index를 찾게 만들어 준다.

그 중에서 2번을 사용 해 주도록 하겠다.

PlayerItem.cs에 새로운 함수를 만들어 준다.

<pre>
<code>
public int checkIndex(WeaponItemInfo[] input)
{
    for(int i = 0; i < input.Length; i++)
    {
        if(input[i].baseAtk == 0)
        {
            return i;
        }
    }

    return -1;
}
</code>
</pre>

baseAtk는 0이 될 수 없기에 baseAtk가 0이 되면 빈 곳이라고 판단하게 하였다.

물론 간단하게 추가할 수 있다는 장점이 있지만.. 부위가 많아지게 되면 함수를 더 만들어 주어야 한다는 단점도 있다.

(볼륨이 커지면 index를 저장하는 방식이 더 나을수도 있겠다는 생각이 든다.)

아무튼, 이 함수를 이용해서 위 두 줄을 아래와 같이 바꾸어 준다.

<pre>
<code>
weapons[checkIndex(weapons)] = weaponinfo;
</code>
</pre>

이렇게 해 주면

![image](https://user-images.githubusercontent.com/66288087/208406047-e0fb5eeb-d1d6-4f45-bc36-67a48d3b6090.png)

위 사진처럼 머신건에 대한 정보가 2번 인덱스에 잘 들어가게 됨을 볼 수 있다.


