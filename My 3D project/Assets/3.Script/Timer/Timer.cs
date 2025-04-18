using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("제한 시간 설정")]
    [SerializeField] private float totalTime = 60f; // 제한 시간 (초)
    private float currentTime;

    [Header("UI 참조")]
    [SerializeField] private TMP_Text timerText; // TextMeshPro 텍스트

    private bool isTimerRunning = true;

    private void Start()
    {
        currentTime = totalTime;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isTimerRunning = false;
            OnTimeOver();
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(currentTime);
        timerText.text = seconds.ToString("D2"); // 00, 01, 02 형태로 표시
    }

    private void OnTimeOver()
    {
        Debug.Log("⏰ 제한 시간 종료!");
        // TODO: 게임 종료 처리, UI 변경 등 추가 가능
    }

    public void StopTimer() => isTimerRunning = false;
    public void ResumeTimer() => isTimerRunning = true;
}