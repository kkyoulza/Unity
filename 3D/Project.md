# 프로젝트 정리 문서

<hr>

## 정보

### 이름 : 유니티 3D를 이용한 RPG 습작
### 참고 자료 및 에셋 : 골드메탈 쿼터뷰 게임 제작 강의, 골드메탈 쿼터뷰 에셋, Polygon Asset, Casual Game BGM

<hr>

## 구현 기능들

<hr>

### 캐릭터 움직임, 대쉬, 구르기

- 방향키로 캐릭터 이동
- 왼쪽 쉬프트 키를 누르면서 움직이면 빠르게 움직일 수 있는 대쉬
- z키를 누르면 MP를 소모하며 전방으로 구르는 구르기 가능

![image](https://user-images.githubusercontent.com/66288087/205892834-94249244-6b5e-49e8-9430-711990429cb8.png)

캐릭터 이동

![image](https://user-images.githubusercontent.com/66288087/205896238-c1e8a25a-5594-4886-80ae-0dfe9ad94633.png)

대쉬

![image](https://user-images.githubusercontent.com/66288087/205893052-36815926-a34b-4be6-a5e7-124f74f46d20.png)

구르기

#### 구현 방식

- bool 변수에 GetAxisRaw, GetButtonDown을 통해 키를 입력받은 상태를 저장하여 캐릭터가 움직이게 하였다.
- 이동에는 RigidBody가 아닌, transform.position을 통하여 PlayerSpeed를 반영하여 움직임을 구현하였다.
- 대쉬에서는 PlayerSpeed를 높게 설정 해 주었다. 대쉬도 bool 변수로 상태 변수를 사용하여 삼항 연산자와 함께 일반 걸음과 대쉬를 구분하였다.
- 구르기는 순간적으로 PlayerSpeed를 두 배로 만들어 주고, 구르는 애니메이션을 추가하여 구현하였다.

[코드 링크](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/Boss_Step1.md)

<hr>

### 캐릭터 공격, 무기 교체

- x키로 공격 발동
- 숫자 키를 눌러 무기를 교체
- 원거리 공격 구현

![image](https://user-images.githubusercontent.com/66288087/205896940-f27f279d-906a-4fd7-a9cf-43db027a2091.png)

근접 공격 모습

![image](https://user-images.githubusercontent.com/66288087/205909070-3e39140e-6e75-41e6-b652-74317cae56d7.png)

원거리 공격 모습

#### 구현 방식

- 코루틴(CoRoutine)을 활용하여 공격 순간에만 BoxCollider 활성화
- 원거리 공격은 Bullet을 TrailRenderer를 활용하여 시각적 효과를 두었고, BoxCollider를 통하여 충돌 판정이 나게 만들었다.
- Prefab화 시켜서 쏘는 순간에 총알이 Instantiate되게 하였음
- 무기 교체는 캐릭터의 이동과 같은 방식으로 키를 설정하여 다른 무기로의 Swap을 해 주고, 캐릭터의 손에 있는 무기 Object의 SetActive를 true/false 시켜 주었음
- 무기는 습득 순서대로 저장되는 GameObject 배열과, 무기 코드 순서대로 저장되는 배열을 두어 관리하였음

[습득, 장착 관련 코드 링크](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/Boss_Step2.md)<br>
[공격 관련 코드 링크](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/Boss_Step3.md)

<hr>

### 몬스터, 보스

- 세 가지 타입의 몬스터
- 세 개의 패턴을 가진 보스


#### 구현 방식
- 코루틴(CoRoutine)을 사용하여 공격 딜레이 설정
- 몬스터 코드를 상속 받아 Boss 코드 구현
- 오브젝트 별로 몬스터 타입을 나타내는 enum을 설정하여 몬스터 별로 행동을 구별하였음

[몬스터 관련 내용 상세](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/Boss_Step5.md)<br>
[보스 관련 내용 상세](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/Boss_Step6.md)

<hr>

### 무기 강화

- 강화 시스템 구현
- 강화 UI 구현

#### 구현 방식

- PlayerItem.cs(아이템 관리용)를 별개로 만들어 강화 코드와 연계하였음

![image](https://user-images.githubusercontent.com/66288087/206086004-ccc16f9b-c536-4b5e-8517-015d2fda029f.png)

강화 시스템 구조

[강화 세부 내용](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/Boss_Step4.md)

<hr>

### 상점

- 아이템 판매 구현
- 일괄 판매 구현

![image](https://user-images.githubusercontent.com/66288087/206086426-8cc87e25-68f0-44bb-95ce-cdf8567651cb.png)

![image](https://user-images.githubusercontent.com/66288087/206086449-f52f83b5-f67a-4827-93f5-1cdc4b150c62.png)


#### 구현 방식

- UI를 관리하는 UIManager.cs 코드에서 아이템 코드를 매개변수로 받아 들여 아이템을 구매하는 함수를 제작하였음

[상점 관련 내용](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/_Boss_Step10.md)

<hr>

### 스탯 강화

- 스탯 창 UI에서 버튼을 통하여 스탯 강화가 가능하게 하였음

![image](https://user-images.githubusercontent.com/66288087/206086622-cb578d70-2a9c-4e2d-9fbd-972b6176d887.png)

#### 구현 방식

- UI Manager 내부에 함수를 만들어 구현

[스탯 강화 관련 내용](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/_Boss_Step10.md)

<hr>

### 던전 스테이지

- 여러 개의 방으로 구성
- 몬스터를 다 잡으면 다음 방으로 향하는 문이 열리게 하였음
- 보스 몬스터까지 잡게 되면 보상을 받을 수 있는 방으로 이동 가능
- 보상은 랜덤
- 캐릭터가 죽었을 때(HP = 0) 행동 구현

![image](https://user-images.githubusercontent.com/66288087/206086873-0760f55c-63e0-4006-93ce-bcea52ba35db.png)


#### 구현 방식

- Trigger를 통하여 Player가 닿게 되면 아래 사진과 같이 설정 한 존에 따라서 다르게 몬스터를 스폰 해 준다.

![image](https://user-images.githubusercontent.com/66288087/206086989-77545a5b-57a9-4c0e-ac6a-7f34668b17cd.png)

- 캐릭터가 죽었을 때의 판단은 PlayerCode.cs(캐릭터 코드)에서 이루어 지며, 캐릭터가 마을로 다시 돌아가게 하였다.(씬 이동)

[스테이지 관련 내용](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/Boss_Step9.md)

<hr>

### 파밍용 돌(스포너 자동 스폰)

- 던전에 들어가지 않고 골드를 벌 수 있는 수단
- 자동으로 일정 시간마다 스폰이 되게 만들어 주었음

#### 구현 방식

- 스포너에 Trigger를 마련 해 두고, 돌 Tag를 가진 물체가 위에 존재하면 시간을 카운트 하지 않고, 존재하지 않을 때, 시간을 카운트 하여 Instantiate를 통해 소환 해 주었다.
- 스포너와 돌 둘 다 Prefab화 시켜 주었다.

[파밍용 돌 관련 내용](https://github.com/kkyoulza/Unity/blob/main/3D/2_Boss/Boss_Step7.md)

<hr>

### 점프 맵





### 점프 점수 개인 Top3 랭킹 판




### UI









