using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

[System.Serializable]
public class Info{

    public int firstScore, secondScore, thirdScore;
    public int totalScore;

    public int cntScore;

    public int fallCountone, fallCounttwo, fallCountthree; // 스테이지 1,2,3에서 떨어진 횟수
    public int totalFallCount; // 전 스테이지 통합 떨어진 횟수

    public int[] topScore; // 탑3 스코어

    public Info(int cnt, int total)
    {
        this.cntScore = cnt;
        this.totalFallCount = total;
    }

    public void reset()
    {
        cntScore = 0;
        totalScore = 0;
        firstScore = 0;
        secondScore = 0;
        thirdScore = 0;
    }

}

public class SaveInformation : MonoBehaviour
{
    public Info info = new Info(0, 0); // 맨 처음에 0으로 시작
    int currentStage = 1; // 현재 스테이지를 나타낸다.

    public bool isLoad; // 맨 처음 파일에서 정보를 로드했는가?

    // Start is called before the first frame update

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

    void Start()
    {
        Debug.Log("New Stage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public void SumScore()
    {
        // 누적 점수에 현재 스테이지 점수를 더한다.
        info.totalScore += info.cntScore;
        Debug.Log("점수 합산 완료, 총 점수 : " + info.totalScore);

    }
    */

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


}
