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

위 코드는 빈 오브젝트를 만들어 삽입하면 된다.

![image](https://user-images.githubusercontent.com/66288087/182818568-263ee2da-1465-4512-af17-be3f2a9085eb.png)

추후에 디자인은 바뀔수도 있다.

<hr>

### 2. 대상 물체 클릭 시 반응이 일어나게 하기

이제 커서를 물체 위에 놓고 맞추게 되면 물체가 맞았음을 감지할 수 있어야 한다.

처음에는 Collider Event를 통하여 할 수 있지 않을까 생각하였지만 정보를 찾아 본 결과 [이곳](https://holika.tistory.com/entry/%EC%9C%A0%EC%96%B4%ED%85%8C%EC%9D%BC-%EA%B0%9C%EB%B0%9C-01-Ray2D%EB%A5%BC-%ED%86%B5%ED%95%9C-2D-%ED%84%B0%EC%B9%98%EC%9D%B4%EB%B2%A4%ED%8A%B8-%EA%B5%AC%ED%98%84)을 참고하여 Ray를 활용한 클릭 반응을 구현하였다.

![image](https://user-images.githubusercontent.com/66288087/182819270-eebdd300-21e7-4935-a8fa-c853b5a8e8ab.png)

우선 대상이 될 물체를 만들어 준다.

RigidBody2D와 CircleCollider2D를 추가 해 주고, 테스트를 위해 RigidBody2D에서 Body Type을 Kinematic으로 바꾸어 준다.

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

여기서 **1 << LayerMask.NameToLayer("Touchable")** 이 부분에서는 **비트 연산자**가 사용되었는데, **LayerMask**는 위 사진에서도 봤던 레이어 번호를 그대로 사용하지 않고 **2를 레이어 번호 수 만큼 제곱시킨 수**를 사용해서 레이어를 감지하게 된다. ([이곳](https://nakedgang.tistory.com/80)을 참고하였다.)

즉, 8번 레이어라면 2의 8제곱인 256인 것이다. 따라서, 00000000001 에서 왼쪽으로 LayerMask.NameToLayer("Touchable")만큼 비트를 이동하라는 의미이다.

LayerMask.NameToLayer("Touchable") 는 유니티 화면에 나오는 10진수 그대로 나오게 된다.

1 << LayerMask.NameToLayer("Touchable") 이 부분 대신에 2의 제곱을 한 숫자를 그대로 넣어도 나오게 된다.

![image](https://user-images.githubusercontent.com/66288087/182822161-6d3c3409-c61c-4671-94db-102f74e66317.png)

물체를 클릭하게 되면 위 사진과 같이 2048(2의 11제곱)과 11이 나오게 됨을 볼 수 있다.

<hr>

### 3. 스코어 보드 생성 및 물체 난이도 분화

우선 타겟을 맞추었을 때 스코어가 올라가게끔 하는 시스템을 만들어 보자.

처음에는 타겟 자체에서 스코어 보드를 통제를 하게끔 하려 했지만 Prefab화가 되면 스크립트에 세팅 해 놓은 UI Text가 초기화 되는 바람에 결국 그 기능은 Manager로 넘기게 되었다.

![image](https://user-images.githubusercontent.com/66288087/183019287-25adf7df-30f3-4150-b1b9-71577e7deb43.png)

위 그림과 같이 Manager가 맞았음을 인지하고 중간에서 점수를 가산시켜주는 역할을 해 주어야 한다.

우선 타겟이 맞았을 때 맞았다는 사실을 전해주기 위해서 Target Prefab에 들어 갈 코드를 작성 해 보도록 하자.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    bool hit = false; // 맞았는지 여부를 나타냄
    GameObject Manager;

    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.Find("Manager");
    }

    // Update is called once per frame
    void Update()
    {
        if(hit == true)
        {
            Manager.GetComponent<ScoreManager>().SetOne();
            Destroy(gameObject); // 터치시 삭제
        }
    }

    public void beHit()
    {
        hit = true;
    }
    
}
</code>
</pre>

Target.cs 코드이다.

여기서 beHit() 함수를 통하여 hit여부를 true로 만들어 준 다음, update에 있는 if문을 통해서 Manager에 있는 ScoreManager.cs 의 함수를 불러 와 점수를 가산 해 준다.

그리고 위에 있던 MousePointer.cs도 아래와 같이 일부를 수정 해 준다.

<pre>
<code>
if (hitDrawer)
{
    Debug.Log("터치!");
    hitDrawer.collider.gameObject.GetComponent<Target>().beHit();
}
</code>
</pre>

**hitDrawer.collider.gameObject** 이 부분을 통하여 Ray를 맞은 타겟의 GameObject를 가져올 수 있으며 Target.cs 내의 beHit()을 실행시킨다.

그 다음에 해당 타겟은 제거된다.

아래는 Manager에서 점수를 더해주는 역할을 하는 ScoreManager.cs이다.

<pre>
<code>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int TargetNum = 0; // 맞춰진 타겟의 종류!
    int cntScore;
    public Text Score;
    float RandomFloatX,RandomFloatY;
    public GameObject TargetPrefab;
    private GameObject Target;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        switch (TargetNum)
        {
            case 1:
                cntScore = int.Parse(Score.text);
                cntScore += 1;
                Score.text = cntScore.ToString();
                TargetNum = 0;
                break;
            case 2:
                cntScore = int.Parse(Score.text);
                cntScore += 2;
                Score.text = cntScore.ToString();
                TargetNum = 0;
                break;
        }

    }

    public void SetOne()
    {
        TargetNum = 1;
    }

    public void SetTwo()
    {
        TargetNum = 2;
    }

}
</code>
</pre>

1점을 더해주는 타겟은 SetOne(), 2점을 더해주는 타겟은 SetTwo()를 실행하여 적절하게 더해 주는 역할을 한다.

![image](https://user-images.githubusercontent.com/66288087/183029145-303f2f9d-fcfb-45af-9200-df3c99ec2f74.png)

위 그림을 보면 타겟을 맞춘 후에 오른쪽 위에 있는 스코어보드가 가산되었음을 볼 수 있다.


<hr>

### 4. 이동하는 물체 만들기(RigidBody2D 적용)

물체가 가만히 있는데 그것을 맞추는 것은 재미가 없을 것이다. 따라서 물체에도 움직임을 주어서 움직이게끔 해 줄 것이다.

테스트를 위하여 물체를 생성하는 버튼과 함수를 만들어 주도록 하자.

함수는 ScoreManager.cs 하단에 추가하였다. (SetTwo() 아래)

<pre>
<code>
public void SpawnTarget()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        RandomFloatX = UnityEngine.Random.Range(-8.2f, 8.4f);
        RandomFloatY = UnityEngine.Random.Range(-4.4f, 4.4f);

        Target = Instantiate(TargetPrefab, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;

        if(RandomFloatX > 0)
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, 0));
        else
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 0));
    }
</code>
</pre>

Target의 RigidBody2D 에서 Body Type을 Dynamic으로 바꾸어 주게 되어 이제 중력의 영향을 받게 되었다.

위 함수에서 랜덤으로 좌표를 설정한 뒤, 왼쪽에 생성되면 오른쪽으로 힘을 주고, 그 반대면 왼쪽으로 힘을 주는 코드를 넣어 주었다.


![image](https://user-images.githubusercontent.com/66288087/183029806-4333ac4d-c576-42e5-be77-ed95f7828d09.png)

버튼은 위 사진과 같이 Anchor를 왼쪽 위로 설정하여 배치시켰다.

![image](https://user-images.githubusercontent.com/66288087/183029893-6764d4d5-adfb-4417-8ae9-7e90d279fb54.png)

Button Component에서 OnClick()시에 SpawnTarget()이 실행되게 설정하였다.

![test](https://user-images.githubusercontent.com/66288087/183033457-3f7fbf1a-c09d-4f2c-8c2a-9bc1330e2525.gif)

실행하면 위와 같이 된다.

#### +알파 : 랜덤 움직임 구현하기

#### 4-1. Case1 - 스크립트를 새로 만들어 일정 시간마다 움직임의 방향을 랜덤으로 부여하는 방법

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetMove : MonoBehaviour
{
    Rigidbody2D rigid;
    float changeTime = 0f;
    int RandomMove;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();        
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        changeTime += Time.deltaTime;

        if (changeTime > 0.5f)
        {
            RandomMove = UnityEngine.Random.Range(0, 4);

            switch (RandomMove)
            {
                case 0:
                    ToRight();
                    changeTime = 0f;
                    break;
                case 1:
                    ToLeft();
                    changeTime = 0f;
                    break;
                case 2:
                    ToUp();
                    changeTime = 0f;
                    break;
                case 3:
                    ToDown();
                    changeTime = 0f;
                    break;
            }

        }
        

    }

    public void ToRight()
    {
        rigid.AddForce(new Vector2(500, 0));
    }

    public void ToLeft()
    {
        rigid.AddForce(new Vector2(-500, 0));
    }

    public void ToUp()
    {
        rigid.AddForce(new Vector2(0,500));
    }

    public void ToDown()
    {
        rigid.AddForce(new Vector2(0,-500));
    }


}
</code>
</pre>

일정 시간마다 다른 방향으로 힘을 받게 만들었다. 저 상태에서 주기를 조정하고 화면 밖으로 나가려 하면 무조건 화면 안쪽으로 순간이동 or 화면 안쪽으로의 방향으로 힘을 받게 만들면 좋을 것 같다.


<hr>

### 5. 물체 디자인 및 애니메이션 적용




<hr>

### 6. 물체 생성기 구축






<hr>

### 7. 게임 시작 ~ 퇴장 시나리오 구축
















