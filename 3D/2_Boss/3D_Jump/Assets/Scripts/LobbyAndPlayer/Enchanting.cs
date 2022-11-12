using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enchanting : MonoBehaviour
{
    int sumGold; // 누적 골드

    public AudioClip successSFX;
    public AudioClip failSFX;
    AudioSource audio;

    int randomNum;
    public bool isWeaponOn; // 무기가 올려져 있는 상태인가?
    public GameObject player; // 플레이어 객체
    UIManager ui;
    
    public WeaponItemInfo EnchantTarget; // 강화 대상의 정보
    PlayerItem playerItem; // 플레이어 아이템 정보
    public int returnIndex = -1; // 강화 내용을 적용 할 index

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
        ui = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManager>();
        audio = GetComponent<AudioSource>();
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

    public void doEnchant(int origin)
    {
        if (!isWeaponOn) // 무기가 올려져 있지 않을 때
            return;

        if (EnchantTarget.enchantCount == EnchantTarget.maxEnchant) // 풀강일 때
            return;

        randomNum = 0;

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        randomNum = UnityEngine.Random.Range(1, 1001); // 1~1000 의 값을 랜덤으로 생성한다.

        sumGold += needGold[EnchantTarget.enchantCount]; // 총합 사용 골드 가산

        if(playerItem.playerCntGold >= needGold[EnchantTarget.enchantCount]) // 돈이 강화 비용보다 더 많이 있을 때
        {
            playerItem.playerCntGold -= needGold[EnchantTarget.enchantCount]; // 돈 까이는 부분
            // saveInfo.info.playerCntGold -= needGold[EnchantTarget.enchantCount];

            playerItem.enchantOrigin -= ui.useOrigin; // 강화 순간에 기원조각 차감
            ui.useOrigin = 0;

            Debug.Log(randomNum + ", 누적 사용 골드 : " + sumGold);
            // Debug.Log(origin + "개의 기원조각 사용, " + origin * 0.5f + "%p의 확률 증가, 사용 후 "+ (randomNum - origin * 5).ToString());

            if (randomNum - origin * 5 <= (int)(percentage[EnchantTarget.enchantCount] * 1000))
            {
                // 기원 조각 개수 1개당 0.5%씩 확률이 늘어나므로 조각 1개당 5를 곱해 주어야 한다.
                StartCoroutine(ui.noticeEtc(4));
                audio.clip = successSFX;
                audio.Play();
                Debug.Log(EnchantTarget.enchantCount + "강 강화 성공!");   

                EnchantSuccess();

            }
            else
            {
                StartCoroutine(ui.noticeEtc(5));
                audio.clip = failSFX;
                audio.Play();
                playerItem.enchantOrigin += 2; // 기원조각 2개 획득!
                Debug.Log("강화 실패..");
            }
        }
        else
        {
            StartCoroutine(ui.noticeEtc(0));
            Debug.Log("강화 비용 부족!");
        }

    }

    void EnchantSuccess()
    {
        // 임시 정보 적용
        switch (EnchantTarget.weaponCode)
        {
            case 1:
                EnchantTarget.enchantAtk += addMeleeAtk[EnchantTarget.enchantCount];
                EnchantTarget.enchantDelay -= minusMeleeDelay[EnchantTarget.enchantCount];
                EnchantTarget.criticalPercent += addMeleeCritical[EnchantTarget.enchantCount];
                break;
            case 2:
                EnchantTarget.enchantAtk += addGunAtk[EnchantTarget.enchantCount];
                EnchantTarget.enchantDelay -= minusGunDelay[EnchantTarget.enchantCount];
                EnchantTarget.criticalPercent += addGunCritical[EnchantTarget.enchantCount];
                break;
            case 3:
                EnchantTarget.enchantAtk += addMGunAtk[EnchantTarget.enchantCount];
                EnchantTarget.enchantDelay -= minusMGunDelay[EnchantTarget.enchantCount];
                EnchantTarget.criticalPercent += addMGunCritical[EnchantTarget.enchantCount];
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

        // returnIndex = -1;

    }



}
