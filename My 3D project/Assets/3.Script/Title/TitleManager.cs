using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameObject titleLighting; // Title 조명, 볼륨 등 묶어놓은 오브젝트

    public void StartGame()
    {
        //  씬 넘어가기 전에 타이틀 라이트 삭제
        if (titleLighting != null)
        {
            Destroy(titleLighting);
        }

        //  메인 게임 씬으로 이동
        SceneManager.LoadScene("Chapter1");
    }

    // 게임 종료 버튼에서 호출
    public void ExitGame()
    {
        // 에디터에선 무시되고, 빌드된 게임에서 정상 종료
        Debug.Log("게임 종료 요청");
        Application.Quit();
    }
}