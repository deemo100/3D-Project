using System.Collections;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    [Header("프리팹들")]
    public GameObject trapPrefab1;
    public GameObject trapPrefab2;
    public GameObject trapPrefab3;

    [Header("생성 위치들")]
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;

    void Start()
    {
        StartCoroutine(SequentialTrapSpawner());
    }

    IEnumerator SequentialTrapSpawner()
    {
        while (true)
        {
            // 1초: 프리팹1
            SpawnTrap(trapPrefab1, spawnPoint1);
            yield return new WaitForSeconds(1f);

            // 2초: 프리팹2
            SpawnTrap(trapPrefab2, spawnPoint2);
            yield return new WaitForSeconds(1f);

            // 3초: 프리팹3
            SpawnTrap(trapPrefab3, spawnPoint3);
            yield return new WaitForSeconds(1f);
        }
    }

    void SpawnTrap(GameObject prefab, Transform point)
    {
        if (prefab == null || point == null) return;

        Quaternion flippedRotation = point.rotation * Quaternion.Euler(0, 180f, 0);
        Instantiate(prefab, point.position, flippedRotation);
    }
}