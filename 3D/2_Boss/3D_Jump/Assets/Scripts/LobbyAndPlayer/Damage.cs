using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Damage : MonoBehaviour
{
    public float movingSpeed;
    public float alphaSpeed;
    public int damage;

    TextMeshPro dmgText;
    Color alpha; // 텍스트의 컬러 관련 정보를 얻을 수 있다.


    // Start is called before the first frame update
    void Start()
    {
        dmgText = GetComponent<TextMeshPro>();
        dmgText.text = damage.ToString();
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
