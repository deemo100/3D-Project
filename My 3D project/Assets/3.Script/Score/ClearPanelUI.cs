using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ClearPanelUI : MonoBehaviour
{
    [Header("클리어 패널 텍스트")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text scoreText;

    [Header("별 오브젝트")]
    [SerializeField] private GameObject[] starObjects;

    [Header("별 획득 조건")]
    [SerializeField] private int comboThreshold = 10;
    [SerializeField] private int scoreThreshold = 5;

    public void SetupClearUI()
    {
        var timer = FindObjectOfType<GameTimer>();
        var comboManager = FindObjectOfType<ComboManager>();
        var scoreManager = FindObjectOfType<ScoreManager>();

        int achievedConditions = 0;

        if (timer != null)
        {
            float timeLeft = timer.GetRemainingTime();
            timeText.text = "TIME: " + Mathf.CeilToInt(timeLeft);
            if (timeLeft > 0f) achievedConditions++;
        }

        if (comboManager != null)
        {
            int maxCombo = comboManager.GetMaxCombo();
            comboText.text = "COMBO: " + maxCombo;
            if (maxCombo >= comboThreshold) achievedConditions++;
        }

        if (scoreManager != null)
        {
            int totalScore = scoreManager.GetTotalScore();
            scoreText.text = "GOLD: " + totalScore;
            if (totalScore >= scoreThreshold) achievedConditions++;
        }

        // 별 UI 표시
        for (int i = 0; i < starObjects.Length; i++)
            starObjects[i].SetActive(i < achievedConditions);

        // ✅ 씬 이름을 기반으로 PlayerPrefs 키 생성
        string sceneName = SceneManager.GetActiveScene().name;           // ex: "Chapter2"
        string stageNumber = sceneName.Replace("Chapter", "Stage");      // → "Stage2"
        string stageKey = stageNumber + "_Stars";                        // → "Stage2_Stars"

        int savedStars = PlayerPrefs.GetInt(stageKey, 0);
        if (achievedConditions > savedStars)
        {
            PlayerPrefs.SetInt(stageKey, achievedConditions);
            PlayerPrefs.Save();
            Debug.Log($"{stageKey} → {achievedConditions}개 저장됨");
        }
    }
}