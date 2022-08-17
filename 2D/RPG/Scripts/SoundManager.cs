using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource audio;

    public AudioClip Score1; // 효과음들
    public AudioClip Score2;
    public AudioClip Score3;
    public AudioClip Minus1;
    public AudioClip Minus2;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayScore1()
    {
        audio.clip = Score1; // 오디오 소스에 Score1 효과음 세팅
        audio.Play(); // 재생
    }

    public void PlayScore2()
    {
        audio.clip = Score2;
        audio.Play();
    }

    public void PlayScore3()
    {
        audio.clip = Score3;
        audio.Play();
    }

    public void PlayMinus1()
    {
        audio.clip = Minus1;
        audio.Play();
    }

    public void PlayMinus2()
    {
        audio.clip = Minus2;
        audio.Play();
    }


}
