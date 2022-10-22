# Step8. UI


이제 아이템 창, 장비 창, 단축키를 보여주는 등의 UI들을 설정하는 작업을 진행 하도록 하겠다.

또한, 앞서 만들어 놓은 강화 창에서 기원 조각 기능을 적용시키고, 그에 대한 디테일을 설정 해 주도록 하겠다.

#### 캐릭터 상태 창

캐릭터 상태 창은 캐릭터의 초상화, 그리고 HP, MP 바가 체력, 마나에 비례하여 변하는 기능을 추가 하도록 하겠다.

그리고 캐릭터 초상화를 누르게 되면 캐릭터의 세부 스탯이 나오게끔 설정 할 예정이다.

![image](https://user-images.githubusercontent.com/66288087/197345349-165bf9d8-ff99-45bb-a4f8-e498e475b949.png)

일단 상태창의 전반적인 모습은 위 사진과 같다.

**체력바를 체력에 맞게 크기 설정하기**

체력바의 크기는 기본적으로 최대 모습인 경우를 설정 해 둔 다음, 현재 체력/최대 체력의 비율을 곱해서 크기 설정을 해 준다.

UIManager.cs에 아래 함수를 추가하여 체력/마력에 맞게 바의 길이를 조절 해 준다.
<pre>
<code>
public void SetBar()
  {
      // 체력에 따라서 Bar의 크기를 설정하는 것이다.
      rectHP.sizeDelta = new Vector2(194 * playerInfo.playerHealth/playerInfo.playerMaxHealth,24);
      rectMP.sizeDelta = new Vector2(194 * playerInfo.playerMana / playerInfo.playerMaxMana, 24);
      cntHP.text = playerInfo.playerHealth.ToString();
      maxHP.text = playerInfo.playerMaxHealth.ToString();
      cntMP.text = playerInfo.playerMana.ToString();
      maxMP.text = playerInfo.playerMaxMana.ToString();
  }
</code>
</pre>

주의할 점은 아래 사진과 같이 좌표의 기준점을 왼쪽으로 잡아 놓아야 체력이 깎이는 것처럼 보이게 된다.

![image](https://user-images.githubusercontent.com/66288087/197345905-863dfa77-7f7a-47d6-a4ee-8dadcfd367c6.png)






#### 아이템 창, 캐릭터 스탯 창, 장비 창

![image](https://user-images.githubusercontent.com/66288087/197345428-ed24d972-f022-4a8d-baba-5ccf59d08ac1.png)

완성된 아이템 창, 스탯 창의 모습




#### 무기 교체 단축 키, 남은 총알 표시, 스킬 쿨타임 표시






