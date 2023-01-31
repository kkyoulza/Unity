# Step5. csv를 이용한 퀘스트 구현

우선 이 글을 처음 적고 있는 시점은 스탯 강화를 구현하기 전이다..

갑작스레 퀘스트를 구현 할 방법을 생각하여 파일로 작은 테스트를 진행하여 정리 해 두고자 앞선 과정을 먼저 정리하려 한다.

물론 그러라고 Step+가 있는 것이지만.. 퀘스트는 중요하기 때문에 Step5를 미리 작성하고자 한다.

<hr>

## 퀘스트 구현의 목표

### 1. 다양한 종류의 퀘스트를 컴포넌트 내 설정으로 설정할 수 있어야 한다.
### 2. 파일을 이용하여 NPC 컴포넌트에서 수치 조정으로만 모든 대화를 불러올 수 있어야 한다.
### 3. 퀘스트 시작점을 알 수 있는 정보를 csv 파일 내부에 넣으며, 해당 정보를 NPC 컴포넌트에서 자체적으로 찾아 진행하여야 한다.

<hr>

## 대화 파일 구성

우선 대화 파일을 구성 해 보도록 하자

대화 파일은 ,로 구분하는 csv 파일 형태로 만들었으며, 엑섹 파일로도 할 수 있게끔 csv파일로 만들었다.

처음에는 메모장으로 작성하였다.

![image](https://user-images.githubusercontent.com/66288087/214528484-c63580b0-c62d-4c53-903e-3ee089dc53cd.png)

위 사진과 같이 구성하였다.

<hr>

## 파일 구성 설명

### Type : String에 들어 갈 종류 결정

대화 창에 있는 것을 대사로 볼 것인지, 퀘스트 설정 또는 보상 을 위한 특수 문자로 볼 것인지를 구분해 주는 부분이다.

0 - 일반 대화
1 - 퀘스트 수락 시 퀘스트 클리어 조건을 적음(조건은 아래 서술)
2 - 보상 지급 시 보상에 대한 정보를 적음(조건은 아래에 있음)

<br>

### NPC_Code : 대화를 시작하는 NPC 코드번호

NPC 자체 스크립트에서 NPC 코드를 기록하며, 해당 코드를 기반으로 대화를 구분한다.

즉, 해당 NPC와 관련된 대화를 시작할 때, **NPC 컴포넌트에서 설정 했던 NPC코드**와 **해당 대화를 매칭**하기 위해 사용하는 부분이다.

(탐색 속도가 너무 길어지지 않도록 NPC 코드별로 파일을 별도로 제작할 것 -> ex) 0~99번 NPC // 100~199번 NPC ...)

<br>

### dialogSeq : 대화 순서, 0부터 시작하고, 연속되어 있음. 대화의 끝은 -1로 표기

각 상황에서의 대화 순서이다. 예를 들어 처음 본 상황에서 대화를 쭉 할 것이고, 친해졌을 때에도 대화를 쭉 할 것이다.

한 상황에서 대화 순서를 구분 해 주기 위해 사용한다.

<br>

### dialogType : 주로 퀘스트가 아닌 일상 대화에 사용, 여러 타입의 대화를 랜덤 값으로 표기함. 각 타입별로 Seq가 존재 함.

위 설명에서는 퀘스트가 없는 경우만을 생각했지만.. 그래도 퀘스트가 있을 때도 몇 번째 퀘스트인지 상황을 구분해 줄 수도 있다.

즉, 각 상황을 구분 해 주는 숫자이다.(실제 형태는 string)

<br>

### Name : 대화 내용에서 말하고 있는 주체 표시

대화 창 좌상단에 나오는 이름 부분이다.

<br>

### dialogString : 실제 대화 내용이 들어 갈 공간

대화 창 중간에 나오는 대사 부분이다. 맨 처음 설명했던 Type에 따라서 해당 부분을 대화 자체로 봐서 그냥 출력 할 것인지, 퀘스트 조건으로 봐서 특정 퀘스트를 시작 할지 결정할 수 있다.

<br>

### isChoose : 선택지를 띄우는 대화 상태인가? 0 - 선택지 x, 1 - 선택지 o

대화의 끝(dialogSeq = -1) 다음에, Type이 1인 라인이 있는지 확인 하고, 만약 있다면 dialogString으로 바로 가서 아래와 같은 내용이 있는지 확인을 한다.

Type이 1이면 퀘스트를 세팅하는 라인이기 때문이다.

<hr>

### dialogString 내부 규칙

-> 만약 퀘스트 조건을 세팅하는 것이라면(Type = 1) 어떻게 대화창에 문자를 적어서 퀘스트를 세팅 할 것인가?

**'.' 으로 구분, 각 임무 종류별로 ':' 으로 구분**

**h(hunting 줄임말).몬스터 코드.잡아야 할 마릿수**
**g(gain 줄임말).아이템 코드.획득해야 할 개수**

ex > (퀘스트 코드 1) 12번 코드 몬스터를 50마리 잡고, 3번 코드 아이템을 30개 획득하시오 그리고 2번 npc한테 가서 알려주시오. --> **h.12.50.1.2:g.3.30.1.2**

#### parsing 순서 

-> 우선 : 으로 파싱을 하게 되면 ["h.12.50","g.3.30"]으로 구분이 되고, .으로 다시 파싱을 하게 되면 ["h","12","50], ["g","3","30"]으로 구분이 된다.

적힌 사항대로 퀘스트 리스트에 추가 해 준다.'

<hr>

## 대화 불러오기 코드 테스트(단순 대화만)

일차적인 테스트를 진행 해 보았다.

퀘스트 같은것 말고, 우선 기본적인 대화로 한 상황만을 불러오는 연습을 해 보았다.

csv 파일을 읽는 것은 구글링 하면 바로 나오는 것이 있었지만.. 너무 복잡해서 응용을 제대로 하지 못할 것 같아 [다른 직관적인 방법](https://foxtrotin.tistory.com/121)을 찾아 적용하였다.

기본적인 파일 읽기로 ReadLine()을 통해 한 줄 씩 읽으면서 Parsing을 진행하는 것이다.

NPC 컴포넌트가 미완성이기에 Manager.cs를 빌려 진행하였다.

```c#
public int setDialogType; // 어떤 대화 종류의 대화 시퀀스를 전부 가져 올 것인지 체크하는 것

public Dictionary<int, string> dialogDictionary = new Dictionary<int, string>(); // 대화 순서에 따른 대화 내용 저장
public Dictionary<int, string> nameDictionary = new Dictionary<int, string>(); // 대화 순서에 따른 이름 저장

public int cntDialogCode; // 현재 대화가 어디까지 진행 되었는가?

void ReadFile(string filePath)
{
    FileInfo fileInfo = new FileInfo(filePath);
    string value = "";

    if (fileInfo.Exists)
    {
        bool isEnd = false;
        StreamReader reader = new StreamReader(filePath);
        while (!isEnd)
        {
            value = reader.ReadLine();
            if (value == null)
            {
                isEnd = true;
                break;
            }
            var data_values = value.Split(',');

            if(data_values[3] == setDialogType.ToString()) // 딕셔너리에 저장
            {
                dialogDictionary.Add(int.Parse(data_values[2]), data_values[5]);
                nameDictionary.Add(int.Parse(data_values[2]), data_values[4]);
            }
        }
        reader.Close();
    }
    else
        value = "파일이 없습니다.";

    Debug.Log(nameDictionary.Count);

    return;
}

public void showDialog()
{
    if (cntDialogCode >= nameDictionary.Count)
    {
        cntDialogCode = 0;
        return;
    }

    DialogNameTxt.text = nameDictionary[cntDialogCode];
    DialogTxt.text = dialogDictionary[cntDialogCode];
    cntDialogCode++;
}
```

setDialogType 이 상황을 나타내는 번호이며, cntDialogCode가 대화 진행 상황이다.

그리고 dialogDictionary와 nameDictionary의 두 개의 딕셔너리를 만들어 주어, 대화 진행에 따라 대사, 말하고 있는자의 이름을 저장 해 준다.

물론, 그것을 저장하는 것은 파일을 읽으면서 setDialogType 여부를 검사하여 맞는 상황인지를 검사 해 준 다음 Add를 통해 추가 해 주는 것이다.

딕셔너리들에 이름과 대사를 저장한 뒤, cntDialogCode를 하나씩 늘려 가면서 대사를 출력 해 준다.

물론 인덱스가 최대 치를 넘었을 때는 다시 리셋 해 준다.

테스트 해 보면

![image](https://user-images.githubusercontent.com/66288087/214533438-eecb78c3-b5bc-4319-820f-4b2661fb3cda.png)

이런 식으로 대사가 나오게 됨을 볼 수 있다.

<hr>

## NPC 제작

받은 골드메탈님의 무료 에셋을 통하여 NPC를 만들어 주었다.

![image](https://user-images.githubusercontent.com/66288087/214766378-cea311a3-77df-4ac0-b635-7162082533d7.png)

이런 식으로 배치하였다.

<hr>

### NPC 스크립트

NPC는 앞서 임시로 Manager.cs에 만들었던 코드들을 그대로 가져와 배치하였으며, NPC와 대화를 해야 한다는 점을 고려하여 대화 시작 함수도 따로 만들어 주었다.

즉,

- **대화 시작 함수**(파일을 읽는 함수를 먼저 호출한 뒤, UI에 대사를 출력하는 함수 호출)
- **UI에 대사를 출력하는 함수**(만약 UI가 꺼져 있다면 UI를 키고, index가 딕셔너리 길이를 넘지 않을 때 대사를 출력한다.)
- **파일을 읽는 함수**(파일을 읽는 함수, NPC 코드와 NPC에 설정 된 상황 변수에 맞는 대화 **뭉탱이**를 가져온다.)

로 일차적으로 설정 하였으며, 퀘스트 정보를 세팅 해 주는 함수 등도 구현 할 예정이다.

위 세 가지 함수가 구현 된 NPC코드를 

**NPC.cs **

```c#
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    // NPC Info
    public int npcCode; // npc 고유의 코드

    public Dictionary<int, string> dialogDictionary = new Dictionary<int, string>(); // 대화 순서 - 대화 내용
    public Dictionary<int, string> nameDictionary = new Dictionary<int, string>(); // 대화 순서 - 이름

    public int setDialogType; // 어떤 상황의 대화 뭉탱이를 가져 올 것인가?
    public int cntDialogCode; // 현재 대화가 어디까지 진행 되었는가? (이것은 대화를 끝낼 때, 무조건 0으로 세팅 해 주어야 한다.)

    public TextMeshPro NPCName;

    // UI Info
    public GameObject DialogPanel; // 대화 창
    public GameObject NextBtn; // 다음 버튼
    public GameObject OKBtn; // 수락 버튼
    public GameObject CancelBtn; // 취소 버튼

    public Text NameTxt; // 대화 주체 이름
    public Text DialogTxt; // 대화 텍스트

    

    // Start is called before the first frame update
    void Awake()
    {
        DialogPanel = GameObject.FindGameObjectWithTag("UIDialog").transform.GetChild(0).gameObject;
        NextBtn = DialogPanel.transform.GetChild(1).gameObject;
        NameTxt = DialogPanel.transform.GetChild(0).gameObject.GetComponent<Text>();
        DialogTxt = DialogPanel.transform.GetChild(2).gameObject.GetComponent<Text>();
        NextBtn.GetComponent<Button>().onClick.AddListener(ShowDialogUI);
        NPCName.text = this.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTalkNPC()
    {
        ReadFile(Application.dataPath + "/test.csv");
        ShowDialogUI();
    }

    void ShowDialogUI() // 이것은 NextBtn에서도 사용 됨
    {
        if (!DialogPanel.activeSelf)
            DialogPanel.SetActive(true);

        if (cntDialogCode >= nameDictionary.Count) // index가 딕셔너리 크기랑 같아지게 되면
        {
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dialogDictionary.Clear(); // 바로 클리어를 해 주어야 다른 상황의 대화 or 다른 NPC와의 대화에서 파일을 잘 읽을 수 있다.
            nameDictionary.Clear();
            cntDialogCode = 0; // 인덱스를 0으로 초기화 하고 리턴한다.
            return;
        }

        NameTxt.text = nameDictionary[cntDialogCode];
        DialogTxt.text = dialogDictionary[cntDialogCode];
        cntDialogCode++;
    }

    public void ReadFile(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        string value = "";

        if (fileInfo.Exists)
        {
            bool isEnd = false;
            StreamReader reader = new StreamReader(filePath);
            while (!isEnd)
            {
                value = reader.ReadLine();
                if (value == null)
                {
                    isEnd = true;
                    break;
                }
                var data_values = value.Split(',');

                if (data_values[1] == npcCode.ToString() && data_values[3] == setDialogType.ToString()) 
                {
                    // 딕셔너리에 저장
                    dialogDictionary.Add(int.Parse(data_values[2]), data_values[5]);
                    nameDictionary.Add(int.Parse(data_values[2]), data_values[4]);
                }
            }
            reader.Close();
        }
        else
            value = "파일이 없습니다.";

        Debug.Log(nameDictionary.Count);

        return;
    }


}
```

NPC와의 대화에서는 UI 등장이 필수이기에 UI를 연결 해 주어야 한다. 하지만, NPC를 Prefab으로 만들어 주게 되면 UI를 public GameObject에 넣은 것이 다 초기화된다.

따라서 코드 내부에 있는 Awake()에서 FindGameObjectWithTag를 이용하여 Panel GameOject를 찾아 내었다. (대화 창 패널은 UIDialog라는 tag 세팅)

![image](https://user-images.githubusercontent.com/66288087/214766842-93b1c8e2-09dc-4927-9199-c8b98860c271.png)

그런데 Dialog는 평소에는 비활성화 된 상태이기 때문에 FindGameObjectWithTag가 통하지 않는다.

따라서 위 계층 사진에서 DialogLine에 UIDialog 태그를 세팅하고, 해당 오브젝트의 자식 오브젝트를 찾아 패널 오브젝트에 넣는 작업을 먼저 진행하였다.

아래는 Awake() 부분을 떼어 왔다.

```c#
 void Awake()
{
    DialogPanel = GameObject.FindGameObjectWithTag("UIDialog").transform.GetChild(0).gameObject;
    NextBtn = DialogPanel.transform.GetChild(1).gameObject;
    NameTxt = DialogPanel.transform.GetChild(0).gameObject.GetComponent<Text>();
    DialogTxt = DialogPanel.transform.GetChild(2).gameObject.GetComponent<Text>();
    NextBtn.GetComponent<Button>().onClick.AddListener(ShowDialogUI);
    NPCName.text = this.name;
}
```

DialogPanel이 위에서 말한 것 처럼 자식 오브젝트를 가져 왔음을 볼 수 있다.

하위 계층에 있는 버튼들도 자식 오브젝트 가져 오듯이 가져왔다.

<hr>

### 플레이어 상호작용 버튼 세팅

이제, 플레이어가 상호작용을 할 수 있게 버튼을 세팅 해 보도록 하자

**상호작용 버튼**은 **PC**로는 **스페이스바**로 세팅하였고, **모바일**로는 일단은 **공격 버튼이 NPC 근처에서 상호작용 버튼으로 바뀌는 것**을 생각하고 있다.

(만약 NPC 근처에 몬스터가 있다면 공격이 안되는 문제가 생길 수 있겠네.. 그렇지만 아예 배치를 근처에 해 놓지 않으면 될거 같기도 하다)

Project Settings에 들어가 상호작용 버튼을 설정한다.

![image](https://user-images.githubusercontent.com/66288087/214769987-0bec0ed3-5db4-4358-96f5-fed18767e5da.png)

Space바에 상호작용 버튼을 세팅 해 주었다.

상호작용 버튼이 작동하는 방식은 아래 그림과 같다.

![image](https://user-images.githubusercontent.com/66288087/214770675-ab10db31-ff74-4ffd-93f9-66f75017c2fe.png)

**OnTriggerStay2D**를 통해 **플레이어 Trigger와 닿아 있는 상태**인 **오브젝트를 감지**한다.

해당 오브젝트가 NPC라면 nearObject에 해당 오브젝트를 세팅 해 준다.

물론 **OnTriggerExit2D**를 통하여 **플레이어와 닿아 있는 상태가 끝**난다면 **nearObject를 null로** 다시 돌린다.

nearObject에 NPC가 있다는 것은 계속해서 NPC 근처에 있다는 것이니 상호작용 버튼을 눌러주면 NPC와 대화가 시작되게 만들어 주면 된다.

**Player.cs코드**

```c#
public void CheckInterAction()
{
    if(interActionKey && nearObject != null)
    {
        Time.timeScale = 0f;
        playerHit.nearObject.GetComponent<NPC>().StartTalkNPC();
    }
}
```

처음에 Player.cs 코드 내에 만든 상호작용 함수이다.

Player.cs 함수에 OnTriggerStay2D, OnTriggerExit2D를 만들어 주어 NPC와 닿으면 nearObject를 채우려 하였다.

하지만, 역시 처음에 몬스터 피격을 할 때가 생각 나는가?

**자식 오브젝트에 있는 Trigger, Collider는 RigidBody가 있는 부모 오브젝트에서 처리** 한다는 사실을..

이번에도 마찬가지다.

NPC를 향해 공격을 하게 되면 **공격 범위만 NPC에 닿게 되어도 nearObject가 채워지게 되었다.**

따라서 nearObject를 PlayerHit에 만들어 주고, playerHit에 OnTriggerStay2D, OnTriggerExit2D를 만들어 주어 NPC와의 상호작용을 처리 해 주었다.

**PlayerHit.cs 코드 중 일부**

```c#
private void OnTriggerStay2D(Collider2D collision)
{

    if (collision.gameObject.layer == 8)
    {
        if (!isHit)
        {
            isHit = true;
            int dmg = (collision.gameObject.GetComponent<Enemy>().monsterAtk - statInfo.playerDefense);                  
            // Debug.Log(GetComponentInChildren<BoxCollider2D>().gameObject.name);
            // Debug.Log(rigid.velocity.x);

            StartCoroutine(playerHit(dmg));

        }

    }

    if(collision.gameObject.layer == 9)
    {
        nearObject = collision.gameObject;
        Debug.Log(nearObject);
    }

}

private void OnTriggerExit2D(Collider2D collision)
{
    if (collision.gameObject.layer == 9)
    {
        nearObject = null;
        Debug.Log(nearObject);
    }

}
```
<br>

![image](https://user-images.githubusercontent.com/66288087/214772504-a4c3035a-1889-4c85-81db-1c77a4328217.png)

NPC 레이어는 위 사진과 같이 9번으로 설정하였기에 collision.gameObject.layer == 9가 될 때, nearObject를 세팅 해 준다.

그리고 Player.cs에서

```c#
 public void CheckInterAction()
{
    if(interActionKey && playerHit.nearObject != null)
    {
        Time.timeScale = 0f;
        playerHit.nearObject.GetComponent<NPC>().StartTalkNPC();
    }
}
```

PlayerHit에 있는 nearObject를 이용하였다.

또한, 상호작용 때, Time.timeScale을 0으로 설정하였는데, 이것은 모바일 게임 특성상 대화를 시작하게 되면 화면을 다 가리게 되어 몬스터가 오거나 다른 상황에서 제대로 대응하기 어렵다.

따라서 잠시 대화를 할 동안에는 시간을 멈추어 주었다.

NPC.cs 코드에서 대화를 보여주는 함수인 ShowDialog()를 잘 보게 되면 대화를 끝마칠 때, Time.timeScale을 1로 설정하여 시간을 다시 흐르게 하는 것을 볼 수 있을 것이다.

<hr>

### 모바일에서의 상호작용

모바일에서는 공격 버튼을 nearObject가 있을 때 한정으로 상호작용 버튼화 시켜주면 된다.

**Player.cs**

```c#
public void GetButtonDown(string whatBtn)
{
    switch (whatBtn)
    {
        case "L":
            directionValue = -1;
            break;
        case "R":
            directionValue = 1;
            break;
        case "Jump":
            jumpKey = true;
            tojump();
            break;
        case "Attack":
            if(playerHit.nearObject == null)
            {
                attackKey = true;
                normalAttack();
            }
            else
            {
                interActionKey = true;
                CheckInterAction();
            }
            break;
    }
}
```

Attack 부분에 새로 조건을 추가 해 주어 NPC 근처에서 상호작용을 할 수 있게 하였다.

<hr>

### 대화 장면

![image](https://user-images.githubusercontent.com/66288087/214774654-3ba31f4e-5b1b-4875-bc07-6b8c2c2e2413.png)

![image](https://user-images.githubusercontent.com/66288087/214774693-79d15016-6a3f-4107-8f96-803189a1de64.png)

Next 버튼을 누르면 대사가 넘어감을 볼 수 있다.

<hr>

## NPC 심화 -> 몬스터 퇴치 퀘스트 부여

몬스터 퇴치 퀘스트 부여 이전에, 먼저 추가를 할 것이 있다.

<hr>

### 보상 규칙 추가

바로 보상을 주는 대화를 추가하는 것이다.

앞서 대화를 저장하는 csv 파일을 봤을 텐데, 거기에서 자동으로 보상을 부여할 수 있게끔 하는 규칙을 추가 할 것이다.

#### Type 수정

Type은 위에서는 0과 1밖에 없었다.

새롭게 2를 추가하여, Type에 2가 나오게 되면 대화 내용을 리워드 지급 관련으로 해석하라는 의미가 된다.

**대화 내용 규칙**은 아래와 같다.

앞서 퀘스트 내용을 주는 대화내용과 형태는 같다.

하지만 일부가 다른데,

e - exp 경험치이다.
g - gold 골드이다.
i - item 아이템이다.

즉, 예를 들어 e.0.1500:g.0.4000:i.2.3: 라고 하면, 경험치 +1500, 골드 +4000, 2번 코드 아이템 +3개 인 것이다.

골드와 경험치에 코드는 형태를 통일시키기 위해 적은 것이다.

<hr>

### 대화 딕셔너리 변경! (리팩토링!)

퀘스트 함수를 만들기 전에 변경해야 할 사항이 있다.

바로 딕셔너리인데.. 너무 고정관념에 빠져 있었다.

**Dictionary**의 **Value 값 속**에 **배열**이 들어가지 못할 것이라 생각했던 것이다...

Value값 속에 배열을 넣게 되면 이름 딕셔너리, 대화 딕셔너리 두 개를 만들 필요가 없어지게 되며, 모든 정보들을 한 번에 가져올 수 있게 되어 Type을 체크해야 하는 퀘스트 체크 함수와도 딱 맞게 된다.

<br>

**NPC.cs 딕셔너리 줄임 버전**

```c#
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    // NPC Info
    public int npcCode; // npc 고유의 코드

    public Dictionary<int, string[]> dicitonaryInfo = new Dictionary<int, string[]>(); // 대화 순서 - 대화 Bundle

    public int setDialogType; // 어떤 상황의 대화 뭉탱이를 가져 올 것인가?
    public int cntDialogCode; // 현재 대화가 어디까지 진행 되었는가? (이것은 대화를 끝낼 때, 무조건 0으로 세팅 해 주어야 한다.)

    public TextMeshPro NPCName;

    // UI Info
    public GameObject DialogPanel; // 대화 창
    public GameObject NextBtn; // 다음 버튼
    public GameObject OKBtn; // 수락 버튼
    public GameObject CancelBtn; // 취소 버튼

    public Text NameTxt; // 대화 주체 이름
    public Text DialogTxt; // 대화 텍스트

    

    // Start is called before the first frame update
    void Awake()
    {
        DialogPanel = GameObject.FindGameObjectWithTag("UIDialog").transform.GetChild(0).gameObject;
        NextBtn = DialogPanel.transform.GetChild(1).gameObject;
        NameTxt = DialogPanel.transform.GetChild(0).gameObject.GetComponent<Text>();
        DialogTxt = DialogPanel.transform.GetChild(2).gameObject.GetComponent<Text>();
        NextBtn.GetComponent<Button>().onClick.AddListener(ShowDialogUI);
        NPCName.text = this.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTalkNPC()
    {
        ReadFile(Application.dataPath + "/test.csv");
        ShowDialogUI();
    }

    void ShowDialogUI() // 이것은 NextBtn에서도 사용 됨
    {
        if (!DialogPanel.activeSelf)
            DialogPanel.SetActive(true);

        if (cntDialogCode >= dicitonaryInfo.Count) // index가 딕셔너리 크기랑 같아지게 되면
        {
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear(); // 바로 클리어를 해 주어야 다른 상황의 대화 or 다른 NPC와의 대화에서 파일을 잘 읽을 수 있다.
            cntDialogCode = 0; // 인덱스를 0으로 초기화 하고 리턴한다.
            return;
        }

        NameTxt.text = dicitonaryInfo[cntDialogCode][4];
        DialogTxt.text = dicitonaryInfo[cntDialogCode][5];
        cntDialogCode++;
    }

    public void ReadFile(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        string value = "";

        if (fileInfo.Exists)
        {
            bool isEnd = false;
            StreamReader reader = new StreamReader(filePath);
            while (!isEnd)
            {
                value = reader.ReadLine();
                if (value == null)
                {
                    isEnd = true;
                    break;
                }
                var data_values = value.Split(',');

                if (data_values[1] == npcCode.ToString() && data_values[3] == setDialogType.ToString())  // NPC 코드와 미리 세팅 된 상황 번호가 같을 때
                {
                    // 필요 정보만을 딕셔너리에 저장
                    dicitonaryInfo.Add(int.Parse(data_values[2]), data_values); // 대화 번들 추가(한 줄로 전부!)
                }
            }
            reader.Close();
        }
        else
            value = "파일이 없습니다.";

        Debug.Log(dicitonaryInfo.Count);

        return;
    }


}
```
<br>

![image](https://user-images.githubusercontent.com/66288087/214989821-b53faed7-379f-4e9b-aab8-8aff23eaf481.png)

**결과는 똑같이** 나오지만, **사용하는 딕셔너리를 하나로 줄이고**, **한 개의 딕셔너리**로 **퀘스트 관련 함수까지 확장**하였기에 더욱 코드가 괜찮아졌다.

<hr>

## 중요! 안드로이드에서 CSV 파일을 읽지 못하는 현상!!!

위 부분에서 NPC와의 대화를 구현하고 기분좋게 안드로이드 어플로 빌드하여 실행을 해 보려는데.. NPC와의 대화가 시작되지 않는 현상이 발생하였다!!!

즉, **안드로이드에서 실행** 시 **csv 파일**을 **읽는 부분**에서 **문제**가 생겨버린 것이었다...

따라서 열심히 구글링을 해 본 결과, 처음에는 [이 분](https://darkfrog.tistory.com/m/entry/Unity3D-%EB%AA%A8%EB%B0%94%EC%9D%BC-%EB%94%94%EB%B0%94%EC%9D%B4%EC%8A%A4%EC%97%90%EC%84%9C-%ED%8C%8C%EC%9D%BC%EC%83%9D%EC%84%B1-%EB%B0%8F-%EC%9D%BD%EA%B3%A0-%EC%93%B0%EA%B8%B0) 것을 찾았다.

모바일에서 파일을 읽는 방법이라고 하길래, 안드로이드에서와 윈도우, 아이폰에서와의 시작 경로가 다르구나 싶어서 적용하여 해 보았다.

하지만, 역시나 모바일에서는 NPC와의 대화가 진행되지 않았다.

다른 글들을 뒤적이다가 처음 csv를 읽는 코드를 짜는 데 많은 도움을 준 **[블로거분의 글](https://foxtrotin.tistory.com/m/128)에서** 실마리를 찾았다.

그분도 바로 안드로이드에서 실험 해 본 결과, 같은 현상이 발생하신듯 하였다.

그분도 [유니티 포럼](https://forum.unity.com/threads/reading-a-text-file-with-stream-reader-on-android.640189/)에서 관련 내용을 찾으셨던 것이었다.

[일본 프로그래머 분의 깃 헙](https://github.com/furukazu/UnityCSVReader)에도 정리되어 있다.

<hr>

## 해결책!

요약하자면 안드로이드에서나, PC에서나 **Resources 폴더**에 **해당 파일(csv)을 저장**하여, 리소스에서 불러오되, **StringReader를 사용**하여 불러오는 방법을 채택하였던 것이다.

코드를 가져오면 아래와 같다.

```c#
public void ReadFile()
{
    TextAsset csvFile;

    csvFile = Resources.Load("CSV/test") as TextAsset;

    StringReader reader = new StringReader(csvFile.text);

    while (reader.Peek() > -1)
    {
        string value = reader.ReadLine();
        var data_values = value.Split(',');

        if (data_values[1] == npcCode.ToString() && data_values[3] == setDialogType.ToString())  // NPC 코드와 미리 세팅 된 상황 번호가 같을 때
        {
            // 필요 정보만을 딕셔너리에 저장
            dicitonaryInfo.Add(int.Parse(data_values[2]), data_values); // 대화 번들 추가(한 줄로 전부!)
        }

    }

}
```

**csv 파일**은 **TextAsset 형태**로 불러오며, **StringReader를 사용**하여 텍스트를 읽어 낸다.

### reader.Peek()

reader.Peek()는 텍스트를 읽으면서 사용할 수 있는 문자가 더 이상 없거나 스트림에서 검색을 지원하지 않을 경우 -1을 출력한다.

따라서 위에 있는 while문은 텍스트를 읽을 수 있을 때 까지 출력 한다는 것이다.

while문 안에는 앞과 비슷하다. StringReader에도 ReadLine()이 있기 때문에 여기서 부터는 동일하게 해 주면 된다.

이렇게 해 주면.. 안드로이드에서도 파일을 읽어서 NPC와 대화를 할 수 있게 됨을 볼 수 있다.

<hr>

### 몬스터 퇴치 로그 및 플레이어-퀘스트 관리 체계 구현

우선, 먼저 생각해야 할 부분이 있다.

바로, 퀘스트에 대한 진행 부분은 NPC가 아닌라, Player쪽에서 관리해야 한다는 것이다.

(물론 본격적인 데이터베이스를 사용하게 되면 그쪽에서 관리하는 것이 맞지만.. 일단 코드로 자료구조를 만들어 관리하는 입장에서는 플레이어가 관리하는 것이 최선이라 생각하였다.

-> 왜냐하면, 플레이어가 씬 이동을 하게 될 때, DontDestroyOnLoad()를 사용하여 사라지지 않게 만들면 씬 이동 시 마다 NPC의 QuestCode와 DialogType등을 재 세팅할 수 있기 때문이다.)

<br>

#### 로그 기록을 위한 코드 연결

로그 기록을 위해서는 코드를 연결 시켜 주어야 한다.

![image](https://user-images.githubusercontent.com/66288087/215460742-0e79e11d-bb25-402a-a9f0-02a8c744a8a9.png)

즉, 위 그림처럼 Player가 몬스터를 퇴치하게 되면 경험치 전달과 함께, 플레이어로 몬스터 퇴치 정보를 다시 보내준다.

**Enemy.cs 코드** 중 경험치를 보내는 함수이다.

```c#
void sendEXP()
{
    // 몬스터가 죽었을 때, 경험치를 보낸다.
    for(int i = 0; i < attackObj.Count; i++)
    {
        attackObj[i].GetComponent<PlayerStats>().playerStat.playerCntExperience += (int)((float)addExp / (float)attackObj.Count);
        attackObj[i].GetComponent<LogManager>().playerLog.addMonsterCount(monsterCode);
        // Debug.Log("Exp Add + "+(int)((float)addExp / (float)attackObj.Count));
    }

}
```
경험치를 보내는 것 바로 아래에 LogManager를 불러 와 해당 몬스터 코드의 퇴치 마릿수를 더하는 부분이 보일 것이다.

<br>

로그 매니저를 Manager속에 배치 시키지 않은 이유는, 퇴치 값을 주기 위해 매니저를 불러 와야 하기 때문이다.

UI를 통해 몬스터 퇴치 수를 표시하게 되면

![image](https://user-images.githubusercontent.com/66288087/215461085-cd02c214-db59-4819-9fe0-4d34ec87e70a.png)

오른쪽 상단 위에 표시 된 부분이 가산되었음을 볼 수 있다.

<hr>

#### 플레이어 로그 매니저 코드

플레이어 속에 있는 로그 매니저 코드이다.

파일로 저장 해 내기 편하게 class를 따로 만들어 그 안에 배열을 넣어 주었으며, 각 몬스터 코드별로 마릿수를 따로 저장하게끔 하였다.

**LogManagger.cs**

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

[System.Serializable]
public class LogRecord
{
    private int[] huntingCount = new int[10]; // 각 몬스터들의 퇴치 마릿수를 저장하기 위한 int 배열 (전체 마릿수는 해당 배열 원소들을 합산하면 된다.)

    // 퀘스트 관련
    public List<int[]> huntList = new List<int[]>(); // 어떤 몬스터를 잡아야 하는지 리스트로 저장 [퀘스트 코드, 몬스터 코드, 잡아야 하는 마릿수, 현재 잡은 마릿수,완료 할 npc 코드]
    public List<int[]> gainList = new List<int[]>(); // 어떤 아이템을 얻어야 하는지 리스트로 저장 [퀘스트 코드, 아이템 코드, 얻어야 하는 개수, 현재 가지고 있는 개수,완료 할 npc 코드]

    public int[,] questComplete = new int[1000,2]; // 퀘스트 완료 여부 기록 ( 0 - 시작 전, 1 - 진행 중, 2 - 완료 ) + 퀘스트 완료 조건 개수
    // 나중에 용량을 늘릴 때, 정보를 옮겨 주어야 한다..? DB로 관리하면 편하긴 하겠다..

    // npc 코드에서 씬이 호출 될 때 마다 npc들을 호출해서 해당되는 퀘스트 코드를 세팅해 줘야 한다..?


    public LogRecord()
    {
        
    }

    void CheckQuest(int index)
    {
        for(int i = 0; i < huntList.Count; i++)
        {
            if(huntList[i][1] == index && huntList[i][3] < huntList[i][2])
            {
                huntList[i][3]++;
            }
        }
    }

    public void addMonsterCount(int index)
    {
        CheckQuest(index);
        this.huntingCount[index]++;
    }

    public int returnMonsterCount(int index)
    {
        return this.huntingCount[index];
    }

}

public class LogManager : MonoBehaviour
{
    public LogRecord playerLog = new LogRecord();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
```

huntingCount에 퇴치 마릿수를 저장하였으며, 아래에 설명 할 퀘스트에 사용 될 huntList와 gainList는 int 배열 타입의 List로 만들어 주었다.

왜 이렇게 저장하였는지는 아래에 설명 하도록 하겠다.

<hr>

### 퀘스트 관련 함수 제작

이제 본격적으로 퀘스트를 제작 해 보도록 하겠다.

일반적인 퀘스트의 종류는 세 가지로 볼 수 있지만.. 엄밀히 따지자면? 두 가지로 볼 수 있을 것이다.

#### 1. 몬스터 퇴치 퀘스트

말 그대로 몬스터를 일정 마리 퇴치하면 퀘스트 조건이 완료된다.

앞서 **dialogString 내부 규칙**이라는 부분을 보았을 것이다.

그 곳에서 **퀘스트임이 체크** 되어 있다면 **대화 내용**을 **퀘스트에 대한 정보로 인식**하게끔 하며, **대화 내용을 그에 걸맞는 규칙에 따라 작성**해야 된다는 내용이었다.

그렇게 만든 규칙이 **(사냥 or 수집 구분).(잡아야 할 몬스터 코드).(잡아야 할 마릿수).(퀘스트 코드).(완료 할 npc 코드)** 이다.

즉, CSV 파일의 Type을 구분해 주어, 1일때는 대화 내용으로부터 위 규칙에 따라서 정보를 추출 해 내는 것이다.

예를 들어, *h.12.30.1.2* 라고 하면 몬스터 코드가 12번인 놈을 30마리 잡고, 2번 npc한테 가서 완료해라 라는 것이며, 해당 퀘스트는 1번 코드인 퀘스트이다.


#### 2. 아이템 수집 퀘스트

1번과 유사하게 하지만 맨 앞에 오는 알파벳이 g가 오게 된다.

**(사냥 or 수집 구분).(수집해야 할 아이템 코드).(수집해야 할 개수).(퀘스트 코드).(완료 할 npc 코드)** 로 유사한 규칙을 가졌음을 볼 수 있다.


#### 3. 특수 퀘스트

사실 특수 퀘스트는 종류가 다양하며, 그에 따른 코드를 일일히 작성하는 것은 어렵다.

따라서 앞선 퀘스트들과 같은 퀘스트 정보를 가지되, 예를 들어 그냥 다른 사람에게 말을 거는 것은 아무 몬스터나 0마리를 잡는 것을 조건으로 걸고, 다른 대상 npc에게 완료하게 되면 그만이다.

이렇게 응용을 할 수 있다.

<hr>

### NPC 컴포넌트 플로우 차트

NPC 컴포넌트를 만들 때 목표 중 3번에 NPC 컴포넌트에서 퀘스트를 알아서 찾아 실행해야 한다고 하였다.

이에 대한 NPC 컴포넌트의 플로우 차트를 정리 해 보았다.

![QuestDiagram](https://user-images.githubusercontent.com/66288087/215468372-2c9e44fb-c66e-474c-b308-8d9a91e988a6.png)

정작 이걸 만들 때는 머릿 속으로만 낑낑대며 했었는데.. 차라리 손으로 메모하면서 했으면 어땠을까 생각이 든다.

<hr>

### NPC 컴포넌트 코드 설명

**NPC.cs 코드 전문**

```c#
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    // NPC Info
    public int npcCode; // npc 고유의 코드
    public int cntQuestCode = -1; // 현재 퀘스트 코드

    public Dictionary<int, string[]> dicitonaryInfo = new Dictionary<int, string[]>(); // 대화 순서 - 대화 Bundle

    public int setDialogType; // 어떤 상황의 대화 뭉탱이를 가져 올 것인가?
    public int cntDialogCode; // 현재 대화가 어디까지 진행 되었는가? (이것은 대화를 끝낼 때, 무조건 0으로 세팅 해 주어야 한다.)

    public TextMeshPro NPCName;

    LogManager playerLogManager;

    // UI Info
    public GameObject DialogPanel; // 대화 창
    public GameObject NextBtn; // 다음 버튼
    public GameObject OKBtn; // 수락 버튼
    public GameObject CancelBtn; // 취소 버튼

    public Text NameTxt; // 대화 주체 이름
    public Text DialogTxt; // 대화 텍스트

    

    // Start is called before the first frame update
    void Awake()
    {
        DialogPanel = GameObject.FindGameObjectWithTag("UIDialog").transform.GetChild(0).gameObject;
        NextBtn = DialogPanel.transform.GetChild(1).gameObject;
        NameTxt = DialogPanel.transform.GetChild(0).gameObject.GetComponent<Text>();
        DialogTxt = DialogPanel.transform.GetChild(2).gameObject.GetComponent<Text>();
        OKBtn = DialogPanel.transform.GetChild(3).gameObject;
        CancelBtn = DialogPanel.transform.GetChild(4).gameObject;

        CancelBtn.GetComponent<Button>().onClick.AddListener(CancelDialog);
        NextBtn.GetComponent<Button>().onClick.AddListener(ShowDialogUI);

        playerLogManager = GameObject.FindGameObjectWithTag("Player").GetComponent<LogManager>();
        NPCName.text = this.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTalkNPC()
    {
        if(cntQuestCode != -1)
        {
            CheckQuest();
            return;
        }
        ReadFile(setDialogType);
        ShowDialogUI();
    }

    void CancelDialog()
    {
        DialogPanel.SetActive(false);
        Time.timeScale = 1.0f;
        dicitonaryInfo.Clear(); // 바로 클리어를 해 주어야 다른 상황의 대화 or 다른 NPC와의 대화에서 파일을 잘 읽을 수 있다.
        cntDialogCode = 0; // 인덱스를 0으로 초기화 하고 리턴한다.
    }

    void ShowDialogUI() // 이것은 NextBtn에서도 사용 됨
    {
        if (!DialogPanel.activeSelf)
            DialogPanel.SetActive(true);

        if (cntDialogCode >= dicitonaryInfo.Count) // index가 딕셔너리 크기랑 같아지게 되면
        {
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear(); // 바로 클리어를 해 주어야 다른 상황의 대화 or 다른 NPC와의 대화에서 파일을 잘 읽을 수 있다.
            cntDialogCode = 0; // 인덱스를 0으로 초기화 하고 리턴한다.
            return;
        }

        // 퀘스트 여부 체크
        if(dicitonaryInfo[cntDialogCode][0] == "1")
        {
            // 퀘스트라면..? -> 퀘스트는 항상 마지막에 준다!
            SetQuest(); // 퀘스트 정보 부여
            cntDialogCode = 0;
            setDialogType++;
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear(); // 바로 클리어를 해 주어야 다른 상황의 대화 or 다른 NPC와의 대화에서 파일을 잘 읽을 수 있다.
            return;
        }
        else if(dicitonaryInfo[cntDialogCode][0] == "2")
        {
            // 보상 부여
            GetReward();
            cntDialogCode = 0;
            setDialogType += 2;
            DialogPanel.SetActive(false);
            Time.timeScale = 1.0f;
            dicitonaryInfo.Clear();
            return;
        }

        // 분기 여부 체크


        NameTxt.text = dicitonaryInfo[cntDialogCode][4];
        DialogTxt.text = dicitonaryInfo[cntDialogCode][5];
        cntDialogCode++;
    }

    public void ReadFile(int inputType)
    {
        TextAsset csvFile;

        csvFile = Resources.Load("CSV/test") as TextAsset;

        StringReader reader = new StringReader(csvFile.text);

        while (reader.Peek() > -1)
        {
            string value = reader.ReadLine();
            var data_values = value.Split(',');

            if (data_values[1] == npcCode.ToString() && data_values[3] == inputType.ToString())  // NPC 코드와 미리 세팅 된 상황 번호가 같을 때
            {
                // 필요 정보만을 딕셔너리에 저장
                dicitonaryInfo.Add(int.Parse(data_values[2]), data_values); // 대화 번들 추가(한 줄로 전부!)
            }

        }

    }

    void CheckQuest()
    {
        // 퀘스트 완료 여부를 따지는 함수

        int count = 0;

        // 사냥 확인
        for(int i=0;i< playerLogManager.playerLog.huntList.Count; i++)
        {
            if (playerLogManager.playerLog.huntList[i][4] == npcCode
                && playerLogManager.playerLog.huntList[i][0] == cntQuestCode
                && playerLogManager.playerLog.huntList[i][2] == playerLogManager.playerLog.huntList[i][3])
            {
                // 완료할 NPC 코드가 일치 + 설정 된 퀘스트 코드랑 맞을 때 + n번째 사냥 조건을 만족했을 때
                count++;
            }
        }

        for(int i=0;i<playerLogManager.playerLog.gainList.Count; i++)
        {
            if (playerLogManager.playerLog.gainList[i][4] == npcCode
                && playerLogManager.playerLog.gainList[i][0] == cntQuestCode
                && playerLogManager.playerLog.gainList[i][2] == playerLogManager.playerLog.gainList[i][3])
            {
                // 완료할 NPC 코드가 일치 + 설정 된 퀘스트 코드랑 맞을 때 + 특정 아이템을 얻는 조건을 만족했을 때
                count++;
            }
        }
        
        if(playerLogManager.playerLog.questComplete[cntQuestCode, 1] == count)
        {
            ReadFile(setDialogType);
            ShowDialogUI();
        }
        else
        {
            // 조건을 만족하지 않았을 때
            ReadFile(setDialogType+1);
            ShowDialogUI();
        }

    }
    
    void GetReward()
    {
        string value = dicitonaryInfo[cntDialogCode][5];
        var data_values = value.Split(':');

        int[] imsiArr = new int[3];

        for (int i = 0; i < data_values.Length; i++)
        {
            // 보상 종류

            var values_info = data_values[i].Split('.');

            if (values_info[0] == "e")
            {
                // 경험치 보상

                playerLogManager.gameObject.GetComponent<PlayerStats>().playerStat.playerCntExperience += int.Parse(values_info[2]);

            }
            else if (values_info[0] == "g")
            {
                // 골드 보상

                playerLogManager.gameObject.GetComponent<PlayerItem>().cntGold += int.Parse(values_info[2]);

            }else if(values_info[0] == "i")
            {
                // 아이템 증가



            }
        }
    }

    void SetQuest()
    {
        // 대화 내용 파싱
        string value = dicitonaryInfo[cntDialogCode][5];
        var data_values = value.Split(':');

        int[] imsiArr = new int[5];

        for(int i = 0; i < data_values.Length; i++)
        {
            var values_info = data_values[i].Split('.');

            if (playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] != 0)
            {
                break; // 한 개의 대화에서 한 종류의 퀘스트만 주어지기 때문에 만약 퀘스트 코드 자리에 있는 값이 0(퀘스트 수행 전)이 아니면 퀘스트받는 것을 끝낸다.(방어장치)
            }

            if(values_info[0] == "h")
            {
                // 만약 사냥 미션이라면

                imsiArr[0] = int.Parse(values_info[3]); // 퀘스트 코드
                imsiArr[1] = int.Parse(values_info[1]); // 잡아야 할 몬스터 코드
                imsiArr[2] = int.Parse(values_info[2]); // 잡아야 할 몬스터 마릿수
                imsiArr[3] = 0; // 현재 잡은 마릿수 (퀘스트를 받는 시점에는 무조건 0으로 초기화 해 줘야 한다.)
                imsiArr[4] = int.Parse(values_info[4]); // 퀘스트를 완료 할 npc 코드

                playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] = 1;
                playerLogManager.playerLog.questComplete[int.Parse(values_info[3]), 1]++; // 퀘스트 조건 카운트 추가
                cntQuestCode = int.Parse(values_info[3]);
                playerLogManager.playerLog.huntList.Add(imsiArr);

            }
            else if(values_info[0] == "g")
            {
                // 획득 미션이라면

                imsiArr[0] = int.Parse(values_info[3]); // 퀘스트 코드
                imsiArr[1] = int.Parse(values_info[1]); // 획득해야 할 아이템 코드
                imsiArr[2] = int.Parse(values_info[2]); // 획득해야 할 아이템 개수
                imsiArr[3] = 0; // 현재 얻은 아이템 개수 -> 퀘스트 아이템이라면 0으로 초기화가 맞지만 기존에 있던 아이템이라면 아이템을 가져 와야 한다.
                                // 이건 PlayerItem 코드에서 아이템 코드를 넣으면 개수를 출력 해 주는 함수를 만들어야 할 것 같다. (나중에 수정!)
                imsiArr[4] = int.Parse(values_info[4]); // 퀘스트를 완료 할 npc 코드

                playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] = 1;
                cntQuestCode = int.Parse(values_info[3]);
                playerLogManager.playerLog.gainList.Add(imsiArr);

            }
        }
      
    }

}
```

<br>

일단 NPC.cs 코드의 전문을 가져 왔다.

여기서 함수들을 하나씩 뜯어 살펴 보도록 하겠다.

가장 먼저, NPC에 있는 변수들을 간단히 설명 하도록 하겠다.

<hr>

#### 변수들 설명

- **npcCode** : NPC 고유 코드
- **cntQuestCode** : 현재 NPC가 진행중인 퀘스트를 표시하는 변수
- **setDialogType** : NPC의 대화 타입(상황)을 나타내는 변수
- **cntDialogCode** : NPC의 대화 상황 속에서 대사 순서를 나타내는 변수
- **dictionaryInfo** : NPC와의 대화 중 한 개의 대화 타입에 속하는 대사들의 뭉탱이를 저장하는 딕셔너리

**setDialogType**에 대해 추가로 설명하자면, **NPC와의 대화**는 **여러 상황**에서 이루어 지게 된다.

예를 들어 NPC가 플레이어에게 임무를 주기 전의 상황과 임무를 수행하고 돌아 온 상황은 다를 것이다.

따라서 그러한 **상황들을 번호로 구분** 해 주게 되며, 그 **번호가 저장되는 변수**가 **setDialogType**이다.

NPC와 대화를 하게 되면, NPC에 기록 된 상황 변수에 따라서 한 상황에 포함되는 모든 대화 번들을 dictionaryInfo 딕셔너리에 저장하게 되고, UI에 차근 차근 표시 해 주게 된다.

**한 번들의 대화가 종료**되면 **상황 변수가 조정**되게 되고, 다시 말을 걸게 되면 조정 된 상황 변수에 따른 대화 번들이 출력 되게 된다.

아래 함수들에 대한 설명에서 자세한 과정을 설명하겠다.

<hr>

이제 함수들에 대해 설명하도록 할텐데 그 중에서 가장 먼저, **NPC에게 처음 말을 걸었을 때 실행**되는 함수인 **StartTalkNPC**를 먼저 살펴 보도록 하자

<hr>

#### StartTalkNPC()

```c#
public void StartTalkNPC()
{
    if(cntQuestCode != -1)
    {
        CheckQuest();
        return;
    }
    ReadFile(setDialogType);
    ShowDialogUI();
}
```

이 곳에서는 **퀘스트가 진행 중인지 여부**를 먼저 따진다. 

**QuestCode를 초기에 -1로 설정** 해 놓았는데, 그대로 **-1이면 퀘스트가 진행 중이지 않은 것**으로 판단하여 **퀘스트 완료 여부를 판단하지 않고** 바로 현재 대화 흐름에 맞는 대사를 불러오게 된다.(ReadFile(setDialogType))

퀘스트 체크는 퀘스트 세팅 함수 다음에 살펴 보도록 하자

<hr>

#### ReadFile(int inputType)

```c#
public void ReadFile(int inputType)
{
    TextAsset csvFile;

    csvFile = Resources.Load("CSV/test") as TextAsset;

    StringReader reader = new StringReader(csvFile.text);

    while (reader.Peek() > -1)
    {
        string value = reader.ReadLine();
        var data_values = value.Split(',');

        if (data_values[1] == npcCode.ToString() && data_values[3] == inputType.ToString()) 
        {
            // NPC 코드도 같고, 미리 세팅 된 상황 번호도 같은 라인일 때
            // 필요 정보만을 딕셔너리에 저장
            dicitonaryInfo.Add(int.Parse(data_values[2]), data_values); 
            // 대화 번들 추가(한 줄로 전부!)
        }

    }

}
```

**파일을 읽는 함수**이다.

CSV 파일을 한 줄씩 읽어 내려가면서 **NPC코드와, 파일에 적힌 NPC 코드가 같고 상황 번호도 파일에 적힌 부분과 같은 라인**을 **딕셔너리에 저장** 해 나간다.

즉, 중요한 부분은 NPC가 누구인지와 어떤 상황인지가 중요하다!

해당 함수에서는 상황 변수를 매개변수로 받아 오게 된다.

이렇게 하지 않고 cntDialogType을 사용해도 되지만 이렇게 한 이유는 **CheckQuest()** 에서 설명하도록 하겠다.

<hr>

#### ShowDialog()

```c#
void ShowDialogUI() // 이것은 NextBtn에서도 사용 됨
{
    if (!DialogPanel.activeSelf)
        DialogPanel.SetActive(true);

    if (cntDialogCode >= dicitonaryInfo.Count) // index가 딕셔너리 크기랑 같아지게 되면
    {
        DialogPanel.SetActive(false);
        Time.timeScale = 1.0f;
        dicitonaryInfo.Clear(); // 바로 클리어를 해 주어야 다른 상황의 대화 or 다른 NPC와의 대화에서 파일을 잘 읽을 수 있다.
        cntDialogCode = 0; // 인덱스를 0으로 초기화 하고 리턴한다.
        return;
    }

    // 퀘스트 여부 체크
    if(dicitonaryInfo[cntDialogCode][0] == "1")
    {
        // 퀘스트라면..? -> 퀘스트는 항상 마지막에 준다!
        SetQuest(); // 퀘스트 정보 부여
        cntDialogCode = 0;
        setDialogType++;
        DialogPanel.SetActive(false);
        Time.timeScale = 1.0f;
        dicitonaryInfo.Clear();
        return;
    }
    else if(dicitonaryInfo[cntDialogCode][0] == "2")
    {
        // 보상 부여
        GetReward();
        cntDialogCode = 0;
        setDialogType += 2;
        DialogPanel.SetActive(false);
        Time.timeScale = 1.0f;
        dicitonaryInfo.Clear();
        return;
    }

    // 분기 여부 체크

    NameTxt.text = dicitonaryInfo[cntDialogCode][4];
    DialogTxt.text = dicitonaryInfo[cntDialogCode][5];
    cntDialogCode++;
}
```

파일을 읽었으니, 이제 **대화를 출력**해야 한다.

초반부에는 **만약 대화 창이 떠 있지 않으면 대화창을 켜 주고**, **cntDialogCode가 딕셔너리 크기랑 같아**지게 되면 **한 상황에서의 대화가 끝**난 것이기에 **딕셔너리를 초기화** 해 주고, **cntDialogCode를 다시 0으로 세팅** 해 준 다음, **함수를 종료** 해 준다.

그리고, 대화 중에 Type이 1이면 퀘스트 조건 부여를 해 주는 단계라는 의미기에 대화 창에 저장 된 대화 내용이 다르게 적혀있다.

이에 해당 대화 내용을 해석하여 퀘스트 조건을 세팅 해 주기 위해 **SetQuest()** 로 이동 해 준다.

Type이 2면 보상을 주기 위해 **GetReward()** 로 이동 해 준다.

<hr>

#### CancelDialog()

```c#
void CancelDialog()
{
    DialogPanel.SetActive(false);
    Time.timeScale = 1.0f;
    dicitonaryInfo.Clear();
    cntDialogCode = 0; // 인덱스를 0으로 초기화 하고 리턴한다.
}
```

**대화 창을 종료** 해 주는 함수이다.

버튼을 통해서 실행이 되며, **UI랑 연결**이 되어야 하는 함수이다.

그런데, NPC는 여러 명이 존재하며 UI는 한 개이므로 무작정 Inspector에서 특정 NPC와 대응시킬 수 없다.

따라서 아래와 같이 Awake()에서 **동적으로 버튼에 OnClick()을 세팅** 해 주게 된다.

```c#
CancelBtn.GetComponent<Button>().onClick.AddListener(CancelDialog);
NextBtn.GetComponent<Button>().onClick.AddListener(ShowDialogUI);
```

일단 세팅은 이렇게 했지만, ShowDialogUI()에서도 UI를 off 하는 부분이 있기에 당장은 사용되지 않는 부분이다.


<hr>

#### SetQuest()

```c#
void SetQuest()
{
    // 대화 내용 파싱
    string value = dicitonaryInfo[cntDialogCode][5];
    var data_values = value.Split(':');

    int[] imsiArr = new int[5];

    for(int i = 0; i < data_values.Length; i++)
    {
        var values_info = data_values[i].Split('.');

        if (playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] != 0)
        {
            break; // 한 개의 대화에서 한 종류의 퀘스트만 주어지기 때문에 만약 퀘스트 코드 자리에 있는 값이 0(퀘스트 수행 전)이 아니면 퀘스트받는 것을 끝낸다.(방어장치)
        }

        if(values_info[0] == "h")
        {
            // 만약 사냥 미션이라면

            imsiArr[0] = int.Parse(values_info[3]); // 퀘스트 코드
            imsiArr[1] = int.Parse(values_info[1]); // 잡아야 할 몬스터 코드
            imsiArr[2] = int.Parse(values_info[2]); // 잡아야 할 몬스터 마릿수
            imsiArr[3] = 0; // 현재 잡은 마릿수 (퀘스트를 받는 시점에는 무조건 0으로 초기화 해 줘야 한다.)
            imsiArr[4] = int.Parse(values_info[4]); // 퀘스트를 완료 할 npc 코드

            playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] = 1;
            playerLogManager.playerLog.questComplete[int.Parse(values_info[3]), 1]++; // 퀘스트 조건 카운트 추가
            cntQuestCode = int.Parse(values_info[3]);
            playerLogManager.playerLog.huntList.Add(imsiArr);

        }
        else if(values_info[0] == "g")
        {
            // 획득 미션이라면

            imsiArr[0] = int.Parse(values_info[3]); // 퀘스트 코드
            imsiArr[1] = int.Parse(values_info[1]); // 획득해야 할 아이템 코드
            imsiArr[2] = int.Parse(values_info[2]); // 획득해야 할 아이템 개수
            imsiArr[3] = 0; // 현재 얻은 아이템 개수 -> 퀘스트 아이템이라면 0으로 초기화가 맞지만 기존에 있던 아이템이라면 아이템을 가져 와야 한다.
                            // 이건 PlayerItem 코드에서 아이템 코드를 넣으면 개수를 출력 해 주는 함수를 만들어야 할 것 같다. (나중에 수정!)
            imsiArr[4] = int.Parse(values_info[4]); // 퀘스트를 완료 할 npc 코드

            playerLogManager.playerLog.questComplete[int.Parse(values_info[3]),0] = 1;
            cntQuestCode = int.Parse(values_info[3]);
            playerLogManager.playerLog.gainList.Add(imsiArr);

        }
    }

}
```

**퀘스트 조건 세팅 함수**이다.

퀘스트는 특정 조건을 만족해 주어야 하며, 해당 조건을 관리 해 주는 것은 Player에 속한 LogManager.cs 컴포넌트이다.

위에서 몬스터 퇴치 마릿수를 기록하는 체계를 간단하게 그림으로 표현 한 것을 봤을 것이다.

그 부분에서 퀘스트 중인 특정 몬스터를 퇴치 했을 때, 퀘스트 조건에 있는 마릿수도 같이 더해 주게 되면 퀘스트 조건을 체크할 수 있을 것이다.

코드를 하나하나 보면

```c#
// 대화 내용 파싱
string value = dicitonaryInfo[cntDialogCode][5];
var data_values = value.Split(':');
```

SetQuest()가 실행되면서 **퀘스트 조건이 담겨있는 대화 내용을 다시 파싱** 하여 **value에 저장**시켜 준다.

이번 파싱은 퀘스트 조건 별로 뭉탱이로 파싱하는 것이다.

ex > "h.1.25" , "g.2.10" 이렇게 퀘스트 세부 임무 별로 구분!

```c#
int[] imsiArr = new int[5];
```

그리고 PlayerLog의 조건 리스트에 들어 갈 int 배열을 하나 만들어 준다.

해당 배열에는 순서대로 **퀘스트 코드, 몬스터 코드, 잡아야 하는 마릿수, 현재 잡은 마릿수,완료 할 npc 코드** 가 들어가게 된다.

```c#
for(int i = 0; i < data_values.Length; i++)
```

그리고 임무 속에서 정보를 끌어내기 위해 **for문을 사용하여 임무별로 참조**를 진행한다.





<hr>

#### CheckQuest()

```c#
void CheckQuest()
{
    // 퀘스트 완료 여부를 따지는 함수

    int count = 0;

    // 사냥 확인
    for(int i=0;i< playerLogManager.playerLog.huntList.Count; i++)
    {
        if (playerLogManager.playerLog.huntList[i][4] == npcCode
            && playerLogManager.playerLog.huntList[i][0] == cntQuestCode
            && playerLogManager.playerLog.huntList[i][2] == playerLogManager.playerLog.huntList[i][3])
        {
            // 완료할 NPC 코드가 일치 + 설정 된 퀘스트 코드랑 맞을 때 + n번째 사냥 조건을 만족했을 때
            count++;
        }
    }

    for(int i=0;i<playerLogManager.playerLog.gainList.Count; i++)
    {
        if (playerLogManager.playerLog.gainList[i][4] == npcCode
            && playerLogManager.playerLog.gainList[i][0] == cntQuestCode
            && playerLogManager.playerLog.gainList[i][2] == playerLogManager.playerLog.gainList[i][3])
        {
            // 완료할 NPC 코드가 일치 + 설정 된 퀘스트 코드랑 맞을 때 + 특정 아이템을 얻는 조건을 만족했을 때
            count++;
        }
    }

    if(playerLogManager.playerLog.questComplete[cntQuestCode, 1] == count)
    {
        ReadFile(setDialogType);
        ShowDialogUI();
    }
    else
    {
        // 조건을 만족하지 않았을 때
        ReadFile(setDialogType+1);
        ShowDialogUI();
    }

}
```



<hr>

#### GetReward()

```c#
void GetReward()
{
    string value = dicitonaryInfo[cntDialogCode][5];
    var data_values = value.Split(':');

    int[] imsiArr = new int[3];

    for (int i = 0; i < data_values.Length; i++)
    {
        // 보상 종류

        var values_info = data_values[i].Split('.');

        if (values_info[0] == "e")
        {
            // 경험치 보상

            playerLogManager.gameObject.GetComponent<PlayerStats>().playerStat.playerCntExperience += int.Parse(values_info[2]);

        }
        else if (values_info[0] == "g")
        {
            // 골드 보상

            playerLogManager.gameObject.GetComponent<PlayerItem>().cntGold += int.Parse(values_info[2]);

        }else if(values_info[0] == "i")
        {
            // 아이템 증가



        }
    }
}
```




<hr>

## 일반 퀘스트 시연


![image](https://user-images.githubusercontent.com/66288087/215468642-35189839-8b9e-4b94-98e7-707ab6d45f73.png)

퀘스트를 받는 모습

![image](https://user-images.githubusercontent.com/66288087/215468764-320430fc-395c-48f2-8111-eb1ef2b6db4b.png)

퀘스트를 받았을 때 오른쪽 상단에 잡아야 할 마릿수가 생겼음을 볼 수 있다.

![image](https://user-images.githubusercontent.com/66288087/215469211-d44a0dbd-a160-4fc4-b628-29218d808f53.png)

퀘스트 완료 시 나오는 스크립트

![image](https://user-images.githubusercontent.com/66288087/215469293-971d730b-38f6-4cb7-a00e-0b799d5b624f.png)

보상을 받고 레벨 업 한 모습 (마릿수가 그대로인 것을 보아 퀘스트 경험치를 통해 레벨 업 했음을 볼 수 있다.)

![image](https://user-images.githubusercontent.com/66288087/215469412-341244be-9803-42bc-ab20-13375a994647.png)

퀘스트 조건을 완료하지 못했을 때 나오는 모습






















