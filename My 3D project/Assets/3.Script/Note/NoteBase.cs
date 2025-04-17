using UnityEngine;

public class NoteBase : MonoBehaviour
{
    public NoteDirection direction;
    public float noteSpeed = 400f;
    public bool judged = false;

    private ObjectPool notePool;
    private Transform centerLeft;
    private Transform centerRight;
    private TimingManager timingManager;
    private EffectManager effectManager; // ✅ 이펙트 매니저 참조

    public void Init(ObjectPool pool, Transform centerL, Transform centerR, NoteDirection dir, TimingManager timing)
    {
        notePool = pool;
        centerLeft = centerL;
        centerRight = centerR;
        direction = dir;
        timingManager = timing;
        judged = false;

        // ✅ 최초 1회만 EffectManager 연결
        if (effectManager == null)
            effectManager = GameObject.FindObjectOfType<EffectManager>();
    }

    void Update()
    {
        Vector3 moveDir = direction == NoteDirection.Left ? Vector3.right : Vector3.left;
        transform.localPosition += moveDir * noteSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MissZone") && !judged)
        {
            judged = true;

            // ✅ Miss 판정 이펙트 출력
            if (effectManager != null)
            {
                effectManager.NoteHitEffect();          // 공통 이펙트
                effectManager.JudgementHitEffect(3);    // 3번 인덱스 = Miss
            }

            ReturnToPool();
            Debug.Log("💥 MissZone 트리거 → Miss 판정 이펙트 출력됨");
        }
    }

    private void ReturnToPool()
    {
        if (timingManager != null)
        {
            var noteList = direction == NoteDirection.Left
                ? timingManager.leftNoteList
                : timingManager.rightNoteList;

            if (noteList.Contains(gameObject))
                noteList.Remove(gameObject);
        }

        if (notePool != null)
            notePool.Return(gameObject);
        else
            Destroy(gameObject);
    }
}