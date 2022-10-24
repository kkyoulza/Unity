using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBtnE : MonoBehaviour
{
    // ���� ����
    public int itemNum;
    public GameObject player;
    PlayerItem playerItem;

    //UI ����
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
        if (!CheckWeaponExist()) // ���� ���⸦ �������� ���ߴٸ�
        {
            color.a = 0.4f;
            color.r = 1.0f;
            color.g = 1.0f;
            color.b = 1.0f;
            img.color = color; // �ش� ��ư�� ������ ���߰�
            btn.enabled = false; // ��ư ��� ��Ȱ��ȭ
        }
        else // ���⸦ �����ߴٸ�
        {
            color.a = 1.0f;
            color.r = 1.0f;
            color.g = 1.0f;
            color.b = 1.0f;
            img.color = color; // ���� ���󺹱�
            btn.enabled = true; // ��ư ��� Ȱ��ȭ

        }
    }

    public bool CheckWeaponExist() // �̰��� UI �Ŵ����� �ű�� �Ű������� ���� �̺�Ʈ ������ ��ư���� �ٸ� �Ű������� �ϰԲ� ����.
    {
        if(playerItem.weapons[0].baseAtk == 0) // ù ��° ������ ���ݷ��� 0�̶�� ���� �ƹ��͵� ���� ���ߴٴ� ��
        {
            return false; // false
        }
        for (int i = 0; i < playerItem.weapons.Length; i++)
        {
            if (playerItem.weapons[i] == null)
                continue;
            if (playerItem.weapons[i].weaponCode == itemNum)
            {
                return true; // ��ư�� �´� ���⸦ �Ծ��ٸ� true
            }
        }

        return false; // �׷��� �ʴٸ� false
    }


}
