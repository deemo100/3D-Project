using UnityEngine;

public class NoteBase : MonoBehaviour
{
    public NoteDirection direction;
    public float noteSpeed = 400f;
    public bool judged = false;

    private ObjectPool notePool;
    private Transform centerLeft;
    private Transform centerRight;

    private float removeDistance = 100f; // 벗어난 거리 허용값

    public void Init(ObjectPool pool, Transform centerL, Transform centerR)
    {
        notePool = pool;
        centerLeft = centerL;
        centerRight = centerR;
        judged = false;
    }

    void Update()
    {
        // 노트 이동
        Vector3 moveDir = direction == NoteDirection.Left ? Vector3.right : Vector3.left;
        transform.localPosition += moveDir * noteSpeed * Time.deltaTime;

        float currentX = transform.localPosition.x;

        if (!judged)
        {
            if (direction == NoteDirection.Left)
            {
                float centerX = centerLeft.localPosition.x;
                if (currentX > centerX + removeDistance)
                {
                    Debug.Log("[NoteBase] LEFT 노트 기준점 벗어남 → 반환");
                    judged = true;
                    ReturnToPool();
                }
            }
            else if (direction == NoteDirection.Right)
            {
                float centerX = centerRight.localPosition.x;
                if (currentX < centerX - removeDistance)
                {
                    Debug.Log("[NoteBase] RIGHT 노트 기준점 벗어남 → 반환");
                    judged = true;
                    ReturnToPool();
                }
            }
        }
    }

    private void ReturnToPool()
    {
        if (notePool != null)
            notePool.Return(gameObject);
        else
            Destroy(gameObject);
    }
}