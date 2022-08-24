using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorStats : MonoBehaviour
{

    private float CharMaxAtk; // �������� ����� �Ϸ� �� �ִ� ���ݷ�
    private float CharMinAtk; // �������� ����� �Ϸ� �� �ּ� ���ݷ�

    private int power;
    private int Acc;
    private int Point;
    private int UsePoint = 0; // ����� Point
    private int AddPower = 0; // Power�� ����� Point
    private int AddAcc = 0; // Acc�� ����� Point

    public void SetPower(int inputPower)
    {
        power = inputPower; // �ʱ� ���� �� ����� �� �ҷ������
    }

    public void SetAcc(int inputAcc)
    {
        Acc = inputAcc; // �ʱ� ���� �� ����� �� �ҷ������
    }

    public void SetPoint(int inputPoint)
    {
        Point = inputPoint; // �ʱ� ���� �� ����� �� �ҷ����� ��
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

    public float CalAndGetMaxAtk() // ũ��Ƽ���� ������ �ʾ��� �� �ִ� ������
    {

        CharMaxAtk = (power + AddPower) * (1.0f);

        return CharMaxAtk;
    }

    public float CalAndGetMinAtk()
    {
        CharMinAtk = (power + AddPower) * (0.65f);

        return CharMinAtk;
    }

    public void UpPower() // ���� 1�� �÷��ִ� ��
    {
        if(Point > 0)
        {
            Point--;
            UsePoint++;
            AddPower++;
        }
        else
        {
            Debug.Log("���� ����Ʈ ����!");
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
            Debug.Log("���� ����Ʈ ����!");
        }
    }

    public void ResetPoint()
    {
        if(UsePoint > 0)
        {
            Debug.Log("Power " + AddPower + " ����Ʈ, Acc " + AddAcc + " ����Ʈ, �� " + UsePoint + " ����Ʈ�� �����Ͽ����ϴ�.");
            AddPower = 0;
            AddAcc = 0;
            Point += UsePoint;
            UsePoint = 0;
        }
        else
        {
            Debug.Log("���� �� ����Ʈ�� �����ϴ�.");
        }
    }


}
