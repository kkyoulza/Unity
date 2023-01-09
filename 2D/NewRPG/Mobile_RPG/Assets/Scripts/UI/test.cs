using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        Debug.Log("awake");
    }
    void Start()
    {
        Debug.Log("start");
    }

    private void OnEnable()
    {
        Debug.Log("enable");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
