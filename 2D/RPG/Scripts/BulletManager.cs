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

    public void discountBullet()
    {
        this.remainedBullet -= 1; // 총알 소비

        Destroy(GameObject.Find(BulletImgName[remainedBullet])); // 총알을 한 발 까고 나면 리스트에서 가장 밖에 있는 원소의 위치에 갈 수 있다.

        if(remainedBullet == 0)
        {
            BulletImgName.Clear();
        }

    }

    public void AllDelete()
    {
        // 남은 총알들을 다 지운다.

        for(int i = 0; i < BulletImgName.Count; i++)
        {
            try
            {
                Destroy(GameObject.Find(BulletImgName[i]));
            }
            catch
            {
                continue;
            }

        }

        BulletImgName.Clear();

    }

    public int GetBulletCount()
    {
        return this.remainedBullet;
    }

    public void SetBulletUI()
    {
        for(int i = 0; i < remainedBullet; i++)
        {
            Vector3 offSet = UIBase.transform.position + new Vector3((-22) + i * (-40), 47, 0); // 나중에 생성된 총알일수록 안쪽에 생성되게끔!

            Image BulletImsi = Instantiate(BulletImg);
            BulletImsi.transform.SetParent(UIBase.transform,false); // UI 팔레트를 부모 객체로 설정해 놓는다.
            BulletImsi.name = "BulImge" + i;
            BulletImsi.transform.position = offSet;

            BulletImgName.Add(BulletImsi.name); // 이름을 저장 해 놨다가 역순으로 Destroy 해 준다.(총알 소비 시)

        }
    }

}
