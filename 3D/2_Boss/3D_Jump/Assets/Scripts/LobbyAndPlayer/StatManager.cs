using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
    GameObject uiManager;
    PlayerCode playerCode;
    UIManager ui;

    // Start is called before the first frame update
    void Awake()
    {
        uiManager = GameObject.FindGameObjectWithTag("uimanager");
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
        ui = uiManager.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SetStatusUI();
    }

    void SetStatusUI()
    {
        ui.Atk.text = playerCode.playerMinAtk.ToString() + " ~ " + playerCode.playerMaxAtk.ToString();
        ui.Str.text = playerCode.playerStrength.ToString();
        ui.Acc.text = playerCode.playerAccuracy.ToString();
    }


}
