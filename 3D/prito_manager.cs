using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

public class Prito_1 : MonoBehaviour
{
    List<Image> currentPoints = new List<Image>();
    List<string> pointString = new List<string>(); // 방향을 string 형태로 기록

    public Button addBtn;

    bool StageOn = false; // stage 세팅 후 true로 on

    public Image UpPoint;
    public Image DownPoint;
    public Image LeftPoint;
    public Image RightPoint;

    public GameObject UIBase;

    int count = 0;
    int RandomInt;
    Vector3 PointPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(StageOn == true && currentPoints.Count > 0) // 스테이지가 열리고, 화살표가 남아있을 때,
        {

            for(int i = 0; i < currentPoints.Count; i++)
            {

                if(Input.GetKey(KeyCode.UpArrow))
                {
                    if (pointString[i] == "up")
                    {
                        Destroy(currentPoints[i]);
                     
                        if (i == currentPoints.Count - 1)
                        {
                            currentPoints.Clear();
                            pointString.Clear(); // string 버전도 다 지우기
                            StageOn = false;
                            Debug.Log("스테이지 종료!");
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("방향키를 잘못 입력했습니다!");
                    }
                }
                else if(Input.GetKey(KeyCode.DownArrow))
                {
                    if (pointString[i] == "down")
                    {
                        Destroy(currentPoints[i]);

                        if (i == currentPoints.Count - 1)
                        {
                            currentPoints.Clear();
                            pointString.Clear(); // string 버전도 다 지우기
                            StageOn = false;
                            Debug.Log("스테이지 종료!");
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("방향키를 잘못 입력했습니다!");
                    }
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    if (pointString[i] == "left")
                    {
                        Destroy(currentPoints[i]);

                        if (i == currentPoints.Count - 1)
                        {
                            currentPoints.Clear();
                            pointString.Clear(); // string 버전도 다 지우기
                            StageOn = false;
                            Debug.Log("스테이지 종료!");
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("방향키를 잘못 입력했습니다!");
                    }
                }
                else if(Input.GetKey(KeyCode.RightArrow))
                {
                    if (pointString[i] == "right")
                    {
                        Destroy(currentPoints[i]);

                        if (i == currentPoints.Count - 1)
                        {
                            currentPoints.Clear();
                            pointString.Clear(); // string 버전도 다 지우기
                            StageOn = false;
                            Debug.Log("스테이지 종료!");
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("방향키를 잘못 입력했습니다!");
                    }

                }

                Thread.Sleep(500);

            }

        }

    }

    public void SetStage1()
    {
        count = 0;
        for (int i = 0; i < 3; i++)
        {
            PointPos = new Vector3(310 + count * 60, 320, 0);
            AddPoint(PointPos);
        }

        count = 0;

        if (StageOn == false)
            StageOn = true;

    }

    public void SetStage2()
    {
        count = 0;
        for (int i = 0; i < 5; i++)
        {
            PointPos = new Vector3(250 + count * 60, 320, 0);
            AddPoint(PointPos);
        }

        count = 0;

        if (StageOn == false)
            StageOn = true;

    }

    public void SetStage3()
    {

    }

    public void SetStage4()
    {

    }

    public void AddPoint(Vector3 StartPos)
    {

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        RandomInt = UnityEngine.Random.Range(0, 4);
        Debug.Log(RandomInt);

        if (RandomInt == 0)
        {
            Image up = Instantiate(UpPoint);
            up.transform.SetParent(UIBase.transform);

            up.transform.position = StartPos;
            currentPoints.Add(up);
            pointString.Add("up");
            count++;
        }
        else if(RandomInt == 1)
        {
            Image down = Instantiate(DownPoint);
            down.transform.SetParent(UIBase.transform);

            down.transform.position = StartPos;
            currentPoints.Add(down);
            pointString.Add("down");
            count++;
        }
        else if (RandomInt == 2)
        {
            Image left = Instantiate(LeftPoint);
            left.transform.SetParent(UIBase.transform);

            left.transform.position = StartPos;
            currentPoints.Add(left);
            pointString.Add("left");
            count++;
        }
        else
        {
            Image right = Instantiate(RightPoint);
            right.transform.SetParent(UIBase.transform);

            right.transform.position = StartPos;
            currentPoints.Add(right);
            pointString.Add("right");
            count++;
        }

    }

    public void DeleteStage()
    {
        if(currentPoints.Count > 0)
        {
            foreach (Image cntImg in currentPoints)
            {
                Debug.Log(cntImg);
                Destroy(cntImg);
            }
            currentPoints.Clear();
            pointString.Clear(); // string 버전도 다 지우기
        }
        else
        {
            Debug.Log("생성된 Point가 없습니다.");
        }

    }

}
