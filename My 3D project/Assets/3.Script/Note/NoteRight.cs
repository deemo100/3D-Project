using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRight : MonoBehaviour
{
    public float notespeed = 400;

    void Update()
    {
        transform.localPosition += Vector3.left * (notespeed * Time.deltaTime);
    }
}
