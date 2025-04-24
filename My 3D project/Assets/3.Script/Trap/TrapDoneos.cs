using UnityEngine;

public class TrapDoneos : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 180f;
    private Rigidbody rb;
    private Vector3 moveDirection;

    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        // 기본 방향 설정 (없으면 오른쪽)
        if (moveDirection == Vector3.zero)
            moveDirection = transform.right;
    }

    void FixedUpdate()
    {
        Vector3 velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        rb.velocity = velocity;

        transform.Rotate(0f, rotationSpeed * Time.fixedDeltaTime, 0f, Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Tile"))
        {
            Destroy(gameObject);
        }

        // ✅ Player와 충돌 시 게임 오버
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
            Debug.Log(" TrapDoneos가 플레이어와 충돌 - 게임 오버!");
        }
    }
}