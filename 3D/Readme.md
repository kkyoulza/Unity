### Unity 3D Practice - 자판기 만들기

<hr>

#### 버튼을 눌러서 다른 모양이 나오게끔 유도하는 자판기를 만들어 보자

- 동적으로 생성하는(instantiate)방식을 사용하였음
- 버튼은 Main Camera 에 Physics Raycaster를 적용하여 특정 물체에 이벤트가 생기게 되면 애니메이션과 동적 생성이 되게끔 하였다.
- 버튼(Cylinder)에 Event Trigger를 넣어 주어 Pointer Click이 되면 애니메이션 효과, DropBox.cs의 DropDown()이 실행되게끔 하였다.
- 옆에 조그맣게 R 버튼을 추가 하여 동적으로 생성하는 Prefab의 색을 변경하려 하였으니 일반적인 색 변경 코드가 적용되지 않았음(Prefab이기 때문에 GamgObject로 인식되지 않아 NullPointException이 되지 않았던 것)
- 따라서 색깔별 Prefab을 만들어 두어, 동적으로 Prefab을 변경하는 방법을 적용해 보려 한다.


