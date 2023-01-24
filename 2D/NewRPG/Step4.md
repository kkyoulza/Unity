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

### - 경험치 및 HP/MP UI 개선 및 코드 수정

경험치 계산을 구현하기 전에 우선, UI를 손 볼 필요가 있다.

경험치 바에서 수치적인 값을 표시하고, HP와 MP도 수치적인 값을 표시 할 필요가 있다.

![image](https://user-images.githubusercontent.com/66288087/214239310-807e7560-30b0-4080-abbd-4dbb0368bff7.png)

위 사진과 같이 텍스트를 배치 해 준다.

Hierarchy 관계도 중요하다.

![image](https://user-images.githubusercontent.com/66288087/214239389-92027b6c-cb66-4c77-8990-2d43e0b0b72e.png)

길이가 변하는 HP,MP,EXP 바는 텍스트와 대등한 라인의 계층을 가지게 하였다.

왜냐하면 바의 길이가 변하게 되면 하위에 있는 오브젝트들도 위치가 변하기 때문이다.

텍스트와 바의 크기는 독립적으로 될 수 있게 만들어 주었다.

<hr>

**코드 수정**

UI에 적용하는 값들은 Manager.cs를 통해 적용하였다.

추가로 변경 할 UI 사항들이 생겼으니 함수들을 만들어 알아보기 쉽게 정리 해 보자(리팩토링?)

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // UI Bar
    public GameObject HPBar;
    public GameObject MPBar;
    public GameObject EXPBar;

    // Bar 길이 정보
    RectTransform HPRect;
    RectTransform MPRect;
    RectTransform EXPRect;

    // 수치 정보
    public int ExpBarLength; // 경험치 바 가로 길이
    float expRate; // 경험치 비율

    StatInformation stat; // 플레이어 스탯

    // 텍스트 모음
    public Text ExpText;
    public Text HPText;
    public Text MPText;
    public Text LVText;

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
        SetEXPBar();
        SetHPMP();
        SetLV();
    }

    void SetEXPBar()
    {
        expRate = ((float)stat.playerCntExperience / (float)stat.playerMaxExperience);
        EXPRect.sizeDelta = new Vector2(ExpBarLength * expRate, 50);
        ExpText.text = stat.playerCntExperience + " ( " + string.Format("{0:0.00}", expRate * 100) + "% ) / " + stat.playerMaxExperience;
    }

    void SetHPMP()
    {
        HPRect.sizeDelta = new Vector2(250 * (stat.playerCntHP / stat.playerMaxHP), 60);
        MPRect.sizeDelta = new Vector2(250 * (stat.playerCntMP / stat.playerMaxMP), 60);

        HPText.text = stat.playerCntHP + " / " + stat.playerMaxHP;
        MPText.text = stat.playerCntMP + " / " + stat.playerMaxMP;

    }

    void SetLV()
    {
        LVText.text = "LV " + stat.playerLevel;
    }
}
```

종류별로 3개의 함수를 만들어 주었다.

- 경험치 바 & 텍스트 변동
- HP,MP 바 & 텍스트 변동
- 레벨 값 변동

<hr>

### string.Format 기능 정리

경험치 비율을 기록하기 위해 string.Format을 통해 비율 값을 소수점 2자리까지 끊어주는 방법을 사용하였다.

string.Format은 소수들의 표시 형태를 정하는 것이다.

예를 들어

```c#
string.Format("{0:0.00} {1:0.000}",1.234,1.2345);
```

중괄호 {  } 속에 있는 것에서 **콜론 왼쪽**에 있는 0,1.. 은 **매개변수의 순서**이다.

**콜론 오른쪽**에 있는 **형태**를 **몇 번째 매개변수에 적용** 할 것인지 설정하는 것이다.

즉, 위 코드에서 출력되는 것은 **1.23 1.234** 이다.

<hr>

위 Format을 통해 경험치의 비율을 출력하였으며, 각 기기마다 다른 경험치 바 길이를 쉽게 설정하기 위해 **ExpBarLength** 라는 **public 변수**를 하나 만들어 주었다.

그리고 expRate도 어차피 비율을 text에 적어야 하기 때문에 변수로 만들어 저장 해 주었다.

<hr>

또한, 위 사진에서 Hierarchy에 Exp Base라고 보일 것이다.

경험치가 거의 없을 때는 경험치 수치 텍스트가 지형에 가려 보이지 않기 때문에 뒤에 하얀 배경을 깔아 주었다.

이렇게 해 주면 아래 사진과 같이 UI가 세팅된다.

<hr>

![image](https://user-images.githubusercontent.com/66288087/214243143-ea9659fe-ef9c-4668-aa53-87f800012061.png)

UI개선 후 모습이다.

<hr>

## -> 몬스터 퇴치 시 경험치 수급 및 레벨 업

이제 몬스터를 퇴치 했을 때 경험치가 수급되게끔 만들어 주자

(위 사진에서는 이미 경험치가 수급 된 모습이긴 하다.)

### - 몬스터를 때린 플레이어 리스트 만들기

몬스터가 잡혔을 때, 어떻게 플레이어를 추적해서 경험치를 더해 줄 수 있을까?

바로 몬스터가 피격당하는 오브젝트인 스킬 오브젝트를 통하여 플레이어에 연결을 할 수 있을 것이다.

스킬 오브젝트에 플레이어 오브젝트를 세팅 해 둔 다음, 몬스터 내부에서 피격 시 때린 플레이어를 저장하여 몬스터가 퇴치되었을 때, 경험치를 배달시켜 주는 것이다.

![image](https://user-images.githubusercontent.com/66288087/214246138-fe3ccdf0-502b-40af-bf8a-b9e755e2d453.png)

위 그림에서 일련의 과정들을 정리하였다.

우선 피격 시에 캐릭터가 가진 스킬 이펙트 오브젝트를 추적하여 몬스터 내부에 때린 놈 리스트를 만들어 보도록 하자

**Enemy.cs 중**

```c#
public List<GameObject> attackObj; // 몬스터를 타격하는 오브젝트 모음

void checkHitList(GameObject input)
{
    GameObject addObj = input.GetComponent<SkillInfo>().player.gameObject;
    // 몬스터를 때린 놈들에 대한 리스트 생성 -> 경험치 분배할 때 사용 예정
    if(attackObj.Count == 0)
    {
        attackObj.Add(addObj);
        return;
    }

    Player inputPlayerInfo = input.GetComponent<SkillInfo>().player;

    for(int i = 0; i < attackObj.Count; i++)
    {
        if (attackObj[i].GetComponent<Player>().userId == inputPlayerInfo.userId) // 이미 등록되어 있다면
            return; // 리턴
    }

    attackObj.Add(addObj); // 등록되어 있는 것이 없다면 새로 등록

}
```

attackObj로 리스트를 만들어 주고, Trigger 이벤트에서 collision.gameObject를 매개변수로 받아 실행하는 checkHitList(input)를 만들어 주었다.

**input으로 들어오는 게임오브젝트**는 **스킬 이펙트**이며, **attackObj에 저장**되는 것은 **Player 오브젝트**이다.

이 점을 잘 유의하여 코드를 이해 해 보도록 하자.

처음에 리스트에 아무것도 없을 때는 무조건 추가하고 함수를 종료한다.

리스트에 무언가가 있다면 for문으로 리스트를 참고 해 준 다음 조건문을 통하여 Player의 userId를 체크 해 준다.

같은 userId를 가진 오브젝트가 이미 있다면 함수를 끝내며, userId가 같은 것이 없을 때는 마지막에 리스트에 추가 해 준다.

for문 속에 있는 조건문에서 attackObj와 inputPlayerInfo를 헷갈리지 말 것!!

inputPlayerInfo는 스킬 이펙트에 있는 Player 오브젝트 속에 있는 Player 컴포넌트이다.

이렇게 만들게 되면

![image](https://user-images.githubusercontent.com/66288087/214248413-b41ca06e-655f-4965-b2e2-3fd314b79aa8.png)

위 사진과 같이 몬스터 내부에 자동으로 몬스터를 때린 플레이어가 추가 되게 된다.

### - 몬스터를 퇴치했을 때, 경험치 분배하기

그렇다면 지금부터는 간단하다.

몬스터가 퇴치되었을 때, 경험치를 추가 해 주는 함수를 작성하면 된다.

**Enemy.cs중**

```c#
void sendEXP()
    {
        // 몬스터가 죽었을 때, 경험치를 보낸다.
        for(int i = 0; i < attackObj.Count; i++)
        {
            attackObj[i].GetComponent<PlayerStats>().playerStat.playerCntExperience += (int)((float)addExp / (float)attackObj.Count); 
        }

    }
```

아까 경험치 수급 내용을 정리한 그림에서처럼, 몬스터의 총 제공 경험치에 몬스터 퇴치에 관여한 모든 플레이어들의 수를 나누어 준 다음, 각 플레이어들에게 경험치를 분배 해 준다.

![image](https://user-images.githubusercontent.com/66288087/214248880-17650a37-35f1-47ec-8045-9d4564c34a7b.png)

그러면 아까 사진과 같이 경험치를 습득하게 될 것이다.

<hr>

### - 레벨 업 여부 체크하여 레벨 업 진행

경험치가 충분하게 쌓였으면, 레벨 업을 해야 한다.

레벨 업에 대한 체크는 경험치 정보를 저장한 PlayerStats.cs에서 하는 것이 좋겠다.

**PlayerStats.cs**

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
    
    public GameObject levelUpEffect;
    Animator levelUpAnim;
    AudioSource levelUpAudio;

    // Start is called before the first frame update
    void Awake()
    {
        playerStat = new StatInformation(); // 파일 저장 로드 여부를 따져서 조건문을 사용할 것 (나중에)
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
            Invoke("offEffect", 0.89f);
        }
    }

    void offEffect()
    {
        levelUpEffect.SetActive(false);
    }
}
```




<hr>

## -> 스탯 창 UI 제작


<hr>

## -> 스탯 강화 로직 제작(스텟 포인트 사용)













