using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletManager : MonoBehaviour
{
    int remainedBullet = 0;
    public Text bulletText;

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
        this.remainedBullet = count;
    }

    public void discountBullet(int count)
    {
        this.remainedBullet -= count;
    }

    public int GetBulletCount()
    {
        return this.remainedBullet;
    }

}
