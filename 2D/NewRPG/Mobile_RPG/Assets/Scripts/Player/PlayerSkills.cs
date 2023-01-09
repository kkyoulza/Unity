using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter Ŭ���� ����� ���� ���ӽ����̽� �߰�

[System.Serializable]
public class Skills
{
    public bool isGain; // ��ų ȹ�� ����
    public bool isUse; // ��ų ����� ����

    public float skillDelay; // ��ų �ִϸ��̼� ������

    public PlayerSkills.classType skillClass; // ��� ���� ���� ��ų�ΰ�?
    public int skillLevel; // �ش� ��ų�� ��ȭ ����

    public int atkCnt; // ��ų�� ���� Ƚ��
    public float skillDmg; // ��ų�� ������
    
    public GameObject skillObj; // ��ų�� ������Ʈ
    public string animTrigger; // �ִϸ��̼� ���� Ʈ���� �̸�
}

public class PlayerSkills : MonoBehaviour
{
    public Skills[] skillInfos;
    Player playerBase;

    public enum classType {common, warrior, magician};

    // Start is called before the first frame update
    void Awake()
    {
        playerBase = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ableAtkSkill(int skillIndex,float skillDuration,float afterDelay) // ��ų �ߵ� (��Ÿ�� ��ų�� ���)
    {
        Animator anim;
        skillInfos[skillIndex].skillObj.SetActive(true);
        anim = skillInfos[skillIndex].skillObj.GetComponent<Animator>();
        anim.SetTrigger(skillInfos[skillIndex].animTrigger);
        playerBase.isAttack = true; // ���������� üũ�Ͽ� �� ���ȿ��� ������ȯ�� �����Ͽ���.

        yield return new WaitForSeconds(skillDuration);

        skillInfos[skillIndex].skillObj.SetActive(false);
        playerBase.isAttack = false; // ������ȯ ������ �� ��ų ����Ʈ ������ �� ������!

        yield return new WaitForSeconds(afterDelay); // �� ������

        skillInfos[skillIndex].isUse = false;
        
    } 




}
