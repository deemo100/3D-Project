using UnityEngine;
using UnityEngine.UI;

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