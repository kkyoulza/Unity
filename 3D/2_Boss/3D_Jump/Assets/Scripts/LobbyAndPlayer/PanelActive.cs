using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelActive : MonoBehaviour
{
    public int itemCode;
    public Image img;
    PlayerCode playerCode;
    // Start is called before the first frame update
    void Awake()
    {
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
    }

    // Update is called once per frame
    void Update()
    {
        isActive();
    }

    public void isActive()
    {
        if(playerCode.cntEquipWeapon == null)
        {
            return;
        }

        if(playerCode.cntEquipWeapon.itemCode == this.itemCode)
        {
            Color color = new Color(1, 1, 1, 1);
            img.color = color;
        }
        else
        {
            Color color = new Color(1, 1, 1, 0.3f);
            img.color = color;
        }

    }


}
