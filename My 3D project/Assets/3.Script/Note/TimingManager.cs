using System.Collections.Generic;
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> leftNoteList = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> rightNoteList = new List<GameObject>();

    [SerializeField] private Transform CenterLeft;
    [SerializeField] private Transform CenterRight;

    [SerializeField] private RectTransform[] timingRectLeft;   // 인덱스: 0=Bad, 1=Good, 2=Perfect
    [SerializeField] private RectTransform[] timingRectRight;

    private Vector2[] timingBoxsLeft;
    private Vector2[] timingBoxsRight;

    [Header("노트 풀")]
    [SerializeField] private ObjectPool notePoolLeft;
    [SerializeField] private ObjectPool notePoolRight;

    [Header("노트 생성")]
    [SerializeField] private Transform spawnPointLeft;
    [SerializeField] private Transform spawnPointRight;
    [SerializeField] private float spawnInterval = 1.0f;

    private float timer = 0f;

    private EffectManager effectManager;

    void Start()
    {
        timingBoxsLeft = new Vector2[timingRectLeft.Length];
        timingBoxsRight = new Vector2[timingRectRight.Length];

        for (int i = 0; i < timingRectLeft.Length; i++)
        {
            timingBoxsLeft[i] = new Vector2(
                CenterLeft.localPosition.x - timingRectLeft[i].rect.width / 2,
                CenterLeft.localPosition.x + timingRectLeft[i].rect.width / 2
            );
        }

        for (int i = 0; i < timingRectRight.Length; i++)
        {
            timingBoxsRight[i] = new Vector2(
                CenterRight.localPosition.x - timingRectRight[i].rect.width / 2,
                CenterRight.localPosition.x + timingRectRight[i].rect.width / 2
            );
        }

        effectManager = FindObjectOfType<EffectManager>();
    }

    void Update()
    {
        // 비활성화된 노트 리스트에서 제거
        CleanNoteList(leftNoteList);
        CleanNoteList(rightNoteList);
    }

    void CleanNoteList(List<GameObject> noteList)
    {
        for (int i = noteList.Count - 1; i >= 0; i--)
        {
            GameObject note = noteList[i];
            if (note == null || !note.activeSelf)
            {
                noteList.RemoveAt(i);
            }
        }
    }

    public void SpawnNote(NoteDirection direction)
    {
        ObjectPool pool = direction == NoteDirection.Left ? notePoolLeft : notePoolRight;
        Transform spawnPoint = direction == NoteDirection.Left ? spawnPointLeft : spawnPointRight;
        List<GameObject> noteList = direction == NoteDirection.Left ? leftNoteList : rightNoteList;

        GameObject note = pool.Get();
        if (note == null) return;

        NoteBase noteBase = note.GetComponent<NoteBase>();
        noteBase.Init(pool, CenterLeft, CenterRight, direction, this);

        note.transform.localPosition = spawnPoint.localPosition;
        note.transform.localRotation = Quaternion.identity;
        note.SetActive(true);

        noteList.Add(note);
    }

    public void CheckTiming(NoteDirection direction)
    {
        List<GameObject> noteList = direction == NoteDirection.Left ? leftNoteList : rightNoteList;
        Vector2[] timingBoxs = direction == NoteDirection.Left ? timingBoxsLeft : timingBoxsRight;
        ObjectPool pool = direction == NoteDirection.Left ? notePoolLeft : notePoolRight;

        for (int i = 0; i < noteList.Count; i++) // ⭐ 역순 ❌ 정순으로 변경
        {
            GameObject note = noteList[i];

            if (note == null || !note.activeSelf)
            {
                noteList.RemoveAt(i);
                i--;
                continue;
            }

            float tNotePosX = note.transform.localPosition.x;

            for (int j = 0; j < timingBoxs.Length; j++)
            {
                if (timingBoxs[j].x <= tNotePosX && tNotePosX <= timingBoxs[j].y)
                {
                    pool.Return(note);
                    noteList.RemoveAt(i);

                    effectManager.NoteHitEffect();
                    effectManager.JudgementHitEffect(j);

                    string[] judgementNames = { "Perfect", "Good", "Bad", "Miss" };
                    Debug.Log($"🎯 판정: {judgementNames[j]} ({j}) | 위치: {tNotePosX}");

                    return; // ⭐ 하나만 판정하고 종료
                }
            }
        }
    }
}