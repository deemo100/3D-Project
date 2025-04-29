using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TMP_Text timerText;

    [Header("팝 애니메이션 설정")]
    [SerializeField] private float popScale = 1.3f; // 얼마나 커질지
    [SerializeField] private float popDuration = 0.1f; // 애니메이션 시간

    private float totalTime;
    private float currentTime;
    private bool isTimerRunning = true;

    private int lastDisplayedSeconds; // 직전 표시된 초
    private Coroutine popCoroutine;

    private void Start()
    {
        RectTransform rectTransform = timerText.GetComponent<RectTransform>();

        Vector2 originalAnchoredPosition = rectTransform.anchoredPosition;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = originalAnchoredPosition + new Vector2
        (
            rectTransform.rect.width * (rectTransform.pivot.x - 0f),
            rectTransform.rect.height * (rectTransform.pivot.y - 0f)
        );
        rectTransform.anchoredPosition += new Vector2(0f, -50f);

        if (SoundManager.Instance != null && SoundManager.Instance.GetGameStartSoundLength() > 0f)
        {
            totalTime = SoundManager.Instance.GetGameStartSoundLength() + 5f;
        }
        else
        {
            Debug.LogWarning("GameTimer: gameStartSound가 설정되지 않았거나 길이를 가져올 수 없습니다!");
            totalTime = 65f;
        }

        currentTime = totalTime;
        lastDisplayedSeconds = Mathf.CeilToInt(currentTime);
        UpdateTimerUI();

        isTimerRunning = false; //  무조건 타이머를 멈추고 시작한다
    }
    private void Update()
    {
        if (!isTimerRunning) return;

        //  게임 클리어 or 게임 오버 상태면 타이머 멈추기
        if (GameManager.Instance != null && (GameManager.Instance.IsGameClear || GameManager.Instance.IsGameOver))
        {
            StopTimer();
            return;
        }

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isTimerRunning = false;
            OnTimeOver();
        }

        UpdateTimerUI();
    }

    
    
    public float GetRemainingTime()
    {
        return currentTime;
    }
    
    private void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(currentTime);

        // 초가 변했을 때만 팝 애니메이션 실행
        if (seconds != lastDisplayedSeconds)
        {
            lastDisplayedSeconds = seconds;
            PlayPopAnimation();
        }

        timerText.text = seconds.ToString("D2");
    }

    private void PlayPopAnimation()
    {
        if (popCoroutine != null)
            StopCoroutine(popCoroutine);

        popCoroutine = StartCoroutine(PopAnimationCoroutine());
    }

    private IEnumerator PopAnimationCoroutine()
    {
        Vector3 originalScale = timerText.transform.localScale;
        Vector3 targetScale = originalScale * popScale;

        float timer = 0f;

        // 스케일 키우기
        while (timer < popDuration)
        {
            timerText.transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        timerText.transform.localScale = targetScale;

        timer = 0f;

        // 스케일 다시 줄이기
        while (timer < popDuration)
        {
            timerText.transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        timerText.transform.localScale = originalScale;
    }

    private void OnTimeOver()
    {
        Debug.Log("제한 시간 종료!");
        GameManager.Instance?.GameOver();
    }

    public void StopTimer() => isTimerRunning = false;
    public void ResumeTimer() => isTimerRunning = true;
}