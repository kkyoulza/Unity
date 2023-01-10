# Step3. 오브젝트 풀링의 활용


원래는 캐릭터 피격, 몬스터 퇴치 후 보상 등을 만들 예정이었으나.. 모바일로 테스트를 해 보니 데미지 스킨이 생길 때 약간 버벅임이 느껴졌다.

따라서 최적화에 대한 필요성을 느끼게 되어 오브젝트 풀링을 공부하였고, 일차적으로는 데미지 스킨 Prefab 소환에 사용 해 보았다.

처음 사용 해 보는 것이라 오브젝트의 생명주기?도 같이 다시 보는 계기가 되어 매우 좋았다.

감상은 여기까지 하고 정리를 시작하겠다.

<hr>

## 오브젝트 풀링 적용 1 - 데미지 텍스트 생성

오브젝트 풀링을 공부할 때, 처음에는 구글에 검색을 하여 한 블로그를 찾았다.

하지만 골드메탈님이 최근 오브젝트 풀링에 대한 강의 영상을 올려 주셔서 해당 영상을 참고하여 적용하였다.

<hr>

### MiniStep1. 오브젝트 풀링의 정의

오브젝트 풀링이란 말 그대로 풀장과 같이 **오브젝트들을 저장해 둘 저장소를 만들어 두고, 그 곳에서 오브젝트를 뽑아서 사용하는 것**을 의미한다.

이는 Prefab에서 Instantiate를 통해 소환하는 작업을 많이 반복하는 곳에서 사용된다.

예를 들어 뱀서라이크 같은 많은 몬스터들이 몰려오는 것을 구현할 때, 그 많은 몬스터들을 소환하고 없애는 작업을 반복하게 되면 메모리상에도 많은 부담이 될 것이다.

데미지 텍스트도 마찬가지이다.

잡몹은 상관 없을 수 있겠지만 보스 몬스터 같은 경우는 오랜 시간동안 전투를 하게 되는데, 그 동안에 생기는 많은 수의 데미지 텍스트도 메모리에 영향을 주게 될 것이다. 

따라서 오브젝트 풀링을 사용 해 주면 아래 그림과 같이 **소환했던 데미지 텍스트를 재 사용**하여 **데미지 텍스트 소환을 아낄 수 있게**된다.

![image](https://user-images.githubusercontent.com/66288087/210320178-25056092-f561-4b56-a07b-c70df9c624aa.png)

그렇다면 오브젝트 풀링을 구체적으로 어떻게 사용 할 수 있을까?

<hr>

### MiniStep2. 오브젝트 풀링의 사용

우선 오브젝트 풀링을 사용 할 오브젝트를 하나 만들어 준다.

씬에서 몬스터 같은 것들을 소환 할 때는 매니저 오브젝트를 만들어서 풀링을 이용하면 되지만 각 몬스터 별로 데미지 텍스트를 소환해야 하기에 몬스터에 풀링 코드를 적용시켜 준다.

풀링 코드에서 준비 할 것은 두 가지다.

1. **소환 할 Prefab의 종류가 들어 간 GameObject 배열**
2. **소환 대기중인 Prefab들이 모여있는 List의 배열**

1번에서는 어떤 종류의 Prefab을 소환할 지, Prefab의 원본을 넣어 준다.

2번에서는 1번 배열 안에 있는 Prefab의 수 만큼 즉, **1번 배열의 크기**만큼 **리스트의 배열**을 만들어 준다.

그 다음, **리스트에서는 사용할 수 있는 오브젝트가 있는지 확인** 한다. (**비활성화 되어 있는 상태**의 놀고 있는 오브젝트)

사용할 수 있는 오브젝트가 있다면 해당 오브젝트를 사용하고, 그렇지 않다면(오브젝트가 하나도 들어 있지 않거나 다 사용중일 때) 새롭게 Instantiate를 해 준다.

이 내용을 코드로 정리하면 아래와 같다.

<br>

**dmgPool.cs**

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dmgPool : MonoBehaviour
{
    // 데미지 텍스트 Prefab을 저장 할 변수 

    public GameObject[] dmgPrefabs;

    // 실제로 풀을 저장 할 리스트 (종류가 N개면 N개의 리스트를 만들어 주어야 함)

    public List<GameObject>[] pools; // 리스트의 배열!!

    void Awake()
    {
        pools = new List<GameObject>[dmgPrefabs.Length]; // pool의 크기는 prefab의 종류만큼!

        for(int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>(); 
        }

        Debug.Log(pools.Length);

    }

    public GameObject GetObj(int index) // 게임 오브젝트 할당!
    {
        GameObject select = null;

        foreach(GameObject skin in pools[index]) // index위치에 있는 prefab을 가진 pools 리스트에 접근 
        {
            if (!skin.activeSelf)
            {
                // 탐색중인 게임 오브젝트가 비활성화 되었으면!
                select = skin; // 할당!
                select.SetActive(true); // 활성화!
                break;
            }
        }

        if (!select)
        {

            select = Instantiate(dmgPrefabs[index], transform); // 두 번쩨 오버로딩 : 소환 할 오브젝트, 소환 할 부모 오브젝트(계층란이 지저분해지지 않게!)
            
            pools[index].Add(select); // 새롭게 풀에 등록!

        }


        return select;
    }

}
```

**주목해야 할 부분**

1. Awake에서 pools List를 1번 배열의 사이즈에 맞게 만들어 주며, for문을 통하여 Prefab의 개수만큼 List를 생성 해 준다.

```c#
List<GameObject>();

```

에서 소괄호를 쓴 이유는 **생성자의 의미로 함수처럼 사용**하기 때문이다.

2. select를 Instantiate해 줄 때, 몬스터의 자식으로 소환시켜 주어 Hierarchy창을 깔끔하게 유지시켜 준다.

<br>

**게임 오브젝트를 할당** 해 주는 함수인 **GetObj(int index)**를 만들어 준다. 

해당 함수는 외부에서 게임 오브젝트가 필요할 때 **오브젝트 풀링에서 게임 오브젝트를 골라서 리턴** 해 주는 함수이다.

어떤 종류의 게임 오브젝트를 원하는 지는 매개변수로 설정 된 index를 이용하면 된다.

반환 할 게임 오브젝트를 담기 위해 지역변수로 GameObject 박스를 하나 생성 해 준다. 처음에는 null을 넣어 준다.

<br>

그리고 해당 Prefab들을 모아 둔 List에 foreach로 접근하여 리스트 내부에서 활성화가 되지 않은 게임 오브젝트를 찾아 활성화를 시킨 다음, break로 반복을 빠져나와 준다.

아래 조건문은 조건에 맞는 오브젝트를 하나도 찾지 못했을 때 조건이 맞게끔 만들어 주었다.

**조건문에 GameObject 변수**를 그냥 넣어 주면 **null이 들어가 있을 때 false를 반환** 해 준다. 

즉, 위 반복문에서 조건에 맞는 오브젝트를 찾지 못했을 때, 그대로 null인 상태일 때 들어가게 되는 것이다.

조건문에 들어가게 되면 새롭게 Instantiate를 진행하여 오브젝트를 할당 해 준 뒤, 재사용을 해야 하기 때문에 Pool에 담아 준다!

<br>

이렇게 해 주고, Enemy.cs도 공격 함수를 일부 수정 해 보도록 하자

```c#
GameObject imsiDmg = pooling.GetObj(0);
```

Instantiate 대신 GetObj()를 넣어 주면 된다. (pooling은 위에서 따로 할당 해 주었다.)

그런데, 여기서 한 가지 더 수정해야 할 부분이 있다.

<hr>

### MiniStep3. 재사용되는 순간의 초기화

바로 데미지의 Start() 부분이다.

처음에는 데미지 텍스트에 오브젝트 풀링을 적용하지 않아서 그냥 Start()에서 데미지 값에 대한 설정을 진행하였는데, 지금은 다르다.

계속해서 재 사용하게 되어 초기화 부분을 조정 해 주어야 한다.

<hr>

**OnEnable() 사용**

처음에는 비활성화 -> 활성화 시에 자동으로 실행되는 함수가 있을 것이라 생각하여 다시 한 번 유니티의 생명주기를 살펴 보았다.

![image](https://user-images.githubusercontent.com/66288087/210334847-72f14e46-14cc-4cf1-b346-c5a0d4c5916d.png)

이 그림([출처](https://itmining.tistory.com/47))을 보게 되면 Awake -> OnEnable -> Start 순서로 됨을 볼 수 있다.

따라서 사각형 객체를 만들어 아래와 같은 코드로 테스트를 진행 해 보았다.

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        Debug.Log("awake");
    }
    void Start()
    {
        Debug.Log("start");
    }

    private void OnEnable()
    {
        Debug.Log("enable");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
```

test.cs 코드이다.

발동되는 함수 이름을 로그로 출력 해 주었다.

그리고, 해당 객체를 비활성화 시킨 다음, 다시 활성화를 해 주었다.

![image](https://user-images.githubusercontent.com/66288087/210335559-226fcf34-2362-4451-a0d8-e39550ed2af2.png)

객체의 상태는 해당 컴포넌트가 비활성화 된 상태에서 등장하였다.

![image](https://user-images.githubusercontent.com/66288087/210335386-5238db50-6406-4cb6-b2d0-dc14c6be5f8b.png)

그 결과, 바로 위 사진과 같이 나오게 되었다.

처음 오브젝트가 등장했을 때는 Awake만 나왔으며, 스크립트를 활성화 시키자 Enable과 Start가 순서대로 나왔다.

그리고 스크립트가 켜진 상태에서 다시 오브젝트를 껐다가 켰을 때와 오브젝트는 활성화 되어 있으며 스크립트만 껐다가 켰을 때 Enable이 **반복적으로** 나오게 됨을 알 수 있었다.

따라서 Start() 에 있던 내용을 Enable()로 옮겨서 실행 해 보았다.

![image](https://user-images.githubusercontent.com/66288087/210336263-b3beeb79-1638-45a6-b80d-c9a99710abc0.png)

그런데.. 데미지가 적용이 되지 않고 0으로 찍혀 나오게 되었다..

<hr>

**함수 사용**

이에 추측컨데, 데미지를 처음에 설정할 때도 Awake에서 데미지 세팅을 했을 때 데미지가 적용되지 않았다.

OnEnable ----- **여기쯤** -----> Start // 이번에도 데미지 텍스트가 생성될 때, 왼쪽 처럼 OnEnable와 Start 사이의 어딘가에서부터 시작을 하게 되어 OnEnable에서 데미지를 세팅 해 준 것이 무용지물이 됐던 것 같다.

??? : 그렇다면 Start에서 설정 해 주면 되지 않냐? ... 라는 질문도 있을 텐데 위에서 강조 한 부분을 보면 Start()는 오브젝트가 사라졌다가 생성되지 않는 이상 반복되어 나오지 않게 된다.

이에, 굳이 OnEnable보다는 따로 함수를 만들어 주어 그곳에 통으로 Start()에 있는 코드를 옮겨주면 어떨까 생각하였다.

```C#
public void setDamage(int dmg)
{
    this.damage = dmg;

    dmgText = GetComponent<TextMeshPro>();
    dmgText.text = damage.ToString();

    alpha = dmgText.color;
    alpha.a = 255f;
    Invoke("inActiveDmg", 1.0f); // 2초 뒤에 데미지가 사라지게!
}

public void inActiveDmg()
{
    gameObject.SetActive(false);
}
```
따라서 이렇게 SetDamage()를 만들어 주어, 데미지를 세팅 해 주었다.

그리고 alpha.a 값도 다시 초기화를 시켜 주었다.

왜냐하면, 데미지 텍스트가 사라질 때 완전 투명해진 상태로 사라졌기 때문이다.

DestroyDmg() 함수도 inActiveDmg()로 바꾸어 주면 완벽하다.

이제, 다시 실행하여 Hierarchy 창을 주목 해 보도록 하자.

<hr>

### Result. 실행 모습

![2d_3_1](https://user-images.githubusercontent.com/66288087/210338171-6da460cb-d159-4617-8fa9-a5018fa8e7ac.gif)

왼쪽 puppet 자식 오브젝트쪽을 주목 해 보면 데미지 텍스트가 최대 2개 나와서 서로 재사용을 하는 모습을 볼 수 있다.

이제, 오브젝트 풀링을 몬스터의 스폰에 적용 해 보도록 해 보겠다.

<hr>

## 오브젝트 풀링 적용 2 - 몬스터 소환

















