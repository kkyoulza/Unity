# Step7. 정보 저장 및 골드 벌이 수단


유니티에서는 씬을 이동하게 되면 DontDestroyOnLoad를 사용하지 않은 오브젝트들이 없어지게 된다.

따라서 정보들을 어딘가에 따로 담아 놓았다가 다시 넣어 주는 방식을 생각하였고, 사용 할 계획이다.

그리고 앞서 만들었던 점프 맵을 끝내고 나면 점수에 일정 비율을 곱해서 골드로 보상하는 방식도 추가 할 계획이다. (계속 반복해서 클리어 할 수록 비율은 줄어들게)

<hr>

### 1. 맵 이동 시 정보 저장

우선 맵을 이동해도 정보가 저장되게끔 해 주겠다.

플레이어의 핵심적인 요소들을 저장시키면 될 것이다.

![image](https://user-images.githubusercontent.com/66288087/195344247-70afb305-9ac3-407c-86cc-1f8f67b4b23f.png)

![image](https://user-images.githubusercontent.com/66288087/195344298-97268833-1e94-4a04-9ee0-6a91688c6902.png)

우선, 점프 맵에서 스코어를 저장했던 것 처럼 빈 오브젝트를 새롭게 하나 만들어 주고, tag를 "saveInfo"로 설정 해 준다.

그 다음에, 오브젝트에 들어 갈 코드를 작성 해 주고, 정보가 들어 갈 클래스도 하나 만들어 준다.

코드 전문은 아래와 같다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[System.Serializable]
public class playerInfo
{
    public WeaponItemInfo[] weapons = new WeaponItemInfo[100]; // 먹은 무기 스탯 정보
    public int playerMaxHealth; // 최대 체력
    public int playerCntHealth; // 현재 체력
    public int playerStrength; // 플레이어 힘 스탯
    public int playerAcc; // 플레이어 명중률
    public long playerCntGold; // 플레이어 현재 골드
    public bool[] isGained = new bool[3]; // 무기를 얻은 현황

}


public class SaveInfos : MonoBehaviour
{
    public playerInfo info = new playerInfo();

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("saveInfo"); // saveInfo Tag를 가진 놈들을 배열에 불러오고
        if (objs.Length > 1) // 만약 이미 전에 생성된 saveObj가 있다면 배열의 길이는 2가 될 것이다.
            Destroy(gameObject); // DontDestroy로 지정된 것은 Awake가 다시 실행되지 않으므로 새로 생성되는 것만 삭제한다.
        DontDestroyOnLoad(gameObject); // 사라지지 않게 선언한다.
        // Debug.Log("Awake_Save");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveItemInfo(WeaponItemInfo weapon) // 무기 스탯 정보 저장
    {
        for(int i = 0; i < info.weapons.Length; i++)
        {
            if (info.weapons[i].baseAtk != 0) // 비어있지 않으면
                continue; // 다음 싸이클로!

            info.weapons[i] = weapon; // 비었을 때 저장!
            info.isGained[weapon.weaponCode] = true; // 무기를 얻은 현황도 갱신
            return; // 저장 후 함수 끝내기!
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
</code>
</pre>

여기서 주목해야 할 점이 Awake() 부분이다.

처음으로 만들어 지는 SaveObject는 DontDestroyOnLoad()를 사용하여 씬을 이동해도 없어지지 않는다.

그런데, 다른 맵에 갔다가 다시 해당 맵으로 오게 되면 없어지지 않은 SaveObject와 기존에 Scene에 세팅 해 놓은 SaveObject가 겹치게 된다.

즉, SaveObject가 2개가 되는 것이다. 따라서 새롭게 생기는 SaveObject를 없애 주어야 한다.

여기서 유니티의 생명 주기를 이용 해 준다. (생명주기에 대한 자세한 설명은 [여기](https://itmining.tistory.com/47) 있다.)

다른 맵에 갔다가 왔을 때는 이미 생성되어 있는 것이 아닌 새롭게 있던 것의 Awake()가 실행 될 것이다. 따라서 saveInfo 태그를 가진 오브젝트가 두 개 이상이라면 현재 새롭게 생성된 것이 삭제가 되는 것이다. (Destroy(gameObject);)

정보의 저장은 PlayerCode에서 불러 와서 Update()에서 실시간으로 저장 해 준다.

<pre>
<code>
void Update()
{
    InputKey();
    Move_Ani();
    Jump();
    Attack();
    ReLoad();
    TrunChar();
    Dodge();
    Swap();
    onUI();
    InterAction();
    saveinfo.savePlayerStats(playerMaxHealth, playerHealth, playerStrength, playerAccuracy, playerItem.playerCntGold);
    // 플레이어 자체 스탯, 골드 양 저장
}
</code>
</pre>

이렇게 저장을 해 주었으면 다시 플레이어에 원상 복구를 해 주는 오브젝트도 있어야 한다.

그런데, 이 때는 DontDestroyOnLoad()를 사용하지 않는다.

왜냐하면 씬을 이동하는 순간에 오브젝트가 생기면서 Awake()가 실행될 때, 다음 씬에 있는 빈 플레이어 데이터에 저장 된 데이터를 다시 세팅 할 것이기 때문이다.

따라서 새롭게 오브젝트를 만들고 DataSet.cs라고 이름지은 데이터 세팅 코드를 적고 오브젝트에 넣어 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSet : MonoBehaviour
{
    SaveInfos saveData; // 저장된 코드
    PlayerItem playerItem; // 플레이어 아이템 코드
    PlayerCode playerCode; // 플레이어 스탯이 저장된 코드
    UIManager uiManager; // UI 매니저


    // Start is called before the first frame update
    void Start()
    {
        saveData = GameObject.FindGameObjectWithTag("saveInfo").GetComponent<SaveInfos>();
        playerItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItem>();
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();

        if(GameObject.FindGameObjectWithTag("uimanager") != null)
        {
            uiManager = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManager>();
        }

        resetData();
        if (uiManager != null)
            reSetUI();

    }

    public void resetData()
    {

        playerCode.playerMaxHealth = saveData.info.playerMaxHealth;
        playerCode.playerHealth = saveData.info.playerCntHealth;
        playerCode.playerStrength = saveData.info.playerStrength;
        playerCode.playerAccuracy = saveData.info.playerAcc;
        playerItem.playerCntGold = saveData.info.playerCntGold;


        for (int i = 0; i < saveData.info.weapons.Length; i++)
        {
            if (saveData.info.weapons[i].baseAtk == 0)
                return;

            playerItem.weapons[i] = saveData.info.weapons[i];
            playerCode.hasWeapons[saveData.info.weapons[i].weaponCode] = true; // 무기를 얻은 여부도 반영 해 준다.
            playerItem.SetEnchantInfo(i); // playerItem -> weapon 반영(실제 데미지가 계산되는 곳으로)

        }
        
    }

    public void reSetUI()
    {
        uiManager.goldTxt.text = saveData.info.playerCntGold.ToString();
    }

}
</code>
</pre>

그런데 앞서 설명에서는 Awake()에서 다시 세팅을 해 준다고 했는데 여기서는 Start()를 사용했다.

왜냐하면 아래와 같은 에러가 뜨기 때문이다.

**IndexOutOfRangeException: Index was outside the bounds of the array. (wrapper stelemref) System.Object.virt_stelemref_class_small_idepth(intptr,object)**

처음에 이러한 에러가 떴을 때, 범위가 넘지 않는다고 생각했었기에 당황했었다.

그런데 차근 차근 디버깅을 해 보면서 오류가 생기는 지점을 탐색 해 본 결과, SaveObject가 만들어져 있는 곳에 다시 가게 될 때 생기게 됨을 알게 되었다.

즉, SaveObject 의 Awake() 와 DataSet.cs의 Awake()의 순서가 꼬이면서 오류가 생긴 것으로 추정되었다.

앞서 살펴 본 생명 주기에 따르면 Awake() -> Start()의 순서대로 주기가 이어진다.(사이에 있는 다른 과정들은 생략하였다.)

그런데 데이터를 다시 플레이어에 세팅하는 것도 Awake()로 하게 되면 빈 SaveObject가 만들어 지고, Awake()에서 SaveObject 개수를 센 다음 겹치지 않게 삭제하는 과정이 지나기 전에 데이터를 다시 세팅하려 하게 되면 데이터가 저장 된 SaveObject가 아닌, 삭제 될 예정인 SaveObject에 접근하는 경우가 생기게 되어 범위가 넘치게 되는 현상이 일어나지 않았나 생각이 들었다.. (범위가 직접적인 원인은 아닐 지 모르지만 Awake() 과정에서 문제가 생김은 맞는 것 같다.)

따라서 DataSet.cs 코드의 Awake() 부분을 Start()로 바꾸어 주었더니 오류가 생기지 않고 실행됨을 볼 수 있었다.

![boss_7_1](https://user-images.githubusercontent.com/66288087/195355758-90831b18-054e-4ad8-a950-e8bf90c4733b.gif)

맵을 이동해도 저장 되는 모습

<hr>

### 2. 점프 맵과 골드 벌이

앞서 점프맵을 만들었던 것을 기억 할 것이다.

점프 맵을 클리어 했을 때, 얻은 점수에 비례하여 코인을 얻게끔 해 주도록 하겠다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Managing : MonoBehaviour
{
    GameObject player; // 플레이어 오브젝트
    GameObject saveObject; // 정보 저장 오브젝트

    SaveInformation saveInfo; // 정보 저장 코드
    Vector3 startPos; // 시작 포지션(세이브 포인트를 먹기 전 리스폰 위치)

    public Text noticeText;
    public Text scoreText;
    public GameObject fallPanel; // 떨어졌을 때 나오는 판넬
    public GameObject panel; // 판넬

    int score; // UI 점수를 갱신할 때, 잠시 현재 점수를 불러와 저장하는 변수

    float onTime = 0f;
    float delTime = 3.0f;
    float onTime2 = 0f; // 떨어졌을 때의 알림 시간
    float delTime2 = 2.0f; // 떨어졌을 때의 알림 최대 지속

    bool isOn = false;
    bool fallNoticeOn = false; // 떨어졌을 때의 알림이 떠 있는 상태?

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        saveObject = GameObject.FindGameObjectWithTag("information");

        saveInfo = saveObject.GetComponent<SaveInformation>();
        startPos = player.transform.position;

        if(saveInfo.GetStage() == 1)
        {
            delTime = 3.0f;
        }
        else if(saveInfo.GetStage() == 2)
        {
            delTime = 5.0f;
            ShowInfoStage2();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            onTime += Time.deltaTime;
            if(onTime > delTime)
            {
                panel.SetActive(false);
                isOn = false;
                onTime = 0f;
            }

        }

        if (fallNoticeOn)
        {
            onTime2 += Time.deltaTime;
            if (onTime2 > delTime2)
            {
                fallPanel.SetActive(false);
                fallNoticeOn = false;
                onTime2 = 0f;
            }
        }
    }

    public void MoveToTarget(Vector3 target)
    {
        // 플레이어를 타겟으로 이동시키는 함수
        player.transform.position = target;
    }

    public void addScore(int num)
    {
        switch (num)
        {
            case 0: // silver
                score = int.Parse(scoreText.text);
                score++;
                saveInfo.addCntScore(1); // 점수 관리 오브젝트에 1점 추가
                scoreText.text = score.ToString();
                break;
            case 1: // gold
                score = int.Parse(scoreText.text);
                score += 10;
                saveInfo.addCntScore(10); // 점수 관리 오브젝트 갱신
                scoreText.text = score.ToString();
                break;
        }
    }


    public void ShowNotices(int num)
    {
        switch (num)
        {
            case 1:
                panel.SetActive(true); // 첫 번째는 내용 변화가 X
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }

                break;
            case 2:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "동전을 먹으면 점수가 올라갑니다!\n 동전을 최대한 많이 먹으면서 골인 지점까지 가면 돼요!";
                break;
            case 3:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "안 보이는 곳에도 동전이 있을 수 있어요!\n 만점을 받기 위해서는 눈썰미가 좋아야겠죠?";
                break;
            case 4:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "방금 먹은 노란색 꼬깔은 세이브 포인트에요!\n 바닥에 떨어지면 세이브 포인트로 복귀한답니다!";
                break;
            case 5:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "전방에 움직이는 파란 색 발판이 보이나요?\n 튕겨 나가지 않게 조심하세요!";
                break;
            case 6:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "앞에 보라색 발판이 보이나요?\n 통! 통! 튀기면서 저 멀리 하늘 위로 올라가 봐요!";
                break;
            case 7:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "앞에 노란색 발판은 크기가 줄었다가 늘었다가 하네요!\n 크기가 커지기를 기다렸다가 가는 것을 추천해요!";
                break;

        }
    }

    public void ShowFallNotice()
    {
        fallPanel.SetActive(true);
        if (!fallNoticeOn) // UI가 사라진 상태에서 UI 생성 시
        {
            fallNoticeOn = true;
        }
        else
        {
            onTime2 = 0f;
        }
    }

    public void ShowInfoStage2()
    {
        isOn = true;
        panel.SetActive(true);
        noticeText.text = "오른쪽, 왼쪽 길 중에 한 곳을 선택하세요!\n 맵에 대한 자세한 설명을 보시려면 물음표에 가까이 가주세요!";
    }
    public void ShowInfoStage2Plus()
    {
        panel.SetActive(true);
        if (!isOn) // UI가 사라진 상태에서 UI 생성 시
        {
            isOn = true;
        }
        else
        {
            onTime = 0f;
        }
        noticeText.text = "오른쪽 길 : 여러 가지 장애물들을 뚫고 가는 코스\n 왼쪽 길 : 세심한 컨트롤이 요구되는 코스\n 한 번 선택하면 바꾸지 못하니 주의!";
    }

    public void StageClearUI()
    {
        panel.SetActive(true);
        noticeText.text = "스테이지 클리어!\n 점수 : " + saveInfo.GetCntScore()+"\n 누적 점수 : "+saveInfo.GetTotalScore();
    }

    public void StageClearGainGoldUI()
    {
        panel.SetActive(true);
        noticeText.text = "전 스테이지 클리어 완료!\n 획득 골드의 10배 만큼 골드를 획득합니다!\n 획득 골드 : "+saveInfo.info.totalScore * 10+"G";
    }

}
</code>
</pre>

JumpMap의 Managing.cs 코드이다.

스테이지 3을 클리어할 때, 얻은 스코어에 값을 곱한 값을 골드로 환산하여 얻는 기능을 추가하였다.


<hr>

### 3. 골드 벌이 수단 추가(광산)


점프맵으로만 돈을 벌기에는 너무 지루하니 골드 벌이 수단을 추가하였다.

광산에 들어가 일정 시간마다 스폰되는 광맥을 부수게 되면 동전이 나오는 방식으로 하였다.

일정시간마다 나오게 하는 것이 핵심이기에 스폰을 담당하는 스포너 오브젝트를 하나 만들어 주도록 하겠다.

![image](https://user-images.githubusercontent.com/66288087/195987829-eca24fe6-6d29-4369-b1fd-65be49a195cf.png)

스폰 오브젝트는 위 사진처럼 가운데 Trigger Collider를 하나 두고(플레이어가 가까이 갔을 때 인식하기 위함), 자식 오브젝트로 Trail Renderer를 추가 하여 표시 할 원을 하나 더 만들어 준다.

![image](https://user-images.githubusercontent.com/66288087/195988343-98252f83-40f2-4d24-a393-dd4b26dbe7f2.png)

그리고 애니메이션 효과로 뱅글뱅글 돌게 해 준다.(부모 오브젝트에!)

그렇게 하면

![image](https://user-images.githubusercontent.com/66288087/195988386-231ce46e-316e-42c7-a26f-7120676947ec.png)

위 사진과 같이 뱅글뱅글 도는 존이 생기게 된다.

처음에 설정한 Trigger Collider에 tag를 추가 해 주고, PlayerCode에 TriggerStay를 넣어주게 되면 포탈과 NPC 대화 존이 되는 것이다.

이제 코드를 넣어 보자

Spawn.cs

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject spawnTypeA; // A타입 광물 - 은 광맥
    public GameObject spawnTypeB; // B타입 광물 - 금 광맥

    public GameObject onObject = null; // 트리거 위에 올려져 있는 오브젝트
    public float spawnTime; // 스폰 타임
    float deltaT; // 지나간 시간

    Vector3 spawnPlace;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (onObject == null) // 올라 가 있는 물체가 없으니 다음 광맥 스폰을 위한 카운트 다운
        {
            deltaT += Time.deltaTime;
        }
            

        if(deltaT >= spawnTime)
        {
            Spawn_Things();
            deltaT = 0f;
        }

        
    }

    public void Spawn_Things()
    {
        int ranNum = Random.Range(0, 4);
        spawnPlace = transform.position;
        spawnPlace.y += 3;
        switch (ranNum)
        {
            case 0:
            case 1:
            case 2:
                Debug.Log(ranNum);
                Instantiate(spawnTypeA, transform.position, transform.rotation);
                break;
            case 3:
                Debug.Log(ranNum);
                Instantiate(spawnTypeB, transform.position, transform.rotation);
                break;
        }
        

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 17)
            onObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 17)
            onObject = null;
    }

}
</code>
</pre>

아이템 습득 때 사용했던 nearObject를 활용한 코드이다.

스포너 위에 돌이 놓여져 있으면 스폰 타이머가 돌아가지 않고, 스폰타이머가 스폰 주기만큼 되었다면 확률에 의해서 두 종류의 광물 중 하나가 생성되게 하였다.

생성된 모습은 아래와 같다.

![image](https://user-images.githubusercontent.com/66288087/195988752-d8db04a4-eb40-48d7-becf-d876ffdfd9cf.png)

**돌 설정**

위 사진에서는 돌이 이미 만들어 진 모습이다..

일단 만드는 과정을 보게 되면

![image](https://user-images.githubusercontent.com/66288087/195988785-0134e893-9d51-4863-847e-80f9dcc05ef6.png)

돌 모양의 에셋을 가져 온 다음, Mesh Collider를 가져 와 준다.

![image](https://user-images.githubusercontent.com/66288087/195988804-c3bd3795-e668-4e85-a6b0-4045ab3c1e4c.png)

Mesh Collider는 Mesh 모양에 맞춰서 Collider가 생성되는 것이다.

맨 위에 있는 Convex를 체크 해 주면 생기게 된다.

두 개를 만들어서 하나는 Collider로 사용하고, 다른 하나는 Trigger로 체크 해 준다.(스폰 타임 체크 용)

그리고 돌은 Enemy 코드를 상속받아 사용하며, Enemy를 상속받는 만큼 돌에 적용되서는 안되는 부분들을 수정 해 준다.

Enemy.cs를 상속받은 Rock.cs이다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Enemy
{
    public GameObject silverC;
    public GameObject goldC;
    public GameObject emeraldC;
    public GameObject rubyC;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead && enemyType == Type.RockA)
        {
            StartCoroutine(DropCoinsA());
        }
        else if(isDead && enemyType == Type.RockB)
        {
            StartCoroutine(DropCoinsB());
        }
    }

    void DropCoins(GameObject dropType, int cntRanNum, int minCount, int upperDistance)
    {
        Debug.Log(cntRanNum + " : " + dropType.name);

        if (cntRanNum >= 1 && cntRanNum < 41)
        {
            // 40%
            for (int i = 0; i < minCount; i++) // silver 5
            {
                GameObject insCoin = Instantiate(dropType, transform.position, transform.rotation);

                Rigidbody coinRigid = insCoin.GetComponent<Rigidbody>();

                Vector3 caseVec = transform.forward * Random.Range(-3, 10) + Vector3.up * Random.Range(15, 20); // 랜덤한 힘으로 배출!

                coinRigid.AddForce(caseVec, ForceMode.Impulse);
                coinRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
            }

        }
        else if (cntRanNum >= 41 && cntRanNum < 71)
        {
            // 30%
            for (int i = 0; i < minCount + upperDistance; i++) // silver 8
            {
                GameObject insCoin = Instantiate(dropType, transform.position, transform.rotation);

                Rigidbody coinRigid = insCoin.GetComponent<Rigidbody>();

                Vector3 caseVec = transform.forward * Random.Range(-3, 10) + Vector3.up * Random.Range(15, 20); // 랜덤한 힘으로 배출!

                coinRigid.AddForce(caseVec, ForceMode.Impulse);
                coinRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
            }
        }
        else if (cntRanNum >= 71 && cntRanNum < 91)
        {
            // 20%
            for (int i = 0; i < minCount + upperDistance*2; i++) // silver 12
            {
                GameObject insCoin = Instantiate(dropType, transform.position, transform.rotation);

                Rigidbody coinRigid = insCoin.GetComponent<Rigidbody>();

                Vector3 caseVec = transform.forward * Random.Range(-3, 10) + Vector3.up * Random.Range(15, 20); // 랜덤한 힘으로 배출!

                coinRigid.AddForce(caseVec, ForceMode.Impulse);
                coinRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
            }
        }
        else
        {
            // 10%
            for (int i = 0; i < minCount + upperDistance * 3; i++) // silver 15
            {
                GameObject insCoin = Instantiate(dropType, transform.position, transform.rotation);

                Rigidbody coinRigid = insCoin.GetComponent<Rigidbody>();

                Vector3 caseVec = transform.forward * Random.Range(-3, 10) + Vector3.up * Random.Range(15, 20); // 랜덤한 힘으로 배출!

                coinRigid.AddForce(caseVec, ForceMode.Impulse);
                coinRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전하는 힘을 주는 것!
            }
        }
    }

    IEnumerator DropCoinsA()
    {
        // yield return new WaitForSeconds(0.3f);

        int ranSilverCount = Random.Range(1, 101);
        int ranGoldCount = Random.Range(1, 101);
        int ranEmerCount = Random.Range(1, 101);
        int ranRubyCount = Random.Range(1, 101);

        DropCoins(silverC, ranEmerCount, 5, 2); // 실버 코인을 최소 5개(40%), 확률이 내려갈 때 마다 4개씩 늘려서 드랍할 것
        // 40%, 30%, 20%, 10%
        DropCoins(goldC, ranGoldCount, 3, 2); // 골드 코인
        DropCoins(emeraldC, ranEmerCount, 1, 1); // 에메랄드 코인

        isDead = false;

        Destroy(gameObject);

        yield return null;

    }

    IEnumerator DropCoinsB()
    {
        // yield return new WaitForSeconds(0.3f);

        int ranSilverCount = Random.Range(1, 101);
        int ranGoldCount = Random.Range(1, 101);
        int ranEmerCount = Random.Range(1, 101);
        int ranRubyCount = Random.Range(1, 101);

        DropCoins(silverC, ranEmerCount, 4, 2); // 실버 코인을 최소 5개(40%), 확률이 내려갈 때 마다 4개씩 늘려서 드랍할 것
        // 40%, 30%, 20%, 10%
        DropCoins(goldC, ranGoldCount, 3, 2); // 골드 코인
        DropCoins(emeraldC, ranEmerCount, 2, 1); // 에메랄드 코인
        DropCoins(rubyC, ranEmerCount, 0, 1); // 루비 코인

        isDead = false;

        Destroy(gameObject);

        yield return null;

    }

}
</code>
</pre>

Enemy.cs를 상속받아 몬스터처럼 피격을 당하는 특성을 가지고 있다. 그리고 Enemy.cs에도 돌 타입들을 추가 해 주어 돌을 구분할 수 있게 하였다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // 몬스터 타입 설정
    public enum Type { A,B,C,Boss,RockA,RockB }; // 변수의 종류를 만든다.
    public Type enemyType; // 적의 타입을 넣을 변수

    // 체력 정보
    public int maxHealth;
    public int cntHealth;

    // 데미지 관련
    public GameObject PosObj; // 데미지 생성 위치에 있는 빈 오브젝트
    GameObject Damage_Prefab; // 데미지 프리팹
    GameObject Damage; // 데미지

    Vector3 dmgPos; // 데미지 위치

    // 물리 관련
    protected Rigidbody rigid;
    public BoxCollider meleeArea; // 공격 범위를 담을 변수
    public BoxCollider boxCollider; // 겉면 collider?

    // 원거리 몬스터 전용
    public GameObject monsterMissile; // 몬스터 미사일 프리팹을 담을 변수

    // 상태 관련
    public bool isAttack; // 공격을 하고 있는가?
    public bool isDead; // 죽은 상태인가?

    // 겉보기
    protected MeshRenderer[] mat;

    // 추적 관련
    public bool isChase; // 추적이 가능한 상황!
    public Transform target; // 추적 대상
    protected NavMeshAgent navi; // UnityEngine.AI를 필수로 쓸 것

    //애니메이션
    protected Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentsInChildren<MeshRenderer>(); // material을 가져오는 방법!!
        navi = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        Invoke("ChaseStart", 2.0f);
        target = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어를 추적하는 것이 default값

    }
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (navi.enabled)
        {// navi가 활성화 되었을 때만 목표를 추적! (기존에는 목표만 잃어버리고 움직이기는 하기 때문에 정지까지 하는 것으로 해 준다!)
            navi.SetDestination(target.position);
            navi.isStopped = !isChase; // 추적을 하고 있지 않을때(false) 정지를 하고(!false = true), 추적을 할 때 멈추는 것을 멈추게(움직이게) 한다.
        }
            
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity(); // 회전 속도 0으로 설정!
    }

    void Targeting()
    {
        // 공격을 하기 위한 타겟 설정
        float targetRadius = 0f;
        float targetRange = 0f;

        if (!isDead && enemyType != Type.Boss && enemyType != Type.RockA && enemyType != Type.RockB) // 죽은 상태가 아니고, 돌,보스가 아닐 때만 타겟팅을 실행
        {
            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3.0f;
                    break;
                case Type.B:
                    targetRadius = 1f; // 타겟을 찾을 두께 (티스토리 참고)
                    targetRange = 12.0f; // 플레이어 타겟팅 범위
                    break;
                case Type.C: // 원거리는 타겟팅이 넓고 정확해야 한다.
                    targetRadius = 0.5f;
                    targetRange = 25.0f; // 플레이어 타겟팅 범위
                    break;
            }
        }
        

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        // 자신의 위치, 구체 반지름, 나아가는 방향(어느 방향으로 쏠 것인가?), 거리, 대상 레이어
        
        if(rayHits.Length > 0 && !isAttack)
        {
            // 플레이어가 몬스터의 레이더 망에 감지됨과 동시에 공격 중이 아니라면!
            StartCoroutine("Attack"); // 공격!

        }
            

    }

    IEnumerator Attack()
    {
        // 일반적인 몬스터는 잠시 정지 후, 공격하고 다시 쫓아가는 패턴으로!

        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.5f); // 애니메이션 동작동안 딜레이!

                meleeArea.enabled = true; // 그 뒤에 박스 활성화를 하여 공격!

                yield return new WaitForSeconds(0.3f); // 공격 박스가 활성화 된 시간

                meleeArea.enabled = false;

                yield return new WaitForSeconds(0.8f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f); // 선 딜레이
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse); // 즉각적인 힘으로 돌격!
                meleeArea.enabled = true; // 돌격하는 동안 박스를 활성화!

                yield return new WaitForSeconds(0.5f); // 공격 박스가 활성화 된 시간
                rigid.velocity = Vector3.zero; // 일정 시간 돌격 후 멈춤!
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2.0f); // 후 딜레이
                break;
            case Type.C: // 미사일을 만들어야 한다.
                yield return new WaitForSeconds(0.4f); // 선 딜레이

                GameObject instantBullet = Instantiate(monsterMissile, transform.position,transform.rotation); // 몬스터와 같은 위치에 미사일 생성
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20; // 총알에 속도를 부여

                yield return new WaitForSeconds(2.0f); // 후 딜레이
                break;
        }


        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }


    void ChaseStart()
    {
        if(enemyType != Type.RockA && enemyType != Type.RockB)
        {
            isChase = true; // 추적을 가능하게 하고
            anim.SetBool("isWalk", true); // 애니메이션 상태를 변경!
        }
    }

    void FreezeVelocity() // 플레이어와 충돌 시 날라가서 추적을 하지 못하는 상황 방지
    {
        if (isChase) // 추적중일 때만 제약!
        {
            rigid.velocity = Vector3.zero; // 속도 0
            rigid.angularVelocity = Vector3.zero; // 회전 속도 0
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Damage_Prefab = Resources.Load("Prefabs/LobbyAndRPG/Damage") as GameObject; // 데미지 프리팹을 GameObject에 가져 온다.

            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            cntHealth -= weapon.Damage;

            Damage = MonoBehaviour.Instantiate(Damage_Prefab);
            Damage.GetComponent<Damage>().damage = weapon.Damage; // 오브젝트 속 데미지 컴포넌트에 있는 데미지 변수 세팅
            Damage.transform.position = PosObj.transform.position;

            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));

        }
        else if (other.tag == "Bullet")
        {
            Damage_Prefab = Resources.Load("Prefabs/LobbyAndRPG/Damage") as GameObject; // 데미지 프리팹을 GameObject에 가져 온다.

            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            cntHealth -= bullet.damage;

            Damage = MonoBehaviour.Instantiate(Damage_Prefab);
            Damage.GetComponent<Damage>().damage = bullet.damage; // 오브젝트 속 데미지 컴포넌트에 있는 데미지 변수 세팅
            Damage.transform.position = PosObj.transform.position;

            Vector3 reactVec = transform.position - other.transform.position;

            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }

    }
    

    IEnumerator OnDamage(Vector3 reactVec) // 피격시 반응 설정
    {
        foreach (MeshRenderer mesh in mat)
        {
            if(enemyType != Type.RockA && enemyType != Type.RockB)
                mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);

        if(cntHealth > 0)
        {
            foreach (MeshRenderer mesh in mat)
            {
                if (enemyType != Type.RockA && enemyType != Type.RockB)
                    mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in mat)
            {
                mesh.material.color = Color.gray;
            }

            gameObject.layer = 7; // rayCast에서와 달리 숫자로 그냥 적는다.
            isDead = true;
            isChase = false;
            if(enemyType == Type.A || enemyType == Type.B)
                meleeArea.enabled = false;
            if(navi != null)
                navi.enabled = false;
            if(anim != null)
                anim.SetTrigger("DoDie");

            if(enemyType != Type.RockA && enemyType != Type.RockB) // 돌이 아닐 때
            {
                reactVec = reactVec.normalized; // 몬스터가 죽을 때 팔짝 뛴 다음에 죽는 모습을 연출하기 위함
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 10, ForceMode.Impulse);
            }

            Destroy(gameObject,2); // 2초 뒤에 Destroy!
        }

    }

}
</code>
</pre>


그리고 두 종류의 돌을 만들어서 금색 돌에서는 코인이 조금 더 나오게끔 하였다.

스폰 타임은 20초로 설정하였다.

따로 함수를 만들어서(DropCoins) 확률과 편차를 넣어 주어 돌마다 다르게 확률을 설정하였다.

![image](https://user-images.githubusercontent.com/66288087/196464790-4afb0029-7bc7-4bbb-8cdb-a0afd6a7da8d.png)

돌을 깨게 되면 위 사진과 같은 결과가 나오게 된다.

다음 스텝에서는 체력, 총알 개수, 아이템 등의 UI와 연결지어 보도록 하겠다.


