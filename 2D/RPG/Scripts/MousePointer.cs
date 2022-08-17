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
        Ray2D ray = new Ray2D(mousePos, Vector2.zero); // 원점 ~ 포인터로 발사되는 레이저

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(mousePos);

            float distance = Mathf.Infinity; // Ray 내에서 감지할 최대 거리

            // RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, distance); // 다 잡음 
            RaycastHit2D hitDrawer = Physics2D.Raycast(ray.origin, ray.direction, distance, 1 << LayerMask.NameToLayer("Touchable")); // 1 << LayerMask.NameToLayer("Touchable") 대신 2048을 써도 됨
            
            if(bullet.GetBulletCount() > 0)
            {
                
                if (hitDrawer) // 맞았을 때는 Stage에서 총알 차감!
                {
                    Debug.Log("터치!");
                    hitDrawer.collider.gameObject.GetComponent<Target>().beHit();
                }
                else
                {
                    bullet.discountBullet(); // 안맞았을 때 총알 차감
                }


            }
            else
            {
                Debug.Log("남은 총알이 없습니다!");
            }


        }

    }
}
