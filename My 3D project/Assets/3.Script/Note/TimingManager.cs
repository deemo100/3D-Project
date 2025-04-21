using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 노트 판정 타이밍, 위치, 오브젝트 풀 관리를 담당하는 매니저
/// - 판정 영역 계산
/// - 노트 리스트 클린업
/// - 노트 판정 체크 (양쪽 동시)
/// </summary>
public class TimingManager : MonoBehaviour
{
    // ========== 노트 리스트 ==========
    [HideInInspector] public List<GameObject> leftNoteList = new();
    [HideInInspector] public List<GameObject> rightNoteList = new();

    // ========== 판정 기준 위치 ==========
    [SerializeField] private Transform centerLeft;
    [SerializeField] private Transform centerRight;

    // ========== 판정 영역 UI ==========
    [SerializeField] private RectTransform[] timingRectsLeft;  // 0:Bad, 1:Good, 2:Perfect
    [SerializeField] private RectTransform[] timingRectsRight;

    private Vector2[] timingBoxesLeft;
    private Vector2[] timingBoxesRight;

    // ========== 오브젝트 풀 ==========
    [Header("노트 풀")]
    [SerializeField] private ObjectPool notePoolLeft;
    [SerializeField] private ObjectPool notePoolRight;

    // ========== 노트 생성 위치 ==========
    [Header("노트 생성 위치")]
    [SerializeField] private Transform spawnPointLeft;
    [SerializeField] private Transform spawnPointRight;

    [SerializeField] private float spawnInterval = 1.0f;
    private float timer = 0f;

    // ========== 참조 매니저 ==========
    private EffectManager effectManager;
    private ScoreManager scoreManager;
    private ComboManager comboManager;
    
    // ========== 판정 이름 ==========
    private readonly string[] judgementNames = { "Perfect", "Good", "Bad", "Miss" };

    // ==================== Unity 이벤트 ====================
    void Start()
    {
        InitTimingBoxes();
        effectManager = FindObjectOfType<EffectManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        comboManager = FindObjectOfType<ComboManager>();
    }

    void Update()
    {
        CleanNoteList(leftNoteList);
        CleanNoteList(rightNoteList);
    }

    // ==================== 초기화 ====================
    private void InitTimingBoxes()
    {
        timingBoxesLeft = new Vector2[timingRectsLeft.Length];
        timingBoxesRight = new Vector2[timingRectsRight.Length];

        for (int i = 0; i < timingRectsLeft.Length; i++)
        {
            float half = timingRectsLeft[i].rect.width / 2;
            timingBoxesLeft[i] = new Vector2(
                centerLeft.localPosition.x - half,
                centerLeft.localPosition.x + half
            );
        }

        for (int i = 0; i < timingRectsRight.Length; i++)
        {
            float half = timingRectsRight[i].rect.width / 2;
            timingBoxesRight[i] = new Vector2(
                centerRight.localPosition.x - half,
                centerRight.localPosition.x + half
            );
        }
    }

    // ==================== 노트 생성 ====================
    public void SpawnNote(NoteDirection direction)
    {
        ObjectPool pool = GetPool(direction);
        Transform spawnPoint = GetSpawnPoint(direction);
        List<GameObject> noteList = GetNoteList(direction);

        GameObject note = pool.Get();
        if (note == null) return;

        var noteBase = note.GetComponent<NoteBase>();
        if (noteBase != null)
        {
            noteBase.Init(pool, centerLeft, centerRight, direction, this);
            note.transform.localPosition = spawnPoint.localPosition;
            note.transform.localRotation = Quaternion.identity;
            note.SetActive(true);
            noteList.Add(note);
        }
    }

    /// <summary>
    /// 양쪽 노트를 동시에 판정하고 결과 인덱스 반환
    /// 성공 시: 0=Perfect, 1=Good, 2=Bad
    /// 실패 시: 3=Miss
    /// </summary>
    public int CheckDualTiming()
    {
        GameObject leftNote = GetFirstActiveNote(leftNoteList);
        GameObject rightNote = GetFirstActiveNote(rightNoteList);

        int leftJudgement = GetJudgementIndex(leftNote, timingBoxesLeft);
        int rightJudgement = GetJudgementIndex(rightNote, timingBoxesRight);

        //  양쪽 판정 모두 성공
        if (leftJudgement >= 0 && rightJudgement >= 0)
        {
            notePoolLeft.Return(leftNote);
            notePoolRight.Return(rightNote);
            leftNoteList.Remove(leftNote);
            rightNoteList.Remove(rightNote);

            int result = Mathf.Max(leftJudgement, rightJudgement);

            // Perfect 또는 Good에만 NoteHitEffect 출력
            if (result <= 1)
                effectManager?.NoteHitEffect();

            //  모든 판정에 JudgementEffect 출력
            effectManager?.JudgementHitEffect(result);

            //  판정 기록
            scoreManager?.AddJudgement(result);

            //  콤보 처리
            if (result >= 2) // Bad or worse
                comboManager?.ResetCombo();
            else
                comboManager?.IncrementCombo();

            return result;
        }

        //  Miss 처리
        if (leftNote != null)
        {
            notePoolLeft.Return(leftNote);
            leftNoteList.Remove(leftNote);
        }
        if (rightNote != null)
        {
            notePoolRight.Return(rightNote);
            rightNoteList.Remove(rightNote);
        }

        effectManager?.JudgementHitEffect(3); // Miss
        scoreManager?.AddJudgement(3);
        comboManager?.ResetCombo();

        return 3; // Miss
    }

    // ==================== 유틸 ====================
    private void CleanNoteList(List<GameObject> noteList)
    {
        noteList.RemoveAll(note => note == null || !note.activeSelf);
    }

    private GameObject GetFirstActiveNote(List<GameObject> list)
    {
        foreach (var note in list)
        {
            if (note != null && note.activeSelf)
                return note;
        }
        return null;
    }

    private int GetJudgementIndex(GameObject note, Vector2[] timingBoxes)
    {
        if (note == null) return -1;

        float x = note.transform.localPosition.x;
        for (int i = 0; i < timingBoxes.Length; i++)
        {
            if (timingBoxes[i].x <= x && x <= timingBoxes[i].y)
                return i;
        }

        return -1;
    }

    // ==================== 도우미 ====================
    private ObjectPool GetPool(NoteDirection dir)
    {
        return dir == NoteDirection.Left ? notePoolLeft : notePoolRight;
    }

    private Transform GetSpawnPoint(NoteDirection dir)
    {
        return dir == NoteDirection.Left ? spawnPointLeft : spawnPointRight;
    }

    private List<GameObject> GetNoteList(NoteDirection dir)
    {
        return dir == NoteDirection.Left ? leftNoteList : rightNoteList;
    }

    private Vector2[] GetTimingBoxes(NoteDirection dir)
    {
        return dir == NoteDirection.Left ? timingBoxesLeft : timingBoxesRight;
    }
}