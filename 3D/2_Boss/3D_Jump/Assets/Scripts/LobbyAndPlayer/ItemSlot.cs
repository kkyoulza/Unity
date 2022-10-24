using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject itemNotice; // 아이템 정보들이 적혀있는 객체
    public Text info;
    public Vector3 pos;
    public int value;
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        itemNotice.transform.position = transform.position + pos;
        switch (value)
        {
            case -1:
                info.text = "골드\n\n이 세계에서 사용 되는 화폐 단위\n강화, 상점 구매 등에 이용 가능\n점프 맵, 던전 클리어, 광물 파밍을 통해 수급 가능";
                break;
            case 2000:
                info.text = "기원 조각\n\n강화 성공을 기원하며 생기게 된 돌\n1 개당 강화 확률 0.5%p 상승\n(100%까지 상승 가능)";
                break;
            case 2001:
                info.text = "HP 포션\n\nHP를 회복시키는 포션\nHP + 30";
                break;
            case 2002:
                info.text = "MP 포션\n\nMP를 회복시키는 포션\nMP + 10";
                break;
            case 10000:
                info.text = "공격력\n\n데미지에 영향을 미치는 수치\n명중률이 높을수록 일정한 데미지가 나온다.";
                break;
            case 10001:
                info.text = "힘(Strength)\n\n공격력의 기준이 되는 수치\n올릴 때 마다 공격력 + 1";
                break;
            case 10002:
                info.text = "명중률(Accuracy)\n\n온전한 공격을 하기 위한 수치\n명중률이 낮을 수록 공격력에 비해 낮은 데미지가 뜰 확률이 높다.\n 올릴 때 마다 명중률 + 1%p";
                break;
            case 10003:
                info.text = "체력(Health Point)\n\n캐릭터의 체력을 결정하는 수치\n체력이 0이 되면 게임 오버가 된다.\n올릴 때 마다 HP + 10";
                break;
            case 10004:
                info.text = "마력(Magic Point)\n\n스킬을 사용하기 위해 지불해야 하는 코스트\n올릴 때 마다 MP + 5";
                break;
        }
        
        itemNotice.SetActive(true);
        Debug.Log("onMouse");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemNotice.SetActive(false);
        Debug.Log("exitMouse");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
