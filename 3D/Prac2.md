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

나머지 fbx 파일도 humanoid로 바꾸어 주었다.

그런데, 애니메이션의 스킨 색이 적용되지 않았음을 볼 수 있다. 아래 사진에서 Extract Texture를 통하여 텍스처를 내보내 주게 되면

![2_6](https://user-images.githubusercontent.com/66288087/182079910-690665f5-d1a0-45e2-b854-393d8482de78.JPG)

![2_7](https://user-images.githubusercontent.com/66288087/182083240-69f65672-9667-4054-b32b-e398f6ab62c2.JPG)

위와 같은 파일들이 생기게 되며, 원본 아바타 색이 잘 적용됨을 볼 수 있다.





<hr>

### 3. 애니메이션 화면 배치 및 



