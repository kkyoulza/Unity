# Step 11. 스코어 파일 저장 및 랭킹 시스템 구현(개인) + 게임 정보 저장 & 로드

<hr>

### 점프맵 도중 포기 기능 & 점수 파일 저장

지금까지는 점프맵을 끝까지 달려 완주해야만 골드 보상을 받고 빠져나올 수 있었다. 그렇지만 완주를 하기 위해서는 시간이 너무 걸리게 된다. 따라서 점프 맵 중간에도 지금까지 받은 점수대로 골드 정산을 받고 나갈 수 있게 하였다. (물론 정산 비율은 10분의 1이다.)

![image](https://user-images.githubusercontent.com/66288087/204979433-0e3a1bd2-8a07-4c04-b9e1-c23ec376c792.png)

스테이지 2,3에서는 1에서 만든 버튼과 Panel을 복사하여 붙여넣기 해 주었다.

(EventSystem도 같이 복사 해 준다.)

수정된 Managing.cs와 Player.cs이다.

**Managing.cs**
<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Managing : MonoBehaviour
{
    GameObject player; // 플레이어 오브젝트
    GameObject saveObject; // 정보 저장 오브젝트
    public GameObject exitUI; // exit UI
    public Text ScoreTxt; // scoreTxt;
    SaveInfos savestats; // playerstat

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
        savestats = GameObject.FindGameObjectWithTag("saveInfo").GetComponent<SaveInfos>();

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
                saveInfo.info.totalScore += 1;
                scoreText.text = score.ToString();
                break;
            case 1: // gold
                score = int.Parse(scoreText.text);
                score += 10;
                saveInfo.addCntScore(10); // 점수 관리 오브젝트 갱신
                saveInfo.info.totalScore += 10;
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
        noticeText.text = "스테이지 점수 : " + saveInfo.GetCntScore()+"\n 누적 점수 : "+saveInfo.GetTotalScore();
    }

    public void StageClearGainGoldUI()
    {
        panel.SetActive(true);
        noticeText.text = "전 스테이지 클리어 완료!\n 완주 시, 획득 골드의 100배 만큼 골드를 획득합니다!\n 획득 골드 : "+saveInfo.info.totalScore * 100+"G";
    }

    public void onExitUI()
    {
        if (!exitUI.activeSelf)
        {
            ScoreTxt.text = saveInfo.info.totalScore.ToString() + " (획득 골드 " + saveInfo.info.totalScore * 10 + ")";
            exitUI.SetActive(true);
        }
        else
        {
            exitUI.SetActive(false);
        }
    }

    public void doExit()
    {
        saveInfo.SaveInfoToFile();
        if (saveInfo.topScore[2] < saveInfo.info.totalScore) // 항상 정렬을 하니 마지막 원소와 비교 해 준다.
        {
            saveInfo.topScore[2] = saveInfo.info.totalScore;
            Array.Sort(saveInfo.topScore);
            Array.Reverse(saveInfo.topScore);
        }
        savestats.info.playerCntGold += saveInfo.info.totalScore * 10;
        saveInfo.info.reset();
        SceneManager.LoadScene("Boss1");
    }

}
</code>
</pre>

**Player.cs**
<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    GameObject manager;
    GameObject saveObject;
    public GameObject startPos; // 시작지점

    SaveInformation saveInfo;
    ChooseRoute choose;
    Managing managing;
    Rigidbody rigid;

    SaveInfos saveStats;

    bool isJumpState = false;
    bool dontMove = false;
    
    Vector3 ReturnPos; // 세이브 포인트를 먹지 않았을 때
    float jumpForce = 60.0f;

    int showNotice = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        manager = GameObject.FindGameObjectWithTag("Manager");
        saveObject = GameObject.FindGameObjectWithTag("information");
      
        ReturnPos = startPos.transform.position;
        managing = manager.GetComponent<Managing>();
        saveInfo = saveObject.GetComponent<SaveInformation>();
        saveStats = GameObject.FindGameObjectWithTag("saveInfo").GetComponent<SaveInfos>();

    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !isJumpState)
        {
            isJumpState = true;
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float h = dontMove ? 0f : Input.GetAxisRaw("Horizontal");
        float v = dontMove ? 0f : Input.GetAxisRaw("Vertical");

        rigid.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "base")
        {
            isJumpState = false;
        }
        else if(collision.gameObject.tag == "under")
        {
            managing.ShowFallNotice();
            rigid.velocity = Vector3.zero;
            managing.MoveToTarget(ReturnPos);

        }

    }

    void gotoNextMap()
    {
        saveInfo.clearCntScore(); // 현재 스테이지 점수 초기화!(새 스테이지로 가기 때문) - 점수를 내보낸 이후로 초기화를 시켜 주어야 한다.
        dontMove = false;

        switch (saveInfo.GetStage())
        {
            case 1:
                saveInfo.stageUp(); // 이곳으로 옮겨 주어야 한 번에 두 스테이지를 건너 뛰는 것을 방지할 수 있음
                SceneManager.LoadScene("Jump_2");
                break;
            case 2:
                saveInfo.stageUp();
                SceneManager.LoadScene("Jump_3");
                break;
            case 3:
                saveInfo.stageUp();
                saveInfo.SaveInfoToFile();
                if (saveInfo.topScore[2] < saveInfo.info.totalScore)
                {
                    saveInfo.topScore[2] = saveInfo.info.totalScore;
                    Array.Sort(saveInfo.topScore);
                    Array.Reverse(saveInfo.topScore);
                }
                
                saveStats.info.playerCntGold += saveInfo.info.totalScore * 100;
                saveInfo.info.reset(); // 점수 리셋
                SceneManager.LoadScene("Boss1");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SavePoint")
        {
            ReturnPos = other.gameObject.transform.position;
            other.gameObject.SetActive(false); // 세이브 포인트를 먹었으니 비활성화
        }

        if(other.gameObject.tag == "gold")
        {
            managing.addScore(1);
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "silver")
        {
            managing.addScore(0);
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "chooseRoute")
        {
            ReturnPos = other.gameObject.transform.position;
            choose = other.gameObject.GetComponent<ChooseRoute>();
            choose.onWall();
            other.gameObject.SetActive(false); // 세이브 포인트를 먹었으니 비활성화
        }


        if (other.gameObject.tag == "goal")
        {
            rigid.velocity = Vector3.zero;
            dontMove = true;
            // saveInfo.SumScore(); // 현재 스테이지 점수를 합산
            if(saveInfo.GetStage() == 3)
            {
                managing.StageClearGainGoldUI();
            }
            else
            {
                managing.StageClearUI();
            }

            Invoke("gotoNextMap",2.0f);

        }

        if(other.gameObject.tag == "question")
        {
            switch (saveInfo.GetStage())
            {
                case 2:
                    managing.ShowInfoStage2Plus();                
                    break;

            }

        }


        if (other.gameObject.tag == "Notice")
        {
            if(other.gameObject.name == "1" && showNotice < 1)
            {
                managing.ShowNotices(1);
                showNotice++;
            }
            else if(other.gameObject.name == "2" && showNotice < 2)
            {
                managing.ShowNotices(2);
                showNotice++;
            }
            else if (other.gameObject.name == "3" && showNotice < 3)
            {
                managing.ShowNotices(3);
                showNotice++;
            }
            else if (other.gameObject.name == "4" && showNotice < 4)
            {
                managing.ShowNotices(4);
                showNotice++;
            }
            else if (other.gameObject.name == "5" && showNotice < 5)
            {
                managing.ShowNotices(5);
                showNotice++;
            }
            else if (other.gameObject.name == "6" && showNotice < 6)
            {
                managing.ShowNotices(6);
                showNotice++;
            }
            else if (other.gameObject.name == "7" && showNotice < 7)
            {
                managing.ShowNotices(7);
                showNotice++;
            }

        }

    }

}
</code>
</pre>

Managing.cs에는 ExitUI를 on/off하는 함수와 ExitUI에서 Exit 버튼을 눌렀을 때, 점프맵 밖으로 완전히 나가게 하는 doExit()함수를 만들어 주었다.

Player.cs에서는 현재 점수를 실시간으로 더하기 위해 동전을 먹을 때, saveObject에 있는 cntScore를 더해주는 부분을 추가 해 주었다.

또한 topScore는 int 배열로 만들어 주어, top3의 점수까지 저장되어 랭킹에 표시할 수 있게 만들었다.

랭킹 UI를 아래와 같이 간단하게 만들어 주었다.

![image](https://user-images.githubusercontent.com/66288087/205025535-11b50fba-52d5-4deb-bdbc-7d6c1da0e3a1.png)

<hr>

### esc키를 통해 게임 종료하기

게임 종료 메뉴 UI를 아래 사진과 같이 만들어 주었다.

![image](https://user-images.githubusercontent.com/66288087/205262077-8634e0e3-e266-427b-a4fa-182ece68cc3d.png)

esc키를 입력 받고, esc키를 누르게 되면 위 사진과 같은 메뉴가 뜨면서 게임 내 시간이 멈추게 설정하였다.

시간을 일시정지 시키는 것은 [여기](https://bluemeta.tistory.com/3)를 참고하였다. (Time.timeScale 이용)

메뉴를 켤 때, Time.timeScale을 0으로 만들어 주어 시간이 멈추게 하였고, 메뉴를 끌 때는 Time.timeScale을 1.0f로 하여 다시 시간이 가게 하였다. 

![image](https://user-images.githubusercontent.com/66288087/205262613-cdb781af-b4fe-4ab8-bade-5c06b53e1ffa.png)

어둡게 설정한 것은 Panel을 하나 더 두어 설정하였다.

<hr>

### 게임 내용 저장

**점프 맵**

점프 맵의 점수들을 저장 해 주도록 해 보자. 파일로 저장시킨 다음, 게임을 시작할 때, 파일의 내용을 Load 하는 방식을 사용한다.

<pre>
<code>
 public void SaveInfoToFile()
{

    string fileName = "jumpScoreInfo";
    string path = Application.dataPath + "/" + fileName + ".dat";

    FileStream fs = new FileStream(path, FileMode.Create); // 파일 통로 생성
    BinaryFormatter formatter = new BinaryFormatter();
    formatter.Serialize(fs, info); // 직렬화 하여 저장

    Debug.Log("파일 저장 완료");

    fs.Close();


}

public void LoadInfoFile()
{
    string fileName = "jumpScoreInfo";
    string path = Application.dataPath + "/" + fileName + ".dat";

    if (File.Exists(path))
    {
        // 만약 파일이 존재하면

        FileStream fs = new FileStream(path, FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        Info infoImsi = formatter.Deserialize(fs) as Info; // 역 직렬화 후, 클래스 형태에 맞는 객체에 다시 저장

        info = infoImsi;

        Debug.Log("저장 된 현재 점수 : " + info.cntScore);
        Debug.Log("저장 된 누적 점수 : " + info.totalScore);

        fs.Close();
    }
    else
    {
        // 파일이 존재하지 않으면
        Debug.Log("파일이 존재하지 않음");
    }
}
</code>
</pre>

파일을 저장하고 불러오는 함수이다.

<pre>
<code>
private void Awake()
{
    GameObject[] objs = GameObject.FindGameObjectsWithTag("information"); // information Tag를 가진 놈들을 배열에 불러오고
    if (objs.Length > 1) // 만약 이미 전에 생성된 Obj가 있다면 배열의 길이는 2가 될 것이다.
        Destroy(gameObject); // DontDestroy로 지정된 것은 Awake가 다시 실행되지 않으므로 새로 생성되는 것만 삭제한다.
    DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 사라지지 않게한다.

    if (!isLoad)
    {
        LoadInfoFile();
        isLoad = true;
    }

    Debug.Log("SaveBase");
}
</code>
</pre>

SaveInformation.cs에서 Awake때 파일을 불러오게끔 해 준다.

![image](https://user-images.githubusercontent.com/66288087/205301904-f9d2cb01-bc49-4a97-ba60-1eefd29285d7.png)

그렇게 해 주면 시작하자마자 전에 했던 스코어가 바로 반영되어 있음을 볼 수 있다.

점프맵과 마찬가지로 Player에 대한 정보도 클래스 안에 넣어서 Serialize 하여 파일로 저장 해 주고, 불러와 준다.

**Player 정보들**









