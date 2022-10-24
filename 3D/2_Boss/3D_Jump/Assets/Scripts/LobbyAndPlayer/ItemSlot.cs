using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject itemNotice; // ������ �������� �����ִ� ��ü
    public Text info;
    public Vector3 pos;
    public int value;
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        itemNotice.transform.position = transform.position + pos;
        switch (value)
        {
            case -1:
                info.text = "���\n\n�� ���迡�� ��� �Ǵ� ȭ�� ����\n��ȭ, ���� ���� � �̿� ����\n���� ��, ���� Ŭ����, ���� �Ĺ��� ���� ���� ����";
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
                info.text = "��(Strength)\n\n���ݷ��� ������ �Ǵ� ��ġ\n�ø� �� ���� ���ݷ� + 1";
                break;
            case 10002:
                info.text = "���߷�(Accuracy)\n\n������ ������ �ϱ� ���� ��ġ\n���߷��� ���� ���� ���ݷ¿� ���� ���� �������� �� Ȯ���� ����.\n �ø� �� ���� ���߷� + 1%p";
                break;
            case 10003:
                info.text = "ü��(Health Point)\n\nĳ������ ü���� �����ϴ� ��ġ\nü���� 0�� �Ǹ� ���� ������ �ȴ�.\n�ø� �� ���� HP + 10";
                break;
            case 10004:
                info.text = "����(Magic Point)\n\n��ų�� ����ϱ� ���� �����ؾ� �ϴ� �ڽ�Ʈ\n�ø� �� ���� MP + 5";
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
