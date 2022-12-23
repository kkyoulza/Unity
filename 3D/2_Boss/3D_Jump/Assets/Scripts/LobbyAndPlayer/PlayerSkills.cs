using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public bool[] meleeSkills = new bool[2];
    // �������� ���� ����� �ߵ� ������ ��ų - ��ų ��Ÿ���� ���ƿԴ��� ���� �Ǵ�
    public bool[] rangeSkills = new bool[2];
    // �� ���� ����� �ߵ� ������ ��ų(�ߵ� ���� Ȯ��) - �������� ����(0��)

    public bool[] isMeleeSkillOn = new bool[2];
    // ������ų�� ���� ������ Ȯ��!
    public bool[] isRangeSkillOn = new bool[2];
    // ���Ÿ���ų�� ���� ������ Ȯ��!

    public float[] meleeCoolTime = new float[2]; // ������ų ��Ÿ��
    public float[] rangeCoolTime = new float[2]; // ���Ÿ���ų ��Ÿ��

    public float[] meleeSkillDurationTime = new float[2]; // �ٰŸ� ��ų ���� �ð�(��Ƽ�� ��ų�̸� 0���� ����)
    public float[] rangeSkillDurationTime = new float[2]; // ���Ÿ� ��ų ���� �ð�(��Ƽ�� ��ų�̸� 0���� ����)


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
        if (!meleeSkills[index]) // ��ų ��Ÿ���� �ƴ϶��
            meleeSkills[index] = true; // ��ų ��Ÿ������ �˷��ְ�
        else // ��ų�� ��Ÿ���̶��
            yield break; // �ڷ�ƾ�� ������.

        yield return new WaitForSeconds(meleeCoolTime[index]); // ��Ÿ�� ����

        meleeSkills[index] = false;

    }

    public IEnumerator onMeleeSkill(int index)
    {
        if (!isMeleeSkillOn[index]) // ��ų ��� ���� �ƴϸ�
            isMeleeSkillOn[index] = true; // ��ų ��������� �˷� �ְ�
        else // ��ų�� ������̶��
            yield break; // �ڷ�ƾ�� ������.

        yield return new WaitForSeconds(meleeSkillDurationTime[index]); // ���� �ð� ����

        isMeleeSkillOn[index] = false;

    }

    public IEnumerator onRangeSkillCoolTime(int index)
    {
        if (!rangeSkills[index]) // ��ų ��Ÿ���� �ƴ϶��
            rangeSkills[index] = true; // ��ų ��Ÿ������ �˷��ְ�
        else // ��ų�� ��Ÿ���̶��
        {
            Debug.Log("��Ÿ��!");
            yield break; // �ڷ�ƾ�� ������.
        }
            

        Debug.Log("��ų ���!");

        yield return new WaitForSeconds(rangeCoolTime[index]);

        Debug.Log("��Ÿ�� end");
        rangeSkills[index] = false;

    }

    public IEnumerator onRangeSkill(int index)
    {
        if (!isRangeSkillOn[index]) // ��ų ��� ���� �ƴϸ�
            isRangeSkillOn[index] = true; // ��ų ��������� �˷� �ְ�
        else // ��ų�� ������̶��
        {
            Debug.Log("��� ��!");
            yield break; // �ڷ�ƾ�� ������.
        }

        yield return new WaitForSeconds(rangeSkillDurationTime[index]); // ���� �ð� ����

        Debug.Log("��ų ��!");

        isRangeSkillOn[index] = false;

    }


}
