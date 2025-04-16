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
        // 타이밍 박스 설정
        timingBoxs = new Vector2[timingRect.Length];

        for (int i = 0; i < timingRect.Length; i++)
        {
            timingBoxs[i].Set(Center.localPosition.x - timingRect[i].rect.width / 2,
                              Center.localPosition.x + timingRect[i].rect.width / 2);
        }
    }

    public void CheckTiming()
    {
        bool isHit = false;
    
        for (int i = boxNoteList.Count - 1; i >= 0; i--)
        {
            GameObject note = boxNoteList[i];
            if (note == null) continue;
    
            float tNotePosX = note.transform.localPosition.x;
    
            for (int j = 0; j < timingBoxs.Length; j++)
            {
                if (timingBoxs[j].x <= tNotePosX && tNotePosX <= timingBoxs[j].y)
                {
                    Destroy(note);
                    boxNoteList.RemoveAt(i);
                    Debug.Log("Hit: " + j);
                    isHit = true;
                    break; // 이 노트는 판정 끝났으니 다음 노트로
                }
            }
        }
    
        if (!isHit)
            Debug.Log("Miss");
    }
    
}
