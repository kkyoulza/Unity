# 3D 캐릭터 애니메이션 연습


## Blend Tree를 통한 캐릭터 움직임 구현하기(복습)


### 1. 애니메이션 소스 준비

#### 준비물 : 애니메이션 소스(fbx)는 [이곳](https://www.mixamo.com/#/) 에서 받아서 사용하였다.

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



