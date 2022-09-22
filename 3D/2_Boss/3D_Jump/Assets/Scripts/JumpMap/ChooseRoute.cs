using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRoute : MonoBehaviour
{
    public GameObject Wall;
    public GameObject ActiveBase;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onWall()
    {
        ActiveBase.SetActive(true);
        Wall.SetActive(true);
    }

}
