using UnityEngine;

public class NoteBase : MonoBehaviour
{
    [Header("노트 속도 설정")]
    public float noteSpeed = 400f;

    [Header("노트 방향")]
    public NoteDirection direction;

    [Header("판정 여부")]
    public bool judged = false;

    // ========== 내부 참조 ==========
    private ObjectPool notePool;
    private TimingManager timingManager;
    private EffectManager effectManager;
    private ComboManager comboManager; // ✅ 콤보 매니저 추가

    private Vector3 moveDir;

    // ========== 초기화 ==========
    public void Init(ObjectPool pool, Transform centerL, Transform centerR, NoteDirection dir, TimingManager timing)
    {
        notePool = pool;
        direction = dir;
        timingManager = timing;
        judged = false;

        moveDir = (direction == NoteDirection.Left) ? Vector3.right : Vector3.left;

        effectManager ??= FindObjectOfType<EffectManager>();
        comboManager ??= FindObjectOfType<ComboManager>(); // ✅ 콤보 매니저 연결
    }

    private void Update()
    {
        transform.localPosition += moveDir * noteSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MissZone") && !judged)
        {
            judged = true;
            HandleMiss();
        }
    }

    // ✅ Miss 처리 (이펙트 + 콤보 초기화)
    private void HandleMiss()
    {
        effectManager?.JudgementHitEffect(3);   // 3번 인덱스 = Miss
        comboManager?.ResetCombo();             // ✅ 콤보 리셋
        ReturnToPool();

        Debug.Log("❌ MissZone 트리거 → Miss 판정 + 콤보 초기화");
    }

    private void ReturnToPool()
    {
        RemoveFromTimingList();

        if (notePool != null)
            notePool.Return(gameObject);
        else
            Destroy(gameObject);
    }

    private void RemoveFromTimingList()
    {
        if (timingManager == null) return;

        var list = direction == NoteDirection.Left
            ? timingManager.leftNoteList
            : timingManager.rightNoteList;

        list.Remove(gameObject);
    }
}