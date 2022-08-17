using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int TargetNum = 0; // ¸ÂÃçÁø Å¸°ÙÀÇ Á¾·ù!
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
