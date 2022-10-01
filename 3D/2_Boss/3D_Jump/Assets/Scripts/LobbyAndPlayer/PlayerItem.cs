using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter Ŭ���� ����� ���� ���ӽ����̽� �߰�

[System.Serializable]
public class WeaponItemInfo
{
    public int weaponCode; // ������ �ڵ�
    public int enchantCount; // ������ ��ȭ ��ġ(n ��)
    public int maxEnchant; // �ش� ������ Ǯ��ȭ ��ġ
    public int baseAtk; // ���̽� ���ݷ�
    public int enchantAtk; // ��ȭ ���ݷ�
    public float baseDelay; // �⺻ ���� ������
    public float enchantDelay; // ��ȭ ������ ���� ��ġ
    public float criticalPercent; // ũ��Ƽ�� Ȯ��
    public Weapon.AtkType type; // ���� ���� Ÿ��

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
    // ������ ����
    public WeaponItemInfo[] weapons; // ������ �������� ��� �� �ִ� �迭�� �����.
    int weaponIndex;
    int maxIndex = 10;

    // ��ȭ ��� ������ ���� ��ü��
    PlayerCode playerInfo; // �÷��̾� ����
    Weapon weapon; // �÷��̾� ���� ����


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

    public void GetInfo(GameObject obj)
    {
        Item item = obj.GetComponent<Item>();

        switch (item.type)
        {
            case (Item.Type.Weapon):

                PlayerCode playerCode = GetComponent<PlayerCode>();
                Weapon weapon = playerCode.WeaponList[item.value].GetComponent<Weapon>();

                WeaponItemInfo weaponinfo = new WeaponItemInfo(item.value, 0,weapon.maxEnchant, weapon.Damage, weapon.AtkDelay, weapon.type,weapon.criticalPercent);
                if (weapon.bullet) // ���� ���Ÿ���� ���� ��ü�� ������ �Ѿ� �������� ���� �� �ش�.
                {
                    Bullet bullet = weapon.bullet.GetComponent<Bullet>();
                    bullet.SetDamage(weapon.Damage);
                }
                weapons[weaponIndex] = weaponinfo;
                weaponIndex++;
                break;
        }

    }

    public void SetEnchantInfo(int returnIndex)
    {
        // ��ȭ �� ������ ���⿡ �����ϴ� ����

        weapon = playerInfo.WeaponList[weapons[returnIndex].weaponCode].GetComponent<Weapon>();

        weapon.Damage = weapons[weapons[returnIndex].weaponCode].baseAtk + weapons[weapons[returnIndex].weaponCode].enchantAtk; // ��ȭ ��ġ��ŭ �þ�Բ�!
        weapon.AtkDelay = weapons[weapons[returnIndex].weaponCode].baseDelay + weapons[weapons[returnIndex].weaponCode].enchantDelay; // �����̴� ��ȭ ��ġ��ŭ �پ���! (�̹� enchantDelay�� ���̳ʽ��̱� ������ +�� ���´�.)
        weapon.criticalPercent = weapons[weapons[returnIndex].weaponCode].criticalPercent; // ��ȭ ��ġ��ŭ �þ�Բ�!

        if(weapons[returnIndex].type == Weapon.AtkType.Range)
        {
            Bullet bullet = weapon.bullet.GetComponent<Bullet>();
            bullet.SetDamage(weapon.Damage);
        }

    }

}
