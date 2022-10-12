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




<hr>

### 3. 골드 벌이 수단 추가(광산)






