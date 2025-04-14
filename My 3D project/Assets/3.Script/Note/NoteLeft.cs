using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteLeft : MonoBehaviour
{
    
    public float notespeed = 400;

    void Update()
    {
        transform.localPosition += Vector3.right * (notespeed * Time.deltaTime);
    }
}
