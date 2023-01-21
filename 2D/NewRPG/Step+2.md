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

## 캐릭터의 점프 애니메이션이 풀리지 않는 현상

![image](https://user-images.githubusercontent.com/66288087/213861653-2053de3f-2df5-409f-9679-c0cd7ae16a4e.png)

캐릭터로 점프를 하다가 위 사진과 같이 분명 땅에 닿았음에도 위 상태에서 일반 상태로 되지 않는 현상이 생기게 되었다.

심지어 움직여 보면 저 상태로 좌/우로 움직이고 **더 아래에 있는 땅**에 **떨어져야** 다시 Idle 상태가 되게 된다.

<hr>

### 원인?

원인은 코드를 살펴보면서 쉽게 유추할 수 있었다.

Player.cs 코드 중 일부를 보자.

```c#
public void checkLanding()
{
    if(rigid.velocity.y < 0)
    {
        // y축 속도가 음수 = 떨어지고 있음
        // 떨어지고 있을 때, 착지 체크를 진행한다.

        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); // 레이저를 그린다. (시각적으로 보기 위함)

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // 진짜 레이저 그리기 (시작위치, 방향, 거리)
        // Platform이라는 이름의 레이어를 가진 것에 닿는지만 체크해라!

        if(rayHit.collider != null)
        {
            // Platform에 닿았을 때

            if (rayHit.distance < 0.8f) // 일정 거리 미만으로 가까워졌을 때
            {

                anim.SetBool("isJump", false); // 점프 애니메이션 해제
                jumpCount = statInfo.playerMaxJumpCount; // 점프 카운트 리셋
            }

        }

    }

}
```

코드 중에서 땅에 닿았는지를 판별하여 점프 애니메이션을 푸는 위 함수를 주목하면 된다.

함수에는 **유일한 조건**으로 y축 속도가 음수일 때!(떨어지고 있을 때)에만 아래로 레이저를 쏴 땅이 있는지를 판단하게 설정 해 두었다.

![image](https://user-images.githubusercontent.com/66288087/213861807-eb13237c-4163-4c7f-8a29-144ee491bf9e.png)

하지만 위 사진을 잘 보면 점프를 하면서 착지할 때, **땅이 아래에 있음을 판단하는 레이저가 닿지 않는 사각지대**로 착지하게 되었음을 볼 수 있다.

그런데, 착지를 저렇게 해서 멈추게 되어 좌/우로 움직이면서 아래에 땅이 있음을 확인시켜 주어도 **유일한 조건**인 **y축 속도**가 **음수가 아니기** 때문에 확인하는 레이저가 사라지고 없게 되는 것이다. (떨어지고 있는 중이 아니기 때문에)

즉!!! 확인하는 조건에 점프 애니메이션이 풀어지지 않았을 때도 추가를 해 주면 되는 것이다.

그러면 아래와 같이 하면 될까??

```c#
if(rigid.velocity.y < 0 || anim.GetBool("isJump"))
```

아래로 떨어지고 있거나 점프 애니메이션이 발동되고 있을 때, 땅을 확인하여 점프 애니메이션을 멈추어라...

뭔가 이상하지 않나???

바로, or 때문에 **일반적으로 점프**를 하게 될 때도 플레이어가 땅에서 떨어지면서 **점프 애니메이션이 시작되는 순간, 바로 점프 애니메이션이 멈추게 된다.**

그렇다면 어떻게 해 주면 좋을까?

사각지대에 착지를 하게 되면 레이저가 사라진다는 것이 문제가 아니었는가?

그렇다면 사각지대에 떨어져 **멈추게 되었지만 점프 애니메이션이 끝나지 않을 때!!** 레이저가 켜져 있으면 된다.

그래야 좌/우로 움직여서 땅 착지여부를 판단하여 다시Idle로 돌아갈 수 있기 때문이다.

```c#
if(rigid.velocity.y < 0 || (rigid.velocity.y == 0 && anim.GetBool("isJump")))
```

바로 위와 같은 방법으로 y축 속도가 음수이거나(**or**) (y축 속도가 0이면서(**and**) 점프 애니메이션이 아직 끝나지 않았을 때) 착지 판단 레이저를 켜 놓는 것이다.

<hr>

### 시연 움짤

![2d_2+_2](https://user-images.githubusercontent.com/66288087/213862141-5587b27e-fca7-4d5e-a16e-2005f6b49eb6.gif)

걸쳐서 착지해도 다시 Idle로 돌아감을 볼 수 있다.


