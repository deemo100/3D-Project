using System.Collections.Generic;
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    public List<GameObject> leftNoteList = new List<GameObject>();
    public List<GameObject> rightNoteList = new List<GameObject>();

    [SerializeField] private Transform CenterLeft;
    [SerializeField] private Transform CenterRight;

    [SerializeField] private RectTransform[] timingRectLeft;  // 인덱스: 0 = Bad, 1 = Good, 2 = Perfect
    [SerializeField] private RectTransform[] timingRectRight; // 인덱스 동일
    private Vector2[] timingBoxs;

    private Vector2[] timingBoxsLeft;
    private Vector2[] timingBoxsRight;

    
    void Start()
    {
        timingBoxsLeft = new Vector2[timingRectLeft.Length];
        timingBoxsRight = new Vector2[timingRectRight.Length];

        for (int i = 0; i < timingRectLeft.Length; i++)
        {
            timingBoxsLeft[i].Set(
                CenterLeft.localPosition.x - timingRectLeft[i].rect.width / 2,
                CenterLeft.localPosition.x + timingRectLeft[i].rect.width / 2
            );
        }

        for (int i = 0; i < timingRectRight.Length; i++)
        {
            timingBoxsRight[i].Set(
                CenterRight.localPosition.x - timingRectRight[i].rect.width / 2,
                CenterRight.localPosition.x + timingRectRight[i].rect.width / 2
            );
        }
    }

    public void CheckTiming(NoteDirection direction)
    {
        List<GameObject> noteList = direction == NoteDirection.Left ? leftNoteList : rightNoteList;
        Vector2[] timingBoxs = direction == NoteDirection.Left ? timingBoxsLeft : timingBoxsRight;

        for (int i = noteList.Count - 1; i >= 0; i--)
        {
            GameObject note = noteList[i];
            if (note == null) continue;

            float tNotePosX = note.transform.localPosition.x;

            for (int j = 0; j < timingBoxs.Length; j++)
            {
                if (timingBoxs[j].x <= tNotePosX && tNotePosX <= timingBoxs[j].y)
                {
                    Destroy(note);
                    noteList.RemoveAt(i);
                    Debug.Log($"[{direction}] 판정 성공! 영역: {j}");
                    break;
                }
            }
        }
    }
}