using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonUI : MonoBehaviour
{
    // ���� UI
    public GameObject rewardUI;
    public Text originTxt;
    public Text rubyTxt;
    public Text emeraldTxt;
    public Text goldTxt;
    public Text silverTxt;
    public Text sumTxt;

    // ���� �ӽ����� ����
    int origin;
    int ruby;
    int emerald;
    int gold;
    int silver;
    int addGold;

    // ���� ���� UI
    PlayerItem playerItem;
    PlayerCode playerCode;
    
    void Awake()
    {
        playerCode = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCode>();
        playerItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setReward()
    {
        // ���� �������� ����!

        origin = Random.Range(20, 50); // Ȯ���� �ƴ� �׳� ���� ���Խ�Ų��.
        int rubyRan = Random.Range(1, 11);
        int emelaldRan = Random.Range(1, 11);
        int goldRan = Random.Range(1, 11);
        int silverRan = Random.Range(1, 11);

        switch (rubyRan)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                ruby = 0;
                break;
            case 5:
            case 6:
            case 7:
                ruby = 1;
                break;
            case 8:
            case 9:
                ruby = 2;
                break;
            case 10:
                ruby = 3;
                break;
        }

        switch (emelaldRan)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                emerald = 1;
                break;
            case 5:
            case 6:
            case 7:
                emerald = 3;
                break;
            case 8:
            case 9:
                emerald = 5;
                break;
            case 10:
                emerald = 7;
                break;
        }

        switch (goldRan)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                gold = 5;
                break;
            case 5:
            case 6:
            case 7:
                gold = 10;
                break;
            case 8:
            case 9:
                gold = 15;
                break;
            case 10:
                gold = 30;
                break;
        }

        switch (silverRan)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                silver = 20;
                break;
            case 5:
            case 6:
            case 7:
                silver = 50;
                break;
            case 8:
            case 9:
                silver = 80;
                break;
            case 10:
                silver = 200;
                break;
        }

        playerItem.enchantOrigin += origin;
        addGold = ruby * 10000 + emerald * 1000 + gold * 100 + silver * 10;
        playerItem.playerCntGold += addGold;

        StartCoroutine(setRewardUI());

    }

    IEnumerator setRewardUI()
    {
        originTxt.text = origin.ToString()+" ��";
        rubyTxt.text = ruby.ToString() + " ��";
        emeraldTxt.text = emerald.ToString() + " ��";
        goldTxt.text = gold.ToString() + " ��";
        silverTxt.text = silver.ToString() + " ��";
        sumTxt.text = "�� : "+addGold.ToString() + " Gold";

        rewardUI.SetActive(true);
        playerCode.isTalk = true;

        yield return new WaitForSeconds(3.0f);

        rewardUI.SetActive(false);
        playerCode.isTalk = false;

        SceneManager.LoadScene("Boss1");

    }


}
