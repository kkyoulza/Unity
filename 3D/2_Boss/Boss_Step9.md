# Step 9. 스테이지 설정

이제는 던전 형식으로 NPC에게 말을 걸어 들어가는 던전에 대한 설정을 하도록 하겠다.

<hr>

### 던전 구상

- 던전은 N개의 방으로 이루어 지도록 구성한다.
- 방마다 적이 스폰되어 해당 적들을 퇴치하게 되면 다음 방 문이 열리는 방식이다.

![image](https://user-images.githubusercontent.com/66288087/200790494-c82c6d26-8b3a-48b5-b708-17e3e24b2d3c.png)

일자형 던전의 모습이다.

#### 스폰 방식

- 스폰 방식은 돌을 스폰하는 것과 유사하게 빈 오브젝트를 만들어 플레이어가 닿게 되면 몬스터가 스폰되게끔 해 주도록 하겠다.
- Trigger 반응시 몬스터를 스폰하는 것은 빈 오브젝트에서 코드로 만들어 관리를 해 줄 예정이다. (OnTriggerEnter)
- 던전 매니저를 만들어 해당 부분에서 잡힌 몬스터의 수를 계수하여 or 일정 지역에서 생존하고 있는 몬스터가 0마리가 되면 문이 열리게끔 할 예정이다.
- 사냥터와 같이 꾸준하게 n마리의 몬스터를 유지하는 컴포넌트도 개발할 것

<hr>

+ 단순히 코드로 1회성 기능을 개발하는 것이 아닌, 재사용할 수 있는 컴포넌트를 개발한다고 생각할 것

<hr>

#### 스폰 컴포넌트

스폰 컴포넌트에서는 스폰 위치를 지정 해 주며, 해당 스테이지에서 모든 몬스터를 퇴치했을 경우 다음 문이 열리게끔 해 준다.

몬스터는 순차적으로 생성된다는 점을 이용하여 GameObject.FindObjectsWithTag()를 이용하여 Update()문에서 객체 리스트들을 불러 와 준다.

그 리스트의 길이가 0이 되면 다음 문이 열리게 되는 것이다.

스폰 컴포넌트의 대략적인 코드는 아래와 같다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStart : MonoBehaviour
{
    public int zoneNum; // 해당 존
    public int mobCount; // 처음에 생성 될 몹 개수

    public bool isZoneClear; // 해당 존 클리어 여부
    public bool isStarted; // 해당 존에 들어 갔는가?
    public bool isSpawned; // 몹이 스폰 되었는가?

    public int xRange; // x 범위 설정
    public int zRange; // z 범위 설정

    public GameObject nextDoor; // 다음으로 열릴 문
    public GameObject[] spawnMonsters; // 현재 범위 안에 있는 몬스터들 리스트(다 잡았는가 확인하기 위함!)

    public GameObject spawnTypeA;
    public GameObject spawnTypeB;
    public GameObject spawnTypeC;
    public GameObject spawnBoss; // boss 소환


    // Start is called before the first frame update
    void Awake()
    {
        if(zoneNum == 0)
        {
            // 0일때는 맨 처음에 바로 소환!
            for(int i = 0; i < mobCount; i++)
            {
                int ran = Random.Range(-xRange, xRange);
                int ran2 = Random.Range(-zRange, zRange);
                Vector3 pos = transform.position;
                pos.x += ran;
                pos.z += ran2;
                Instantiate(spawnTypeA, pos, transform.rotation);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpawnMonsters();
        findMonster();
        checkMonster();
    }

    void SpawnMonsters()
    {
        if(isStarted && !isSpawned)
        {
            switch (zoneNum)
            {
                case 1:
                    for(int i = 0; i < mobCount/3; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeA, pos, transform.rotation);
                    }

                    for (int i = mobCount / 3; i < mobCount * 2 / 3; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeB, pos, transform.rotation);
                    }

                    for (int i = mobCount * 2 / 3; i < mobCount; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeC, pos, transform.rotation);
                    }

                    isSpawned = true;
                    break;
                case 2:
                    for (int i = 0; i < mobCount / 3; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeA, pos, transform.rotation);
                    }

                    for (int i = mobCount / 3; i < mobCount * 2 / 3; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeB, pos, transform.rotation);
                    }

                    for (int i = mobCount * 2 / 3; i < mobCount-1; i++)
                    {
                        int ran = Random.Range(-xRange, xRange);
                        int ran2 = Random.Range(-zRange, zRange);
                        Vector3 pos = transform.position;
                        pos.x += ran;
                        pos.z += ran2;
                        Instantiate(spawnTypeC, pos, transform.rotation);
                    }

                    isSpawned = true;
                    break;
                case 3:
                    Instantiate(spawnBoss, transform.position, transform.rotation); // 보스
                    isSpawned = true;
                    break;

            }
        }
    }
    void findMonster()
    {
        
        for(int i = 0; i < spawnMonsters.Length; i++)
        {
            spawnMonsters[i] = null;
        }
        // Collider에 닿고 있는 몬스터들을 찾는 것
        spawnMonsters = GameObject.FindGameObjectsWithTag("Enemy");
    }

    void checkMonster()
    {
        // 몬스터를 다 처치했는지 확인하는 것

        if(spawnMonsters.Length == 0 && !isZoneClear && (isStarted || zoneNum == 0))
        {
            Debug.Log("몬스터가 다 사라졌다!");
            Animator ani = nextDoor.GetComponent<Animator>();
            ani.SetTrigger("Open");
            AudioSource aud = nextDoor.GetComponent<AudioSource>();
            aud.Play();
            isZoneClear = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && zoneNum != 0 && !isStarted)
        {
            isStarted = true;
        }
    }

}
</code>
</pre>

처음 스폰을 제외하고는 플레이어가 존에 입장했을 때 스폰을 하게 된다.

![image](https://user-images.githubusercontent.com/66288087/201333669-a86e0d15-c5fb-4004-abd2-0a552d142b6b.png)

우선 스폰 하고자 하는 위치를 지정 한 다음, 위 사진과 같이 설정을 할 수 있도록 컴포넌트 화 하였다.

ZoneNum - 던전 안에서 방의 순서를 나타낸다. 사실 기능 상으로는 0번 방 or 그렇지 않음으로만 구분되지만 세부적인 정보 저장 때 활용할 계획이다.

Mob Count - 소환 할 몹의 개수이다.

isZoneClear - 존에 있던 몬스터를 다 퇴치하게 되면 해당 존을 클리어 한 것으로 간주한다.

isStarted - 존에 입장하여 몬스터의 스폰을 시작해도 되는가를 표기해 주는 bool 변수

isSpawned - 존에 몬스터가 스폰되었는가를 표기 해 주는 bool 변수(스폰이 Update문에 있기 때문에 한 번만 소환 해 주기 위함이다.)

xRange - 소환 위치에서 랜덤으로 좌표 격차를 설정하기 위함 (x 좌표)

zRange - 소환 위치에서 랜덤으로 좌표 격차를 설정하기 위함 (z 좌표)

NextDoor - 클리어 시에 다음 존으로 향하는 문을 열기 위해 오브젝트로 설정하였음

SpawnMonsters - 해당 존에 입장했을 때 소환을 시작 한다는 점에서 GameObject.FindObjectsWithTag()를 이용하여 몬스터의 정보를 저장하기 위한 GameObject 배열

SpawnTypeA,B,C,Boss - 스폰을 하기 위해 생성할 Prefab

<hr>

![image](https://user-images.githubusercontent.com/66288087/201334605-d5e926f0-4208-4869-8c2c-ff01b80eabcb.png)

입장 시에 플레이어가 위 사진과 같은 Trigger에 닿게 되면 스폰이 시작되게 된다.(isStarted)

맨 처음 존(zoneNum = 0)에서는 isStarted를 하지 않아도 체크할 수 있게끔 조건에 zoneNum == 0을 넣어 두었다.

(생각해 보니 UI에서 isStarted를 체크해도 될 일 같다..)

아무튼 이런 식으로 몬스터를 소환하여 보스까지 퇴치하게끔 해 준다.

<hr>

### 보상 보물상자

보상 보물상자는 아래 사진과 같이 에셋을 사용하였다.

![image](https://user-images.githubusercontent.com/66288087/201475721-111ee324-2388-4397-b234-ef07448d5831.png)

주변도 그럴싸하게 오브젝트들을 추가로 배치시켜 주었다.

보상 상자는 NPC에게 말을 거는 것과 같이 존을 만들어 주어 상호작용을 해 주면 박스가 열리면서 보상이 랜덤으로 나오게끔 해 주었다.

던전 내부의UI는 자체적으로 UI 매니저를 만들어 보상을 산정하는 코드를 넣어 두었다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonUI : MonoBehaviour
{
    // 보상 UI
    public GameObject rewardUI;
    public Text originTxt;
    public Text rubyTxt;
    public Text emeraldTxt;
    public Text goldTxt;
    public Text silverTxt;
    public Text sumTxt;

    // 정보 임시저장 변수
    int origin;
    int ruby;
    int emerald;
    int gold;
    int silver;
    int addGold;

    // 정보 연결 UI
    PlayerItem playerItem;
    PlayerCode playerCode;
    
    void Awake()
    {
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
        playerItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setReward()
    {
        // 보상 랜덤으로 설정!

        origin = Random.Range(20, 50); // 확률이 아닌 그냥 값을 대입시킨다.
        int rubyRan = Random.Range(1, 11);
        int emelaldRan = Random.Range(1, 11);
        int goldRan = Random.Range(1, 11);
        int silverRan = Random.Range(1, 11);

        switch (rubyRan)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                ruby = 0;
                break;
            case 5:
            case 6:
            case 7:
                ruby = 1;
                break;
            case 8:
            case 9:
                ruby = 2;
                break;
            case 10:
                ruby = 3;
                break;
        }

        switch (emelaldRan)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                emerald = 1;
                break;
            case 5:
            case 6:
            case 7:
                emerald = 3;
                break;
            case 8:
            case 9:
                emerald = 5;
                break;
            case 10:
                emerald = 7;
                break;
        }

        switch (goldRan)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                gold = 5;
                break;
            case 5:
            case 6:
            case 7:
                gold = 10;
                break;
            case 8:
            case 9:
                gold = 15;
                break;
            case 10:
                gold = 30;
                break;
        }

        switch (silverRan)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                silver = 20;
                break;
            case 5:
            case 6:
            case 7:
                silver = 50;
                break;
            case 8:
            case 9:
                silver = 80;
                break;
            case 10:
                silver = 200;
                break;
        }

        playerItem.enchantOrigin += origin;
        addGold = ruby * 10000 + emerald * 1000 + gold * 100 + silver * 10;
        playerItem.playerCntGold += addGold;

        StartCoroutine(setRewardUI());

    }

    IEnumerator setRewardUI()
    {
        originTxt.text = origin.ToString()+" 개";
        rubyTxt.text = ruby.ToString() + " 개";
        emeraldTxt.text = emerald.ToString() + " 개";
        goldTxt.text = gold.ToString() + " 개";
        silverTxt.text = silver.ToString() + " 개";
        sumTxt.text = "합 : "+addGold.ToString() + " Gold";

        rewardUI.SetActive(true);
        playerCode.isTalk = true;

        yield return new WaitForSeconds(3.0f);

        rewardUI.SetActive(false);
        playerCode.isTalk = false;

        SceneManager.LoadScene("Boss1");

    }

}
</code>
</pre>

기원 조각은 20개~50개 사이의 개수를 랜덤으로 주게끔 설정하였으며, 나머지는 40%, 30%, 20%, 10% 의 확률로 보상을 차등 지급하였다.

광맥을 채집할 때 나오는 동전을 주는 개념으로 설정하였으며, 모두 최대 보상을 받게 되면 42,000골드와 50개의 기원조각을 챙길 수 있게 하였다.

(개수에 대한 것은 나중에 밸런스를 맞춰 보도록 하겠다.)

보상UI는 아래와 같다.

![image](https://user-images.githubusercontent.com/66288087/201475861-f80db90f-48f9-4f70-8e42-440cd3b12fad.png)

테스트용 화면의 크기에 맞춰서 그렇지, 실제 풀 화면으로 해 보면 나쁘지 않은 비율이다.

아무튼, 보상을 받고 나서는 로비로 다시 이동하게끔 해 주었다.

<hr>

### 캐릭터가 죽었을 때의 이벤트

캐릭터는 체력이 0 이하가 되면 죽게 된다.

<pre>
<code>
void Update()
{
    InputKey();
    if (!isTalk) // 대화중이지 않을 때만 가능하게!
    {
        Move_Ani(); // 캐릭터 움직임
        Jump(); // 점프
        Attack(); // 공격
        ReLoad(); // 재장전
        TrunChar(); // 캐릭터 회전
        Dodge(); // 캐릭터 구르기
        Swap(); // 캐릭터 무기 변경
        InterAction(); // 상호작용
    }
    onUI(); // 캐릭터 UI창 열기
    calStatus(); // 캐릭터 스탯 계산
    checkHP(); // 남은 HP체크
    saveinfo.savePlayerStats(playerMaxHealth, playerHealth, playerMana,playerMaxMana, playerStrength, playerAccuracy, playerItem.playerCntGold,
        playerItem.enchantOrigin,playerItem.cntHPPotion,playerItem.cntMPPotion);
    // 플레이어 자체 스탯, 골드 양 저장
}

void checkHP()
{
    if(playerHealth <= 0 && !isTalk)
    {
        isDamage = true;
        isTalk = true;
        anim.SetBool("isDie", true);
        StartCoroutine(GoLobby());
    }
}

IEnumerator GoLobby()
{
    StartCoroutine(ui.noticeEtc(9999));

    yield return new WaitForSeconds(3.0f); // 3초 대기

    isTalk = false;
    isDamage = false;
    anim.SetBool("isDie", false);
    playerHealth = playerMaxHealth;
    playerMana = playerMaxMana;

    yield return null;

    SceneManager.LoadScene("Boss1");

    yield return null;

}
</code>
</pre>

캐릭터가 죽었는지 판단은 PlayerCode에서 이루어 지며, HP가 0 이하가 되면 움직이지 못하는 상태가 되는 동시에(isTalk), 애니메이션 발동, 3초 후 로비로 이동하게 된다.

(코루틴을 이용하였다.)

던전을 도는 모습은 나중에 완성 후, 영상으로 업로드 할 예정이다.

이제 다음은 잡화상점(포션, 총알 등)을 만들고, 던전 내부에서 총알 수집 등의 기능을 만들며, 던전 입장, 점프 맵 입장 시 퀘스트 형태를 추가 해 보도록 하겠다.

