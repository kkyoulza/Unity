using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[System.Serializable]
public class playerInfo
{
    public WeaponItemInfo[] weapons = new WeaponItemInfo[100]; // ���� ���� ���� ����
    public int playerMaxHealth; // �ִ� ü��
    public int playerCntHealth; // ���� ü��
    public int playerStrength; // �÷��̾� �� ����
    public int playerAcc; // �÷��̾� ���߷�
    public long playerCntGold; // �÷��̾� ���� ���
    public bool[] isGained = new bool[3]; // ���⸦ ���� ��Ȳ

}


public class SaveInfos : MonoBehaviour
{
    public playerInfo info = new playerInfo();

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("saveInfo"); // saveInfo Tag�� ���� ����� �迭�� �ҷ�����
        if (objs.Length > 1) // ���� �̹� ���� ������ saveObj�� �ִٸ� �迭�� ���̴� 2�� �� ���̴�.
            Destroy(gameObject); // DontDestroy�� ������ ���� Awake�� �ٽ� ������� �����Ƿ� ���� �����Ǵ� �͸� �����Ѵ�.
        DontDestroyOnLoad(gameObject); // ������� �ʰ� �����Ѵ�.
        Debug.Log("Awake_Save");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveItemInfo(WeaponItemInfo weapon) // ���� ���� ���� ����
    {
        for(int i = 0; i < info.weapons.Length; i++)
        {
            if (info.weapons[i].baseAtk != 0) // ������� ������
                continue; // ���� ����Ŭ��!

            info.weapons[i] = weapon; // ����� �� ����!
            info.isGained[weapon.weaponCode] = true; // ���⸦ ���� ��Ȳ�� ����
            return; // ���� �� �Լ� ������!
        }
    }

    public void savePlayerStats(int maxHealth,int cntHealth,int strength,int acc,long gold)
    {
        info.playerMaxHealth = maxHealth;
        info.playerCntHealth = cntHealth;
        info.playerStrength = strength;
        info.playerAcc = acc;
        info.playerCntGold = gold;
    }

}
