using UnityEngine;

public class NoteController : MonoBehaviour
{
    TimingManager timingManager;

    private void Start()
    {
        timingManager = FindObjectOfType<TimingManager>();
        if (timingManager == null)
            Debug.LogError("TimingManager가 존재하지 않습니다!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D)
            || Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("입력 감지됨");
            timingManager.CheckTiming(NoteDirection.Left);
            timingManager.CheckTiming(NoteDirection.Right);
        }
    }
}