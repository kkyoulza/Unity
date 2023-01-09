using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter 클래스 사용을 위해 네임스페이스 추가

[System.Serializable]
public class Skills
{
    public bool isGain; // 스킬 획득 여부
    public bool isUse; // 스킬 사용중 여부

    public float skillDelay; // 스킬 애니메이션 딜레이

    public PlayerSkills.classType skillClass; // 어느 직업 전용 스킬인가?
    public int skillLevel; // 해당 스킬의 강화 레벨

    public int atkCnt; // 스킬의 공격 횟수
    public float skillDmg; // 스킬의 데미지
    
    public GameObject skillObj; // 스킬의 오브젝트
    public string animTrigger; // 애니메이션 실행 트리거 이름
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

    public IEnumerator ableAtkSkill(int skillIndex,float skillDuration,float afterDelay) // 스킬 발동 (평타도 스킬로 취급)
    {
        Animator anim;
        skillInfos[skillIndex].skillObj.SetActive(true);
        anim = skillInfos[skillIndex].skillObj.GetComponent<Animator>();
        anim.SetTrigger(skillInfos[skillIndex].animTrigger);
        playerBase.isAttack = true; // 공격중임을 체크하여 이 동안에는 방향전환을 제한하였다.

        yield return new WaitForSeconds(skillDuration);

        skillInfos[skillIndex].skillObj.SetActive(false);
        playerBase.isAttack = false; // 방향전환 제한은 딱 스킬 이펙트 딜레이 때 까지만!

        yield return new WaitForSeconds(afterDelay); // 후 딜레이

        skillInfos[skillIndex].isUse = false;
        
    } 




}
