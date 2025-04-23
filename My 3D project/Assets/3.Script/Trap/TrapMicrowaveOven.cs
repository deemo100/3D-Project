using UnityEngine;
using System.Collections;

public class TrapMicrowaveOven : MonoBehaviour
{
    public float rotateDuration = 0.2f;  // 회전 시간
    public float pauseTime = 0.15f;      // 멈춤 시간

    void Start()
    {
        StartCoroutine(RotateTrapSequence());
    }

    IEnumerator RotateTrapSequence()
    {
        while (true)
        {
            // 0 → 10도 (예고)
            yield return StartCoroutine(SmoothRotateY(0f, 10f, rotateDuration));
            yield return new WaitForSeconds(pauseTime);

            // 10 → 90도 (완전히 열림)
            yield return StartCoroutine(SmoothRotateY(10f, 90f, rotateDuration));
            yield return new WaitForSeconds(pauseTime);

            // 90 → 100도 (끝까지 열림)
            yield return StartCoroutine(SmoothRotateY(90f, 100f, rotateDuration * 0.5f)); // 짧게 더 열기
            yield return new WaitForSeconds(pauseTime * 0.5f);

            // 100 → 0도 (급하게 닫힘)
            yield return StartCoroutine(SmoothRotateY(100f, 0f, rotateDuration));

            yield return new WaitForSeconds(1f); // 다음 반복까지 대기
        }
    }

    IEnumerator SmoothRotateY(float startY, float endY, float duration)
    {
        float time = 0f;

        if (endY - startY > 180f) startY += 360f;
        else if (startY - endY > 180f) endY += 360f;

        while (time < duration)
        {
            float y = Mathf.Lerp(startY, endY, time / duration);
            transform.localEulerAngles = new Vector3(0, y % 360f, 0);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0, endY % 360f, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
            Debug.Log("🔥 전자레인지 트랩 - 플레이어와 충돌하여 게임 오버");
        }
    }
}