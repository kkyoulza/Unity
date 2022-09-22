using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolTimeManager : MonoBehaviour
{
    public Text CoolText;
    public bool coolOn;
    float coolTime;
    int cool;

    // Start is called before the first frame update
    void Start()
    {
        coolOn = true;
        SetAble();
    }

    // Update is called once per frame
    void Update()
    {
        if(coolOn == false)
        {
            coolUpdate();
        }
        
    }

    public void SetCoolTime(float coolTime)
    {
        this.coolTime = coolTime;
        CoolText.text = coolTime.ToString();
    }

    void coolUpdate()
    {
        this.coolTime = this.coolTime - Time.deltaTime;
        cool = (int)coolTime;
        CoolText.text = cool.ToString();

    }

    public void SetAble()
    {
        CoolText.text = "사용 가능";
    }

}
