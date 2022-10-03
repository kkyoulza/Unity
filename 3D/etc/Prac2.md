# 3D 캐릭터 애니메이션 연습


## Blend Tree를 통한 캐릭터 움직임 구현하기(복습)


### 1. 애니메이션 소스 준비

#### 준비물 : 애니메이션 소스(fbx)는 [이곳](https://www.mixamo.com/#/) 에서 받아 사용하였다.

![2_1](https://user-images.githubusercontent.com/66288087/182062799-7f475d63-752b-43d4-a30d-ad0ad9cf6ff6.JPG)

회원가입만 하면 Characters에서 원하는 스킨을 선택한 뒤, Animations에서 원하는 동작 fbx 파일을 무료로 받을 수 있다.

![2_2](https://user-images.githubusercontent.com/66288087/182063069-dd8d36d7-89a7-4b68-9650-2c29cd0f4b99.JPG)

위와 같이 다양한 움직임들이 있음을 볼 수 있다.

사용한 fbx는 Idle, Left Turn(idle), Right Turn(idle), Walking, Left Turn(Walking), Right Turn(Walking), Running, Left Turn(Running), Right Turn(Running) 의 **9가지** 종류를 사용하였다.

<hr>

### 2. 애니메이션 소스 가공

![2_3](https://user-images.githubusercontent.com/66288087/182063585-55eca7e0-d8a8-431c-a818-dfb52d210706.JPG)

**각 fbx 파일 Inspector → Rig → Animation Type 을 Humanoid로 바꾸어 주어야 사람 형태로 인식하고 애니메이션이 제대로 작동된다.**

(이 부분을 생략하게 되면 아래 과정들을 그대로 한다고 해도 캐릭터가 특정 구간을 벗어나지 못하고 순간이동되게 된다.)

또한, 위 Configure을 누르게 되면 아래 사진과 같이 세부적으로 관절(?)포인트 설정이 가능하게 나온다.

![2_4](https://user-images.githubusercontent.com/66288087/182066834-8e123b69-1050-4771-a2b6-1069ab94ff1d.JPG)

![2_5](https://user-images.githubusercontent.com/66288087/182069830-8e13d941-bd5f-45a5-8408-8ad75ff893a1.JPG)

또한 Muscles & Settings를 눌러 근육 테스트를 할 수 있으며, 극한의 자세들을 테스트하여 포인트들을 점검할 수 있다. 

![image](https://user-images.githubusercontent.com/66288087/182103406-ec30b664-cfb4-4a07-a0b5-952ed49bee9e.png)

그리고 Animation 부분에서 Loop Time에 체크 해 준다. (계속해서 움직이면 반복되어야 하기 때문)

나머지 fbx 파일도 humanoid로 바꾸어 주었다.

그런데, 애니메이션의 스킨 색이 적용되지 않았음을 볼 수 있다. 아래 사진에서 Extract Texture를 통하여 텍스처를 내보내 주게 되면

![2_6](https://user-images.githubusercontent.com/66288087/182079910-690665f5-d1a0-45e2-b854-393d8482de78.JPG)

![2_7](https://user-images.githubusercontent.com/66288087/182083240-69f65672-9667-4054-b32b-e398f6ab62c2.JPG)

위와 같은 파일들이 생기게 되며, 원본 아바타 색이 잘 적용됨을 볼 수 있다.

![2_8](https://user-images.githubusercontent.com/66288087/182084139-afb6b63b-3c8a-4e99-a222-a195f0edbc73.JPG)

아바타 텍스처 색이 적용된 모습

<hr>

### 3. 애니메이터 제작 및 Blender Tree 설정

![2_9](https://user-images.githubusercontent.com/66288087/182095755-b2835882-2c5c-4d76-8c8b-46e8160f1251.jpg)

Window → Anomation → Anomator 를 통해서 Animator를 만들어 준다.

빈 Animator가 생기면 Player에 드래그 해 주어 Component로 추가 해 준다.

Animator 속에 Blend Tree를 추가 해 준다.

![2_11](https://user-images.githubusercontent.com/66288087/182098689-01bed1f0-dba0-4e93-881b-925c396a5c82.jpg)

우리는 앞으로 가면서 좌/우로 틀수 있어야 하기에 2D 좌표계가 적절한 설정일 듯 하다. 따라서 2D Cartesian으로 설정 해 준다.

![2_10](https://user-images.githubusercontent.com/66288087/182099074-296140b9-4eb5-422c-a7d0-99ea3d02a789.JPG)

그리고 Motion Field를 추가 해 주면 위 사진과 같이 된다.

Motion Field를 9개 추가 해 준다.

![image](https://user-images.githubusercontent.com/66288087/182099200-11707d30-d380-453d-b52d-a8e717f698ba.png)

그러면 위 사진과 같이 한 Field가 각 좌표를 차지하고 있는 모습을 볼 수 있을 것이다.

이제 아래와 같이 좌표계를 먼저 정리 해 주도록 하자.

![image](https://user-images.githubusercontent.com/66288087/182099481-00df213a-3b57-4a36-aaeb-4dbfd8adbc12.png)

그러면 이렇게 9개의 위치를 잡게 된 것을 볼 수 있을 것이다.

맨 위부터 셀 때 첫 번째 행은 달리고 있는 상태이다. 순서대로 **달리면서 왼쪽 회전 / 앞으로 달림 / 달리면서 오른쪽 회전** 이다.

두 번째 행은 걷는 상태이며 달리는 것과 유사하게 사용할 수 있다.

마지막 세 번째 행은 Idle 상태이다. 기본적으로 X = 0,Y = 0 일때 정면 Idle이며, X = -1, Y = 0 (Idle 왼쪽 회전), X = 1, Y = 0 (Idle 오른쪽 회전) 이다.

이제 상태에 맞게 앞서 다운로드 받고 가공한 fbx파일 속에 있는 애니메이션 파일을 넣어 주도록 하자.

![image](https://user-images.githubusercontent.com/66288087/182100004-3b58cae3-aed9-449d-8e3f-8c23ae03d9ee.png)

위에 있던 그림이랑 순서는 다르지만 좌표 위치는 같게 삽입하였다.

그리고 Animator 좌측에서 Parameter를 2개 만들어 준 다음, X, Y에 대응시켜 준다.

그러면 이제 Blend Tree에 대한 준비는 끝났다. 이제 Blend Tree Parameter를 조절할 코드를 만들어 보도록 하자.

<hr>

### 4. Blender Tree 작동 코드 작성

우선 Player 안에 넣을 코드이기 때문에 Player 내부 Animator Component를 private로 받아서 쓰게 된다.


<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAni : MonoBehaviour
{
    private Animator CharAnimation = null;

    private int HorFloat = Animator.StringToHash("Horz");
    private int VerFloat = Animator.StringToHash("Vert");

    // Start is called before the first frame update
    void Start()
    {
        CharAnimation = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float Ver = Input.GetAxis("Vertical");
        float Hor = Input.GetAxis("Horizontal");

        CharAnimation.SetFloat(HorFloat, Hor, 0.2f, Time.deltaTime);
        CharAnimation.SetFloat(VerFloat, Ver, 0.2f, Time.deltaTime);

    }
}
</code>
</pre>


CharAnimation = GetComponent<Animator>(); 를 통해서 Player에 있는 Animator를 가져오게 되며, Animator.StringToHash를 사용하여 SetFloat에서 Animator의 Parameter 값을 처리하는 시간을 줄였다.

또한, Input.GetAxis를 통해서 가로 키(←,→/a,d)와 세로 키(↑,↓,w,s)를 받을 때 받는 float 값을 SetFloat를 통하여 실시간으로 Parameter에 값이 전달되게 하였다.

코드를 Player에 넣은 후 실행을 해 보도록 하자

![image](https://user-images.githubusercontent.com/66288087/182120058-38f14ecd-269e-4cce-b18b-54124ba09300.png)

플레이어가 잘 움직이고 있음을 볼 수 있다.
  
그렇지만 플레이어가 먼 거리를 갔을 때는 플레이어가 잘 보이지 않는다는 점이 걸리게 된다. 따라서 플레이어 뒤에 카메라가 계속 따라다니게 하고자 한다.

<hr>


### 5. 플레이어 추적 3인칭 카메라 구현 도전


- 아이디어 : 우선 플레이어의 자식으로 카메라를 옮긴 다음에 좌표계를 초기화 하고, 좌/우 방향키(Horizon)가 입력될 때 Rotation만 바꾸어 주면 되지 않을까 생각한다.

(그런데 Player 하위에 넣기만 해도 카메라가 따라 다니는 형태가 나왔다. 흔들리는 부분만 줄이게 되면 좋을 것 같다.)

![image](https://user-images.githubusercontent.com/66288087/182122114-d406e3a2-59f0-43fb-9801-041f505bbb57.png)

우선 카메라를 Player 하위에 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/182122604-81ae518c-67e3-4662-a8de-27ef8aa2d83b.png)

그 다음, 카메라 Inspector에서 좌표 Reset을 해 주면 아래 사진과 같이 된다.

![image](https://user-images.githubusercontent.com/66288087/182122709-be400432-cdfd-4edb-b948-3ea43fcbbc75.png)

카메라가 Player의 바닥에 있음을 볼 수 있다.

![image](https://user-images.githubusercontent.com/66288087/182123153-241b9c64-f095-4f43-81af-5db14ce31626.png)

이제 카메라의 좌표를 고쳐 보도록 하자. 플레이어의 살짝 뒤, 위에 있으며 아래로 Rotation이 적용되게 하면 아래 사진과 같이 된다.

![image](https://user-images.githubusercontent.com/66288087/182123331-7876d5eb-c9e2-4aec-bc38-47aa162ac882.png)

이제 움직여 보도록 하자.

![image](https://user-images.githubusercontent.com/66288087/182123403-c5043196-8ac0-4f3a-bd2b-980a2dff0b4e.png)

회전도 잘 이루어지고, 플레이어의 뒤를 카메라가 잘 따라가고 있음을 볼 수 있다.

한 가지 단점이라면 카메라가 너무 흔들린다는 점이다. 이 것은 다음에 코드를 통해 개선을 해 보던지 해야할 것 같다.

<hr>

간단한 연습이지만 너무 글을 늘어지게 쓴 것 같다. 

이 것을 발판 삼아 추후에 다양한 3D 컨텐츠를 만들어 보고자 한다.
