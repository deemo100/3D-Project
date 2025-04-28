using UnityEngine;

public class GoalPlase : MonoBehaviour
{
    NoteManager noteManager;

    void Start()
    {
        noteManager = FindObjectOfType<NoteManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.noMovePlayer = false;

            if (noteManager != null)
                noteManager.gameObject.SetActive(false);

            // ✅ 게임 클리어 호출
            GameManager.Instance?.GameClear();
        }
    }
}