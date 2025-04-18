using UnityEngine;

// 일단 최적화. 다만 지금은 노트가 닿기만 하면 음악 재생이랑 수정 필요함.

[RequireComponent(typeof(AudioSource))]
public class CenterFlame : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private bool hasPlayedAudio = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasPlayedAudio && other.CompareTag("Note"))
        {
            audioSource.Play();
            hasPlayedAudio = true;
        }
    }
}
