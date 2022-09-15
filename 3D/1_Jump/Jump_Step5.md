# Step5. 스테이지 3 구성

![image](https://user-images.githubusercontent.com/66288087/189924590-d2d9b5a7-5adc-47bc-b1e6-6f842e05592e.png)

스테이지 3의 컨셉은 한 바퀴를 도는 컨셉이다. 점프맵에서 카메라 구도를 바꿀 수 없게 설정해 놓아 뒤에서 앞으로 오는 맵을 만드는 것에는 제한이 있다.

따라서 앞으로 올 때는 떨어질 위험이 없을 수 있게끔 미끄럼틀과 비슷한 개념으로 설계하였다.

<hr>

**추가 된 발판들**

1. 페이크 발판

![image](https://user-images.githubusercontent.com/66288087/189924953-26e805b7-22b7-4eec-bc04-f26e4dd14234.png)

말 그대로 딛을 수 없는 발판이다.

대신 색을 딛을 수 있는 발판보다 진하게 설정하여 구분할 수 있게 하였다.

한 번 빠지면 웬만해서는 빠지지 않는다.


2. 뱅뱅이(?)

![jump_3_1](https://user-images.githubusercontent.com/66288087/189925713-46fc7219-b1b9-47a4-a386-70d162468443.gif)

뱅글뱅글 돌면서 진로를 막는 발판이다. 도는 속도가 빨라 은근히 뚫기 어렵다.


3. 뱅뱅발판

![jump_3_2](https://user-images.githubusercontent.com/66288087/189926650-6195dcf7-ec34-4d12-9280-9a9b1bc365a5.gif)

뱅글뱅글 도는 발판이다. 타이밍에 맞게 발판을 밟고 다음 발판으로 뛰어 주어야 한다.

생각보다 타이밍을 잡기 어렵다. 예측하여 점프를 해야 한다.


4. 자동 시소 발판

![jump_3_3](https://user-images.githubusercontent.com/66288087/189927928-4670c426-f6d7-4ab0-837a-b4e8008f1348.gif)

자동으로 시소처럼 움직이는 발판이다.

이 발판에는 마찰력을 0으로 설정 해 두어, 최대한 잘 미끄러지게 설정하여 난이도를 올렸다.

옆으로 뛰는 것이 난이도가 있기에, 뛰기 전 발판에 벽을 설치해 둘 예정이다.


<hr>


**파일로 점수 저장**

점수를 바이너리 파일의 형태로 저장 할 예정이다.

쓰임새는 개인 최고 점수를 볼 수 있는 게시판 등을 제작 할 때 사용할 예정이다.

이에 SaveInformation.cs의 코드에도 변화를 주었다.

<pre>
<code>
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

[System.Serializable]
public class Info{

    public int firstScore, secondScore, thirdScore;
    public int totalScore;

    public int cntScore;

    public int fallCountone, fallCounttwo, fallCountthree; // 스테이지 1,2,3에서 떨어진 횟수
    public int totalFallCount; // 전 스테이지 통합 떨어진 횟수


    public Info(int cnt, int total)
    {
        this.cntScore = cnt;
        this.totalFallCount = total;
    }

}
</code>
</pre>

우선 정보를 저장하는 변수들을 따로 클래스를 만들어 넣어 주었다.

그리고 바이너리 파일로 저장할 수 있게 System.IO와 System.Runtime.Serialization.Formatters.Binary를 사용하였다.

또한, 클래스에 [Serializable] 어트리뷰트를 지정한다.

<pre>
<code>
public class SaveInformation : MonoBehaviour
{
    Info info = new Info(0, 0); // 맨 처음에 0으로 시작

    int currentStage = 1; // 현재 스테이지를 나타낸다.

    // Start is called before the first frame update

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 사라지지 않게한다.
        Debug.Log("SaveBase");
    }

    void Start()
    {
        Debug.Log("New Stage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SumScore()
    {
        // 누적 점수에 현재 스테이지 점수를 더한다.
        info.totalScore += info.cntScore;
        Debug.Log("점수 합산 완료, 총 점수 : " + info.totalScore);

    }

    public void addCntScore(int add)
    {
        info.cntScore += add;

        switch (currentStage)
        {
            case 1:
                info.firstScore += add;
                break;
            case 2:
                info.secondScore += add;
                break;
            case 3:
                info.thirdScore += add;
                break;

        }
    }

    public int GetStage()
    {
        return currentStage;
    }

    public int GetCntScore()
    {
        return info.cntScore;
    }

    public int GetTotalScore()
    {
        return info.totalScore;
    }

    public void clearCntScore()
    {
        // 새 스테이지로 이동했을 때, 현재 점수를 초기화 한다.
        info.cntScore = 0;
    }

    public void stageUp()
    {
        if (currentStage == 3) // 3스테이지를 클리어하면
            currentStage = 1; // 1로 초기화 하고
        else // 그렇지 않으면
            currentStage++; // 스테이지를 더한다.
    }

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
            Info info = formatter.Deserialize(fs) as Info; // 역 직렬화 후, 클래스 형태에 맞는 객체에 다시 저장

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

}
</code>
</pre>

파일로 저장하는 함수와, 파일에서 내용을 불러오는 함수를 추가해 주었으며, Info 객체를 만들어서 그 안에 값을 저장 해 주는 과정으로 바꾸어 주었다.

그리고 player.cs 코드 중 일부를 바꾸어 준다.

<pre>
<code>
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
                SceneManager.LoadScene("Lobby");
                break;
        }
    }
</code>
</pre>

스테이지 3에서 골인지점에 도달했을 때, 파일을 저장한 다음 로비로 넘어가게 만들어 주었다.

![image](https://user-images.githubusercontent.com/66288087/190379726-86a078fd-4250-4bab-a1a3-24ac1846d88a.png)

파일이 저장된 모습이다.

<hr>

**로비**

![image](https://user-images.githubusercontent.com/66288087/190380872-b83f17b6-ba95-4c36-8a0a-219c70dab4a0.png)

로비의 초기 모습이다.

앞으로 개발 할 보스 전투에서도 사용할 새로운 플레이어도 같이 배치 하였다.

일단 1단계 개발 부분은 여기까지이다.
