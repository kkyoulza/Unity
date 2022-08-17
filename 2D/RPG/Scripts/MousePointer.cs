using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    public GameObject pointerPrefab;
    private GameObject pointerRed;
    Vector3 mousePos;
    BulletManager bullet;


    // Start is called before the first frame update
    void Start()
    {
        pointerRed = Instantiate(pointerPrefab) as GameObject;
        bullet = GetComponent<BulletManager>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = -1;
        pointerRed.transform.position = mousePos;
        Ray2D ray = new Ray2D(mousePos, Vector2.zero); // ���� ~ �����ͷ� �߻�Ǵ� ������

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(mousePos);

            float distance = Mathf.Infinity; // Ray ������ ������ �ִ� �Ÿ�

            // RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, distance); // �� ���� 
            RaycastHit2D hitDrawer = Physics2D.Raycast(ray.origin, ray.direction, distance, 1 << LayerMask.NameToLayer("Touchable")); // 1 << LayerMask.NameToLayer("Touchable") ��� 2048�� �ᵵ ��
            
            if(bullet.GetBulletCount() > 0)
            {
                
                if (hitDrawer) // �¾��� ���� Stage���� �Ѿ� ����!
                {
                    Debug.Log("��ġ!");
                    hitDrawer.collider.gameObject.GetComponent<Target>().beHit();
                }
                else
                {
                    bullet.discountBullet(); // �ȸ¾��� �� �Ѿ� ����
                }


            }
            else
            {
                Debug.Log("���� �Ѿ��� �����ϴ�!");
            }


        }

    }
}
