using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter Ŭ���� ����� ���� ���ӽ����̽� �߰�

[System.Serializable]
public class StatInformation
{
    public float playerMaxSpeed;
    public float playerJumpPower;
    public int playerMaxJumpCount;
    public int playerHP;
    public int playerMP;

    public int playerStrength;
    public int playerIntelligence;
    public int playerDefense;
    public int playerDodge;

    public StatInformation()
    {
        playerMaxSpeed = 3f;
        playerJumpPower = 5f;
        playerMaxJumpCount = 2;
        playerHP = 50;
        playerMP = 10;

        playerStrength = 10;
        playerIntelligence = 5;
        playerDefense = 3;
        playerDodge = 1;
    }

}

public class PlayerStats : MonoBehaviour
{
    public StatInformation playerStat;

    // Start is called before the first frame update
    void Awake()
    {
        playerStat = new StatInformation(); // ���� ���� �ε� ���θ� ������ ���ǹ��� ����� �� (���߿�)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
