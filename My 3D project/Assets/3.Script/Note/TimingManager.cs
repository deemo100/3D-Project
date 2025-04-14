using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    
    public List<GameObject> boxNoteList = new List<GameObject>();
    
    [SerializeField] private Transform Center;
    [SerializeField] private RectTransform[] timingRect;
    Vector2[] timingBoxs;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
