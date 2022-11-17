using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject itemNotice; // ������ �������� �����ִ� ��ü
    public PlayerItem playerItem; // �÷��̾� ���� ����
    public PlayerCode playerCode; // �÷��̾� ���� ����
    public Text Name; // ���� �̸�
    public Text Atk; // ���� ���ݷ�
    public Text Delay; // ���� ������

    // ������ ������ ��� �� ����
    public Text info;
    public Vector3 pos;
    public int value;

    public bool isUpBtn; // ������ �÷��ִ� ��ư�ΰ�?
    public GameObject infoPanel; // ���� ��ȭ �� ���Ǵ� ������� ��� �ȳ�
    public Text statName;
    public Text useOriginTxt;

    string[] names = new string[] { "", "�ƴٸ�Ƽ�� �ظ�", "��", "�ӽ� ��" };

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
                info.text = "���\n\n�� ���迡�� ��� �Ǵ� ȭ�� ����\n��ȭ, ���� ���� � �̿� ����\n���� ��, ���� Ŭ����, ���� �Ĺ��� ���� ���� ����";
                break;
            case 1:

                for(int i = 0; i < playerItem.weapons.Length; i++)
                {
                    if (playerItem.weapons[i] == null || playerItem.weapons[0].baseAtk == 0)
                    {
                        Name.text = "�̽���";
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
                        Name.text = "�̽���";
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
                        Name.text = "�̽���";
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
                info.text = "��� ����\n\n��ȭ ������ ����ϸ� ����� �� ��\n1 ���� ��ȭ Ȯ�� 0.5%p ���\n(100%���� ��� ����)";
                break;
            case 2001:
                info.text = "HP ����\n\nHP�� ȸ����Ű�� ����\nHP + 30";
                break;
            case 2002:
                info.text = "MP ����\n\nMP�� ȸ����Ű�� ����\nMP + 10";
                break;
            case 10000:
                info.text = "���ݷ�\n\n�������� ������ ��ġ�� ��ġ\n���߷��� �������� ������ �������� ���´�.";
                break;
            case 10001:
                if (isUpBtn)
                {
                    statName.text = "�� (Str)";
                    useOriginTxt.text = Mathf.Pow(2, playerCode.strEnchantCnt).ToString() + " �� �Ҹ�";
                }
                else
                {
                    info.text = "��(Strength)\n\n���ݷ��� ������ �Ǵ� ��ġ\n�ø� �� ���� �� + 5";
                }
                break;
            case 10002:
                if (isUpBtn)
                {
                    statName.text = "���� (Acc)";
                    useOriginTxt.text = Mathf.Pow(3, playerCode.accEnchantCnt).ToString() + " �� �Ҹ�";
                }
                else
                {
                    info.text = "���߷�(Accuracy)\n\n������ ������ �ϱ� ���� ��ġ\n���߷��� ���� ���� ���ݷ¿� ���� ���� �������� �� Ȯ���� ����.\n �ø� �� ���� ���߷� �ҷ� ����";
                }
                break;
            case 10003:
                if (isUpBtn)
                {
                    statName.text = "ü�� (HP)";
                    useOriginTxt.text = (10 * (playerCode.HPEnchantCnt + 1)).ToString() + " �� �Ҹ�";
                }
                else
                {
                    info.text = "ü��(Health Point)\n\nĳ������ ü���� �����ϴ� ��ġ\nü���� 0�� �Ǹ� ���� ������ �ȴ�.\n�ø� �� ���� HP + 10";
                }
                break;
            case 10004:
                if (isUpBtn)
                {
                    statName.text = "���� (MP)";
                    useOriginTxt.text = (20 * (playerCode.MPEnchantCnt + 1)).ToString() + " �� �Ҹ�";
                }
                else
                {
                    info.text = "����(Magic Point)\n\n��ų�� ����ϱ� ���� �����ؾ� �ϴ� �ڽ�Ʈ\n�ø� �� ���� MP + 5";
                }
                break;
            case 20000:
                info.text = "�������� ��������� �̿��Ͽ� ������ ��ȭ�� �� �ֽ��ϴ�.\n\n ��ȭ�� �� ���� �� ���� ��������� �ʿ��մϴ�.";
                break;
            case 90001:
                info.text = "ĳ���� ��� ���� (����Ű Y)\n\nĳ������ ��� ����(��ȭ ��ġ, ���ݷ� ��)�� �� �� �ֽ��ϴ�.";
                break;
            case 90002:
                info.text = "ĳ���� ���� ���� (����Ű U)\n\nĳ������ ���� ������ �� �� �ֽ��ϴ�.\n���������� ��������� �Ҹ��Ͽ� ��ȭ�� �����մϴ�.";
                break;
            case 90003:
                info.text = "������ ���� (����Ű I)\n\n������ �������� �� �� �ֽ��ϴ�.";
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
