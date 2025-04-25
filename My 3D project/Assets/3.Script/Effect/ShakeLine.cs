using UnityEngine;
using System.Collections;

public class ShakeLine : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void ShakeElectric(float duration = 0.2f, float magnitude = 3f, float frequency = 60f)
    {
        StartCoroutine(ShakeElectricCoroutine(duration, magnitude, frequency));
    }

    private IEnumerator ShakeElectricCoroutine(float duration, float magnitude, float frequency)
    {
        float elapsed = 0f;
        float interval = 1f / frequency;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            rectTransform.anchoredPosition = originalPosition + new Vector2(offsetX, offsetY);

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        rectTransform.anchoredPosition = originalPosition;
    }
}