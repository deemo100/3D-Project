using UnityEngine;
using UnityEngine.UI;

public class NoteRight : MonoBehaviour
{
    public float notespeed = 400;
    private Image NoteImage;

    private void Start()
    {
        NoteImage = GetComponent<Image>();
    }

    void Update()
    {
        transform.localPosition += Vector3.left * notespeed * Time.deltaTime;
    }

    public void HideNote()
    {
        NoteImage.enabled = false;
    }
}