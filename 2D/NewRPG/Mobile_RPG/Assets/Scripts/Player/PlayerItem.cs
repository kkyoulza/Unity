using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class itemInfo
{
    Item.ItemType itemType;

    public int itemCode;
    public int itemCnt;
    public bool isIntergrated; // ���� ������ �����Ѱ�?(���� ��� ��Ÿ �������� ���ļ� �����ϴ� ��ó��!)
    Sprite itemImg; // ������ �̹���

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
