using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject itemNotice; // 아이템 정보들이 적혀있는 객체
    public PlayerItem playerItem; // 플레이어 무기 정보
    public PlayerCode playerCode; // 플레이어 스탯 정보
    public Text Name; // 무기 이름
    public Text Atk; // 무기 공격력
    public Text Delay; // 무기 딜레이

    // 아이템 설명이 들어 갈 내용
    public Text info;
    public Vector3 pos;
    public int value;

    public bool isUpBtn; // 스탯을 올려주는 버튼인가?
    public GameObject infoPanel; // 스탯 강화 시 사용되는 기원조각 비용 안내
    public Text statName;
    public Text useOriginTxt;

    string[] names = new string[] { "", "아다만티움 해머", "총", "머신 건" };

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUpBtn)
        {
            infoPanel.transform.position = transform.position + pos;
        }
        else
        {
            itemNotice.transform.position = transform.position + pos;
        }

        switch (value)
        {
            case -1:
                info.text = "골드\n\n이 세계에서 사용 되는 화폐 단위\n강화, 상점 구매 등에 이용 가능\n점프 맵, 던전 클리어, 광물 파밍을 통해 수급 가능";
                break;
            case 1:

                for(int i = 0; i < playerItem.weapons.Length; i++)
                {
                    if (playerItem.weapons[i] == null || playerItem.weapons[0].baseAtk == 0)
                    {
                        Name.text = "미습득";
                        Atk.text = "-";
                        Delay.text = "-";
                        break;
                    }

                    if (value == playerItem.weapons[i].weaponCode)
                    {
                        Name.text = names[value] + " ( +" + playerItem.weapons[i].enchantCount + " )";
                        Atk.text = (playerItem.weapons[i].baseAtk + playerItem.weapons[i].enchantAtk).ToString();
                        Delay.text = (playerItem.weapons[i].baseDelay + playerItem.weapons[i].enchantDelay).ToString();
                        break;
                    }
                    
                    
                } 
                break;
            case 2:

                for (int i = 0; i < playerItem.weapons.Length; i++)
                {
                    if (playerItem.weapons[i] == null || playerItem.weapons[0].baseAtk == 0)
                    {
                        Name.text = "미습득";
                        Atk.text = "-";
                        Delay.text = "-";
                        break;
                    }

                    if (value == playerItem.weapons[i].weaponCode)
                    {
                        Name.text = names[value] + " ( +" + playerItem.weapons[i].enchantCount + " )";
                        Atk.text = (playerItem.weapons[i].baseAtk + playerItem.weapons[i].enchantAtk).ToString();
                        Delay.text = (playerItem.weapons[i].baseDelay + playerItem.weapons[i].enchantDelay).ToString();
                        break;
                    }

                    
                }
                break;
            case 3:

                for (int i = 0; i < playerItem.weapons.Length; i++)
                {
                    if (playerItem.weapons[i] == null || playerItem.weapons[0].baseAtk == 0)
                    {
                        Name.text = "미습득";
                        Atk.text = "-";
                        Delay.text = "-";
                        break;
                    }

                    if (value == playerItem.weapons[i].weaponCode)
                    {
                        Name.text = names[value] + " ( +" + playerItem.weapons[i].enchantCount + " )";
                        Atk.text = (playerItem.weapons[i].baseAtk + playerItem.weapons[i].enchantAtk).ToString();
                        Delay.text = (playerItem.weapons[i].baseDelay + playerItem.weapons[i].enchantDelay).ToString();
                        break;
                    }
 
                }
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
                if (isUpBtn)
                {
                    statName.text = "힘 (Str)";
                    useOriginTxt.text = Mathf.Pow(2, playerCode.strEnchantCnt).ToString() + " 개 소모";
                }
                else
                {
                    info.text = "힘(Strength)\n\n공격력의 기준이 되는 수치\n올릴 때 마다 힘 + 5";
                }
                break;
            case 10002:
                if (isUpBtn)
                {
                    statName.text = "명중 (Acc)";
                    useOriginTxt.text = Mathf.Pow(3, playerCode.accEnchantCnt).ToString() + " 개 소모";
                }
                else
                {
                    info.text = "명중률(Accuracy)\n\n온전한 공격을 하기 위한 수치\n명중률이 낮을 수록 공격력에 비해 낮은 데미지가 뜰 확률이 높다.\n 올릴 때 마다 명중률 소량 증가";
                }
                break;
            case 10003:
                if (isUpBtn)
                {
                    statName.text = "체력 (HP)";
                    useOriginTxt.text = (10 * (playerCode.HPEnchantCnt + 1)).ToString() + " 개 소모";
                }
                else
                {
                    info.text = "체력(Health Point)\n\n캐릭터의 체력을 결정하는 수치\n체력이 0이 되면 게임 오버가 된다.\n올릴 때 마다 HP + 10";
                }
                break;
            case 10004:
                if (isUpBtn)
                {
                    statName.text = "마나 (MP)";
                    useOriginTxt.text = (20 * (playerCode.MPEnchantCnt + 1)).ToString() + " 개 소모";
                }
                else
                {
                    info.text = "마나(Magic Point)\n\n스킬을 사용하기 위해 지불해야 하는 코스트\n올릴 때 마다 MP + 5";
                }
                break;
            case 20000:
                info.text = "마을에서 기원조각을 이용하여 스탯을 강화할 수 있습니다.\n\n 강화를 할 수록 더 많은 기원조각이 필요합니다.";
                break;
            case 90001:
                info.text = "캐릭터 장비 정보 (단축키 Y)\n\n캐릭터의 장비 정보(강화 수치, 공격력 등)를 볼 수 있습니다.";
                break;
            case 90002:
                info.text = "캐릭터 스탯 정보 (단축키 U)\n\n캐릭터의 스탯 정보를 볼 수 있습니다.\n마을에서는 기원조각을 소모하여 강화도 가능합니다.";
                break;
            case 90003:
                info.text = "아이템 정보 (단축키 I)\n\n아이템 정보들을 볼 수 있습니다.";
                break;

        }
        if (isUpBtn)
        {
            infoPanel.SetActive(true);
        }
        else
        {
            itemNotice.SetActive(true);
        }
        Debug.Log("onMouse");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUpBtn)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            itemNotice.SetActive(false);
        }
        Debug.Log("exitMouse");
    }

}
