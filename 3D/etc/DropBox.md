### Unity 3D Practice - 자판기 만들기

<hr>

#### 버튼을 눌러서 다른 모양이 나오게끔 유도하는 자판기를 만들어 보자

- 동적으로 생성하는(instantiate)방식을 사용하였음
- 버튼은 Main Camera 에 Physics Raycaster를 적용하여 특정 물체에 이벤트가 생기게 되면 애니메이션과 동적 생성이 되게끔 하였다.
- 버튼(Cylinder)에 Event Trigger를 넣어 주어 Pointer Click이 되면 애니메이션 효과, DropBox.cs의 DropDown()이 실행되게끔 하였다.
- 옆에 조그맣게 R 버튼을 추가 하여 동적으로 생성하는 Prefab의 색을 변경하려 하였으니 일반적인 색 변경 코드가 적용되지 않았음(Prefab이기 때문에 GamgObject로 인식되지 않아 NullPointException이 되지 않았던 것)
- 따라서 색깔별 Prefab을 만들어 두어, 동적으로 Prefab을 변경하는 방법을 적용해 보려 한다. --> 성공!


<hr>

### 흰색 큐브 Prefab이 생성되는 모습

![dropBox1](https://user-images.githubusercontent.com/66288087/181695599-1b50a1ea-2ba9-4cf7-a4f5-6ce43b0a7f7e.JPG)


### 빨간색 큐브 Prefab이 생성되는 모습

![dropBox2](https://user-images.githubusercontent.com/66288087/181695604-1a272177-775d-4d4f-b516-6fa6b64c1d23.JPG)


#### 하나의 Prefab에 색을 바꿔 씌우는 것이 아니라 두 개의 Prefab을 각각 만들어 두었다.

![dropBox3](https://user-images.githubusercontent.com/66288087/181695606-d68f4929-0635-4dd1-a031-fa5e31de68f8.JPG)
