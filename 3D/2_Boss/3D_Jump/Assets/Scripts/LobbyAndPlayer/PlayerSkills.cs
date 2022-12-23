using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public bool[] meleeSkills = new bool[2];
    // 근접공격 무기 착용시 발동 가능한 스킬 - 스킬 쿨타임이 돌아왔는지 여부 판단
    public bool[] rangeSkills = new bool[2];
    // 총 무기 착용시 발동 가능한 스킬(발동 여부 확인) - 스프레드 슈팅(0번)

    public bool[] isMeleeSkillOn = new bool[2];
    // 근접스킬이 지속 중인지 확인!
    public bool[] isRangeSkillOn = new bool[2];
    // 원거리스킬이 지속 중인지 확인!

    public float[] meleeCoolTime = new float[2]; // 근접스킬 쿨타임
    public float[] rangeCoolTime = new float[2]; // 원거리스킬 쿨타임

    public float[] meleeSkillDurationTime = new float[2]; // 근거리 스킬 지속 시간(액티브 스킬이면 0으로 설정)
    public float[] rangeSkillDurationTime = new float[2]; // 원거리 스킬 지속 시간(액티브 스킬이면 0으로 설정)


    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator onMeleeSkillCoolTime(int index)
    {
        if (!meleeSkills[index]) // 스킬 쿨타임이 아니라면
            meleeSkills[index] = true; // 스킬 쿨타임임을 알려주고
        else // 스킬이 쿨타임이라면
            yield break; // 코루틴을 끝낸다.

        yield return new WaitForSeconds(meleeCoolTime[index]); // 쿨타임 계산용

        meleeSkills[index] = false;

    }

    public IEnumerator onMeleeSkill(int index)
    {
        if (!isMeleeSkillOn[index]) // 스킬 사용 중이 아니면
            isMeleeSkillOn[index] = true; // 스킬 사용중임을 알려 주고
        else // 스킬이 사용중이라면
            yield break; // 코루틴을 끝낸다.

        yield return new WaitForSeconds(meleeSkillDurationTime[index]); // 지속 시간 계산용

        isMeleeSkillOn[index] = false;

    }

    public IEnumerator onRangeSkillCoolTime(int index)
    {
        if (!rangeSkills[index]) // 스킬 쿨타임이 아니라면
            rangeSkills[index] = true; // 스킬 쿨타임임을 알려주고
        else // 스킬이 쿨타임이라면
        {
            Debug.Log("쿨타임!");
            yield break; // 코루틴을 끝낸다.
        }
            

        Debug.Log("스킬 사용!");

        yield return new WaitForSeconds(rangeCoolTime[index]);

        Debug.Log("쿨타임 end");
        rangeSkills[index] = false;

    }

    public IEnumerator onRangeSkill(int index)
    {
        if (!isRangeSkillOn[index]) // 스킬 사용 중이 아니면
            isRangeSkillOn[index] = true; // 스킬 사용중임을 알려 주고
        else // 스킬이 사용중이라면
        {
            Debug.Log("사용 중!");
            yield break; // 코루틴을 끝낸다.
        }

        yield return new WaitForSeconds(rangeSkillDurationTime[index]); // 지속 시간 계산용

        Debug.Log("스킬 끝!");

        isRangeSkillOn[index] = false;

    }


}
