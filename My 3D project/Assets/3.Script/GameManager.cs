using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // ✅ 노트 생성 멈추기
        var noteManager = FindObjectOfType<NoteManager>();
        if (noteManager != null)
            noteManager.gameObject.SetActive(false);

        // ✅ UI 활성화
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // ✅ 플레이어 조작 멈추기
        PlayerController.noMovePlayer = true;

        // ✅ 플레이어 애니메이션 - Dead Trigger
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
            player.TriggerDeathAnimation();
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}