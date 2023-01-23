# Step4. 캐릭터 성장, 강화

RPG에서 빠질 수 없는 것이 캐릭터의 성장이다.

캐릭터의 성장의 지표는 플레이어의 레벨과 스탯에서 나오게 된다.

따라서 PlayereStats.cs에 플레이어 레벨과 경험치를 추가 해 주도록 하자

<hr>

## -> 경험치 스탯 추가 및 경험치 바 

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

[System.Serializable]
public class StatInformation
{
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
```

경험치 수치는 현재 레벨 x 10으로 임시로 설정 해 주었다.

<br>

이제, 경험치 바를 만들어 주고, 실시간으로 갱신 해 주기 위해 Manager.cs에 새롭게 경험치 바를 세팅 해 주도록 하자

![image](https://user-images.githubusercontent.com/66288087/214018251-febc0ba3-fa74-47c5-a6f7-ad2e53a1e8d7.png)

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public GameObject HPBar;
    public GameObject MPBar;
    public GameObject EXPBar;

    RectTransform HPRect;
    RectTransform MPRect;
    RectTransform EXPRect;

    StatInformation stat;

    // Start is called before the first frame update
    void Awake()
    {
        HPRect = HPBar.GetComponent<RectTransform>();
        MPRect = MPBar.GetComponent<RectTransform>();
        EXPRect = EXPBar.GetComponent<RectTransform>();
    }

    private void Start()
    {
        stat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().playerStat;
    }

    // Update is called once per frame
    void Update()
    {
        HPRect.sizeDelta = new Vector2(250 * (stat.playerCntHP / stat.playerMaxHP), 60);
        MPRect.sizeDelta = new Vector2(250 * (stat.playerCntMP / stat.playerMaxMP), 60);
        EXPRect.sizeDelta = new Vector2(1080 * ((float)stat.playerCntExperience / (float)stat.playerMaxExperience), 50);
    }
}
```

경험치 바의 가로 길이는 1080이기 때문에 경험치 비율에 1080을 곱해 주었으며, 경험치 수치를 int로 설정하여 비율을 구하기 위해 나누는 순간 소수점이 버려지기 때문에 무조건 0이 나오게 된다.

따라서 각 수치마다 명시적으로 float로 형 변환을 해 준 다음 나누게 되면 비율에 따른 경험치 바 길이 조정을 할 수 있다.

<hr>

아, 물론 경험치 바도 HP,MP바와 같이 Anchor를 왼쪽으로 설정 해 주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/214018802-80c09fd4-57e7-434e-987e-454e21e4ab50.png)

<hr>

## -> 경험치 계산 및 레벨업



<hr>

## -> 몬스터 퇴치 시 경험치 수급


<hr>

## -> 스탯 창 UI 제작


<hr>

## -> 스탯 강화 로직 제작(스텟 포인트 사용)













