using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameObject titleLighting;       // 타이틀 조명 및 공용 오브젝트
    public GameObject chapterSetPanel;     // 챕터 선택 UI 패널

    // 챕터 선택 패널 열기
    public void StartGame()
    {
        if (chapterSetPanel != null)
            chapterSetPanel.SetActive(true);
    }

    // 챕터 씬 로드 (ex: Chapter1, Chapter2 ...)
    public void LoadChapter(string chapterName)
    {
        Debug.Log($" 씬 전환 요청: {chapterName}");

        if (titleLighting != null)
            Destroy(titleLighting);

        SceneManager.LoadScene(chapterName);
    }

    // 챕터 패널 닫기
    public void BackToMain()
    {
        if (chapterSetPanel != null)
            chapterSetPanel.SetActive(false);
    }

    // 게임 종료
    public void ExitGame()
    {
        Application.Quit();
    }
}