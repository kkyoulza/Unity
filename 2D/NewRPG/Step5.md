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

대화 창에 있는 것을 대사로 볼 것인지, 퀘스트 설정을 위한 특수 문자로 볼 것인지를 구분해 주는 부분이다.

0 - 일반 대화
1 - 퀘스트 수락 시 퀘스트 클리어 조건을 적음(조건은 아래 서술)

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

ex > 12번 코드 몬스터를 50마리 잡고, 3번 코드 아이템을 30개 획득하시오 --> **h.12.50:g.3.30:**

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

                if (data_values[1] == npcCode.ToString() && data_values[3] == setDialogType.ToString()) // NPC 코드와 미리 세팅 된 상황 번호가 같을 때
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

OnTriggerStay2D를 통해 플레이어 Trigger와 닿아 있는 상태인 오브젝트를 감지한다.

해당 오브젝트가 NPC라면 nearObject에 해당 오브젝트를 세팅 해 준다.

물론 OnTriggerExit2D를 통하여 플레이어와 닿아 있는 상태가 끝난다면 nearObject를 null로 다시 돌린다.

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

<hr>

### 대화 장면

![image](https://user-images.githubusercontent.com/66288087/214774654-3ba31f4e-5b1b-4875-bc07-6b8c2c2e2413.png)

![image](https://user-images.githubusercontent.com/66288087/214774693-79d15016-6a3f-4107-8f96-803189a1de64.png)

Next 버튼을 누르면 대사가 넘어감을 볼 수 있다.

<hr>

## NPC 심화 -> 몬스터 퇴치 퀘스트 부여



















