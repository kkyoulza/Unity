using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
    GameObject uiManager;
    PlayerCode playerCode;
    PlayerItem playerItem;
    UIManager ui;

    public Text useOriginTxt;

    // Start is called before the first frame update
    void Awake()
    {
        uiManager = GameObject.FindGameObjectWithTag("uimanager");
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
        playerItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItem>();
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
        ui.HPTxt.text = playerCode.playerMaxHealth.ToString();
        ui.MPTxt.text = playerCode.playerMaxMana.ToString();
    }

    public void EnchantStat(int code)
    {
        switch (code)
        {
            case 10001:
                if (playerItem.enchantOrigin >= (int)Mathf.Pow(2, playerCode.strEnchantCnt))
                {
                    playerItem.enchantOrigin -= (int)Mathf.Pow(2, playerCode.strEnchantCnt);
                    playerCode.strEnchantCnt++;

                    playerCode.playerStrength += 5;
                    useOriginTxt.text = Mathf.Pow(2, playerCode.strEnchantCnt).ToString() + " 개 소모";
                }
                else
                {
                    StartCoroutine(ui.noticeEtc(0));
                }
                break;
            case 10002:
                if (playerItem.enchantOrigin >= (int)Mathf.Pow(3, playerCode.accEnchantCnt))
                {
                    playerItem.enchantOrigin -= (int)Mathf.Pow(3, playerCode.accEnchantCnt);
                    playerCode.accEnchantCnt++;

                    playerCode.playerAccuracy += 10;
                    useOriginTxt.text = Mathf.Pow(3, playerCode.accEnchantCnt).ToString() + " 개 소모";
                }
                else
                {
                    StartCoroutine(ui.noticeEtc(0));
                }
                break;
            case 10003:
                if (playerItem.enchantOrigin >= (10 * (playerCode.HPEnchantCnt + 1)))
                {
                    playerItem.enchantOrigin -= (10 * (playerCode.HPEnchantCnt + 1));
                    playerCode.HPEnchantCnt++;

                    playerCode.playerMaxHealth += 10;
                    useOriginTxt.text = (10 * (playerCode.HPEnchantCnt + 1)).ToString() + " 개 소모";
                }
                else
                {
                    StartCoroutine(ui.noticeEtc(0));
                }
                break;
            case 10004:
                if (playerItem.enchantOrigin >= (20 * (playerCode.MPEnchantCnt + 1)))
                {
                    playerItem.enchantOrigin -= (20 * (playerCode.MPEnchantCnt + 1));
                    playerCode.MPEnchantCnt++;

                    playerCode.playerMaxMana += 5;
                    useOriginTxt.text = (10 * (playerCode.MPEnchantCnt + 1)).ToString() + " 개 소모";
                }
                else
                {
                    StartCoroutine(ui.noticeEtc(0));
                }
                break;

        }
    }

}
