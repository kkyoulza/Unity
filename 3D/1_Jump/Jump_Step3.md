# Step3. 아이템 생성 및 스테이지 밸런싱

어느 게임에서든 아이템은 정말 중요한 요소이다.

점수를 올려 주거나, 특별한 행동을 할 수 있게 하는 등 중요한 역할을 한다.

따라서 이곳에서도 아이템을 추가 해 보도록 하겠다.

![image](https://user-images.githubusercontent.com/66288087/188579529-4abdae60-be23-4bb4-bb47-0f8e43b14d25.png)

![image](https://user-images.githubusercontent.com/66288087/188579674-d9079a15-7f43-431d-bba8-09896a66860e.png)

Polygon Starter Pack에서 동전 모델을 가져와서 금색, 은색을 씌워 주어 위 사진들과 같이 금, 은화를 만들었다.

금화는 조금 더 높은 점수를 올려 주기에 Particle System을 이용하여 파티클 효과를 더해 주었다.

![image](https://user-images.githubusercontent.com/66288087/188579877-81e05211-ef96-474e-a334-5d77cf988a21.png)

효과는 위 사진과 같이 설정 해 주었다. Loop를 체크 해 주어야 계속해서 파티클이 생성된다.


이제, 플레이어에서 아이템에 닿았을 때, 점수를 올려주는 코드를 작성 해 보자. (0.0.2에 있다.)

**Player.cs 코드 중 일부**
<pre>
<code>
private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SavePoint")
        {
            ReturnPos = other.gameObject.transform.position;
            other.gameObject.SetActive(false); // 세이브 포인트를 먹었으니 비활성화
        }

        if(other.gameObject.tag == "gold")
        {
            managing.addScore(1);
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "silver")
        {
            managing.addScore(0);
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Notice")
        {
            if(other.gameObject.name == "1" && showNotice < 1)
            {
                managing.ShowNotices(1);
                showNotice++;
            }
            else if(other.gameObject.name == "2" && showNotice < 2)
            {
                managing.ShowNotices(2);
                showNotice++;
            }
            else if (other.gameObject.name == "3" && showNotice < 3)
            {
                managing.ShowNotices(3);
                showNotice++;
            }
            else if (other.gameObject.name == "4" && showNotice < 4)
            {
                managing.ShowNotices(4);
                showNotice++;
            }
            else if (other.gameObject.name == "5" && showNotice < 5)
            {
                managing.ShowNotices(5);
                showNotice++;
            }
            else if (other.gameObject.name == "6" && showNotice < 6)
            {
                managing.ShowNotices(6);
                showNotice++;
            }
            else if (other.gameObject.name == "7" && showNotice < 7)
            {
                managing.ShowNotices(7);
                showNotice++;
            }

        }

    }
</code>
</pre>

SavePoint를 먹었을 때 실행되는 코드에 아이템도 추가 해 주면 된다.


Managing.cs 코드에 아래 함수를 추가 해 준다.
<pre>
<code>
 public void addScore(int num)
    {
        switch (num)
        {
            case 0: // silver
                score = int.Parse(scoreText.text);
                score++;
                scoreText.text = score.ToString();
                break;
            case 1: // gold
                score = int.Parse(scoreText.text);
                score += 10;
                scoreText.text = score.ToString();
                break;
        }
    }
</code>
</pre>


<hr>

**UI 안내문 수정**






