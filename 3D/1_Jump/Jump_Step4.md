# Step4. 스테이지 2 추가 (0.0.3)

**이관 정보, 복사 데이터 구분**

이제 새로운 스테이지로 넘어가는 부분을 추가 해 주도록 하겠다.

또한, 점수 정보들을 씬이 넘어가더라도 유지되게끔 해 줄 필요가 있다.

따라서 빈 오브젝트를 만들고, **DontDestroyOnLoad(gameObject);** 를 통하여 정보가 이관되게끔 해 줄 예정이다.

그렇지만 스테이지 1~3의 누적 점수들은 이름과 함께 파일에 저장 해 줌으로써, 게시판에 랭킹표와 같은 기능들도 넣어 주는 것이 좋을 것 같다는 생각이 들었다.

관련한 오브젝트와 코드는 아래에서 다시 언급하도록 하겠다.

<hr>

**2 스테이지 구성**

![image](https://user-images.githubusercontent.com/66288087/188884167-68104fba-e195-4776-8814-9fb349577aba.png)

2스테이지의 일부이다.

2스테이지의 전체적인 컨셉은 두 갈래길이다.

왼쪽은 세심한 컨트롤을 이용하여 침착하게 이동해야 하고, 오른쪽은 다양한 기믹 발판들을 잘 통과하면서 이동해야 하는 컨셉이다.

양 갈래의 길 중에서 한 갈래만을 선택한 뒤, 골인 지점에서 다시 만날 수 있게 할 예정이다.

또한, 코인 점수 역시 같은 점수로 배분할 예정이기에 한 길을 선택한 뒤, 다른 길은 선택할 수 없다.

선택 세이브 포인트를 먹었을 때, 벽이 활성화 되기에 세이브 포인트를 먹지 않고 코인을 먹은 다음, 다시 돌아와서 추가적으로 코인을 먹는 꼼수(?)를 방지하기 위해서 세이브 포인트를 늘리고, 통로를 좁게 하는 등의 시도를 해 보았지만 역시, 가장 좋은 것은 먹지 않고서는 진행하지 못하게 하여 사용자로 하여금 먹게 만들어 주는 것이다.

따라서 대쉬+점프로 닿을 수 없는 거리로 발판을 설정 한 다음, 선택 세이브 포인트(chooseRoute)를 먹게 되면 중간 발판이 생기게 만들어 주었다.

![image](https://user-images.githubusercontent.com/66288087/188893253-ffac8369-0297-457a-a4a9-30b47366ed53.png)

투명 발판 위치를 나타내었으며, 발판의 위치를 수정한 사진이다.

<hr>

![image](https://user-images.githubusercontent.com/66288087/189127817-45d34d7d-b272-454a-b5c2-52d49272ef0a.png)

2 스테이지의 전체 모습을 찍어 보았다.

앞서 설명하였듯이, 왼쪽/오른쪽 스테이지를 처음에 선택할 수 있게 하였다.

![image](https://user-images.githubusercontent.com/66288087/189130052-f3607f8e-d526-4f1a-ba79-7b507a2708b2.png)

그림과 같이 좌, 우에 있는 선택 세이브 포인트를 먹게 되면 앞으로 갈 수 있는 발판이 생겨남과 동시에, 돌아갈 수 있는 문이 막히게 된다.

이렇게 스테이지 구분을 해 놓은 이유는 좌, 우에서 얻을 수 있는 최대 점수를 같게 설계하였기 때문이다.

Player.cs 에서 세이브 포인트 등에 닿았을 때 OnTriggerEnter 이벤트 트리거를 사용하였다.

이번에도 마찬가지이다.

<pre>
<code>
private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "chooseRoute")
        {
            ReturnPos = other.gameObject.transform.position;
            choose = other.gameObject.GetComponent<ChooseRoute>();
            choose.onWall();
            other.gameObject.SetActive(false); // 세이브 포인트를 먹었으니 비활성화
        }


    }


</code>
</pre>

코드의 일부이다. 여기서는 선택 세이브 포인트에서도 코드를 하나 넣어 주어야 한다. 왜냐하면 왼쪽 것을 먹었느냐, 오른쪽 것을 먹었느냐에 따라서 활성화 해야 하는 벽이 다르기 때문이다. (둘 다 활성화 시키면 되긴 하겠구나... 이 것을 적으면서 깨달았다...)

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRoute : MonoBehaviour
{
    public GameObject Wall;
    public GameObject ActiveBase;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onWall()
    {
        ActiveBase.SetActive(true);
        Wall.SetActive(true);
    }

}
</code>
</pre>

ActiveBase는 앞으로 나아갈 수 있는 발판이고, Wall은 뒤로 가는 길을 막을 벽이다.

여기서 적은 onWall()을 세이브 포인트 비활성화 전에 써 주어야 한다. (당연한 것이지만 한 번 실수한 적이 있다... ㅠ) 

<hr>

**왼쪽 길 구성**

왼쪽 길은 컨트롤 요소적인 것에 치중할 수 있게 기믹 발판은 바운스 발판으로 최소화 하였다.


<hr>

**오른쪽 길 구성**

오른쪽 길은 다양한 기믹 발판들을 만들어 두어, 방해 공작을 뚫고 골인 지점으로 나아가는 것이 목표이다.

따라서 천천히 멈추고 기믹 패턴을 파악하는 것이 중요하다.

새로 만든 기믹 패턴들을 소개하겠다.

1. 미는 장애물

![jump_2_push](https://user-images.githubusercontent.com/66288087/189132237-70ab108f-0025-4b5b-9083-69e04d9f71e8.gif)

말 그대로 미는 장애물이다.

플레이어가 지나갈 때 발판 밖으로 밀어 떨어트리는 역할을 한다.


2. 돌림 장애물

![jump_2_rotate](https://user-images.githubusercontent.com/66288087/189133123-cf77e69d-6b17-4f7d-8315-24a7c69c1f11.gif)

회전문같이 뱅글뱅글 도는 장애물이다.

원래 도는 막대의 높이를 좀 더 높이려 하였지만 난이도가 너무 높아지고, 골드도 배치할 것이기에 롤백하였다.

![image](https://user-images.githubusercontent.com/66288087/189133940-1b66766f-367a-4dde-afa2-b7fe4d1ecbae.png)

이러한.. 배치도 있기 때문이다.

물론, 오른쪽 길에는 초행길에 자주 떨어질 수 있기 때문에 세이브 포인트 배치 빈도를 높였다.


3. 커졌다 작아졌다 ver1,2

![jump_3_resize](https://user-images.githubusercontent.com/66288087/189134825-2933d80a-604a-4262-8b47-9afe9a4f35b2.gif)

커졌다 작아졌다를 반복하며, 발판 위를 점거하는 장애물이다.

타이밍이 다른 두 가지 버전이 있다.

스테이지 3에서도 쏠쏠하게 사용 할 예정이다.

<hr>

코인 배치까지 마치게 된 스테이지 전경은 아래와 같다.

![image](https://user-images.githubusercontent.com/66288087/189333844-179cfe70-488f-4664-8051-0c0604146817.png)

마지막 파이프 부분에서 파이프 바닥쪽 tag를 base가 아니게 바꿔 줌으로써 역으로 올라와 반대쪽 방향으로 갈 수 없게 만들었다.

<hr>

**스테이지 1 -> 2 이동 시 점수 등의 정보 저장**

이제 스테이지를 변경할 때도 누적으로 점수가 저장 되어야 한다.

아까 **DontDestroyOnLoad(gameObject);** 를 이용한다고 했었다.

이 곳에 오브젝트를 넣고 누적 점수를 저장하는 SaveInformation.cs 코드를 작성 해 보도록 하자.


<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInformation : MonoBehaviour
{

    int currentStage = 1; // 현재 스테이지를 나타낸다.

    int firstScore, secondScore, thirdScore; // 스테이지 1,2,3 스코어
    int totalScore; // 전 스테이지 통합 스코어

    int cntScore;

    int fallCountone, fallCounttwo, fallCountthree; // 스테이지 1,2,3에서 떨어진 횟수
    int totalFallCount; // 전 스테이지 통합 떨어진 횟수

    // Start is called before the first frame update

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 사라지지 않게한다.
        Debug.Log("SaveBase");
    }

    void Start()
    {
        cntScore = 0;
        totalScore = 0; // 맨 처음에 0으로 시작
        Debug.Log("New Stage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SumScore()
    {
        // 누적 점수에 현재 스테이지 점수를 더한다.
        totalScore += cntScore;
        Debug.Log("점수 합산 완료, 총 점수 : " + totalScore);

    }

    public void addCntScore(int add)
    {
        cntScore += add;

        switch (currentStage)
        {
            case 1:
                firstScore += add;
                break;
            case 2:
                secondScore += add;
                break;
            case 3:
                thirdScore += add;
                break;

        }
    }

    public int GetStage()
    {
        return currentStage;
    }

    public void clearCntScore()
    {
        // 새 스테이지로 이동했을 때, 현재 점수를 초기화 한다.
        cntScore = 0;
    }
    
    public void stageUp()
    {
        if (currentStage == 3) // 3스테이지를 클리어하면
            currentStage = 0; // 0으로 초기화 하고
        else // 그렇지 않으면
            currentStage++; // 스테이지를 더한다.
    }

}
</code>
</pre>

현재가 몇 스테이지인지 나타내어 주는 currentStage가 있고, 그것을 통하여 각 스테이지별 점수를 저장 해 준다.

그리고 스테이지를 넘어갈 때, 이번 스테이지에서 얻은 점수를 더해 주는 SumScore()함수도 하나 만들어 두었다.

그리고 점수를 더해 준 다음, 현재 스코어를 초기화 해 주는 clearCntScore()도 하나 만들어 주었다.

<details>
<summary>SumScore() 마지막 부분에 현재 점수 초기화 부분을 왜 추가하지 않았는가?</summary>

<br>
그런데, SumScore() 마지막 부분에 clearCntScore()의 내용을 넣어 주면 안되냐는 말도 있을 것이다.

그렇지만 점수를 더하고 초기화 하는 것을 한 곳에 묶어 놓으면 스테이지를 넘어 갈 때만 쓰이게 된다.

스테이지를 넘어가지 않더라도 중간 점수를 더해 주는 것이 필요한 경우가 생기면 쓸 수도 있다.

</details>

이렇게 만들어 주고, Player에서 goal tag에 닿는 부분을 수정 해 준다.

<pre>
<code>
if(other.gameObject.tag == "goal")
{
    switch (saveInfo.GetStage())
    {
        case 1:
            saveInfo.SumScore(); // 현재 스테이지 점수를 합산
            saveInfo.stageUp();
            saveInfo.clearCntScore(); // 현재 스테이지 점수 초기화!(새 스테이지로 가기 때문)
            SceneManager.LoadScene("Jump_2");
            break;
        case 2:
            saveInfo.SumScore(); // 현재 스테이지 점수를 합산
            saveInfo.stageUp();
            saveInfo.clearCntScore(); // 현재 스테이지 점수 초기화!(새 스테이지로 가기 때문)
            // SceneManager.LoadScene("Jump_3"); // 이건 아직 없지만.. 추가 해 준다.
            break;
    }


}
</code>
</pre>

이렇게 해 주고, 1스테이지~2스테이지를 클리어 해 보자

![image](https://user-images.githubusercontent.com/66288087/189487097-2044c61c-c707-4a3f-a831-3ae9e421ed49.png)

위 사진과 같이 나오게 될 것이다.
