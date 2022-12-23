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
    public int playerCntMP; // ���� ����
    public int playerMaxMP; // �ִ� ����
    public int playerStrength; // �÷��̾� �� ����
    public int playerAcc; // �÷��̾� ���߷�

    public long playerCntGold; // �÷��̾� ���� ���
    public int enchantOrigin; // ��ȭ ��� ���� ����
    public int HPPotion; // HP���� ����
    public int MPPotion; // MP���� ����

    public int strCnt; // str��ȭ Ƚ��
    public int accCnt; // acc��ȭ Ƚ��
    public int HPCnt; // HP��ȭ Ƚ��
    public int MPCnt; // MP��ȭ Ƚ��

    public bool[] isGained = new bool[3]; // ���⸦ ���� ��Ȳ

}


public class SaveInfos : MonoBehaviour
{
    public playerInfo info = new playerInfo();
    public bool isLoad;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("saveInfo"); // saveInfo Tag�� ���� ����� �迭�� �ҷ�����
        if (objs.Length > 1) // ���� �̹� ���� ������ saveObj�� �ִٸ� �迭�� ���̴� 2�� �� ���̴�.
            Destroy(gameObject); // DontDestroy�� ������ ���� Awake�� �ٽ� ������� �����Ƿ� ���� �����Ǵ� �͸� �����Ѵ�.
        DontDestroyOnLoad(gameObject); // ������� �ʰ� �����Ѵ�.
        if (!isLoad)
        {
            LoadInfoFile();
            isLoad = true;
        }
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


    public void SaveInfoToFile()
    {

        string fileName = "PlayerInfo";
        string path = Application.dataPath + "/" + fileName + ".dat";

        FileStream fs = new FileStream(path, FileMode.Create); // ���� ��� ����
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fs, info); // ����ȭ �Ͽ� ����

        Debug.Log("���� ���� �Ϸ�");

        fs.Close();

        Application.Quit();

    }

    public void LoadInfoFile()
    {
        string fileName = "PlayerInfo";
        string path = Application.dataPath + "/" + fileName + ".dat";

        if (File.Exists(path))
        {
            // ���� ������ �����ϸ�

            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            playerInfo infoImsi = formatter.Deserialize(fs) as playerInfo; // �� ����ȭ ��, Ŭ���� ���¿� �´� ��ü�� �ٽ� ����

            info = infoImsi;

            fs.Close();
        }
        else
        {
            // ������ �������� ������
            Debug.Log("������ �������� ����");
        }
    }

}
