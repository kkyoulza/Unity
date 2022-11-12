using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject nextDoor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            // open the door
            Animator ani = nextDoor.GetComponent<Animator>();
            ani.SetTrigger("Open");
            AudioSource aud = nextDoor.GetComponent<AudioSource>();
            aud.Play();
        }
    }
}
