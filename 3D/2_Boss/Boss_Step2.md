# Step2. 아이템 습득 및 장착, 근접 공격

이제 RPG하면 빠질 수 없는 아이템을 만들어 보도록 하자.

필드에 떨어진 아이템에 다가가 습득을 하는 기능을 먼저 구현 해 보도록 하자

우선 골드메탈님의 에셋에서 아이템 프리팹을 하나 가져 와 준다.

그리고 있어 보이게 아래 사진과 같이 아이템 오브젝트의 각도를 Z축으로 기울여 준다.

![image](https://user-images.githubusercontent.com/66288087/191007657-551a3064-8544-41c4-8c70-39c475e2e124.png)

이렇게 기울여 준 다음, 프리팹 자식에 빈 오브젝트를 하나 생성 해 준다.

**빛 효과 추가**

그 다음 Light 컴포넌트를 추가 해 준다.

![image](https://user-images.githubusercontent.com/66288087/191007767-d9a4fc61-b5b8-416f-a53c-f6e3d9cece6f.png)

위와 같이 범위를 조금 줄여주고, 밝기를 좀 높여주었으며, 아이템 색깔에 맞게 빛 색도 바꾸어 주었다.

**파티클 추가**

빛에 이어 파티클 시스템도 추가 해 주었다.

Material은 Default-Line으로 하였으며, 색은 아이템 색과 유사하게 설정하였다.

![image](https://user-images.githubusercontent.com/66288087/191009600-3204ccb1-08bf-4b63-a6ea-bf73183ac3bb.png)

Limit Velocity over Lifetime에 있는 Drag를 1로 설정하여 파티클이 너무 퍼져 나가는 것을 억제하였다.

(좋은 아이템에만 과한 효과를 주어야 하기 때문)

![image](https://user-images.githubusercontent.com/66288087/191009640-85f2be7c-451d-4986-9d1a-280aebff91ea.png)

또한, Size도 천천히 줄어들게 설정 해 주었다.

**아이템 자체 코드 설정**

아이템에 추가적인 효과를 주고, 아이템 자체에 대한 정보를 저장하기 위해 코드를 하나 만들어 준다.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Armor, Coin, Weapon }; // 열거형 타입
    public Type type;
    public int value;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }
}
</code>
</pre>







