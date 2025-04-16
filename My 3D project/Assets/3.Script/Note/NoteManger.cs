using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NoteManager : MonoBehaviour
{
    [Header("노트 프리팹")]
    [SerializeField] private GameObject notePrefabLeft;
    [SerializeField] private GameObject notePrefabRight;

    [Header("노트 스폰 위치")]
    [SerializeField] private Transform spawnPointLeft;
    [SerializeField] private Transform spawnPointRight;

    [Header("노트 삭제 기준점 (중앙)")]
    [SerializeField] private Transform centerLeft;
    [SerializeField] private Transform centerRight;

    [Header("오브젝트 풀")]
    [SerializeField] private ObjectPool notePoolLeft;
    [SerializeField] private ObjectPool notePoolRight;

    [Header("타이밍 매니저")]
    [SerializeField] private TimingManager timingManager;

    [Header("생성 속도")]
    [SerializeField] private int bpm = 120;
    private double currentTime = 0d;

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (bpm > 0 && currentTime >= 60d / bpm)
        {
            SpawnNote(NoteDirection.Left);
            SpawnNote(NoteDirection.Right);
            currentTime -= 60d / bpm;
        }
    }

    private void SpawnNote(NoteDirection dir)
    {
        // 풀, 프리팹, 스폰 위치 선택
        ObjectPool pool = dir == NoteDirection.Left ? notePoolLeft : notePoolRight;
        Transform spawnPos = dir == NoteDirection.Left ? spawnPointLeft : spawnPointRight;
        GameObject prefab = dir == NoteDirection.Left ? notePrefabLeft : notePrefabRight;

        // 풀에서 꺼내기
        GameObject note = pool.Get();
        note.transform.localPosition = spawnPos.localPosition;
        note.transform.SetParent(this.transform, false);

        // NoteBase 초기화
        NoteBase noteBase = note.GetComponent<NoteBase>();
        if (noteBase != null)
        {
            noteBase.direction = dir;
            noteBase.Init(pool, centerLeft, centerRight); // 💡 핵심 부분!
        }
        else
        {
            Debug.LogWarning("NoteBase 스크립트가 노트에 없습니다!");
        }

        // TimingManager에 리스트 추가
        if (dir == NoteDirection.Left)
            timingManager.leftNoteList.Add(note);
        else
            timingManager.rightNoteList.Add(note);
    }
}