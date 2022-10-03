using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowItemState : MonoBehaviour
{
    int index;
    public int itemCode;
    public GameObject player;
    PlayerItem playerItem;  // 플레이어 아이템 정보
    WeaponItemInfo weaponInfo; // 무기 세부 정보

    public GameObject itemInfoUI; // 아이템 정보 UI 틀
    public Sprite item;
    public Image itemImg;
    public Text itemName;
    public Text itemEnchantCount;
    public Text itemAtk;
    public Text itemDelay;
    public Text itemCritical;


    // Start is called before the first frame update
    void Start()
    {
        playerItem = player.GetComponent<PlayerItem>();
        itemImg.sprite = item;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void findIndex()
    {
        for(int i = 0; i < playerItem.weapons.Length; i++)
        {
            if (playerItem.weapons[i] == null)
                break;
        }
    }

    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        itemInfoUI.SetActive(false);
    }
}
