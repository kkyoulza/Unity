# 프로젝트 정리 문서

<hr>

## 정보

### 이름 : 유니티 3D를 이용한 RPG 습작
### 참고 자료 및 에셋 : 골드메탈 쿼터뷰 게임 제작 강의, 골드메탈 쿼터뷰 에셋, Polygon Asset, Casual Game BGM

<hr>

## 구현 기능들

### 캐릭터 움직임, 대쉬, 구르기

- 방향키로 캐릭터 이동
- 왼쪽 쉬프트 키를 누르면서 움직이면 빠르게 움직일 수 있는 대쉬
- z키를 누르면 MP를 소모하며 전방으로 구르는 구르기 가능

![image](https://user-images.githubusercontent.com/66288087/205892834-94249244-6b5e-49e8-9430-711990429cb8.png)

캐릭터 이동

![image](https://user-images.githubusercontent.com/66288087/205892965-858d05b9-4cd2-4848-be82-a847ed212290.png)

대쉬

![image](https://user-images.githubusercontent.com/66288087/205893052-36815926-a34b-4be6-a5e7-124f74f46d20.png)

구르기

#### 구현 방식

- bool 변수에 GetAxisRaw, GetButtonDown을 통해 키를 입력받은 상태를 저장하여 캐릭터가 움직이게 하였다.
- 이동에는 RigidBody가 아닌, transform.position을 통하여 PlayerSpeed를 반영하여 움직임을 구현하였다.
- 대쉬에서는 PlayerSpeed를 높게 설정 해 주었다. 대쉬도 bool 변수로 상태 변수를 사용하여 삼항 연산자와 함께 일반 걸음과 대쉬를 구분하였다.
- 구르기는 순간적으로 PlayerSpeed를 두 배로 만들어 주고, 구르는 애니메이션을 추가하여 구현하였다.

[코드 링크]()


### 캐릭터 공격, 무기 교체






### 몬스터, 보스




### 무기 강화




### 상점




### 스탯 강화





### 던전 스테이지





### 파밍용 돌(스포너 자동 스폰)





### 점프 맵





### 점프 점수 개인 Top3 랭킹 판










