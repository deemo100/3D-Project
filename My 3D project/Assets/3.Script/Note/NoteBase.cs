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

    public void Init(ObjectPool pool, Transform centerL, Transform centerR, NoteDirection dir, TimingManager timing)
    {
        notePool = pool;
        centerLeft = centerL;
        centerRight = centerR;
        direction = dir;
        timingManager = timing;
        judged = false;
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
            ReturnToPool();
            //Debug.Log("💥 트리거 감지로 노트 반환됨");
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