using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;  // 게임 오버용 패널
    [SerializeField] private GameObject gameClearPanel; // ✅ 게임 클리어용 패널 추가

    private bool isGameOver = false;
    private bool isGameClear = false;
    public bool IsGameOver => isGameOver;
    public bool IsGameClear => isGameClear;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SoundManager.Instance?.PlayGameStartBGM();
    }
    
    public void GameOver()
    {
        if (isGameOver || isGameClear) return;

        isGameOver = true;

        var noteManager = FindObjectOfType<NoteManager>();
        if (noteManager != null)
            noteManager.gameObject.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true); // ✅ 게임 오버 패널만 활성화

        PlayerController.noMovePlayer = true;

        var player = FindObjectOfType<PlayerController>();
        if (player != null)
            player.TriggerDeathAnimation();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SoundManager.Instance?.PlayGameOverSound();
        SoundManager.Instance?.StopBGM();
    }

    public void GameClear()
    {
        if (isGameOver || isGameClear) return;

        isGameClear = true;

        var noteManager = FindObjectOfType<NoteManager>();
        if (noteManager != null)
            noteManager.gameObject.SetActive(false);

        if (gameClearPanel != null)
            gameClearPanel.SetActive(true);

        // ✅ 클리어 UI 세팅
        var clearPanelUI = FindObjectOfType<ClearPanelUI>();
        if (clearPanelUI != null)
            clearPanelUI.SetupClearUI();

        PlayerController.noMovePlayer = true;

        var player = FindObjectOfType<PlayerController>();
        if (player != null)

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SoundManager.Instance?.PlayGoalSound();
        SoundManager.Instance?.StopBGM();
    }
    
    public void RestartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("Chapter1");
    }


 
    public void ExitGame()
    {
        SceneManager.LoadScene("Title");
    }
    
}