using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Damage : MonoBehaviour
{
    public float movingSpeed;
    public float alphaSpeed;
    public int damage;

    public bool isGainMode; // 혹시 동전을 얻을 때 사용하는 것인가?
    public bool isGainOrigin; // 기원조각을 얻는 것인가?

    TextMeshPro dmgText;
    Color alpha; // 텍스트의 컬러 관련 정보를 얻을 수 있다.


    // Start is called before the first frame update
    void Start()
    {
        dmgText = GetComponent<TextMeshPro>();
        if (isGainMode)
        {
            dmgText.text = "+" + damage.ToString() + "G";
        }
        else if (isGainOrigin)
        {
            dmgText.text = "+1 origin";
        }
        else
        {
            dmgText.text = damage.ToString();
        }

        alpha = dmgText.color;
        Invoke("DestroyDmg", 2.0f); // 2초 뒤에 데미지가 사라지게!
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, 1 * movingSpeed * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        dmgText.color = alpha;
    }

    void DestroyDmg()
    {
        Destroy(gameObject);
    }

}
