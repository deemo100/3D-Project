using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class NoteManger : MonoBehaviour
{
    public int bpm = 0;
    double currentTime = 0d;
    
    // 노트 생성 기준
    [SerializeField] private Transform NotcAppcar = null;
    // 노트 생성
    [SerializeField] private GameObject Note = null;
    // 타이밍 매니저 참조
    TimingManager timingManager;

    private void Start()
    {
        timingManager = GetComponent<TimingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
 
        if (currentTime >= 60d / bpm)
        {
            GameObject tNoteLeft = 
                Instantiate(Note, NotcAppcar.position, quaternion.identity);
            
            tNoteLeft.transform.SetParent(this.transform);
            timingManager.boxNoteList.Add(tNoteLeft);
            
            currentTime -= 60d / bpm;
        }
        

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            timingManager.boxNoteList.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }
}
