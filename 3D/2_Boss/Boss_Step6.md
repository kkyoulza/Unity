# Step 6. 보스


### 에셋 및 Collider 세팅

앞서 만들었던 몬스터를 확장하여 보스를 제작 해 보자

우선 받은 에셋에서 보스 Prefab을 끌어다 Scene에 넣어 준다.

![image](https://user-images.githubusercontent.com/66288087/194699709-365522f7-beb9-43bf-bdc2-b1d339d91c0f.png)

그런데 받은 에셋에서 Boss의 Scale을 변경하게 되면 애니메이션 크기와 Scene에 있는 보스의 크기가 맞지 않는 현상이 있다.

![image](https://user-images.githubusercontent.com/66288087/194700603-5f60c8a0-e7d9-4143-8732-c880ef9ae242.png)
![image](https://user-images.githubusercontent.com/66288087/194700567-b9f3719e-213c-4754-8b00-85e390d7ea99.png)

이 때는 위 사진처럼 Boss하위 오브젝트인 Mesh Object의 Scale을 1,1,1로 맞추어 준 다음 겉에 있는 Boss의 Scale을 바꾸어 주면 애니메이션 크기와 차이 없이 크기를 바꿀 수 있게 된다.

이제 보스 패턴을 위한 준비를 해 보도록 하자.

골드메탈님이 기획한 패턴은 3가지가 있다.

미사일 발사, 돌 굴리기, 순간이동 & 찍기

미사일 발사를 위해서는 미사일을 Prefab화 하여 생성하면 되고, 돌 역시 Prefab화 하여 생성하면 된다.

![image](https://user-images.githubusercontent.com/66288087/194701372-c031d8a9-6a23-4643-be60-687090bb1d3c.png)

순간이동과 찍기 판정을 하기 위해 Boss 자식 오브젝트에 빈 오브젝트를 만들고 BoxCollider를 Trigger로 추가하여 넣어 준다.




