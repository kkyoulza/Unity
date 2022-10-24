using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBtnE : MonoBehaviour
{
    // 정보 관련
    public int itemNum;
    public GameObject player;
    PlayerItem playerItem;

    //UI 관련
    Image img;
    Color color;
    Button btn;

    private void Awake()
    {
        playerItem = player.GetComponent<PlayerItem>();
        img = GetComponent<Image>();
        btn = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckWeaponExist()) // 만약 무기를 습득하지 못했다면
        {
            color.a = 0.4f;
            color.r = 1.0f;
            color.g = 1.0f;
            color.b = 1.0f;
            img.color = color; // 해당 버튼의 투명도를 낮추고
            btn.enabled = false; // 버튼 기능 비활성화
        }
        else // 무기를 습득했다면
        {
            color.a = 1.0f;
            color.r = 1.0f;
            color.g = 1.0f;
            color.b = 1.0f;
            img.color = color; // 투명도 원상복구
            btn.enabled = true; // 버튼 기능 활성화

        }
    }

    public bool CheckWeaponExist() // 이것은 UI 매니저에 옮기고 매개변수를 만들어서 이벤트 시작을 버튼마다 다른 매개변수로 하게끔 하자.
    {
        if(playerItem.weapons[0].baseAtk == 0) // 첫 번째 원소의 공격력이 0이라는 것은 아무것도 얻지 못했다는 것
        {
            return false; // false
        }
        for (int i = 0; i < playerItem.weapons.Length; i++)
        {
            if (playerItem.weapons[i] == null)
                continue;
            if (playerItem.weapons[i].weaponCode == itemNum)
            {
                return true; // 버튼에 맞는 무기를 먹었다면 true
            }
        }

        return false; // 그렇지 않다면 false
    }


}
