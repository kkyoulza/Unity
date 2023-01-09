using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class itemInfo
{
    Item.ItemType itemType;

    public int itemCode;
    public int itemCnt;
    public bool isIntergrated; // 통합 보관이 가능한가?(예를 들어 기타 아이템을 겹쳐서 보관하는 것처럼!)
    Sprite itemImg; // 아이템 이미지

}
public class PlayerItem : MonoBehaviour
{
    itemInfo[] itemList;

    public int cntGold = 0;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addGold(int num)
    {
        cntGold += num;
    }

}
