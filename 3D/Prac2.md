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





<hr>


### 5. 플레이어 추적 3인칭 카메라 구현 도전


- 아이디어 : 우선 플레이어의 자식으로 카메라를 옮긴 다음에 좌표계를 초기화 하고, 좌/우 방향키(Horizon)가 입력될 때 Rotation만 바꾸어 주면 되지 않을까 생각한다.


