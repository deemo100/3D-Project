using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRight : MonoBehaviour
{
    
    public float notespeed = 400;

    UnityEngine.UI.Image NoteImage;

    private void Start()
    {
        NoteImage = GetComponent<UnityEngine.UI.Image>();
    }
    
    void Update()
    {
        transform.localPosition += Vector3.left * notespeed * Time.deltaTime;
    }
    
    public void HideNonte()
    {
        NoteImage.enabled = false;
    }

}