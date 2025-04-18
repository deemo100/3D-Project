using UnityEngine;

/// <summary>
/// 노트 개별 이동 및 MissZone 충돌 처리 담당.
/// 초기화 시 방향, 타이밍 매니저, 이펙트 매니저 연결.
/// </summary>
public class NoteBase : MonoBehaviour
{
    // ========== 노트 기본 설정 ==========
    [Header("노트 속도 설정")]
    public float noteSpeed = 400f;

    [Header("노트 방향")]
    public NoteDirection direction;

    [Header("판정 여부")]
    public bool judged = false;

    // ========== 내부 참조 ==========
    private ObjectPool notePool;
    private Transform centerLeft;
    private Transform centerRight;
    private TimingManager timingManager;
    private EffectManager effectManager;

    private Vector3 moveDir; // 이동 방향 캐싱

    // ========== 초기화 ==========
    public void Init(ObjectPool pool, Transform centerL, Transform centerR, NoteDirection dir, TimingManager timing)
    {
        notePool = pool;
        centerLeft = centerL;
        centerRight = centerR;
        direction = dir;
        timingManager = timing;
        judged = false;

        moveDir = (direction == NoteDirection.Left) ? Vector3.right : Vector3.left;
        effectManager ??= FindObjectOfType<EffectManager>();
    }

    // ========== 이동 처리 ==========
    private void Update()
    {
        transform.localPosition += moveDir * noteSpeed * Time.deltaTime;
    }

    // ========== 미스 판정 처리 ==========
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MissZone") && !judged)
        {
            judged = true;
            HandleMiss();
        }
    }

    private void HandleMiss()
    {
        effectManager?.NoteHitEffect();
        effectManager?.JudgementHitEffect(3); // 3번 인덱스 = Miss

        ReturnToPool();
        Debug.Log(" MissZone 트리거 → Miss 판정 이펙트 출력됨");
    }

    // ========== 반환 및 리스트 제거 ==========
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