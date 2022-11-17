using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[System.Serializable]
public class playerInfo
{
    public WeaponItemInfo[] weapons = new WeaponItemInfo[100]; // 먹은 무기 스탯 정보
    public int playerMaxHealth; // 최대 체력
    public int playerCntHealth; // 현재 체력
    public int playerCntMP; // 현재 마나
    public int playerMaxMP; // 최대 마나
    public int playerStrength; // 플레이어 힘 스탯
    public int playerAcc; // 플레이어 명중률
    public long playerCntGold; // 플레이어 현재 골드
    public int enchantOrigin; // 강화 기원 조각 개수
    public int HPPotion; // HP포션 개수
    public int MPPotion; // MP포션 개수
    public int strCnt; // str강화 횟수
    public int accCnt; // acc강화 횟수
    public int HPCnt; // HP강화 횟수
    public int MPCnt; // MP강화 횟수
    public bool[] isGained = new bool[3]; // 무기를 얻은 현황

}


public class SaveInfos : MonoBehaviour
{
    public playerInfo info = new playerInfo();

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("saveInfo"); // saveInfo Tag를 가진 놈들을 배열에 불러오고
        if (objs.Length > 1) // 만약 이미 전에 생성된 saveObj가 있다면 배열의 길이는 2가 될 것이다.
            Destroy(gameObject); // DontDestroy로 지정된 것은 Awake가 다시 실행되지 않으므로 새로 생성되는 것만 삭제한다.
        DontDestroyOnLoad(gameObject); // 사라지지 않게 선언한다.
        Debug.Log("Awake_Save");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveItemInfo(WeaponItemInfo weapon) // 무기 스탯 정보 저장
    {
        for(int i = 0; i < info.weapons.Length; i++)
        {
            if (info.weapons[i].baseAtk != 0) // 비어있지 않으면
                continue; // 다음 싸이클로!

            info.weapons[i] = weapon; // 비었을 때 저장!
            info.isGained[weapon.weaponCode] = true; // 무기를 얻은 현황도 갱신
            return; // 저장 후 함수 끝내기!
        }
    }

    public void savePlayerStats(int maxHealth, int cntHealth, int cntMP, int maxMP, int strength,int acc,long gold,int originCnt,int HPPotion,int MPPotion)
    {
        info.playerMaxHealth = maxHealth;
        info.playerCntHealth = cntHealth;
        info.playerCntMP = cntMP;
        info.playerMaxMP = maxMP;
        info.playerStrength = strength;
        info.playerAcc = acc;
        info.playerCntGold = gold;
        info.enchantOrigin = originCnt;
        info.HPPotion = HPPotion;
        info.MPPotion = MPPotion;
    }

    public void saveStatCnt(int strCnt, int accCnt, int HPCnt,int MPCnt)
    {
        info.strCnt = strCnt;
        info.accCnt = accCnt;
        info.HPCnt = HPCnt;
        info.MPCnt = MPCnt;
    }

}
