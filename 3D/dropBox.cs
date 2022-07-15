using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropBox : MonoBehaviour
{
    public GameObject dropTarget;
    public Text RedNotice;
    MeshRenderer OutputColor;

    bool Red = false;

    public void SetRed()
    {
        if(Red == false)
        {
            Red = true;
            RedNotice.text = "RED ON";
        }
        else
        {
            Red = false;
            RedNotice.text = "RED OFF";
        }
    }

    public void DropDown()
    {

        //System.Threading.Thread.Sleep(1000); // 1000ms = 1s
        // OutputColor = dropTarget.GetComponent<MeshRenderer>();
        Instantiate(dropTarget, transform.position, Quaternion.identity);

        if(Red == true)
        {
            //OutputColor.material.color = Color.red;
            
            
        }

        
    }
    
}
