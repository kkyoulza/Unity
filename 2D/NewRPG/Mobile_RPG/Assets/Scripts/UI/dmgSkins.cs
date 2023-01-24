using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class dmgSkins : MonoBehaviour
{
    TextMeshPro dmgText;
    Color alpha;

    public bool isUsePool;
    public float movingSpeed;
    public float alphaSpeed;
    public int damage;

    public bool isPlayerHit; // �÷��̾ �°� �ִ°�?

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, 1 * movingSpeed * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        dmgText.color = alpha;
    }

    public void setDamage(int dmg)
    {
        this.damage = dmg;

        dmgText = GetComponent<TextMeshPro>();
        dmgText.text = damage.ToString();

        alpha = dmgText.color;
        alpha.a = 255f;
        if(isUsePool)
            Invoke("inActiveDmg", 1.0f); // 1�� �ڿ� �������� �������!
        else
            Invoke("DestroyObj", 1.0f);
    }

    public void DestroyObj()
    {
        Destroy(gameObject);
    }

    public void inActiveDmg()
    {
        gameObject.SetActive(false);
    }

}
