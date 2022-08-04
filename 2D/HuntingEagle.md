# 목적 : 메이플스토리 내 독수리 사냥 구현

## 원본 게임 사진 (출처 - [맑음](https://www.youtube.com/watch?v=mnXyQrDXhvc)님 동영상)
![image](https://user-images.githubusercontent.com/66288087/182817316-1c2abc11-fbfa-487a-a51f-175f35a3fc25.png)

<hr>

### 1. Input.mousePosition을 이용한 저격 커서 만들기

<pre>
<code>
public GameObject pointerPrefab;
private GameObject pointerRed;
Vector2 mousePos;


// Start is called before the first frame update
void Start()
{
    pointerRed = Instantiate(pointerPrefab) as GameObject;
    Cursor.visible = false;
}

void Update()
    {
        mousePos = Input.mousePosition;
        mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(mousePos);
        pointerRed.transform.position = mousePos;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(mousePos);
        }

    }
</code>
</pre>

Input.mousePosition을 통하여 마우스 포인터의 위치를 가져온 다음, 그곳에 시작 시에 생성한 Prefab을 위치시켜 저격 커서가 마우스를 따라 다니게끔 하였다.

Prefab이 들어갈 GameObject와 생성된 GameObject가 할당 될 부분을 만들어 주어 Instantiate를 한 다음에도 위치를 쉽게 가져올 수 있게 하였다.

![image](https://user-images.githubusercontent.com/66288087/182818568-263ee2da-1465-4512-af17-be3f2a9085eb.png)

추후에 디자인은 바뀔수도 있다.

<hr>

### 2. 대상 물체 클릭 시 반응이 일어나게 하기

이제 커서를 물체 위에 놓고 맞추게 되면 물체가 맞았음을 감지할 수 있어야 한다.

처음에는 Collider Event를 통하여 할 수 있지 않을까 생각하였지만 정보를 찾아 본 결과 [이곳](https://holika.tistory.com/entry/%EC%9C%A0%EC%96%B4%ED%85%8C%EC%9D%BC-%EA%B0%9C%EB%B0%9C-01-Ray2D%EB%A5%BC-%ED%86%B5%ED%95%9C-2D-%ED%84%B0%EC%B9%98%EC%9D%B4%EB%B2%A4%ED%8A%B8-%EA%B5%AC%ED%98%84)을 참고하여 Ray를 활용한 클릭 반응을 구현하였다.

![image](https://user-images.githubusercontent.com/66288087/182819270-eebdd300-21e7-4935-a8fa-c853b5a8e8ab.png)

우선 대상이 될 물체를 만들어 준다.

그리고 물체 Inspector에서 Layer를 새롭게 하나 만들어 준다.

![image](https://user-images.githubusercontent.com/66288087/182822006-ebb53fd5-4a0e-490b-afaf-a6aefcbc7e37.png)

11번 레이어인 "Touchable"레이어를 하나 만들어 주었다.

그 다음, RayCast를 활용한 부분을 1번에 나왔던 코드에 보강시켜 주면 아래와 같은 코드가 나오게 된다.

<pre>
<code>
void Update()
    {
        mousePos = Input.mousePosition;
        mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(mousePos);
        pointerRed.transform.position = mousePos;
        Ray2D ray = new Ray2D(mousePos, Vector2.zero); // 원점 ~ 포인터

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(mousePos);

            float distance = Mathf.Infinity; // Ray 내에서 감지할 최대 거리
            
            RaycastHit2D hitDrawer = Physics2D.Raycast(ray.origin, ray.direction, distance, 1 << LayerMask.NameToLayer("Touchable")); // Touchable 레이어만 잡는다.

            if (hitDrawer)
            {
                Debug.Log("터치!");
                Debug.Log(1 << LayerMask.NameToLayer("Touchable"));
                Debug.Log(LayerMask.NameToLayer("Touchable"));
            }

        }

    }
</code>
</pre>

Update 부분만 변하였기에 그 부분만 가져오게 되었다.

처음에 ray를 쏘게 되는데 원점(Vector2.zero)에서 포인터의 위치를 향해 쏘게 설정하였다.

그리고 마우스가 눌리게 되면(Input.GetMouseButton(0)) ray가 발사되며 "Touchable"이라는 이름의 레이어를 가지는 물체의 Collider를 감지하게 된다.

여기서 1 << LayerMask.NameToLayer("Touchable") 이 부분에서는 비트 연산자가 사용되었는데, LayerMask는 비트 연산자를 사용해서 레이어를 감지하게 된다. ([이곳](https://nakedgang.tistory.com/80)을 참고하였다.)

즉, 8번 레이어라면 2의 8제곱인 256인 것이다. 따라서, 00000000001 에서 왼쪽으로 LayerMask.NameToLayer("Touchable")만큼 비트를 이동하라는 의미이다.

LayerMask.NameToLayer("Touchable") 는 유니티 화면에 나오는 10진수 그대로 나오게 된다.

1 << LayerMask.NameToLayer("Touchable") 이 부분 대신에 2의 제곱을 한 숫자를 그대로 넣어도 나오게 된다.

![image](https://user-images.githubusercontent.com/66288087/182822161-6d3c3409-c61c-4671-94db-102f74e66317.png)

물체를 클릭하게 되면 위 사진과 같이 2048(2의 11제곱)과 11이 나오게 됨을 볼 수 있다.




