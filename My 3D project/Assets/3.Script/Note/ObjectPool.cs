using System.Collections.Generic;
using UnityEngine;

// 최적화. 수정 여지 있음

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 100;
    [SerializeField] private bool allowDynamicExpansion = true;

    private Queue<GameObject> pool = new Queue<GameObject>();
    // 풀에 어떤 오브젝트가 들어있는지 추적하기 위한 해시셋
    private HashSet<GameObject> pooledObjects = new HashSet<GameObject>();
    
    private void Awake()
    {
        InitializePool();
    }
    
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = CreateNewObject();
            obj.SetActive(false);
            pool.Enqueue(obj);
            pooledObjects.Add(obj);
        }
    }
    
    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        return obj;
    }
    
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
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else if (allowDynamicExpansion)
        {
            obj = CreateNewObject();
        }
        else
        {
            Debug.LogWarning(" 풀에 더 이상 오브젝트가 없으며 확장도 비활성화됨");
            return null;
        }

        obj.SetActive(true);
        pooledObjects.Remove(obj); // 확장된 객체도 포함됨
        return obj;
    }
    
    public void Return(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning(" Null 오브젝트는 반환할 수 없습니다.");
            return;
        }

        if (pooledObjects.Contains(obj))
        {
            Debug.LogWarning("️ 중복 반환 시도됨: 이미 풀에 있는 오브젝트입니다.");
            return;
        }

        obj.SetActive(false);
        pool.Enqueue(obj);
        pooledObjects.Add(obj);
    }
    
}