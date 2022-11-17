using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSet : MonoBehaviour
{
    SaveInfos saveData; // 저장된 코드
    PlayerItem playerItem; // 플레이어 아이템 코드
    PlayerCode playerCode; // 플레이어 스탯이 저장된 코드
    UIManager uiManager; // UI 매니저


    // Start is called before the first frame update
    void Start()
    {
        saveData = GameObject.FindGameObjectWithTag("saveInfo").GetComponent<SaveInfos>();
        playerItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItem>();
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();

        if(GameObject.FindGameObjectWithTag("uimanager") != null)
        {
            uiManager = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManager>();
        }

        resetData();
        if (uiManager != null)
            reSetUI();

    }

    public void resetData()
    {

        playerCode.playerMaxHealth = saveData.info.playerMaxHealth;
        playerCode.playerHealth = saveData.info.playerCntHealth;
        playerCode.playerMana = saveData.info.playerCntMP;
        playerCode.playerMaxMana = saveData.info.playerMaxMP;
        playerCode.playerStrength = saveData.info.playerStrength;
        playerCode.playerAccuracy = saveData.info.playerAcc;
        playerItem.playerCntGold = saveData.info.playerCntGold;
        playerItem.enchantOrigin = saveData.info.enchantOrigin;
        playerItem.cntHPPotion = saveData.info.HPPotion;
        playerItem.cntMPPotion = saveData.info.MPPotion;
        playerCode.strEnchantCnt = saveData.info.strCnt;
        playerCode.accEnchantCnt = saveData.info.accCnt;
        playerCode.HPEnchantCnt = saveData.info.HPCnt;
        playerCode.MPEnchantCnt = saveData.info.MPCnt;


        for (int i = 0; i < saveData.info.weapons.Length; i++)
        {
            if (saveData.info.weapons[i].baseAtk == 0)
                return;

            playerItem.weapons[i] = saveData.info.weapons[i];
            playerCode.hasWeapons[saveData.info.weapons[i].weaponCode] = true; // 무기를 얻은 여부도 반영 해 준다.
            playerItem.SetEnchantInfo(i); // playerItem -> weapon 반영(실제 데미지가 계산되는 곳으로)

        }
        
    }

    public void reSetUI()
    {
        uiManager.goldTxt.text = saveData.info.playerCntGold.ToString();
    }


}
