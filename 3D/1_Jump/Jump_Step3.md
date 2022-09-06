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


몇 번 플레이를 해 본 결과, 안내문 출력이 너무 빠르게 갱신되어 미처 설명을 읽지 못하였음에도 불구하고 다음 안내문이 출력되는 상황이 발생하였다.
 
즉, 안내 트리거 사이의 간격이 너무 좁았던 것 같다는 생각이 들고, 플레이에 관련이 없는 내용을 출력하는 부분도 있어서 대사 코드를 아래와 같이 수정함과 동시에 트리거의 간격을 늘려 주었다.


<pre>
<code>
public void ShowNotices(int num)
    {
        switch (num)
        {
            case 1:
                panel.SetActive(true); // 첫 번째는 내용 변화가 X
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }

                break;
            case 2:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "동전을 먹으면 점수가 올라갑니다!\n 동전을 최대한 많이 먹으면서 골인 지점까지 가면 돼요!";
                break;
            case 3:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "안 보이는 곳에도 동전이 있을 수 있어요!\n 만점을 받기 위해서는 눈썰미가 좋아야겠죠?";
                break;
            case 4:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "방금 먹은 노란색 꼬깔은 세이브 포인트에요!\n 바닥에 떨어지면 세이브 포인트로 복귀한답니다!";
                break;
            case 5:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "전방에 움직이는 파란 색 발판이 보이나요?\n 튕겨 나가지 않게 조심하세요!";
                break;
            case 6:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "앞에 보라색 발판이 보이나요?\n 통! 통! 튀기면서 저 멀리 하늘 위로 올라가 봐요!";
                break;
            case 7:
                panel.SetActive(true);
                if (!isOn) // UI가 사라진 상태에서 UI 생성 시
                {
                    isOn = true;
                }
                else
                {
                    onTime = 0f;
                }
                noticeText.text = "앞에 노란색 발판은 크기가 줄었다가 늘었다가 하네요!\n 크기가 커지기를 기다렸다가 가는 것을 추천해요!";
                break;

        }
    }
</code>
</pre>

![image](https://user-images.githubusercontent.com/66288087/188596902-b0d44109-5ca2-4e88-ba70-7fa71a1cd0b4.png)

위 사진은 트리거의 위치 변화를 체크 한 사진이다.

간격이 수정되지 않은 부분이 있기는 해도 이 것은 내용을 플레이에 관련이 있게 수정 해 주었다.

또한, 밑에 언급되겠지만 6번 트리거에서도 좁은 길에 위치 했던 트리거를 일반 발판을 추가 하여 내용을 조금 더 여유롭게 볼 수 있게 하였다.

그 외에도 세이브 포인트와 겹친 트리거는 세이브 포인트 트리거 크기에 맞게 수정 해 주어서, 세이브 포인트를 먹지 않았음에도 세이브 포인트를 먹었다는 문구가 나오는 현상을 수정하였다.

![image](https://user-images.githubusercontent.com/66288087/188599780-d1401d38-2289-4c53-82df-bb7dce463376.png)

그리고 바닥에 떨어지면 처음 위치(or 세이브 포인트)로 돌아 간다는 부분은 직접 바닥에 떨어졌을 때, 2초 가량 나오게 하는 것으로 수정하였다.

이것 역시 2초 내에 다시 떨어지게 되면 남은 시간이 다시 리필되게 하였다.

<hr>

**배경음악, 효과음 추가**

이제, 게임에서 빼 놓을 수 없는 배경음악과 효과음을 추가 해 보도록 하자.

배경음악은 [여기](https://assetstore.unity.com/packages/audio/music/casual-game-bgm-5-135943)에서, 효과음은 [여기](https://assetstore.unity.com/packages/audio/sound-fx/free-casual-game-sfx-pack-54116)에서 무료 에셋을 받아 사용하였다.

배경음악은 메인 카메라에 Audiuo Source를 넣어 준 다음, Play on Awake를 통하여 장면이 시작되면 바로 실행되게 하였다.

![image](https://user-images.githubusercontent.com/66288087/188599988-ced6dc5e-9fb0-4b30-92ed-c5bbda528628.png)

또한 배경음악은 계속 해서 반복 되어야 하기에 Loop를 체크 해 주었다.

이제는 코인을 먹거나 세이브 포인트를 먹었을 때, 효과음이 출력 되게끔 해 보자.

플레이어에게 코드를 하나 더 추가 해 주어, 오디오 클립들을 할당 해 준 다음, 부딪힌 물체의 태그에 맞게 Audio Source의 Clip에 할당 해 주어서 재생을 해 준다.
 
새롭게 작성한 코드는 아래와 같다.


**playerSFX.cs**
<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    AudioSource audioS;
    public AudioClip silverSFX;
    public AudioClip goldSFX;
    public AudioClip savePointSFX;
    public AudioClip fallToUnder;


    // Start is called before the first frame update
    void Start()
    {
        audioS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "silver")
        {
            audioS.clip = silverSFX;
            audioS.Play();
        }
        else if(other.gameObject.tag == "gold")
        {
            audioS.clip = goldSFX;
            audioS.Play();
        }
        else if (other.gameObject.tag == "SavePoint")
        {
            audioS.clip = savePointSFX;
            audioS.Play();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "under")
        {
            audioS.clip = fallToUnder;
            audioS.Play();
        }
    }
}
</code>
</pre>

또한, 아래로 떨어졌을 때도 효과음이 출력되게끔 하였다.

<hr>

**스테이지 밸런싱**

앞서 코인을 만들었다. 그 코인을 어떻게 배치하느냐에 따라서 난이도가 달라질 수 있다.

![image](https://user-images.githubusercontent.com/66288087/188600632-25052b08-c91c-4507-be35-d605b30e7338.png)

기본적으로 코인은 진행 방향에 맞게 발판 위, 점프 경로에 배치하였다.

그렇지만 더 많은 점수를 주는 골드 코인은 사각지대 또는 진행방향에서 자연스럽게 먹지 못하는 경로에 배치해 두어 난이도를 소폭 상승시켰다.

![image](https://user-images.githubusercontent.com/66288087/188600681-3e657003-4726-44f2-96b4-3ba3094d170b.png)

진행 경로에서 이탈 된 곳에 골드를 배치하였다.

![image](https://user-images.githubusercontent.com/66288087/188600696-0ad1d963-b762-4928-9708-d5572e7cf983.png)

사각지대에 골드를 배치하였다.


앞서 설명을 출력하게 만드는 트리거의 위치를 조정하였다고 하였다.

여기서 이전 포스팅에서와 달리 발판에 대한 위치도 조정이 들어가게 되었다.

![image](https://user-images.githubusercontent.com/66288087/188600865-861c7857-abe3-44c8-b340-cf1ddfbaddc3.png)

우선 좁은 발판이 끝나고 바로 바운스 발판이 나왔던 것과는 달리 일반 발판을 하나 추가 해 두어, 설명을 보면서 숨을 고를 수 있게 하였다.

또한, 바운스 발판 위쪽에 투명한 Box Collider를 만들어 두어, 매우 높게 뛰어서 바로 골인 지점에 도착하는 것을 방지하였다.

![image](https://user-images.githubusercontent.com/66288087/188600940-dd2336cb-a30a-43c5-bf14-5215a12d3c74.png)

또한, 처음에는 노란 발판을 어렵게 만들어 두어 어려운 길로 가느냐, 아니면 돌아서 가지만 일반 발판들로만 이루어진 쪽으로 가느냐를 선택하게 하려 하였지만 노란 색 발판이 생각보다 어렵지 않게 갈 수 있게 만들어 져서 샛길이 의미 없다고 판단하여 삭제하였다.

![image](https://user-images.githubusercontent.com/66288087/188601012-e3febd13-f73c-46b0-b95c-33042b87d624.png)

또한, 파티클과 Box Trigger를 통하여 골인지점을 제작 해 주었다.

![image](https://user-images.githubusercontent.com/66288087/188601062-c1dde72b-00aa-4309-bfa3-3b311916b1db.png)

파티클의 모양을 Edge로 설정 해 주고 Rotation을 해 주었더니 위 사진 처럼 포탈과 같은 형태가 나오게 되었다.

다에는 2 스테이지 Scene을 만들고, Scene 이동, 점수 저장 등의 기능을 구현 해 보도록 하겠다.



