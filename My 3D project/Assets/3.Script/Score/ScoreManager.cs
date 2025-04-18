using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // ✅ 판정별 카운트 배열: 0 = Perfect, 1 = Good, 2 = Bad, 3 = Miss
    private int[] judgementCounts = new int[4];

    // ==================== API ====================

    /// <summary>
    /// 판정 인덱스에 따라 카운트 증가
    /// </summary>
    public void AddJudgement(int index)
    {
        if (index < 0 || index >= judgementCounts.Length)
        {
            Debug.LogWarning($"잘못된 판정 인덱스: {index}");
            return;
        }

        judgementCounts[index]++;
    }

    /// <summary>
    /// 특정 판정 인덱스의 카운트 반환
    /// </summary>
    public int GetJudgementCount(int index)
    {
        if (index < 0 || index >= judgementCounts.Length) return 0;
        return judgementCounts[index];
    }

    /// <summary>
    /// 전체 판정 결과 문자열 반환 (결과창에 사용)
    /// </summary>
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
}