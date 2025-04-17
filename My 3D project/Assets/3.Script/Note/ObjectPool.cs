using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 100;

    private Queue<GameObject> pool = new Queue<GameObject>();

    // 풀에 어떤 오브젝트가 들어있는지 추적하기 위한 해시셋
    private HashSet<GameObject> pooledObjects = new HashSet<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
            pooledObjects.Add(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            pooledObjects.Remove(obj);
            obj.SetActive(true);
            return obj;
        }

        // 💡 자동 확장: 없으면 새로 만들기!
        GameObject newObj = Instantiate(prefab, transform);
        newObj.SetActive(true);
        return newObj;
    }

    public void Return(GameObject obj)
    {
        if (obj == null || pooledObjects.Contains(obj)) return;

        obj.SetActive(false);
        pool.Enqueue(obj);
        pooledObjects.Add(obj);
    }
}