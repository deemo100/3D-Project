using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterFlame : MonoBehaviour
{
    
    AudioSource myaudio;
    [SerializeField] bool musicStart = false;

    private void Start()
    {
            myaudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!musicStart)
        {
            if(other.CompareTag("Note"))
            {
                myaudio.Play();
                musicStart = true;
            }
        }
    }
}
