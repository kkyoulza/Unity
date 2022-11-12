using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enchanting : MonoBehaviour
{
    int sumGold; // ���� ���

    public AudioClip successSFX;
    public AudioClip failSFX;
    AudioSource audio;

    int randomNum;
    public bool isWeaponOn; // ���Ⱑ �÷��� �ִ� �����ΰ�?
    public GameObject player; // �÷��̾� ��ü
    UIManager ui;
    
    public WeaponItemInfo EnchantTarget; // ��ȭ ����� ����
    PlayerItem playerItem; // �÷��̾� ������ ����
    public int returnIndex = -1; // ��ȭ ������ ���� �� index

    // ��ȭ �ý��� ������
    public int[] addMeleeAtk = new int[] { 2, 2, 3, 3, 5, 6, 7, 8, 10, 30 }; // ���� ���� ���ݷ� ������
    public int[] addGunAtk = new int[] { 1, 1, 2, 3, 4, 4, 5, 5, 6, 15 }; // �� ���ݷ� ���� ��
    public int[] addMGunAtk = new int[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 10 }; // �ӽŰ� ���ݷ� ���� ��

    public float[] minusMeleeDelay = new float[] { 0, 0, 0, 0, 0, 0.01f, 0.01f, 0.01f, 0.02f, 0.05f };
    public float[] minusGunDelay = new float[] { 0, 0, 0, 0, 0, 0.005f, 0.005f, 0.005f, 0.005f, 0.02f };
    public float[] minusMGunDelay = new float[] { 0, 0, 0, 0, 0, 0.005f, 0.005f, 0.01f, 0.01f, 0.03f };

    public float[] percentage = new float[] { 0.8f, 0.75f, 0.7f, 0.65f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0.1f }; // ���� Ȯ��

    public float[] addMeleeCritical = new float[] { 0, 0, 0, 0, 0, 0, 0.05f, 0.05f, 0.15f, 0.25f }; // ���� ũ��Ƽ�� Ȯ�� ������ (max 50%)
    public float[] addGunCritical = new float[] { 0, 0, 0, 0, 0, 0, 0.05f, 0.05f, 0.1f, 0.15f }; // �Ϲ� �� ũ��Ƽ�� Ȯ�� ������ (max 35%)
    public float[] addMGunCritical = new float[] { 0, 0, 0, 0, 0, 0, 0.02f, 0.03f, 0.05f, 0.15f }; // �ӽ� �� ũ��Ƽ�� Ȯ�� ������ (max 25%)

    public int[] needGold = new int[] { 1000, 1100, 1200, 1300, 1400, 2000, 2400, 2800, 3200, 4500 }; // �ʿ� ���


    //���� ���̺�
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
        EnchantTarget = weaponInfo; // ���⼭�� �ܼ� �� ���簡 �ƴ϶� �� ��ü�� �������� ���̴�.
    }

    public void ResetTarget()
    {
        EnchantTarget = null;
    }

    public void doEnchant(int origin)
    {
        if (!isWeaponOn) // ���Ⱑ �÷��� ���� ���� ��
            return;

        if (EnchantTarget.enchantCount == EnchantTarget.maxEnchant) // Ǯ���� ��
            return;

        randomNum = 0;

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        randomNum = UnityEngine.Random.Range(1, 1001); // 1~1000 �� ���� �������� �����Ѵ�.

        sumGold += needGold[EnchantTarget.enchantCount]; // ���� ��� ��� ����

        if(playerItem.playerCntGold >= needGold[EnchantTarget.enchantCount]) // ���� ��ȭ ��뺸�� �� ���� ���� ��
        {
            playerItem.playerCntGold -= needGold[EnchantTarget.enchantCount]; // �� ���̴� �κ�
            // saveInfo.info.playerCntGold -= needGold[EnchantTarget.enchantCount];

            playerItem.enchantOrigin -= ui.useOrigin; // ��ȭ ������ ������� ����
            ui.useOrigin = 0;

            Debug.Log(randomNum + ", ���� ��� ��� : " + sumGold);
            // Debug.Log(origin + "���� ������� ���, " + origin * 0.5f + "%p�� Ȯ�� ����, ��� �� "+ (randomNum - origin * 5).ToString());

            if (randomNum - origin * 5 <= (int)(percentage[EnchantTarget.enchantCount] * 1000))
            {
                // ��� ���� ���� 1���� 0.5%�� Ȯ���� �þ�Ƿ� ���� 1���� 5�� ���� �־�� �Ѵ�.
                StartCoroutine(ui.noticeEtc(4));
                audio.clip = successSFX;
                audio.Play();
                Debug.Log(EnchantTarget.enchantCount + "�� ��ȭ ����!");   

                EnchantSuccess();

            }
            else
            {
                StartCoroutine(ui.noticeEtc(5));
                audio.clip = failSFX;
                audio.Play();
                playerItem.enchantOrigin += 2; // ������� 2�� ȹ��!
                Debug.Log("��ȭ ����..");
            }
        }
        else
        {
            StartCoroutine(ui.noticeEtc(0));
            Debug.Log("��ȭ ��� ����!");
        }

    }

    void EnchantSuccess()
    {
        // �ӽ� ���� ����
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

        // ���� ������ �ؾ� �Ǵ� �� �˾Ҵµ� ������ ���� ���Ѵ�. -> C���� ������ ���� ȣ���̶� ����ϰ� ���Ѵ�.
        // ��, enchantTarget���� playerItem�� weapons ���Ҹ� �޾Ҵµ� �װ��� �ܼ� �� ���簡 �ƴϾ��� ���̴�.
        // 

        //playerItem.weapons[returnIndex].enchantAtk = EnchantTarget.enchantAtk;
        //playerItem.weapons[returnIndex].enchantDelay = EnchantTarget.enchantDelay;
        //playerItem.weapons[returnIndex].criticalPercent = EnchantTarget.criticalPercent;
        //playerItem.weapons[returnIndex].enchantCount++;

        playerItem.SetEnchantInfo(returnIndex); // Weapon�� ���� ����
        // saveInfo.SaveItemInfo(playerItem.weapons[playerItem.returnIndex(EnchantTarget.weaponCode)]); // ������ ���� ���̺�

        // returnIndex = -1;

    }



}
