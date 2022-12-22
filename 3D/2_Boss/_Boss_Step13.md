# Step 13. 스킬 제작


알피지에서 빠질 수 없는 것이 스킬이다.

**<목표>**
무기별로 사용할 수 있는 스킬을 2개씩 만들고, 스킬 쿨타임을 볼 수 있는 기능까지 만들어 보고자 한다.

<hr>

### PlayerSkills.cs 제작

플레이어의 스킬을 관리하는 스크립트를 제작 해 보도록 하자.

<pre>
<code>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public bool[] meleeSkills = new bool[2];
    // 근접공격 무기 착용시 발동 가능한 스킬(스킬이 발동중인지 여부를 판단)
    public bool[] rangeSkills = new bool[2];
    // 총 무기 착용시 발동 가능한 스킬(발동 여부 확인) - 스프레드 슈팅(0번)

    public float[] meleeCoolTime = new float[2];
    public float[] rangeCoolTime = new float[2];


    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator onMeleeSkill(int index)
    {
        if (!meleeSkills[index]) // 스킬 사용 중이 아니면
            meleeSkills[index] = true; // 스킬 사용중임을 알려 주고
        else // 스킬이 사용중이라면
            yield break; // 코루틴을 끝낸다.

        yield return new WaitForSeconds(meleeCoolTime[index]);

        meleeSkills[index] = false;

    }

    public IEnumerator onRangeSkill(int index)
    {
        if (!rangeSkills[index]) // 스킬 사용 중이 아니면
            rangeSkills[index] = true; // 스킬 사용중임을 알려 주고
        else // 스킬이 사용중이라면
            yield break; // 코루틴을 끝낸다.

        Debug.Log("스킬 사용!");

        yield return new WaitForSeconds(rangeCoolTime[index]);

        Debug.Log("쿨타임 on");
        rangeSkills[index] = false;

    }


}

</code>
</pre>

일단, 근거리 무기와 원거리 무기의 스킬 사용 여부를 나타내는 bool 배열 2개, 그리고 각 스킬의 쿨타임을 나타내는 int 배열 2개를 만들었다.

버프 형식은 가동 시간도 있어야 하니 일단 스킬 구현 후, bool 변수를 추가하여 조정 해 주도록 하자.







