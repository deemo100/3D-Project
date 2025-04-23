using System.Collections;
using UnityEngine;

public class TrapDrawer : MonoBehaviour
{
    private Vector3 originalPosition;
    public float moveDistance = 0.732f;
    public float moveDuration = 0.2f;
    public float stayTime = 1f; // 이동 후 & 복귀 후 대기 시간
    public float startDelay = 0f; // 추가: 시작 전 대기 시간

    void Start()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(StartDelayThenMove());
    }

    IEnumerator StartDelayThenMove()
    {
        yield return new WaitForSeconds(startDelay); // ✅ 시작 전에 1초 대기
        StartCoroutine(MoveAndReturnLoop());
    }

    IEnumerator MoveAndReturnLoop()
    {
        while (true)
        {
            // 1. 예고 움직임 (살짝 앞으로)
            Vector3 slightForward = originalPosition + new Vector3(0, 0, moveDistance * 0.1f);
            yield return StartCoroutine(MoveToPosition(slightForward, moveDuration * 0.5f));
            yield return new WaitForSeconds(0.1f);

            // 2. 본 이동 (정해진 거리만큼 앞으로)
            Vector3 fullForward = originalPosition + new Vector3(0, 0, moveDistance);
            yield return StartCoroutine(MoveToPosition(fullForward, moveDuration));
            yield return new WaitForSeconds(stayTime);

            // 3. 원위치 복귀 전 예고 (살짝 더 나감)
            Vector3 overForward = fullForward + new Vector3(0, 0, moveDistance * 0.1f);
            yield return StartCoroutine(MoveToPosition(overForward, moveDuration * 0.5f));
            yield return new WaitForSeconds(0.1f);

            // 4. 원래 위치로 복귀
            yield return StartCoroutine(MoveToPosition(originalPosition, moveDuration));
            yield return new WaitForSeconds(stayTime);
        }
    }

    IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 start = transform.localPosition;
        float time = 0f;

        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = target;
    }
}