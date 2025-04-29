using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;
    [SerializeField] private GameObject pausePanel;

    [Header("Countdown")]
    [SerializeField] private CountdownManager countdownManager;

    private bool isGameOver = false;
    private bool isGameClear = false;
    private bool isPaused = false;
    private bool isGameStarted = false;
    private bool isCountingDown = false; //  추가: 카운트다운 중 여부

    public bool IsGameOver => isGameOver;
    public bool IsGameClear => isGameClear;
    public bool IsPaused => isPaused;
    public bool IsGameStarted => isGameStarted;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (countdownManager != null)
        {
            isCountingDown = true;
            countdownManager.OnCountdownFinished += OnStartCountdownEnd;
            countdownManager.StartCountdown();
        }
        else
        {
            Debug.LogError("CountdownManager가 없습니다!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //  카운트다운 중에는 ESC 막기
            if (isCountingDown)
                return;

            if (!isGameOver && !isGameClear && isGameStarted)
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }
    }
    
    private void OnStartCountdownEnd()
    {
        countdownManager.OnCountdownFinished -= OnStartCountdownEnd;
        isCountingDown = false;

        isGameStarted = true;
        PlayerController.noMovePlayer = false;
        Time.timeScale = 1f;

        FindObjectOfType<NoteManager>()?.StartSpawningNotes();
        FindObjectOfType<GameTimer>()?.ResumeTimer(); //  여기서 시작
        SoundManager.Instance?.PlayGameStartBGM();
    }
    
    private void OnResumeCountdownEnd()
    {
        countdownManager.OnCountdownFinished -= OnResumeCountdownEnd;
        isCountingDown = false;

        isPaused = false;
        Time.timeScale = 1f;

        FindObjectOfType<GameTimer>()?.ResumeTimer(); // 여기서 시작
        SoundManager.Instance?.ResumeBGM();
    }

    public void GameOver()
    {
        if (isGameOver || isGameClear) return;

        isGameOver = true;

        FindObjectOfType<NoteManager>()?.gameObject.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        PlayerController.noMovePlayer = true;
        FindObjectOfType<PlayerController>()?.TriggerDeathAnimation();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SoundManager.Instance?.PlayGameOverSound();
        SoundManager.Instance?.StopBGM();
    }

    public void GameClear()
    {
        if (isGameOver || isGameClear) return;

        isGameClear = true;

        FindObjectOfType<NoteManager>()?.gameObject.SetActive(false);

        if (gameClearPanel != null)
            gameClearPanel.SetActive(true);

        FindObjectOfType<ClearPanelUI>()?.SetupClearUI();

        PlayerController.noMovePlayer = true;
        FindObjectOfType<PlayerController>()?.TriggerDeathAnimation();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SoundManager.Instance?.PlayGoalSound();
        SoundManager.Instance?.StopBGM();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SoundManager.Instance?.ResetBGMTime(); // 이어듣기 방지
        SceneManager.LoadScene("Chapter1");
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        FindObjectOfType<GameTimer>()?.StopTimer(); // 타이머도 정지
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SoundManager.Instance?.PauseBGM(); // BGM도 정지
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        StartCoroutine(ResumeGameAfterCountdown());
    }

    private System.Collections.IEnumerator ResumeGameAfterCountdown()
    {
        if (countdownManager != null)
        {
            isCountingDown = true;
            countdownManager.OnCountdownFinished += OnResumeCountdownEnd;
            countdownManager.StartCountdown();
        }
        else
        {
            Debug.LogError("CountdownManager가 없습니다");
        }

        yield return null;
    }
}