## memo

### 1 - 터치를 통한 물체 움직임 연습

![image](https://user-images.githubusercontent.com/66288087/206468367-00536407-9ab8-4232-becc-1484e73ba283.png)

네모를 드래그로 움직일 수 있게 하였으며, 각 오브젝트 별로 코드를 넣어 터치, 드래그를 인지할 수 있게 하였다.

가운데 text에 터치, 드래그 여부를 넣어 주었다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class touch : MonoBehaviour
{
    public Text txt;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseUp()
    {
        txt.text = gameObject.name + "Up";
        Debug.Log(gameObject.name + "Up");
    }

    private void OnMouseDown()
    {
        txt.text = gameObject.name + "Down";
        Debug.Log(gameObject.name + "Down");
    }

    private void OnMouseDrag()
    {
        txt.text = gameObject.name + "Drag";
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y,10);
        this.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        Debug.Log(gameObject.name + "Drag");
    }

}
</code>
</pre>

각 오브젝트에 들어간 터치 코드

#### 중요한 점

여기서 OnMouseDrag를 보게 되면 mousePos의 z 좌표가 10임을 볼 수 있다.

mousePos는 Vector이므로 점이 아니라 방향이다.

[링크](https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html)에서는 z 거리만큼 떨어진 지점의 위치로 변환한다고 나와 있다.

![image](https://user-images.githubusercontent.com/66288087/206471718-5be7c19f-9366-44d7-b13e-96dce7cce5f6.png)

위 사진을 보면 카메라는 z가 -10인 위치에서 화면을 바라보고 있다.

따라서, z좌표를 0으로 설정 해 두면, 카메라 시작점과 겹치게 되어, 드래그 하는 순간 화면에서 보이지 않게 된다.

