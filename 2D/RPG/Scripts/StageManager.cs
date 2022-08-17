using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement; // Scene�� �����ϱ� ����!

public class StageManager : MonoBehaviour
{
    float RandomFloatX, RandomFloatY; // ���� ��ġ
    
    public GameObject TargetPrefab1; // Ÿ�� 1
    public GameObject TargetPrefab2; // Ÿ�� 2
    public GameObject TargetPrefab3; // Ÿ�� 3
    public GameObject Bomb1; // ��ź 1
    private GameObject Target; // �������� ���� �� Ÿ��


    private List<String> Target1Name = new List<String>();
    private List<String> Bomb1Name = new List<String>();

    public GameObject Clear;
    public Text ScoreTxt; // Ŭ����� ������ ���ھ� �ؽ�Ʈ
    public GameObject GameOver;

    public Text Score; // ���ܿ� �ִ� ���ھ� �ؽ�Ʈ

    int remainTarget1 = -1; // ���� Ÿ���� ���� �ǽð����� ������ �ֱ� ���� ��, �� ���� 0�� �Ǹ� ���� ���������� ����.
    // -1 > üũ�� ���� �ʴ� ����

    BulletManager bullet;

    // Start is called before the first frame update
    void Start()
    {
        bullet = GetComponent<BulletManager>(); // bullet Manager�� �����´�.
        Stage1();

    }

    // Update is called once per frame
    void Update()
    {
        
        Stage1Check();
        GameOverCheck();

    }

    public void SpawnTarget1(int num)
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        RandomFloatX = UnityEngine.Random.Range(-8.2f, 8.4f);
        RandomFloatY = UnityEngine.Random.Range(-4.4f, 4.4f);

        Target = Instantiate(TargetPrefab1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;

        Target.name = "Target1_" + num; // �̸� + ���ڸ� �����Ͽ� �̸� ����(�������� �ʱ�ȭ �� ���� ���� �ʱ�ȭ)
        Target1Name.Add(Target.name); // ����Ʈ�� ����(���� �����ÿ��� ��ź�� ������ �� �����ϱ� ����)

        if (RandomFloatX > 0)
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, 0));
        else
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 0));
    }

    public void SpawnBomb1(int num)
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        RandomFloatX = UnityEngine.Random.Range(-8.2f, 8.4f);
        RandomFloatY = UnityEngine.Random.Range(-4.4f, 4.4f);

        Target = Instantiate(Bomb1, new Vector2(RandomFloatX, RandomFloatY), Quaternion.identity) as GameObject;

        Target.name = "Bomb1_" + num; // �̸� + ���ڸ� �����Ͽ� �̸� ����(�������� �ʱ�ȭ �� ���� ���� �ʱ�ȭ)
        Bomb1Name.Add(Target.name); // ����Ʈ�� ����(���� �����ÿ��� ��ź�� ������ �� �����ϱ� ����)

        if (RandomFloatX > 0)
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500, 0));
        else
            Target.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 0));
    }

    public void Stage1()
    {
        remainTarget1 = 10;
        GetComponent<BulletManager>().AddBullet(15);
        for(int i = 0; i < remainTarget1; i++)
        {
            SpawnTarget1(i); // Ÿ�� 10�� ��ȯ 
            Debug.Log("Ÿ��" + i + "�� ��ȯ");
            if (i < 5) // ��ź 5�� ��ȯ
                SpawnBomb1(i);
        }

    }

    public void GameOverCheck()
    {
        if(remainTarget1 > 0 && (GetComponent<BulletManager>().GetBulletCount() == 0))
        {
            //Ÿ���� ������ �� �Ѿ��� �� �Һ��Ͽ��ٸ�..!
            Debug.Log("�Ѿ��� �� �Һ��Ͽ����ϴ�! ���� ����!");

            GameOver.SetActive(true);

            for (int i = 0; i < Target1Name.Count; i++)
            {
                try
                {
                    Destroy(GameObject.Find(Target1Name[i]));
                }
                catch
                {
                    Debug.Log("���� Ÿ���� �־ ��ŵ!");
                    continue;
                }

            }

            for (int i = 0; i < Bomb1Name.Count; i++)
            {
                try
                {
                    Destroy(GameObject.Find(Bomb1Name[i]));
                }
                catch
                {
                    Debug.Log("�̹� ���� ��ź�� �־ ���� ���� �����մϴ�!");
                    continue;
                }

            }
            Bomb1Name.Clear();
            Target1Name.Clear();

            remainTarget1 = -1; // checkOff���·� ����!


            Invoke("BackToTheMap", 2); // 2���� BackToTheMap ȣ��



        }
    }

    public void Stage1Check()
    {
        if(remainTarget1 == 0)
        {
            Debug.Log("�������� 1 Ŭ����! ��ź�� �����մϴ�.");

            Clear.SetActive(true);
            ScoreTxt.text = (int.Parse(Score.text) + GetComponent<BulletManager>().GetBulletCount() * 10).ToString();
            // Ÿ�� ���ھ� + ���� �Ѿ� ���� * 10


            for(int i = 0; i < Bomb1Name.Count; i++)
            {
                try
                {
                    Destroy(GameObject.Find(Bomb1Name[i]));
                }
                catch
                {
                    Debug.Log("�̹� ���� ��ź�� �־ ���� ���� �����մϴ�!");
                    continue;
                }

            }
            Bomb1Name.Clear();
            Target1Name.Clear();

            remainTarget1 = -1; // checkOff���·� ����!
        }
    }

    public void MinusTarget1()
    {
        remainTarget1--;
        bullet.discountBullet(); // ������ �� �Ѿ� ����
    }

    public void BackToTheMap()
    {
        SceneManager.LoadScene("MainMap");
    }

}
