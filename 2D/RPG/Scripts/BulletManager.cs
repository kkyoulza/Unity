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
            Debug.Log("�Ѿ��� �� �������� �� ������ �� �ֽ��ϴ�.");

        }

    }

    public void discountBullet()
    {
        this.remainedBullet -= 1; // �Ѿ� �Һ�

        Destroy(GameObject.Find(BulletImgName[remainedBullet])); // �Ѿ��� �� �� ��� ���� ����Ʈ���� ���� �ۿ� �ִ� ������ ��ġ�� �� �� �ִ�.

        if(remainedBullet == 0)
        {
            BulletImgName.Clear();
        }

    }

    public void AllDelete()
    {
        // ���� �Ѿ˵��� �� �����.

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
            Vector3 offSet = UIBase.transform.position + new Vector3((-22) + i * (-40), 47, 0); // ���߿� ������ �Ѿ��ϼ��� ���ʿ� �����ǰԲ�!

            Image BulletImsi = Instantiate(BulletImg);
            BulletImsi.transform.SetParent(UIBase.transform,false); // UI �ȷ�Ʈ�� �θ� ��ü�� ������ ���´�.
            BulletImsi.name = "BulImge" + i;
            BulletImsi.transform.position = offSet;

            BulletImgName.Add(BulletImsi.name); // �̸��� ���� �� ���ٰ� �������� Destroy �� �ش�.(�Ѿ� �Һ� ��)

        }
    }

}
