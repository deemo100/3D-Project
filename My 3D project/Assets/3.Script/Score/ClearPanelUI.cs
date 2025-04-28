using UnityEngine;
using TMPro;

public class ClearPanelUI : MonoBehaviour
{
    [Header("클리어 패널 텍스트")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text scoreText;

    [Header("별 오브젝트")]
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;

    public void SetupClearUI()
    {
        var timer = FindObjectOfType<GameTimer>();
        var comboManager = FindObjectOfType<ComboManager>();
        var scoreManager = FindObjectOfType<ScoreManager>();

        int achievedConditions = 0;

        if (timer != null)
        {
            float timeLeft = timer.GetRemainingTime();
            timeText.text = "TIME: " + Mathf.CeilToInt(timeLeft).ToString();

            if (timeLeft > 0f)
                achievedConditions++;
        }

        if (comboManager != null)
        {
            int maxCombo = comboManager.GetMaxCombo();
            comboText.text = "COMBO: " + maxCombo.ToString();

            if (maxCombo >= 10)
                achievedConditions++;
        }

        if (scoreManager != null)
        {
            int totalScore = scoreManager.GetTotalScore();
            scoreText.text = "GOLD: " + totalScore.ToString();

            if (totalScore >= 5)
                achievedConditions++;
        }

        // ✅ 조건 만족 수에 따라 별 활성화
        if (achievedConditions >= 1)
            star1.SetActive(true);
        if (achievedConditions >= 2)
            star2.SetActive(true);
        if (achievedConditions >= 3)
            star3.SetActive(true);
    }
}