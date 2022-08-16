# 목적 : 메이플스토리 내 독수리 사냥 구현

## 원본 게임 사진 (출처 - [맑음](https://www.youtube.com/watch?v=mnXyQrDXhvc)님 동영상)
![image](https://user-images.githubusercontent.com/66288087/182817316-1c2abc11-fbfa-487a-a51f-175f35a3fc25.png)

<hr>

### 1. Input.mousePosition을 이용한 저격 커서 만들기
<details>
    <summary>MousePointer.cs 코드 초안</summary><!-- Summary 밑에는 무조건 한 줄을 띄우기-->
    
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
</details>

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

<details>
    <summary>MousePointer.cs Update부분 (펼쳐서 볼 수 있다.)</summary>
    
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
</details>

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

<details>
    <summary>Target.cs (펼치기)</summary>
    
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
</details>

Target.cs 코드이다.

여기서 beHit() 함수를 통하여 hit여부를 true로 만들어 준 다음, update에 있는 if문을 통해서 Manager에 있는 ScoreManager.cs 의 함수를 불러 와 점수를 가산 해 준다.

그리고 위에 있던 MousePointer.cs도 아래와 같이 일부를 수정 해 준다.

<details>
    <summary>MousePointer.cs 중 일부(펼치기)</summary>
    
    if (hitDrawer)
    {
        Debug.Log("터치!");
        hitDrawer.collider.gameObject.GetComponent<Target>().beHit();
    }

</details>

**hitDrawer.collider.gameObject** 이 부분을 통하여 Ray를 맞은 타겟의 GameObject를 가져올 수 있으며 Target.cs 내의 beHit()을 실행시킨다.

그 다음에 해당 타겟은 제거된다.

아래는 Manager에서 점수를 더해주는 역할을 하는 ScoreManager.cs이다.

<details>
    <summary>ScoreManager.cs 초안(펼치기)</summary>

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
</details>

1점을 더해주는 타겟은 SetOne(), 2점을 더해주는 타겟은 SetTwo()를 실행하여 적절하게 더해 주는 역할을 한다.

![image](https://user-images.githubusercontent.com/66288087/183029145-303f2f9d-fcfb-45af-9200-df3c99ec2f74.png)

위 그림을 보면 타겟을 맞춘 후에 오른쪽 위에 있는 스코어보드가 가산되었음을 볼 수 있다.

##### <20220809추가>

그렇다면 어떤 물체는 1점 올려주고, 어떤 물체는 2점 올려주고.. 그런 것을 어떻게 구분할 수 있을까?

![image](https://user-images.githubusercontent.com/66288087/183624042-796afaa1-4849-4f84-9eb0-cfb080b9e2c9.png)

Tag 기능을 이용하였다.

Prefab에 Tag를 설정 해 두어 Tag를 통하여 점수를 주는 것을 구분하였다.
 
<details>
    <summary>Target.cs (펼치기)</summary>
    
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
                if(this.tag == "Score1") // 태그에 따라서 물체를 구분한다.
                {
                    Manager.GetComponent<ScoreManager>().SetOne();
                }
                else if(this.tag == "Minus1")
                {
                    Manager.GetComponent<ScoreManager>().MinusOne();
                }

                Destroy(gameObject); // 터치시 삭제
            }
        }

        public void beHit()
        {
            hit = true;
        }

    }
</details>
       
<details>
    <summary>ScoreManager.cs (펼치기)</summary>
    
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ScoreManager : MonoBehaviour
    {
        int TargetNum = 0; // 맞춰진 타겟의 종류!
        int cntScore;
        int Rand_Spawn;
        public Text Score;
        float RandomFloatX,RandomFloatY;
        public GameObject TargetPrefab1;
        public GameObject TargetPrefab2;
        public GameObject TargetPrefab3;
        public GameObject Bomb1;
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
                case -1: // 점수가 깎이는 물체를 맞췄을 경우
                    cntScore = int.Parse(Score.text);
                    if (cntScore > 0) // 깎일 점수가 있다면?
                    {
                        cntScore -= 1; // cut!
                        Score.text = cntScore.ToString();
                    }
                    TargetNum = 0;
                    break;
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

        public void MinusOne()
        {
            TargetNum = -1;
        }

        public void SpawnTarget()
        {
            Rand_Spawn = UnityEngine.Random.Range(0,3);
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            RandomFloatX = UnityEngine.Random.Range(-8.2f, 8.4f);
            RandomFloatY = UnityEngine.Random.Range(-4.4f, 4.4f);

            if(Rand_Spawn >= 0 && Rand_Spawn < 2) // 테스트용이긴 하지만 랜덤으로 다른 종류의 Prefab이 생성되게 하였다.
            {
                Target = Instantiate(TargetPrefab1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;
            }
            else if(Rand_Spawn == 2)
            {
                Target = Instantiate(Bomb1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;
            }


            if (RandomFloatX > 0)
                Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, 0));
            else
                Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 0));
        }

    }
</details>

코드에서 볼 수 있듯이 맞추면 점수가 깎이게 되는 폭탄도 추가하였다. 테스트용으로 생성 버튼을 누를 경우 25%의 확률로 나오게 설정 해 놓았다.

![image](https://user-images.githubusercontent.com/66288087/183623012-f1900147-3044-4a62-a008-7dfb19e65cfa.png)

디자인은 추후에 변경될 수 있다.


<hr>

### 4. 이동하는 물체 만들기(RigidBody2D 적용)

물체가 가만히 있는데 그것을 맞추는 것은 재미가 없을 것이다. 따라서 물체에도 움직임을 주어서 움직이게끔 해 줄 것이다.

테스트를 위하여 물체를 생성하는 버튼과 함수를 만들어 주도록 하자.

함수는 ScoreManager.cs 하단에 추가하였다. (SetTwo() 아래)

<details>
    <summary>ScoreManager.cs 내부 SpawnTarget()</summary>
    
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
</details>

Target의 RigidBody2D 에서 Body Type을 Dynamic으로 바꾸어 주게 되어 이제 중력의 영향을 받게 되었다.

위 함수에서 랜덤으로 좌표를 설정한 뒤, 왼쪽에 생성되면 오른쪽으로 힘을 주고, 그 반대면 왼쪽으로 힘을 주는 코드를 넣어 주었다.


![image](https://user-images.githubusercontent.com/66288087/183029806-4333ac4d-c576-42e5-be77-ed95f7828d09.png)

버튼은 위 사진과 같이 Anchor를 왼쪽 위로 설정하여 배치시켰다.

![image](https://user-images.githubusercontent.com/66288087/183029893-6764d4d5-adfb-4417-8ae9-7e90d279fb54.png)

Button Component에서 OnClick()시에 SpawnTarget()이 실행되게 설정하였다.

![test](https://user-images.githubusercontent.com/66288087/183033457-3f7fbf1a-c09d-4f2c-8c2a-9bc1330e2525.gif)

실행하면 위와 같이 된다.

<hr>

#### +알파 : 랜덤 움직임 구현하기

#### 4-1. 스크립트를 새로 만들어 일정 시간마다 움직임의 방향을 랜덤으로 부여하는 방법

<details>
    <summary>TargetMove.cs (펼치기)</summary>
    
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
</details>

일정 시간마다 다른 방향으로 힘을 받게 만들었다. 저 상태에서 주기를 조정하고 화면 밖으로 나가려 하면 무조건 화면 안쪽으로 순간이동 or 화면 안쪽으로의 방향으로 힘을 받게 만들면 좋을 것 같다.

#### 4-2. 4-1방법의 코드 보완 ( 화면 범위 안에 존재하게 하기 + 최대 속도 설정 및 제한(최대 속도를 통해 난이도 조절) )

<details>
    <summary>TargetMove.cs 코드 보완</summary>
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public class TargetMove : MonoBehaviour
    {
        Rigidbody2D rigid;
        float changeTime = 0f;
        int RandomMove;
        float TargetA_MaxVel = 10f;

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

            if (transform.position.x < -8.3f)
            {
                ToRight();

            }

            if (transform.position.x > 8.3f)
            {
                ToLeft();

            }

            if (transform.position.y > 4.4f)
            {
                ToDown();

            }

            if (transform.position.y < -4.4f)
            {
                ToUp();
            }



            if (changeTime > 2.0f)
            {
                Debug.Log(rigid.velocity);
                RandomMove = UnityEngine.Random.Range(0, 3);

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
                        ToDown();
                        changeTime = 0f;
                        break;
                    case 3:
                        ToUp();
                        changeTime = 0f;
                        break;
                }

            }

        }

        public void ToRight()
        {

            if(rigid.velocity.x < TargetA_MaxVel) // 오른쪽 방향(+ x 방향)으로 최대 속도 미만일 경우, 
            {
                rigid.AddForce(new Vector2(50, 0));
            }
            else
            {
                rigid.velocity = new Vector2(TargetA_MaxVel, rigid.velocity.y);
            }


        }

        public void ToLeft()
        {
            if (rigid.velocity.x > TargetA_MaxVel*(-1))
            {
                rigid.AddForce(new Vector2(-50, 0));
            }
            else
            {
                rigid.velocity = new Vector2(TargetA_MaxVel*(-1), rigid.velocity.y);
            }
        }

        public void ToUp()
        {
            if(rigid.velocity.y < TargetA_MaxVel)
            {
                rigid.AddForce(new Vector2(0, 100));
            }
            else
            {
                rigid.velocity = new Vector2(rigid.velocity.x, TargetA_MaxVel);
            }

        }

        public void ToDown()
        {
            if (rigid.velocity.y > TargetA_MaxVel*(-1))
            {
                rigid.AddForce(new Vector2(0, -100));
            }
            else
            {
                rigid.velocity = new Vector2(rigid.velocity.x, TargetA_MaxVel*(-1));
            }
        }

    }
</details>

최대 속도를 설정한 뒤, 최대 속도 미만이면 AddForce를 통하여 해당 방향으로 힘을 주는 함수 4개를 제작하였다.

화면 범위 밖에 나가게 되면 반대 방향으로 힘을 주는 것이 우선순위가 되게 하였다.

타겟들의 속도를 Debug.Log()를 통하여 출력함으로써 속도를 확인 해 본 결과 아래 사진과 같이 최대 제한속도 10f 에서 13f 정도의 속도로 제한 속도의 30%정도를 초과하는 범위의 속도를 가짐을 볼 수 있다.

![image](https://user-images.githubusercontent.com/66288087/183368110-641d9455-4615-4ddb-9a25-8c4f5cb0453d.png)


<hr>

### 5. 물체 디자인 및 애니메이션 적용 + (사운드)

#### Asperite를 이용한 간단한 도트 디자인

Asperite를 이용하여 도트를 찍게 된다.

![image](https://user-images.githubusercontent.com/66288087/183372086-a19da19a-3bd1-493e-88b0-b4309a3174b5.png)

간단하게 디자인을 마치고 프로펠러가 돌아가는 모습을 프레임을 추가하여 그려준다.

![image](https://user-images.githubusercontent.com/66288087/183372747-12bd9d47-6733-42f7-9db0-a1d1875c0577.png)

export sprite sheet를 통하여 유니티에서도 사용할 수 있게 저장 해 준다.

![image](https://user-images.githubusercontent.com/66288087/183372938-7bd64362-91fd-4db8-81cc-e03ff4234467.png)

Asset을 불러 온 다음, 위 사진과 같이 Multiple, Pixels per unit, Filter Mode를 바꾸어 준다.

Pixels Per Unit은 화면에 드래그 하여 바꾸어 가면서 적절한 숫자를 맞추어 준다.

그리고 Sprite Editor를 통하여 세부 설정으로 들어 가 준다.

![image](https://user-images.githubusercontent.com/66288087/183373664-815697c3-4486-4911-9560-245095d2ec79.png)

Slice를 눌러 sprite를 적절하게 잘라준다. 동일한 위치에 물체를 위치시켜야 한다.

![image](https://user-images.githubusercontent.com/66288087/183374028-cbd13259-4642-4645-a551-e378212518bf.png)

위와 같이 나눠진 것들을 드래그 하여 화면으로 드래그 해 주면 애니메이터 파일이 생성되게 된다. (애니메이션을 생성 할 범위만 드래그 해야 한다.)

기본적으로 드래그하여 생성된 애니메이션이 첫 애니메이션이면 Idle 상태가 된다.

이제 해당 오브젝트를 Prefab으로 만들고 스크립트를 적용시켜 보도록 하자.(대상 Prefab자리도 다 바꾸어야 한다.)

4번이 같이 적용된 상태의 게임 플레이 화면은 아래와 같다.

![image](https://user-images.githubusercontent.com/66288087/183376054-1bd86021-9abc-4f54-9233-42427f5162ed.png)

움짤로 따지는 못했지만 화면 밖으로 프로펠러 슬라임이 나가지 않는 모습이다.

4번 코드에 있는 최대 속도와 ScoreManager.cs 내부에 있는 것들을 조절하여 (Tag를 통하여 대상을 구분할 예정) 난이도에 따라서 점수를 차등적으로 얻을 수 있게 할 예정이다.

<hr>

#### + 알파 : 대상을 맞추었을 때 사라지는 애니메이션 만들기


#### 5-1. 파괴되는 애니메이션 도트 디자인

Asperite를 이용하여 슬라임이 피격되어 사라지는 애니메이션을 제작하였다.

![image](https://user-images.githubusercontent.com/66288087/184605577-fde4e0a2-e059-47d6-a14f-26a8c06a3369.png)

완성본은 아래와 같다.

![Target1_Destroy](https://user-images.githubusercontent.com/66288087/184605101-5d4e630f-9408-401b-a5c5-b10bc814329c.gif)

이제 Asperite에서 Sprite Sheet로 Export하여 Unity에 넣어 보도록 하자.


#### 5-2. Sprite 가공 및 애니메이션 적용(트리거 사용)

Unity에 넣어 주어 앞서 애니메이션을 적용한 것과 같이 Multiple + Pixel 조정 + Slice를 해 준 다음 애니메이션을 생성 해 준다.

![image](https://user-images.githubusercontent.com/66288087/184605371-51294daf-0238-45ae-81d7-d028d99bf092.png)

애니메이션을 제작 한 다음, 일부 프레임을 제거하였다. 13프레임 -> 5프레임 으로 반 이상을 대폭 줄임으로써 빠르게 터지는 것을 표현하였다.

여기서 애니메이션을 적용하는 것에 문제가 생기게 되었다.

![image](https://user-images.githubusercontent.com/66288087/184606042-fac77cbc-06cf-45a8-adb9-a154be9486be.png)

코드에서 볼 수 있듯이 애니메이션 트리거를 적용한 뒤 바로 Destroy를 적용하게 되면 애니메이션이 재생되지 못하고 사라지게 되어 애니메이션을 만든 의미가 없어지게 된다.

그래서 Invoke를 이용하여 Destroy를 따로 빼 내어 만들어 낸 함수를 애니메이션이 끝난 뒤에 실행되게끔 하면 아래 그림과 같이 클릭 판정 뒤에 나오는 파괴 애니메이션에서 중복으로 클릭이 가능한 현상이 발생하게 된다.

![image](https://user-images.githubusercontent.com/66288087/184607843-1081ffb4-5c51-47e2-9499-bba7c1e65b7c.png)

따라서 삭제중임을 알려 주는 tag(DeleteState)를 하나 추가 해 주어 클릭 판정이 나면 태그를 바로 변경시킨 다음에 애니메이션을 재생시켜서 중복으로 클릭되는 현상을 방지하였다.

(점수가 추가되는 것은 tag를 검사 한 다음에 이루어지기 때문에 태그를 변경시켜도 충분하였다! - 사실 레이어를 변경시키는 방법을 바로 찾지 못하여서 태그로 선회한 것이다..)

![image](https://user-images.githubusercontent.com/66288087/184608111-83b31854-c145-4873-b59b-0ca476d36c84.png)


![destroy_ani](https://user-images.githubusercontent.com/66288087/184609347-9bedd8aa-b2a5-474e-a436-05bda881d245.gif)

애니메이션이 적용된 움짤이다.


<hr>

#### + 사운드 적용

사운드는 Unity Asset Store에서 적절한 것들을 가져 와 사용하였다.

Manager에 SoundManager.cs를 하나 더 만들어 효과음을 내는 것으로 사용하였다.

<details>
    <summary>SoundManage.cs (펼치기)</summary>
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SoundManager : MonoBehaviour
    {
        AudioSource audio;

        public AudioClip Score1;
        public AudioClip Score2;
        public AudioClip Score3;
        public AudioClip Minus1;
        public AudioClip Minus2;

        // Start is called before the first frame update
        void Start()
        {
            audio = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlayScore1()
        {
            audio.clip = Score1;
            audio.Play();
        }

        public void PlayScore2()
        {
            audio.clip = Score2;
            audio.Play();
        }

        public void PlayScore3()
        {
            audio.clip = Score3;
            audio.Play();
        }

        public void PlayMinus1()
        {
            audio.clip = Minus1;
            audio.Play();
        }

        public void PlayMinus2()
        {
            audio.clip = Minus2;
            audio.Play();
        }

    }
</details>

Target.cs 에서 SoundManager와 연결시켜 준다.


<details>
    <summary>Target.cs 와 SoundManager.cs 연결(펼치기)</summary>
    
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
                if(this.tag == "Score1") // Tag Check
                {
                    Manager.GetComponent<SoundManager>().PlayScore1();
                    Manager.GetComponent<ScoreManager>().SetOne();
                }
                else if(this.tag == "Minus1")
                {
                    Manager.GetComponent<SoundManager>().PlayMinus1();
                    Manager.GetComponent<ScoreManager>().MinusOne();
                }

                Destroy(gameObject); // 터치시 삭제
            }
        }

        public void beHit()
        {
            hit = true;
        }

    }
</details>


BGM은 MainCamera에 Audio Source를 추가하여 넣어 준다. BGM과 효과음이 추가되었음을 볼 수 있다.


<hr>

### 6. 물체 생성기 구축

이제 다양한 난이도의 대상들의 디자인 및 태그로 구분을 하였다면 이제 수동으로 버튼을 눌러 생성하는 것이 아닌, 스테이지가 일정 시간이 지나면 자동으로 시작되게끔 만들어 주도록 하자.

스테이지 카운트 다운, 클리어, 클리어하지 못함을 알려주는 것도 디자인 해야 할 것이다.



<hr>

+ 알파 : 총알의 개수를 제한하여 총알의 개수를 가시적으로 나타낼 수 있는 기능도 추가할 것(총알 디자인도 하자)

총알 관리자(BulletManager.cs)를 추가하여 남은 총알의 개수가 1개 이상일 경우에만 대상을 맞출 수 있게 하였다.

이와 더불어 기존 코드들의 일부분도 수정할 부분이 생겼다.

우선 BulletManager.cs

<details>
    <summary>BulletManager.cs (펼치기)</summary>
   
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class BulletManager : MonoBehaviour
    {
        int remainedBullet = 0; // 남은 총알 개수
        public Text bulletText;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            bulletText.text = remainedBullet.ToString(); // 실시간으로 남은 총알 개수를 최신화 해 준다.
        }

        public void AddBullet(int count)
        {
            this.remainedBullet = count;
        }

        public void discountBullet(int count)
        {
            this.remainedBullet -= count;
        }

        public int GetBulletCount()
        {
            return this.remainedBullet;
        }

    }
</details>

    
<details>
    <summary>MousePointer.cs (펼치기)</summary>
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MousePointer : MonoBehaviour
    {
        public GameObject pointerPrefab;
        private GameObject pointerRed;
        Vector3 mousePos;
        BulletManager bullet;


        // Start is called before the first frame update
        void Start()
        {
            pointerRed = Instantiate(pointerPrefab) as GameObject;
            bullet = GetComponent<BulletManager>(); // 총알 매니저 컴포넌트를 불러 온다.
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            mousePos = Input.mousePosition;
            mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(mousePos);
            mousePos.z = -1; // 마우스 포인터가 대상 물체의 앞에 나오게끔!
            pointerRed.transform.position = mousePos;
            Ray2D ray = new Ray2D(mousePos, Vector2.zero); // 원점 ~ 포인터로 발사되는 레이저

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(mousePos);

                float distance = Mathf.Infinity; // Ray 내에서 감지할 최대 거리

                // RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, distance); // 다 잡음 
                RaycastHit2D hitDrawer = Physics2D.Raycast(ray.origin, ray.direction, distance, 1 << LayerMask.NameToLayer("Touchable")); // 1 << LayerMask.NameToLayer("Touchable") 대신 2048을 써도 됨

                if(bullet.GetBulletCount() > 0) // 남은 총알 개수를 불러 온 다음 쏠 수 있는 총알이 있다면(1 이상)
                {
                    bullet.discountBullet(1); // 총알 차감
                    if (hitDrawer)
                    {
                        Debug.Log("터치!");
                        hitDrawer.collider.gameObject.GetComponent<Target>().beHit();
                    }

                }
                else // 총알이 없다면 실제 시나리오에서는 대상을 다 맞췄는지 여부를 계산 한 다음 이동!
                {
                    Debug.Log("남은 총알이 없습니다!");
                }


            }

        }
    }
</details>

그리고 버튼과 총알 개수를 나타내는 텍스트UI를 추가하였다.

![image](https://user-images.githubusercontent.com/66288087/183625020-93d3bebe-16d1-432c-b406-59d164572970.png)

실행 결과, 총알이 없는 상태에서는 아무리 클릭 해도 점수의 변동이 없음을 볼 수 있다.

![image](https://user-images.githubusercontent.com/66288087/183625206-732d43ae-d3a6-43fa-b568-5d0efe0be5cb.png)

총알을 충전하게 되면 이렇게 점수를 얻고 차감됨을 볼 수 있다.

지금은 생성 버튼을 누를 때도 총알이 나가지만 실전에서는 자동으로 생성되므로 괜찮다.

<hr>

220810 - 총알의 Max 개수를 10개로 제한하고, 총알의 개수가 0개일 때만 테스트로 리필할 수 있게 하였다.

결과물은 다음 사진과 같다.

![image](https://user-images.githubusercontent.com/66288087/183871282-dd52d88b-2737-4a8f-9c9c-d2a86054e052.png)

총알이 10개가 충전되어 가득 찬 모습이다.

![image](https://user-images.githubusercontent.com/66288087/183871404-0fbbd2db-1ad3-4310-8238-67cd86dee4dd.png)

총알을 쏘게 되면 총알 그림이 사라지고 있다.

총알 그림을 소환하고 없애는 방식은 프리토 구애의 춤 구현 시도에서 사용했던 방법을 사용하여 구현하였다.


<details>
    <summary>BulletManager.cs (펼치기)</summary>
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class BulletManager : MonoBehaviour
    {
        int remainedBullet = 0;
        public Text bulletText;
        List<string> BulletImgName = new List<string>();
        public Image BulletImg;

        public GameObject UIBase;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            bulletText.text = remainedBullet.ToString();
        }

        public void AddBullet(int count)
        {
            if(remainedBullet == 0)
            {
                BulletImgName.Clear();
                this.remainedBullet = count;
                SetBulletUI();
            }
            else
            {
                Debug.Log("총알이 다 떨어졌을 때 충전할 수 있습니다.");

            }

        }

        public void discountBullet(int count)
        {
            this.remainedBullet -= count;

            Destroy(GameObject.Find(BulletImgName[remainedBullet]));

        }

        public int GetBulletCount()
        {
            return this.remainedBullet;
        }

        public void SetBulletUI()
        {
            for(int i = 0; i < remainedBullet; i++)
            {
                Vector3 offSet = UIBase.transform.position + new Vector3(340 - i * 50, -160, 0);

                Image BulletImsi = Instantiate(BulletImg);
                BulletImsi.transform.SetParent(UIBase.transform,false);
                BulletImsi.name = "BulImge" + i;
                BulletImsi.transform.position = offSet;

                BulletImgName.Add(BulletImsi.name);

            }
        }

    }
</details>

Canvas에서 Image를 만들고 Sprite를 총알 그림으로 바꾸어 준 다음 Prefab화를 해 주어 코드에서 활용하였다.(BulletImg)


<hr>

### 7. 게임 시작 ~ 퇴장 시나리오 구축

게임에 대한 진행을 주관하는 코드를 구현하여 게임 시나리오를 구축 해 보도록 하겠다.

게임 진행을 주관하는 Stage Manager.cs를 만들어 주었다.

Score Manager.cs에 있던 Spawn 함수를 가져와서 스테이지에 맞게 다듬어 주었다.

<details>
    <summary>StageManager.cs (펼치기)</summary>
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System;

    public class StageManager : MonoBehaviour
    {
        float RandomFloatX, RandomFloatY; // 생성 위치

        public GameObject TargetPrefab1; // 타겟 1
        public GameObject TargetPrefab2; // 타겟 2
        public GameObject TargetPrefab3; // 타겟 3
        public GameObject Bomb1; // 폭탄 1
        private GameObject Target; // 동적으로 생성 된 타겟


        private List<String> Target1Name = new List<String>();
        private List<String> Bomb1Name = new List<String>();

        int remainTarget1 = -1; // 남은 타겟의 수를 실시간으로 갱신해 주기 위한 것, 이 것이 0이 되면 다음 스테이지로 간다.
        // -1 > 체크를 하지 않는 상태


        // Start is called before the first frame update
        void Start()
        {
            Stage1();
        }

        // Update is called once per frame
        void Update()
        {
            Stage1Check();
        }

        public void SpawnTarget1(int num)
        {
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            RandomFloatX = UnityEngine.Random.Range(-8.2f, 8.4f);
            RandomFloatY = UnityEngine.Random.Range(-4.4f, 4.4f);

            Target = Instantiate(TargetPrefab1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;

            Target.name = "Target1_" + num; // 이름 + 숫자를 적용하여 이름 변경(스테이지 초기화 시 마다 숫자 초기화)
            Target1Name.Add(Target.name); // 리스트에 저장(게임 오버시에나 폭탄이 남았을 때 제거하기 위함)

            if (RandomFloatX > 0)
                Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, 0));
            else
                Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 0));
        }

        public void SpawnBomb1(int num)
        {
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            RandomFloatX = UnityEngine.Random.Range(-8.2f, 8.4f);
            RandomFloatY = UnityEngine.Random.Range(-4.4f, 4.4f);

            Target = Instantiate(Bomb1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;

            Target.name = "Bomb1_" + num; // 이름 + 숫자를 적용하여 이름 변경(스테이지 초기화 시 마다 숫자 초기화)
            Bomb1Name.Add(Target.name); // 리스트에 저장(게임 오버시에나 폭탄이 남았을 때 제거하기 위함)

            if (RandomFloatX > 0)
                Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, 0));
            else
                Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 0));
        }

        public void Stage1()
        {
            remainTarget1 = 10;
            for(int i = 0; i < 10; i++)
            {
                SpawnTarget1(i); // 타겟 10개 소환
                Debug.Log("타겟" + i + "개 소환");
                if (i < 5) // 폭탄 5개 소환
                    SpawnBomb1(i);
            }

        }

        public void Stage1Check()
        {
            if(remainTarget1 == 0)
            {
                Debug.Log("스테이지 1 클리어! 폭탄을 제거합니다.");
                for(int i = 0; i < Bomb1Name.Count; i++)
                {
                    try
                    {
                        Destroy(GameObject.Find(Bomb1Name[i]));
                    }
                    catch
                    {
                        Debug.Log("이미 맞춘 폭탄이 있어서 다음 것을 제거합니다!");
                        continue;
                    }

                }
                Bomb1Name.Clear();
                Target1Name.Clear();

                remainTarget1 = -1; // checkOff상태로 변경!
            }
        }

        public void MinusTarget1()
        {
            remainTarget1--;
        }

    }
</details>


함수는 다음과 같이 설정하였다.

1. 스테이지 시작 함수
2. 타겟 생성 함수
3. 폭탄 생성 함수
4. 스테이지 클리어 여부 체크 함수
5. 남은 타겟 개수를 갱신해 주는 함수
6. 시간 초과시 게임 오버를 체킹하는 함수(구현 예정)

우선 스테이지 생성 함수에는 2번과 3번 함수를 for문을 통해서 설정된 개수만큼 객체를 spawn 해 주는 역할을 한다.

그리고 클리어, 게임 오버시에 남은 객체들을 제거 해 주기 위하여 생성된 객체의 이름을 저장 해 주는 List를 만들어 주었다.

그리고 남은 타겟 수를 넣을 변수를 만들어 주고(클리어 여부 체크를 위함) 커서로 타켓을 클릭했을 때, 개수가 줄어들게 만들어 준다.

또한, Update에 남은 타겟 수가 0이 되면 스테이지를 클리어 판정을 하고 남은 타겟 변수를 -1로 하여 계속하여 클리어가 되는 현상을 방지하였다.

![image](https://user-images.githubusercontent.com/66288087/184317972-1979ff8a-cde5-4e37-b8f7-63079aa549ee.png)

클리어시에 나오는 Log

Target.cs 변경
    
<details>
    <summary>Target.cs 변경점 Code (펼치기)</summary>
    
    void Update()
    {
        if(hit == true)
        {
            if(this.tag == "Score1") // Tag Check
            {
                Manager.GetComponent<SoundManager>().PlayScore1();
                Manager.GetComponent<StageManager>().MinusTarget1();
                Manager.GetComponent<ScoreManager>().SetOne();
            }
            else if(this.tag == "Minus1")
            {
                Manager.GetComponent<SoundManager>().PlayMinus1();
                Manager.GetComponent<ScoreManager>().MinusOne();
            }

            Destroy(gameObject); // 터치시 삭제
        }
    }

</details>


ScoreManager.cs 변경

<details>
    <summary>ScoreManager.cs 변경점 Code (펼치기)</summary>
    
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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            switch (TargetNum)
            {
                case -1:
                    cntScore = int.Parse(Score.text);
                    if (cntScore > 0)
                    {
                        cntScore -= 1;
                        Score.text = cntScore.ToString();
                    }
                    TargetNum = 0;
                    break;
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

        public void MinusOne()
        {
            TargetNum = -1;
        }
    }
</details>

또한, TargetMove.cs에서도 타겟의 tag별로 속도를 다르게 하기 위해서 최대 속도를 넣을 변수를 하나 만들고 태그별로 최대 속도를 다르게 할 수 있게 하였다.

(if문을 아낄 수 있는 방법)

아래 코드는 TargetMove.cs의 일부이다. Down,Right,Left,Up 모두 같게 적용하였다.

<details>
    <summary>ToDown() 함수 (펼치기)</summary>
    
    public void ToDown()
    {

        if (this.tag == "Score1")
            MaxVel = TargetA_MaxVel;
        else if (this.tag == "Minus1")
            MaxVel = Bomb1_MaxVel;

        if (rigid.velocity.y > MaxVel * (-1))
        {
            rigid.AddForce(new Vector2(0, -100));
        }
        else
        {
            rigid.velocity = new Vector2(rigid.velocity.x, MaxVel * (-1));
        }
    }
</details>

<hr>

이제 6번 함수 구현과 더불어 다음 스테이지로 넘어가는 것 구현 + 모든 스테이지를 클리어 했을 때 원래의 맵으로 돌아가게 하는 것도 구현 할 예정이다.

220813

총알을 다 소비하였는데 타겟이 남아 있는 경우에는 게임 오버가 된다. 그 부분을 추가하였다.

StageManager.cs 중...

<details>
    <summary> (펼치기)</summary>
    
    public void GameOverCheck()
    {
        if((GetComponent<BulletManager>().GetBulletCount() == 0) && remainTarget1 > 0)
        {
            //타겟이 남았을 때 총알을 다 소비하였다면..!
            Debug.Log("총알을 다 소비하였습니다! 게임 오버!");

            GameOver.SetActive(true);

            for (int i = 0; i < Target1Name.Count; i++)
            {
                try
                {
                    Destroy(GameObject.Find(Target1Name[i]));
                }
                catch
                {
                    Debug.Log("맞춘 타겟이 있어서 스킵!");
                    continue;
                }

            }

            for (int i = 0; i < Bomb1Name.Count; i++)
            {
                try
                {
                    Destroy(GameObject.Find(Bomb1Name[i]));
                }
                catch
                {
                    Debug.Log("이미 맞춘 폭탄이 있어서 다음 것을 제거합니다!");
                    continue;
                }

            }
            Bomb1Name.Clear();
            Target1Name.Clear();

            remainTarget1 = -1; // checkOff상태로 변경!


            Invoke("BackToTheMap", 2); // 2초후 BackToTheMap 호출



        }
    }
    
    public void BackToTheMap()
    {
        SceneManager.LoadScene("MainMap");
    }
</details>
    

</code>
</pre>

그리고 해당 함수를 Update에 위치시키면 게임 오버 여부를 판단 해 준다.

![image](https://user-images.githubusercontent.com/66288087/184470077-18cb0b9d-801d-452f-8733-65e7c5fd9a8d.png)

게임 오버

![image](https://user-images.githubusercontent.com/66288087/184470089-c5c0acfa-5e06-4cb6-9277-ebcbd946237e.png)

그리고 원래의 횡스크롤 맵으로 돌아가게 된다.

횡스크롤 맵은 현재 연습하고 있는 맵으로써 다른 미니게임들과도 연계할 예정이다.

<hr>

+알파 : 스토리 추가하기

기존 횡스크롤 맵에서 NPC 제작


<hr>

## 발생한 버그 Fix

### 1. 러브샷 클리어에서 게임 오버가 나오는 현상

남은 총알 1개로 대상을 맞추었을 때, 총알과 대상이 동시에 0개가 되면 무조건 GameOver가 나오게 되는 현상이 발생하였다.

이것은 게임 오버의 조건을 남은 타겟의 수가 1개 이상 이면서(and) 총알의 개수가 0개가 되면 게임 오버가 나오게끔 한 것 때문인 듯 하다.

![image](https://user-images.githubusercontent.com/66288087/184611215-71b30004-4ebe-4d8b-8e67-6947f9136eaf.png)

총알 개수가 먼저 감소되고, 타겟 개수가 감소되기 전에 게임 오버 여부를 판단하기에 이렇게 된 것이다.

![image](https://user-images.githubusercontent.com/66288087/184611429-fb4328af-231e-4ed4-87b5-c14a60732db6.png)

따라서 대상을 맞추었을 때 총알이 감소하는 것을 따로 빼내어 남은 타겟 수 감소 바로 뒤에 배치시켰다. (한 개의 함수로 묶어서 선후관계를 명확하게 하였다.)

수정된 코드는 코드 파일에서 볼 수 있다.


### 2. 대상이 초반에 겹쳐 있는 현상

