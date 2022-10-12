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


    //정보 세이브
    SaveInfos saveInfo;


    private void Awake()
    {
        EnchantTarget = null;
        isWeaponOn = false;
        playerItem = player.GetComponent<PlayerItem>();
        saveInfo = GameObject.FindGameObjectWithTag("saveInfo").GetComponent<SaveInfos>();
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
        EnchantTarget = weaponInfo; // 여기서는 단순 값 복사가 아니라 그 자체를 가져오는 것이다.
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

        if(playerItem.playerCntGold >= needGold[EnchantTarget.enchantCount])
        {
            playerItem.playerCntGold -= needGold[EnchantTarget.enchantCount];
            // saveInfo.info.playerCntGold -= needGold[EnchantTarget.enchantCount];

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
        else
        {
            Debug.Log("강화 비용 부족!");
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

        playerItem.SetEnchantInfo(returnIndex); // Weapon에 정보 갱신
        // saveInfo.SaveItemInfo(playerItem.weapons[playerItem.returnIndex(EnchantTarget.weaponCode)]); // 아이템 정보 세이브
    }



}
