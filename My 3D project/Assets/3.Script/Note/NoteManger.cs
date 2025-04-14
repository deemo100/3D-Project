using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class NoteManger : MonoBehaviour
{
    public int bgm = 0;
    double currentTime = 0d;
    
    // 노트 생성 기준
    [SerializeField] private Transform NotcAppcarLeft;
 
    // 노트 생성
    [SerializeField] private GameObject NotcLeft;
    
    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= 60d / bgm)
        {
            GameObject tNotcLeft = 
                Instantiate(NotcLeft, NotcAppcarLeft.position, quaternion.identity);
            tNotcLeft.transform.SetParent(this.transform);
            currentTime -= 60d / bgm;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            Destroy(other.gameObject);
        }
    }
}
