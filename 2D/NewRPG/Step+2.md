# Step+ 2. Platform Effector2D를 이용하여 발판 만들기

Step+는 뭔가 외전격의 느낌이다..

<hr>

## Platform Effector 2D

Platform Effector 2D는 플랫포머 게임에서 사용 될 수 있는 좋은 컴포넌트이다.

횡스크롤 RPG게임에서 떠 있는 발판에 점프를 할 때, 발판에 머리를 박고 떨어진 경험이 있는가?

특별한 컨셉을 가진 것이 아니라면 거의 없을 것이다.

즉, 캐릭터가 점프를 하여 발판을 아래에서 위로 뚫고는 가는데, 다시 플레이어가 착지할 때는 발판이 활성화가 되어 밟을 수 있어야 한다는 것이다.

![image](https://user-images.githubusercontent.com/66288087/212875152-b3e81260-6c47-40cc-b448-9a7cbf2fdb05.png)

이런 식으로 캐릭터의 머리가 발판을 뚫고 있는 상태이지만

![image](https://user-images.githubusercontent.com/66288087/212875272-8e213bb1-7455-4912-9fbf-ce2b990bc7b5.png)

점프를 해서 올라가게 되면 발판이 활성화 되어 캐릭터가 밟을 수 있게 되는 것이다.

<br>

이 것을 도와주는 것이 Platform Effector 2D이다.

<hr>

### Platform Effector 2D의 사용

활용 방법은 빈 오브젝트를 만들어, 그곳에 BoxCollider2D를 추가한다.

그 다음에 Platform Effector 2D도 추가 해 주고, BoxCollider2D에 있는 Used By Effector를 체크 해 준다.

![image](https://user-images.githubusercontent.com/66288087/212878296-74584f58-d285-41c6-a7c6-910863e551a2.png)

그리고 Platform Effector 2D에 있는 Rotational Offset은 0으로 설정 해 준다.

![image](https://user-images.githubusercontent.com/66288087/212878534-043f6da6-9084-4eae-b2c3-5c53029bc9ee.png)

그러면 위 사진과 같이 나오게 된다.

위 사진은 위에서 떨어질 때는 Collider가 작동하여 발판의 역할을 하게 된다는 의미로 추측된다.

Offset을 변경시키면 아래 사진과 같이 된다.

![image](https://user-images.githubusercontent.com/66288087/212878861-b4cb78cc-88a0-4fa3-9b01-fb0208c317b8.png)

이렇게 발판의 각도를 배경에 맞게 조절할 수 있다.

<hr>

### 시연 움짤

발판을 설정하고 시연을 해 보도록 하자

![2d_2+_1](https://user-images.githubusercontent.com/66288087/212879961-579c914e-2d1e-4422-8441-08a94d80ba72.gif)

<hr>



