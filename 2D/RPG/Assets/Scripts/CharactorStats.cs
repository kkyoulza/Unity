using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorStats : MonoBehaviour
{

    private float CharMaxAtk; // 수식으로 계산이 완료 된 최대 공격력
    private float CharMinAtk; // 수식으로 계산이 완료 된 최소 공격력

    private int power;
    private int Acc;
    private int Point;
    private int UsePoint = 0; // 사용한 Point
    private int AddPower = 0; // Power에 사용한 Point
    private int AddAcc = 0; // Acc에 사용한 Point

    public void SetPower(int inputPower)
    {
        power = inputPower; // 초기 세팅 및 저장된 값 불러오기용
    }

    public void SetAcc(int inputAcc)
    {
        Acc = inputAcc; // 초기 세팅 및 저장된 값 불러오기용
    }

    public void SetPoint(int inputPoint)
    {
        Point = inputPoint; // 초기 세팅 및 저장된 값 불러오기 용
    }


    public int GetPower()
    {
        return (power + AddPower);
    }

    public int GetAcc()
    {
        return (Acc+AddAcc);
    }

    public int GetPoint()
    {
        return Point;
    }

    public float CalAndGetMaxAtk() // 크리티컬이 터지지 않았을 때 최대 데미지
    {

        CharMaxAtk = (power + AddPower) * (1.0f);

        return CharMaxAtk;
    }

    public float CalAndGetMinAtk()
    {
        CharMinAtk = (power + AddPower) * (0.65f);

        return CharMinAtk;
    }

    public void UpPower() // 스탯 1을 올려주는 것
    {
        if(Point > 0)
        {
            Point--;
            UsePoint++;
            AddPower++;
        }
        else
        {
            Debug.Log("스탯 포인트 부족!");
        }
    }

    public void UpAcc()
    {
        if (Point > 0)
        {
            Point--;
            UsePoint++;
            AddAcc++;
        }
        else
        {
            Debug.Log("스탯 포인트 부족!");
        }
    }

    public void ResetPoint()
    {
        if(UsePoint > 0)
        {
            Debug.Log("Power " + AddPower + " 포인트, Acc " + AddAcc + " 포인트, 총 " + UsePoint + " 포인트를 리셋하였습니다.");
            AddPower = 0;
            AddAcc = 0;
            Point += UsePoint;
            UsePoint = 0;
        }
        else
        {
            Debug.Log("리셋 할 포인트가 없습니다.");
        }
    }


}
