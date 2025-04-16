using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    TimingManager timingManager;
    
    private void Start()
    {
        timingManager = FindObjectOfType<TimingManager>();
        if (timingManager == null)
            Debug.LogError("TimingManager를 찾을 수 없습니다!");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D)
            || Input.GetKeyDown(KeyCode.F))
        {
                timingManager.CheckTiming();
        }
    }
}
