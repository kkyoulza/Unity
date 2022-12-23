using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

[System.Serializable]
public class WeaponItemInfo
{
    public int weaponCode; // 아이템 코드
    public int enchantCount; // 아이템 강화 수치(n 강)
    public int maxEnchant; // 해당 아이템 풀강화 수치
    public int baseAtk; // 베이스 공격력
    public int enchantAtk; // 강화 공격력
    public float baseDelay; // 기본 무기 딜레이
    public float enchantDelay; // 강화 딜레이 감소 수치
    public float criticalPercent; // 크리티컬 확률
    public Weapon.AtkType type; // 무기 공격 타입

    public WeaponItemInfo(int code,int count,int maxCnt,int Atk,float delay,Weapon.AtkType type,float critical)
    {
        this.weaponCode = code;
        this.enchantCount = count;
        this.maxEnchant = maxCnt;
        this.baseAtk = Atk;
        this.baseDelay = delay;
        this.enchantAtk = 0;
        this.enchantDelay = 0.0f;
        criticalPercent = critical;
        this.type = type;
    }

}


public class PlayerItem : MonoBehaviour
{
    // 아이템 정보
    public WeaponItemInfo[] weapons; // 아이템 정보들이 들어 가 있는 배열을 만든다.
    public long playerCntGold; // 플레이어가 현재 가진 골드.
    public int enchantOrigin; // 강화 기원조각 개수
    public int cntHPPotion; // 현재 포션 개수
    public int cntMPPotion; // 현재 마나포션 개수

    int weaponIndex;
    int maxIndex = 10;

    // 강화 결과 저장을 위한 객체들
    PlayerCode playerInfo; // 플레이어 정보
    Weapon weapon; // 플레이어 원본 무기


    private void Awake()
    {
        weaponIndex = 0;
        weapons = new WeaponItemInfo[maxIndex];
        playerInfo = GetComponent<PlayerCode>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
                weapons[checkIndex(weapons)] = weaponinfo;
                // weaponIndex++;
                break;
        }

    }

    public void SetEnchantInfo(int returnIndex)
    {
        // 강화 후 정보를 무기에 적용하는 과정

        int weaponCode = weapons[returnIndex].weaponCode; // 무기 고유의 코드를 받아와야 한다.

        weapon = playerInfo.WeaponList[weaponCode].GetComponent<Weapon>(); // 무기 자체를 불러 온다.(WeaponList)

        weapon.Damage = weapons[returnIndex].baseAtk + weapons[returnIndex].enchantAtk; // 강화 수치만큼 늘어나게끔! (weapons에서는 returnIndex를 그대로!)
        weapon.AtkDelay = weapons[returnIndex].baseDelay + weapons[returnIndex].enchantDelay; // 딜레이는 강화 수치만큼 줄어들게! (이미 enchantDelay가 마이너스이기 때문에 +를 적는다.)
        weapon.criticalPercent = weapons[returnIndex].criticalPercent; // 강화 수치만큼 늘어나게끔!

        if(weapons[returnIndex].type == Weapon.AtkType.Range)
        {
            Bullet bullet = weapon.bullet.GetComponent<Bullet>();
            bullet.SetDamage(weapon.Damage);
        }

    }

    public int returnIndex(int itemCode)
    {
        int index = 0;

        for(int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].weaponCode == itemCode)
            {
                index = i;
                return index;
            }
        }

        index = -1; // 아직 안먹었을 때

        return index;
    }

}
