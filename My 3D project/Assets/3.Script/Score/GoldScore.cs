using UnityEngine;

public class GoldScore : MonoBehaviour
{
    public int scoreValue = 1;        // 획득 시 점수
    public float rotationSpeed = 45f; // 초당 회전 속도 (도 단위)

    void Update()
    {
        // 월드 기준 Y축 회전
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddScore(scoreValue);
                Debug.Log($"골드 먹음! 현재 점수: {scoreManager.GetTotalScore()}");
            }
            else
            {
                Debug.LogWarning("⚠ ScoreManager를 찾을 수 없습니다!");
            }

            SoundManager.Instance?.PlayGoldPickupSound();
            Destroy(gameObject);
        }
    }
}