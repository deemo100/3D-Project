using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int[] judgementCounts = new int[4];
    private int totalScore = 0; //  골드 점수 저장

    public void AddJudgement(int index)
    {
        if (index < 0 || index >= judgementCounts.Length)
        {
            Debug.LogWarning($"잘못된 판정 인덱스: {index}");
            return;
        }
        judgementCounts[index]++;
    }

    public int GetJudgementCount(int index)
    {
        if (index < 0 || index >= judgementCounts.Length) return 0;
        return judgementCounts[index];
    }

    public string GetResultSummary()
    {
        string[] labels = { "Perfect", "Good", "Bad", "Miss" };
        string result = "";

        for (int i = 0; i < labels.Length; i++)
        {
            result += $"{labels[i]} : {judgementCounts[i]}\n";
        }
        return result;
    }

    //  골드 획득 처리
    public void AddScore(int amount)
    {
        totalScore += amount;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }
}