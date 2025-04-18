using UnityEngine;
using UnityEngine.UI;

// 최적화 noteleft, right를 하나로 통합

public class NoteUI : MonoBehaviour
{
    public float noteSpeed = 400f;

    [SerializeField] private NoteDirection direction = NoteDirection.Left;

    private void Update()
    {
        Vector3 moveDir = direction == NoteDirection.Left ? Vector3.right : Vector3.left;
        transform.localPosition += moveDir * noteSpeed * Time.deltaTime;
    }
}