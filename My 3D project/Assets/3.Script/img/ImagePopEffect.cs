using UnityEngine;
using System.Collections;

public class ImagePopEffect : MonoBehaviour
{
    [Header("팝 설정")]
    [SerializeField] private float popScale = 1.2f; // 얼마나 커질지
    [SerializeField] private float popDuration = 0.1f; // 커지는데 걸리는 시간
    [SerializeField] private float shrinkDuration = 0.1f; // 줄어드는데 걸리는 시간

    private Vector3 originalScale;
    private Coroutine popCoroutine;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        popCoroutine = StartCoroutine(PopLoopCoroutine());
    }

    private IEnumerator PopLoopCoroutine()
    {
        while (true)
        {
            // 커졌다가
            yield return StartCoroutine(PopCoroutine());
            // 줄어들자마자 바로 다음 팝 시작 (쉬는 시간 없음!)
        }
    }

    private IEnumerator PopCoroutine()
    {
        Vector3 targetScale = originalScale * popScale;
        float timer = 0f;

        // ✅ 커지는 구간
        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        timer = 0f;

        // ✅ 줄어드는 구간
        while (timer < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / shrinkDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}