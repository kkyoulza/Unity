using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

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
        playerStat = new StatInformation(); // 파일 저장 로드 여부를 따져서 조건문을 사용할 것 (나중에)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
