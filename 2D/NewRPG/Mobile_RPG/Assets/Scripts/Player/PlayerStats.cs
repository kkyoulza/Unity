using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter Ŭ���� ����� ���� ���ӽ����̽� �߰�

[System.Serializable]
public class StatInformation
{
    public string playerName;

    public int playerLevel;
    public int playerCntExperience;
    public int playerMaxExperience;

    public float playerMaxSpeed;
    public float playerJumpPower;
    public int playerMaxJumpCount;

    public float playerMaxHP;
    public float playerCntHP;
    public float playerMaxMP;
    public float playerCntMP;

    public int playerStrength;
    public int playerIntelligence;
    public int playerDefense;
    public int playerDodge;

    public float afterDelay;

    public StatInformation()
    {
        playerName = "James";

        playerLevel = 1;
        playerMaxExperience = playerLevel * 10;

        playerMaxSpeed = 3f;
        playerJumpPower = 5f;
        playerMaxJumpCount = 1;

        playerMaxHP = 50;
        playerCntHP = 50;
        playerMaxMP = 10;
        playerCntMP = 10;

        playerStrength = 10;
        playerIntelligence = 5;
        playerDefense = 3;
        playerDodge = 1;

        afterDelay = 0.2f;
    }

    public void minusOrAddHP(int num)
    {
        playerCntHP += num;
    }

}

public class PlayerStats : MonoBehaviour
{
    // ���� ����
    public StatInformation playerStat;
    
    // ���� �� ����
    public GameObject levelUpEffect;
    Animator levelUpAnim;
    AudioSource levelUpAudio;

    // Start is called before the first frame update
    void Awake()
    {
        playerStat = new StatInformation(); // ���� ���� �ε� ���θ� ������ ���ǹ��� ����� �� (���߿�)
        levelUpAnim = levelUpEffect.GetComponent<Animator>();
        levelUpAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        if(playerStat.playerCntExperience >= playerStat.playerMaxExperience)
        {
            playerStat.playerCntExperience -= playerStat.playerMaxExperience;
            playerStat.playerLevel++;
            playerStat.playerMaxExperience = playerStat.playerLevel * 10;

            levelUpEffect.SetActive(true);
            levelUpAnim.SetTrigger("LevelUp");
            levelUpAudio.Play();
            Invoke("offEffect", 0.8f);
        }
    }

    void offEffect()
    {
        levelUpEffect.SetActive(false);
    }
}
