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




