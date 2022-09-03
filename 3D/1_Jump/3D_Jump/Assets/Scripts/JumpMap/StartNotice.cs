using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartNotice : MonoBehaviour
{
    public Text seconds;
    int remainedTime = 3;
    float delta = 0f;

    public GameObject startWalls;
    public GameObject startPanel;

    // Start is called before the first frame update
    // 3초 후 시작한다는 것을 알려 주는 매니저
    void Start()
    {
        seconds.text = remainedTime.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        delta += Time.deltaTime;
        remainedTime = 3 - (int)delta;
        seconds.text = remainedTime.ToString();

        if (delta >= 3f)
        {
            startWalls.SetActive(false);
            startPanel.SetActive(false);

        }

    }
}
