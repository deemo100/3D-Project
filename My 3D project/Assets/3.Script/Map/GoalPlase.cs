using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPlase : MonoBehaviour
{
    AudioSource audioSource;
    NoteManager noteManager;
    
    // Start is called before the first frame update
    void Start()
    {
        noteManager = FindObjectOfType<NoteManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();
            PlayerController.noMovePlayer = false;
            
            if (noteManager != null)
                noteManager.gameObject.SetActive(false);
        }
    }
}
