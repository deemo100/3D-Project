using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("==== 판정 사운드 ====")]
    public AudioClip perfectSound;
    public AudioClip goodSound;
    public AudioClip badSound;
    public AudioClip missSound;

    [Header("==== 기타 사운드 ====")]
    public AudioClip goldPickupSound;

    [Header("==== 게임 이벤트 사운드 ====")]
    public AudioClip goalSound;

    [Header("==== 시스템 사운드 ====")]
    public AudioClip gameStartSound;
    public AudioClip gameOverSound;

    private AudioSource sfxSource; // 효과음 전용
    private AudioSource bgmSource; // BGM 전용
    private float pausedBGMTime = 0f; // 일시정지용 BGM 재생 위치 저장

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        var audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 2)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            bgmSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            sfxSource = audioSources[0];
            bgmSource = audioSources[1];
        }

        sfxSource.loop = false;
        bgmSource.loop = true;

        // 저장된 볼륨 불러오기
        bgmSource.volume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }

    private void Update()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            Debug.Log($" BGM 재생 중: {bgmSource.clip.name}");
        }
    }

    // ===================== 판정 사운드 =====================
    public void PlayJudgementSound(int result)
    {
        switch (result)
        {
            case 0: PlaySound(sfxSource, perfectSound); break;
            case 1: PlaySound(sfxSource, goodSound); break;
            case 2: PlaySound(sfxSource, badSound); break;
            case 3: PlaySound(sfxSource, missSound); break;
        }
    }

    // ===================== 기타 사운드 =====================
    public void PlayGoldPickupSound()
    {
        PlaySound(sfxSource, goldPickupSound);
    }

    // ===================== 시스템 사운드 =====================
    public void PlayGameStartBGM()
    {
        if (gameStartSound != null)
        {
            bgmSource.clip = gameStartSound;
            bgmSource.time = 0f; // 항상 처음부터 재생
            bgmSource.Play();
            Debug.Log($" BGM 시작: {gameStartSound.name}");
        }
        else
        {
            Debug.LogWarning("️ gameStartSound가 설정되어 있지 않습니다!");
        }
    }

    public void PlayGoalSound()
    {
        PlaySound(sfxSource, goalSound);
    }

    public void PlayGameOverSound()
    {
        PlaySound(sfxSource, gameOverSound);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PauseBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            pausedBGMTime = bgmSource.time;
            bgmSource.Pause();
            Debug.Log($"⏸️ BGM 일시정지 - 위치 저장: {pausedBGMTime:F2}s");
        }
    }

    public void ResumeBGM()
    {
        if (bgmSource != null)
        {
            Debug.Log($"[ResumeBGM] clip: {bgmSource.clip?.name ?? "null"}, pausedTime: {pausedBGMTime}");

            if (bgmSource.clip == null)
            {
                Debug.LogWarning(" Resume 시 clip이 null입니다! 이어재생 불가");
                return;
            }

            bgmSource.time = pausedBGMTime;
            bgmSource.Play();
            Debug.Log($" BGM 이어 재생 - {pausedBGMTime:F2}s부터");
        }
    }

    public void ResetBGMTime()
    {
        pausedBGMTime = 0f;
    }

    // ===================== 볼륨 제어 =====================
    public float GetBGMVolume() => bgmSource != null ? bgmSource.volume : 0f;
    public float GetSFXVolume() => sfxSource != null ? sfxSource.volume : 0f;

    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
    }

    public void SaveVolume()
    {
        if (bgmSource != null) PlayerPrefs.SetFloat("BGMVolume", bgmSource.volume);
        if (sfxSource != null) PlayerPrefs.SetFloat("SFXVolume", sfxSource.volume);
        PlayerPrefs.Save();
    }

    // ===================== 내부 공통 메서드 =====================
    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (clip != null) source.PlayOneShot(clip);
    }

    public float GetGameStartSoundLength()
    {
        return gameStartSound != null ? gameStartSound.length : 0f;
    }
}