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
    public AudioClip goalSound; //  골 도착 사운드
    
    [Header("==== 시스템 사운드 ====")]
    public AudioClip gameStartSound;
    public AudioClip gameOverSound;

    private AudioSource sfxSource;   // 효과음 전용
    private AudioSource bgmSource;   // BGM 전용

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
        
        // AudioSource 준비
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
        bgmSource.loop = true; // BGM은 루프
    }

    // ===================== 판정 사운드 =====================
    public void PlayJudgementSound(int result)
    {
        switch (result)
        {
            case 0: // Perfect
                PlaySound(sfxSource, perfectSound);
                break;
            case 1: // Good
                PlaySound(sfxSource, goodSound);
                break;
            case 2: // Bad
                PlaySound(sfxSource, badSound);
                break;
            case 3: // Miss
                PlaySound(sfxSource, missSound);
                break;
        }
    }
    
    public float GetGameStartSoundLength()
    {
        return gameStartSound != null ? gameStartSound.length : 0f;
    }
    

    // ===================== 골드 획득 사운드 =====================
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
        }
    }

    public void PlayGoalSound()
    {
        PlaySound(sfxSource, goalSound);
    }
    
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlayGameOverSound()
    {
        PlaySound(sfxSource, gameOverSound);
    }

    // ===================== 내부 공통 메서드 =====================
    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (clip != null)
            source.PlayOneShot(clip);
    }
}