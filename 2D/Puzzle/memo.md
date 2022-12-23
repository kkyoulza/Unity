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

<hr>

### 2 - 터치해서 옆 블록에 닿으면 블록의 위치를 바꾸어 보자

#### 생각 했던 점

- 각 박스는 한 개의 툴과 일대 일로 대응한다.
- 박스를 드래그하여 옆 박스로 이동했을 때, Trigger 반응이 일어나게 된다.
- 둘 사이의 위치를 바꾸는 함수를 발동한다.

**둘 사이의 위치를 바꾸는 함수?**

코루틴으로 제작하였다.

<pre>
<code>
IEnumerator Move(GameObject target)
{
    if (!isTarget)
    {
        yield break;
    }
    GameObject imsi;
    imsi = target.GetComponent<touch>().tool;
    target.GetComponent<touch>().tool = this.tool;
    this.tool = imsi;

    isMove = true;

    // yield return new WaitForSeconds(0.2f);

    target.transform.position = target.GetComponent<touch>().tool.transform.position;
    Debug.Log(name + "move");
    transform.position = tool.transform.position;
    txt.text = "Change";

    GetComponent<BoxCollider2D>().enabled = false;
    target.GetComponent<BoxCollider2D>().enabled = false;

    yield return new WaitForSeconds(0.1f);

    isMove = false;
    isTarget = false;
    GetComponent<BoxCollider2D>().enabled = true;
    target.GetComponent<BoxCollider2D>().enabled = true;


    yield return null;
}
</code>
</pre>

OnTriggerEnter2D를 통해 얻은 collision.gameObject를 target으로 받아 실행된다.

Trigger 반응이 서로 일어나기에 위치가 바뀌고 또 바뀌면 제자리가 된다. 따라서 바뀌는 것이 한 번만 일어날 수 있게 OnMouseDrag() 함수에서 isTarget 변수를 true로 만들어 주었다.

이렇게 하면 아래 움짤과 같이 된다.

![puzzle_1](https://user-images.githubusercontent.com/66288087/206721290-511f2cbb-d0d4-46a5-822f-0462f51dbd5c.gif)

처음에 삑사리가 났는데.. 삑사리가 좀 난다는 것이 문제점이다.

이 문제를 해결하기 위해, 블럭 스왑을 위한 다른 방식을 생각 해 봐야할 듯 하다.

(예를 들어 블럭을 선택하고 방향에 맞춰 드래그 하면 고정으로 바뀌게? Trigger로 닿지 않고)












