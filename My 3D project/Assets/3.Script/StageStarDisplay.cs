using UnityEngine;

public class StageStarDisplay : MonoBehaviour
{
    [SerializeField] private string stageKey = "Stage1_Stars"; // 저장 키 이름
    [SerializeField] private GameObject[] starIcons;            // 별 3개 연결

    private void OnEnable()
    {
        UpdateStars();
    }

    public void UpdateStars()
    {
        int starCount = PlayerPrefs.GetInt(stageKey, 0); // 최신 값

        for (int i = 0; i < starIcons.Length; i++)
            starIcons[i].SetActive(i < starCount);
    }

}