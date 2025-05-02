using UnityEngine;
using UnityEngine.UI;

public class StageButtonManager : MonoBehaviour
{
    [System.Serializable]
    public class StageButtonData
    {
        public string stageKey;                      // 저장된 별 키 (예: "Stage1_Stars")
        public int unlockRequiredStars = 1;          // 열기 위해 필요한 총 별 개수
        public Button stageButton;                   // 스테이지 버튼
        public GameObject[] starIcons;               // 별 UI 아이콘 (최대 3개)
        public GameObject[] starObjects;             // STAR1, STAR2, STAR3 오브젝트들
        public GameObject lockOverlay;               // 잠금 상태용 덮개 이미지
    }

    [Header("스테이지 버튼들")]
    public StageButtonData[] stageButtons;

    private void Start()
    {
        foreach (var data in stageButtons)
        {
            int starCount = PlayerPrefs.GetInt(data.stageKey, 0);

            //  UI 별 표시
            for (int i = 0; i < data.starIcons.Length; i++)
            {
                data.starIcons[i].SetActive(i < starCount);
            }

            //  STAR1, STAR2, STAR3 오브젝트 활성화
            for (int i = 0; i < data.starObjects.Length; i++)
            {
                data.starObjects[i].SetActive(i < starCount);
            }

            // 잠금 조건 판단
            bool isUnlocked = data.unlockRequiredStars <= 0 || IsStageUnlocked(data.unlockRequiredStars);

            //  잠금 해제 상태
            data.stageButton.interactable = isUnlocked;

            if (data.lockOverlay != null)
                data.lockOverlay.SetActive(!isUnlocked);

            Debug.Log($" {data.stageKey} → {starCount}개");
        }
    }

    /// <summary>
    /// 전체 별 개수를 합산해 조건 이상이면 true
    /// </summary>
    private bool IsStageUnlocked(int requiredStars)
    {
        int totalStars = 0;

        foreach (var data in stageButtons)
        {
            totalStars += PlayerPrefs.GetInt(data.stageKey, 0);
        }

        return totalStars >= requiredStars;
    }
}