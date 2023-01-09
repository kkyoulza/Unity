using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public GameObject HPBar;
    public GameObject MPBar;

    RectTransform HPRect;
    RectTransform MPRect;

    StatInformation stat;

    // Start is called before the first frame update
    void Awake()
    {
        HPRect = HPBar.GetComponent<RectTransform>();
        MPRect = MPBar.GetComponent<RectTransform>();
    }

    private void Start()
    {
        stat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().playerStat;
    }

    // Update is called once per frame
    void Update()
    {
        HPRect.sizeDelta = new Vector2(250 * (stat.playerCntHP / stat.playerMaxHP), 60);
        MPRect.sizeDelta = new Vector2(250 * (stat.playerCntMP / stat.playerMaxMP), 60);
    }
}
