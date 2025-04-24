using UnityEngine;

public class GoldScore : MonoBehaviour
{
    public int scoreValue = 1;       // 획득 시 점수
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
            // 점수 증가 처리
            //GameManager.Instance.AddScore(scoreValue);

            // 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}