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
