using UnityEngine;

public class TrapTile : MonoBehaviour
{
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // ✅ Player와 충돌 시 게임 오버
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
            Debug.Log(" TrapDoneos가 플레이어와 충돌 - 게임 오버!");
        }
    }
    
}
